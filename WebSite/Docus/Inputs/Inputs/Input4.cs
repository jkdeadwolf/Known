﻿namespace WebSite.Docus.Inputs.Inputs;

class Input4 : BaseComponent
{
    private Input? input;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.OnReadOnlyChanged, OnReadOnlyChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<Input>("搜索", "Search")
               .Set(f => f.Type, InputType.Search)
               .Build(value => input = value);
    }

    private void OnVisibleChanged(bool value) => input?.SetVisible(value);
    private void OnEnabledChanged(bool value) => input?.SetEnabled(value);
    private void OnReadOnlyChanged(bool value) => input?.SetReadOnly(value);
    private void SetValue() => input?.SetValue("test");
    private string? GetValue() => input?.Value;
}