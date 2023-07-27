﻿namespace WebSite.Docus.Input.Dates;

class Date1 : BaseComponent
{
    private Date? date;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<Date>("日期", "Date").Value("2023-01-01").Build(value => date = value);
    }

    private void OnVisibleChanged(bool value) => date?.SetVisible(value);
    private void OnEnabledChanged(bool value) => date?.SetEnabled(value);
    private void SetValue() => date?.SetValue($"{DateTime.Now:yyyy-MM-dd}");
    private string? GetValue() => date?.Value;
}