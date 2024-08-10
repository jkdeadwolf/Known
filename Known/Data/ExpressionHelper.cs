﻿namespace Known.Data;

class ExpressionHelper(SqlBuilder builder)
{
    private readonly SqlBuilder builder = builder;

    internal string WhereSql { get; private set; }
    internal Dictionary<string, object> Parameters { get; private set; }

    public void RouteExpression<T>(Expression<Func<T, bool>> expression)
    {
        WhereSql = string.Empty;
        Parameters = [];
        if (expression.Body is BinaryExpression be)
            RouteExpression<T>(be.Left, be.Right, be.NodeType);
        else
            RouteExpression<T>(expression.Body);
    }

    //public string RouteExpression<T, T1>(Expression<Func<T, T1, bool>> expression)
    //{
    //    WhereSql = string.Empty;
    //    Parameters = [];
    //    if (expression.Body is BinaryExpression be)
    //    {
    //        var left = RouteExpression<T1>(be.Left);
    //        WhereSql += $"{left}";
    //        WhereSql += CastType(be.NodeType);
    //        var right = RouteExpression<T>(be.Right);
    //        WhereSql += $"{right}";
    //    }
    //    return WhereSql;
    //}

    private object RouteExpression<T>(Expression exp, object paramName = null)
    {
        if (exp is NewArrayExpression ae)
            return RouteExpression<T>(ae);

        if (exp is ConstantExpression ce)
            return ce.Value;

        if (exp is MemberExpression me) //d.Enabled
        {
            if (!exp.ToString().StartsWith("value"))
                return RouteExpression<T>(me);
            return RouteExpressionValue(exp, paramName);
        }

        if (exp is BinaryExpression be)
        {
            RouteExpression<T>(be.Left, be.Right, be.NodeType);
        }
        else if (exp is MethodCallExpression mce)
        {
            return RouteExpression<T>(mce);
        }
        else if (exp is UnaryExpression ue) //!d.Enabled  ue.NodeType=Not  ue.Operand =d.Enabled
        {
            if (ue.NodeType == ExpressionType.Not)
                WhereSql += "Not";
            RouteExpression<T>(ue.Operand);
        }
        return null;
    }

    private void RouteExpression<T>(Expression left, Expression right, ExpressionType type)
    {
        var field = RouteExpression<T>(left);
        if (field != null)
            WhereSql += $"{field}";
        WhereSql += CastType(type);
        var value = RouteExpression<T>(right);
        if (value == null)
        {
            if (WhereSql.EndsWith("="))
                WhereSql = WhereSql[..^1] + " is null";
            else if (WhereSql.EndsWith("<>"))
                WhereSql = WhereSql[..^2] + " is not null";
        }
        else if (field != null)
        {
            var param = GetParameterName(field);
            WhereSql += $"@{param}";
            Parameters[param] = value;
        }
    }

    private string RouteExpression<T>(NewArrayExpression ae)
    {
        var sb = new StringBuilder();
        //var index = 0;
        foreach (Expression ex in ae.Expressions)
        {
            //var key = $"{fieldName}{index++}";
            //Parameters[key] = ExpressionRouter(ex);
            //sb.Append($"@{key},");
            sb.Append(RouteExpression<T>(ex));
            sb.Append(',');
        }
        return sb.ToString(0, sb.Length - 1);
    }

    private object RouteExpression<T>(MethodCallExpression mce)
    {
        if (mce.Method.Name == nameof(string.Contains))
            return SetLikeWhere<T>(mce, "%{0}%");
        else if (mce.Method.Name == nameof(string.StartsWith))
            return SetLikeWhere<T>(mce, "{0}%");
        else if (mce.Method.Name == nameof(string.EndsWith))
            return SetLikeWhere<T>(mce, "%{0}");
        else if (mce.Method.Name == nameof(WhereExtension.In))
            return SetArrayWhere<T>(mce, "in");
        else if (mce.Method.Name == nameof(WhereExtension.NotIn))
            return SetArrayWhere<T>(mce, "not in");
        else if (mce.Method.Name == nameof(WhereExtension.Between))
            return SetArrayWhere<T>(mce, "between");
        else
            return RouteExpressionValue(mce);
    }

    private string RouteExpression<T>(MemberExpression me)
    {
        var name = me.Member.Name;
        var type = me.Member.DeclaringType;
        if (type == typeof(EntityBase))
            type = typeof(T);
        var field = builder.GetColumnName(type, name);
        if (WhereSql.EndsWith("Not"))
        {
            WhereSql = WhereSql[..^3] + $"{field}='False'";
            return null;
        }
        else if (me.Member.ToString().Contains("Boolean"))
        {
            WhereSql += $"{field}='True'";
            return null;
        }
        return field;
    }

    private object RouteExpressionValue(Expression exp, object paramName = null)
    {
        var value = Expression.Lambda(exp).Compile().DynamicInvoke();
        if (value == null)
            return null;

        if (value is int[] ints)
            return GetArrayWhere(paramName, ints);

        if (value is string[] values)
            return GetArrayWhere(paramName, values);

        return value;
    }

    private object GetArrayWhere<TItem>(object paramName, TItem[] values)
    {
        var sb = new StringBuilder();
        var index = 0;
        foreach (var item in values)
        {
            var key = $"{paramName}{index++}";
            Parameters[key] = item;
            sb.Append($"@{key},");
        }
        return sb.ToString(0, sb.Length - 1);
    }

    private object SetLikeWhere<T>(MethodCallExpression mce, string format)
    {
        var isNot = WhereSql.EndsWith("Not");
        if (isNot)
            WhereSql = WhereSql[..^3];
        if (mce.Object == null)
        {
            var field = RouteExpression<T>(mce.Arguments[0]);
            var arg1 = RouteExpression<T>(mce.Arguments[1]);
            var operate = isNot ? "not in" : "in";
            WhereSql += $"{field} {operate} ({arg1})";
        }
        else if (mce.Object.NodeType == ExpressionType.MemberAccess)
        {
            var field = RouteExpression<T>(mce.Object);
            var value = RouteExpression<T>(mce.Arguments[0]);
            var operate = isNot ? "not like" : "like";
            var param = GetParameterName(field);
            Parameters[param] = string.Format(format, value);
            WhereSql += $"{field} {operate} @{param}";
        }
        return null;
    }

    private object SetArrayWhere<T>(MethodCallExpression mce, string format)
    {
        if (mce.Object == null)
        {
            var field = RouteExpression<T>(mce.Arguments[0]);
            if (format == "between")
            {
                var arg1 = RouteExpression<T>(mce.Arguments[1]);
                var arg2 = RouteExpression<T>(mce.Arguments[2]);
                if (arg1 is DateTime && builder.DatabaseType == DatabaseType.Access)
                    WhereSql += $"{field} between #{arg1}# and #{arg2}#";
                else
                    WhereSql += $"{field} between '{arg1}' and '{arg2}'";
            }
            else
            {
                var param = GetParameterName(field);
                var arg1 = RouteExpression<T>(mce.Arguments[1], param);
                WhereSql += $"{field} {format} ({arg1})";
            }
        }
        else if (mce.Object.NodeType == ExpressionType.MemberAccess)
        {
            var field = RouteExpression<T>(mce.Object);
            var value = RouteExpression<T>(mce.Arguments[0]);
            var param = GetParameterName(field);
            Parameters[param] = string.Format(format, value);
            WhereSql += $"{field} {format} (@{param})";
        }
        return null;
    }

    private static string GetParameterName(object field)
    {
        var fields = field.ToString().Split('.');
        var name = fields.Length > 1 ? fields[1] : fields[0];
        return name.TrimStart('`', '\"', '[').TrimEnd('`', '\"', ']');
    }

    private static string CastType(ExpressionType type)
    {
        switch (type)
        {
            case ExpressionType.And:
            case ExpressionType.AndAlso:
                return " and ";
            case ExpressionType.Equal:
                return "=";
            case ExpressionType.GreaterThan:
                return ">";
            case ExpressionType.GreaterThanOrEqual:
                return ">=";
            case ExpressionType.LessThan:
                return "<";
            case ExpressionType.LessThanOrEqual:
                return "<=";
            case ExpressionType.NotEqual:
                return "<>";
            case ExpressionType.Or:
            case ExpressionType.OrElse:
                return " or ";
            case ExpressionType.Add:
            case ExpressionType.AddChecked:
                return "+";
            case ExpressionType.Subtract:
            case ExpressionType.SubtractChecked:
                return "-";
            case ExpressionType.Divide:
                return "/";
            case ExpressionType.Multiply:
            case ExpressionType.MultiplyChecked:
                return "*";
            default:
                return null;
        }
    }
}