﻿namespace Known.Data;

public static class QueryExtension
{
    public static QueryBuilder<T> Query<T>(this Database db) where T : class, new() => new(db);
    public static QueryBuilder<T> Select<T>(this Database db) where T : class, new() => new QueryBuilder<T>(db).Select();
}