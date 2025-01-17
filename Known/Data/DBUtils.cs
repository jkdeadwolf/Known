﻿namespace Known.Data;

public class DBUtils
{
    internal static void RegisterConnections()
    {
        var connections = Config.App.Connections;
        if (connections == null || connections.Count == 0)
            return;

        AppHelper.LoadConnections(connections);
        var dbFactories = connections.ToDictionary(k => k.DatabaseType.ToString(), v => v.ProviderType);
        if (dbFactories != null && dbFactories.Count > 0)
        {
            foreach (var item in dbFactories)
            {
                if (!DbProviderFactories.GetProviderInvariantNames().Contains(item.Key))
                {
                    DbProviderFactories.RegisterFactory(item.Key, item.Value);
                }
            }
        }
    }

    internal static async Task InitializeAsync()
    {
        var db = Database.Create();
        int? count = null;
        try
        {
            count = await db.CountAsync<SysModule>();
        }
        catch
        {
        }

        if (count == null)
        {
            Logger.Info("Data table is initializing...");
            var name = db.DatabaseType.ToString();
            foreach (var item in Config.CoreAssemblies)
            {
                var script = Utils.GetResource(item, $"{name}.sql");
                if (string.IsNullOrWhiteSpace(script))
                    continue;

                await db.ExecuteAsync(script);
            }
            Logger.Info("Data table is initialized");
        }
    }

    public static object ConvertTo<T>(IDataReader reader)
    {
        var dic = GetDictionary(reader);
        var type = typeof(T);
        if (type == typeof(Dictionary<string, object>))
            return dic;

        var obj = Activator.CreateInstance<T>();
        var properties = TypeHelper.Properties(type);
        foreach (var item in dic)
        {
            var property = properties.FirstOrDefault(p => p.Name.Equals(item.Key, StringComparison.CurrentCultureIgnoreCase));
            if (property != null)
            {
                var value = Utils.ConvertTo(property.PropertyType, item.Value);
                property.SetValue(obj, value);
            }
        }
        if (obj is EntityBase)
        {
            (obj as EntityBase).SetOriginal(dic);
        }
        return obj;
    }

    public static Dictionary<string, object> ToDictionary(object value)
    {
        if (value is Dictionary<string, object> dictionary)
            return dictionary;

        var dic = Utils.MapTo<Dictionary<string, object>>(value);
        return dic ?? [];
    }

    internal static Dictionary<string, object> GetDictionary(IDataReader reader)
    {
        var dic = new Dictionary<string, object>();
        for (int i = 0; i < reader.FieldCount; i++)
        {
            var name = reader.GetName(i).Replace("_", "");
            var value = reader[i];
            dic[name] = value == DBNull.Value ? null : value;
        }
        return dic;
    }

    public static byte[] GetExportData<T>(PagingCriteria criteria, List<T> pageData)
    {
        if (criteria.ExportColumns == null || criteria.ExportColumns.Count == 0 || pageData.Count == 0)
            return null;

        var excel = ExcelFactory.Create();
        var sheet = excel.CreateSheet("Sheet1");
        var index = 0;
        var headStyle = new StyleInfo { IsBorder = true, IsBold = true, FontColor = Color.White, BackgroundColor = Utils.FromHtml("#6D87C1") };
        foreach (var item in criteria.ExportColumns)
        {
            sheet.SetCellValue(0, index++, item.Name, headStyle);
        }

        var rowIndex = 0;
        var isDictionary = typeof(T) == typeof(Dictionary<string, object>);
        foreach (var data in pageData)
        {
            rowIndex++;
            index = 0;
            foreach (var item in criteria.ExportColumns)
            {
                var cellStyle = new StyleInfo { IsBorder = true };
                var value = isDictionary
                          ? (data as Dictionary<string, object>).GetValue(item.Id)
                          : TypeHelper.GetPropertyValue(data, item.Id);
                if (item.Type == FieldType.Switch || item.Type == FieldType.CheckBox)
                    value = Utils.ConvertTo<bool>(value) ? "是" : "否";
                else if (item.Type == FieldType.Date)
                {
                    value = Utils.ConvertTo<DateTime?>(value)?.Date;
                    cellStyle.Custom = Config.DateFormat;
                }
                else if (item.Type == FieldType.DateTime)
                    cellStyle.Custom = Config.DateTimeFormat;
                else if (item.Type == FieldType.Number)
                    value = GetNumberValue(value);
                else if (!string.IsNullOrWhiteSpace(item.Category))
                    value = Cache.GetCodeName(item.Category, value?.ToString());
                sheet.SetCellValue(rowIndex, index++, value, cellStyle);
            }
        }

        var stream = excel.SaveToStream();
        return stream.ToArray();
    }

    private static object GetNumberValue(object value)
    {
        if (decimal.TryParse(value?.ToString(), out var number))
            return number;

        return value;
    }
}