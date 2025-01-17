﻿namespace Known.AntBlazor.Components;

public class AntLanguage : BaseComponent
{
    private ActionInfo current;

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        current = Language.GetLanguage(Context.CurrentLanguage);
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<AntDropdown>()
               .Set(c => c.Context, Context)
               .Set(c => c.Icon, "translation")
               .Set(c => c.Text, current?.Icon)
               .Set(c => c.Items, Language.Items.Where(i => i.Visible).ToList())
               .Set(c => c.OnItemClick, OnLanguageChanged)
               .Build();
    }

    private async void OnLanguageChanged(ActionInfo info)
    {
        current = info;
        Context.CurrentLanguage = current.Id;
        await JS.SetCurrentLanguageAsync(current.Id);
        Navigation.Refresh(true);
    }
}