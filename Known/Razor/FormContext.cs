﻿namespace Known.Razor;

class FormContext : FieldContext
{
    internal dynamic Data
    {
        get
        {
            var obj = new ExpandoObject();
            foreach (var item in Fields)
            {
                obj.TryAdd(item.Key, item.Value.GetFieldValue());
            }
            return obj;
        }
    }

    internal Dictionary<string, List<IBrowserFile>> Files
    {
        get
        {
            var dic = new Dictionary<string, List<IBrowserFile>>();
            foreach (var item in Fields)
            {
                if (item.Value is KUpload)
                {
                    var upload = item.Value as KUpload;
                    if (upload.Files != null && upload.Files.Count > 0)
                        dic.TryAdd(item.Key, upload.Files);
                }
            }
            return dic;
        }
    }

    internal bool Validate()
    {
        var errors = new List<string>();
        foreach (var item in Fields)
        {
            if (!item.Value.Validate())
                errors.Add(item.Key);
        }

        return errors.Count == 0;
    }

    internal bool ValidateCheck(bool isPass)
    {
        var errors = new List<string>();
        foreach (var item in Fields)
        {
            if (!item.Value.Validate())
                errors.Add(item.Key);
        }

        return errors.Count == 0;
    }

    internal void Clear()
    {
        foreach (var item in Fields)
        {
            item.Value.ClearFieldValue();
        }
    }

    internal void SetData(object data)
    {
        Model = data;
        foreach (var item in Fields)
        {
            var value = DicModel != null && DicModel.ContainsKey(item.Key) ? DicModel[item.Key] : null;
            item.Value.SetFieldValue(value);
        }
    }

    internal void SetReadOnly(bool readOnly)
    {
        ReadOnly = readOnly;
        foreach (var item in Fields)
        {
            item.Value.SetFieldReadOnly(readOnly);
        }
    }

    internal void SetEnabled(bool enabled)
    {
        foreach (var item in Fields)
        {
            item.Value.SetFieldEnabled(enabled);
        }
    }
}