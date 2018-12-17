﻿using System;
using System.Collections.Generic;
using System.Data;
using Known.Extensions;
using Known.Mapping;

namespace Known.Data
{
    public class AutoMapper
    {
        public static T GetBaseEntity<T>(DataRow row) where T : BaseEntity
        {
            var entity = GetEntity<T>(row);
            entity.IsNew = false;
            return entity;
        }

        public static List<T> GetBaseEntities<T>(DataTable data) where T : BaseEntity
        {
            if (data == null || data.Rows.Count == 0)
                return null;

            var lists = new List<T>();
            foreach (DataRow row in data.Rows)
            {
                lists.Add(GetBaseEntity<T>(row));
            }
            return lists;
        }

        public static T GetEntity<T>(DataRow row)
        {
            if (row == null)
                return default(T);

            var entity = Activator.CreateInstance<T>();
            var properties = typeof(T).GetColumnProperties();
            foreach (var property in properties)
            {
                var column = new ColumnInfo(property);
                if (row.Table.Columns.Contains(column.ColumnName))
                {
                    var value = GetPropertyValue(property.PropertyType, row[column.ColumnName]);
                    property.SetValue(entity, value, null);
                }
            }

            return entity;
        }

        public static List<T> GetEntities<T>(DataTable data)
        {
            if (data == null || data.Rows.Count == 0)
                return null;

            var lists = new List<T>();
            foreach (DataRow row in data.Rows)
            {
                lists.Add(GetEntity<T>(row));
            }
            return lists;
        }

        private static object GetPropertyValue(Type type, object value)
        {
            if (type.IsSubclassOf(typeof(BaseEntity)))
            {
                var entity = Activator.CreateInstance(type) as BaseEntity;
                entity.Id = value.ToString();
                entity.IsNew = false;
                return entity;
            }

            return Utils.ConvertTo(type, value);
        }
    }
}
