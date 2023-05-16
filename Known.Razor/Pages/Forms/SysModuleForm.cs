﻿namespace Known.Razor.Pages.Forms;

[Dialog(1000, 690)]
class SysModuleForm : BaseForm<SysModule>
{
    private ColumnGrid grid;
    private SysModule model;
    private List<ColumnInfo> columns;

    protected override void OnInitialized()
    {
        model = TModel;
        columns = model.Columns ?? new List<ColumnInfo>();
        base.OnInitialized();
    }

    protected override void BuildFields(FieldBuilder<SysModule> builder)
    {
        builder.Hidden(f => f.Id);
        builder.Hidden(f => f.ParentId);
        builder.Table(table =>
        {
            table.ColGroup(10, 15, 10, 15, 10, 15, 10, 15);
            BuildHead(table, builder);
            table.FormListRef<ColumnGrid>("列表栏位", 8, 300, attr =>
            {
                attr.Add(nameof(ColumnGrid.ReadOnly), ReadOnly)
                    .Add(nameof(ColumnGrid.IsModule), true)
                    .Add(nameof(ColumnGrid.Data), columns);
                table.Reference<ColumnGrid>(value => grid = value);
            });
        });
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);
        base.BuildButtons(builder);
    }

    private void OnSave()
    {
        SubmitAsync(data =>
        {
            data.ColumnData = Utils.ToJson(grid.Data);
            return Platform.Module.SaveModuleAsync(data);
        });
    }

    private void BuildHead(RenderTreeBuilder table, FieldBuilder<SysModule> builder)
    {
        table.Tr(attr =>
        {
            builder.Field<Text>(f => f.Code).Build();
            builder.Field<Text>(f => f.Name).Build();
            builder.Field<Text>(f => f.Icon).Build();
            builder.Field<Number>(f => f.Sort).Build();
        });
        table.Tr(attr =>
        {
            builder.Field<Text>(f => f.Description).ColSpan(3).Build();
            builder.Field<Text>(f => f.Target).ColSpan(3).Build();
        });
        table.Tr(attr =>
        {
            table.Th("", "选项");
            table.Td(attr =>
            {
                attr.ColSpan(3);
                builder.Field<CheckBox>(f => f.Enabled).IsInput(true).Set(f => f.Text, "启用").Build();
            });
            builder.Field<Text>(f => f.Note).ColSpan(3).Build();
        });
        table.Tr(attr =>
        {
            table.Th("", "按钮");
            table.Td("inline", attr =>
            {
                attr.ColSpan(7);
                builder.Field<Text>(f => f.ButtonData).IsInput(true).ReadOnly(true).Build();
                if (!ReadOnly)
                    table.Div("right", attr => table.Button(FormButton.Config, Callback(SetButton)));
            });
        });
        table.Tr(attr =>
        {
            table.Th("", "操作");
            table.Td("inline", attr =>
            {
                attr.ColSpan(7);
                builder.Field<Text>(f => f.ActionData).IsInput(true).ReadOnly(true).Build();
                if (!ReadOnly)
                    table.Div("right", attr => table.Button(FormButton.Config, Callback(SetAction)));
            });
        });
    }

    private void SetButton() => ConfigButton(ButtonGrid.KeyButton, model.ButtonData);
    private void SetAction() => ConfigButton(ButtonGrid.KeyAction, model.ActionData);

    private void ConfigButton(string name, string value)
    {
        UI.Show<ButtonGrid>(name, new(400, 400), action: attr =>
        {
            attr.Set(c => c.Key, name)
                .Set(c => c.Value, value)
                .Set(c => c.OnValueChanged, v =>
                {
                    if (name == ButtonGrid.KeyButton)
                    {
                        model.ButtonData = v;
                        form.Fields[nameof(SysModule.ButtonData)].SetValue(v);
                    }
                    else if (name == ButtonGrid.KeyAction)
                    {
                        model.ActionData = v;
                        form.Fields[nameof(SysModule.ActionData)].SetValue(v);
                    }
                });
        });
    }
}