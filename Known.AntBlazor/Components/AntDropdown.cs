﻿namespace Known.AntBlazor.Components;

public class AntDropdown : Dropdown
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    [Parameter] public Context Context { get; set; }
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Text { get; set; }
    [Parameter] public string TextIcon { get; set; }
    [Parameter] public string TextButton { get; set; }
    [Parameter] public List<ActionInfo> Items { get; set; }
    [Parameter] public Action<ActionInfo> OnItemClick { get; set; }

    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(string);
        base.OnInitialized();
        if (!string.IsNullOrWhiteSpace(Icon))
            ChildContent = BuildIcon;
        else if (!string.IsNullOrWhiteSpace(Text))
            ChildContent = BuildText;
        else if (!string.IsNullOrWhiteSpace(TextIcon))
            ChildContent = BuildTextIcon;
        else if (!string.IsNullOrWhiteSpace(TextButton))
            ChildContent = BuildTextButton;

        if (Items != null && Items.Count > 0)
            Overlay = BuildOverlay;
    }

    private void BuildIcon(RenderTreeBuilder builder)
    {
        builder.Component<Icon>().Set(c => c.Type, Icon).Set(c => c.Theme, "outline").Build();
        if (!string.IsNullOrWhiteSpace(Text))
            builder.Span(Text);
    }

    private void BuildText(RenderTreeBuilder builder)
    {
        builder.OpenElement("a").Class("ant-dropdown-link").PreventDefault().Children(() =>
        {
            builder.Markup(Text);
            builder.Component<Icon>().Set(c => c.Type, "down").Build();
        }).Close();
    }

    private void BuildTextIcon(RenderTreeBuilder builder)
    {
        builder.Span().Role("img").Text(TextIcon).Close();
    }

    private void BuildTextButton(RenderTreeBuilder builder)
    {
        builder.Component<Button>()
               .Set(c => c.ChildContent, b =>
               {
                   b.Markup(TextButton);
                   b.Component<Icon>().Set(c => c.Type, "down").Build();
               })
               .Build();
    }

    private void BuildOverlay(RenderTreeBuilder builder)
    {
        builder.Component<Menu>().Set(c => c.ChildContent, BuildMenu).Build();
    }

    private void BuildMenu(RenderTreeBuilder builder)
    {
        foreach (var item in Items)
        {
            builder.Component<MenuItem>()
                   .Set(c => c.Key, item.Id)
                   .Set(c => c.Disabled, !item.Enabled)
                   .Set(c => c.ChildContent, b => BuildMenuItem(b, item))
                   .Build();
        }
    }

    private void BuildMenuItem(RenderTreeBuilder builder, ActionInfo item)
    {
        builder.Div().OnClick(this.Callback<MouseEventArgs>(e => OnItemClick?.Invoke(item)))
               .Children(() => BuildItemName(builder, item))
               .Close();
    }

    private void BuildItemName(RenderTreeBuilder builder, ActionInfo item)
    {
        if (!string.IsNullOrWhiteSpace(item.Icon))
        {
            builder.Component<Icon>()
                   .Set(c => c.Type, item.Icon)
                   .Set(c => c.Theme, "outline")
                   .Build();
        }
        var itemName = Context?.Language.GetString(item);
        builder.Span(itemName);
    }
}