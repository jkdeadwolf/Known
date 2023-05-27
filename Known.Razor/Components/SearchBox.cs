﻿namespace Known.Razor.Components;

public class SearchBox : BaseComponent
{
    private string error;
    private string key;

    [Parameter] public bool Required { get; set; } = true;
    [Parameter] public string Placeholder { get; set; }
    [Parameter] public Action<string> OnSearh { get; set; }

    public bool Validate()
    {
        error = string.Empty;
        if (string.IsNullOrEmpty(key) && Required)
        {
            error = "error";
            return false;
        }

        return true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div($"search-box {error}", attr =>
        {
            builder.Input(attr => attr.Placeholder(Placeholder).OnChange(Callback<ChangeEventArgs>(e => OnKeyChanged(e))));
            builder.Icon("fa fa-search", attr => attr.OnClick(Callback(OnClick)));
        });
    }

    private void OnKeyChanged(ChangeEventArgs e) => key = e?.Value?.ToString();

    private void OnClick()
    {
        if (!Validate())
            return;

        OnSearh?.Invoke(key);
    }
}