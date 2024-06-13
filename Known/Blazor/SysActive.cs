﻿namespace Known.Blazor;

class SysActive : BaseComponent
{
    private FormModel<SystemInfo> model;

    [Parameter] public Action<bool> OnCheck { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        model = new FormModel<SystemInfo>(Context);
        model.AddRow().AddColumn(c => c.ProductId);
        model.AddRow().AddColumn(c => c.ProductKey);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            model.Data = await Platform.System.GetSystemAsync();
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        UI.BuildResult(builder, "403", Config.AuthStatus);
        builder.Div("kui-form-auth", () =>
        {
            UI.BuildForm(builder, model);
            builder.FormPageButton(() =>
            {
                UI.Button(builder, new ActionInfo(Context, "OK", ""), this.Callback<MouseEventArgs>(OnAuthAsync));
            });
        });
    }

    private async void OnAuthAsync(MouseEventArgs args)
    {
        if (!model.Validate())
            return;

        var result = await Platform.System.SaveKeyAsync(model.Data);
        UI.Result(result, () => OnCheck?.Invoke(result.IsValid));
    }
}