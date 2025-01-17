﻿namespace Known.Data;

class OracleBuilder : SqlBuilder
{
    public override string Prefix => ":";

    public override string GetDateSql(string name, bool withTime = true)
    {
        var format = "yyyy-mm-dd";
        if (withTime)
            format += " hh24:mi:ss";
        return $"to_date(:{name},'{format}')";
    }
}