﻿namespace Known.Pages;

[Route("/profile/password")]
public class PasswordEditForm : BaseForm<PwdFormInfo>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model = new FormModel<PwdFormInfo>(Context, true)
        {
            LabelSpan = 4,
            WrapperSpan = 6,
            Data = new PwdFormInfo()
        };
    }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            builder.Div("form-password", () =>
            {
                base.BuildForm(builder);
                builder.FormButton(() =>
                {
                    UI.Button(builder, Language["Button.ConfirmUpdate"], this.Callback<MouseEventArgs>(OnSaveAsync), "primary");
                });
            });
        });
    }

    private async void OnSaveAsync(MouseEventArgs arg)
    {
        if (!Model.Validate())
            return;

        var result = await Platform.Auth.UpdatePasswordAsync(Model.Data);
        UI.Result(result);
    }
}