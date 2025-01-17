﻿namespace Known.Pages;

class ModuleForm : BaseStepForm
{
    private StepForm stepForm;

    private int StepCount
    {
        get
        {
            if (Model.Data.Target != ModuleType.Page.ToString())
                return 1;

            var page = Model.Data.Page;
            if (page != null && page.Columns.Exists(c => c.IsViewLink))
                return IsPageEdit ? 2 : 4;

            return IsPageEdit ? 1 : 3;
        }
    }

    internal EntityInfo Entity { get; set; }
    internal FlowInfo Flow { get; set; }

    [Parameter] public FormModel<SysModule> Model { get; set; }
    [Parameter] public bool IsPageEdit { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.SmallLabel = true;
        Model.OnFieldChanged = OnFieldChanged;
        if (!IsPageEdit)
        {
            Model.AddRow().AddColumn(c => c.Code)
                          .AddColumn(c => c.Name)
                          .AddColumn(c => c.Icon, c => c.Template = BuildIconField);
            Model.AddRow().AddColumn(c => c.Target, c =>
            {
                c.Category = nameof(ModuleType);
                c.Type = FieldType.RadioList;
                c.Span = 16;
            }).AddColumn(c => c.Sort, c => c.Span = 4)
              .AddColumn(c => c.Enabled, c => c.Span = 4);
            Model.AddRow().AddColumn(c => c.Url, c => c.Span = 8)
                          .AddColumn(c => c.Description, c => c.Span = 16);
            Model.AddRow().AddColumn(c => c.Note, c => c.Type = FieldType.TextArea);
            Step.AddStep("BasicInfo", BuildDataForm);
            Step.AddStep("ModelSetting", BuildModuleModel);
            //Step.AddStep("FlowSetting", BuildModuleFlow);
        }
        Step.AddStep("PageSetting", BuildModulePage);
        Step.AddStep("FormSetting", BuildModuleForm);

        Entity ??= DataHelper.GetEntity(Model.Data.EntityData);
    }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.Cascading(this, b =>
        {
            b.Component<StepForm>()
             .Set(c => c.Model, Step)
             .Set(c => c.IsView, Model.IsView)
             .Set(c => c.StepCount, StepCount)
             .Set(c => c.OnSave, SaveAsync)
             .Build(value => stepForm = value);
        });
    }

    private void BuildDataForm(RenderTreeBuilder builder) => UI.BuildForm(builder, Model);

    private void BuildIconField(RenderTreeBuilder builder)
    {
        builder.Component<IconPicker>()
               .Set(c => c.ReadOnly, Model.IsView)
               .Set(c => c.AllowClear, true)
               .Set(c => c.Value, Model.Data.Icon)
               .Set(c => c.ValueChanged, o => Model.Data.Icon = o?[0]?.Icon)
               .Build();
    }

    private void BuildModuleModel(RenderTreeBuilder builder)
    {
        builder.Component<EntityDesigner>()
               .Set(c => c.ReadOnly, Model.IsView)
               .Set(c => c.Model, Model.Data.EntityData)
               .Set(c => c.OnChanged, model => Model.Data.EntityData = model)
               .Build();
    }

    private void BuildModuleFlow(RenderTreeBuilder builder)
    {
        builder.Component<FlowDesigner>()
               .Set(c => c.ReadOnly, Model.IsView)
               .Set(c => c.Entity, Entity)
               .Set(c => c.Model, Model.Data.FlowData)
               .Set(c => c.OnChanged, model => Model.Data.FlowData = model)
               .Build();
    }

    private void BuildModulePage(RenderTreeBuilder builder)
    {
        builder.Component<PageDesigner>()
               .Set(c => c.ReadOnly, Model.IsView)
               .Set(c => c.Entity, Entity)
               .Set(c => c.Model, Model.Data.Page)
               .Set(c => c.OnChanged, model =>
               {
                   Model.Data.Page = model;
                   stepForm.SetStepCount(StepCount);
               })
               .Build();
    }

    private void BuildModuleForm(RenderTreeBuilder builder)
    {
        builder.Component<FormDesigner>()
               .Set(c => c.ReadOnly, Model.IsView)
               .Set(c => c.Entity, Entity)
               .Set(c => c.Flow, Flow)
               .Set(c => c.Model, Model.Data.Form)
               .Set(c => c.OnChanged, model => Model.Data.Form = model)
               .Build();
    }

    private async Task<bool> SaveAsync(bool isComplete = false)
    {
        if (!Model.Validate())
            return false;

        await Model.SaveAsync(isComplete);
        return true;
    }

    private void OnFieldChanged(string columnId)
    {
        if (columnId == nameof(SysModule.Target))
        {
            stepForm.SetStepCount(StepCount);
        }
    }
}

class IconInfo
{
    public string Icon { get; set; }
}

class IconPicker : BasePicker<IconInfo>
{
    private const string KeyCustom = "Custom";
    private readonly TabModel tab = new();
    private Dictionary<string, List<IconInfo>> icons = [];
    private string searchKey;

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Title = Language["Title.SelectIcon"];
        foreach (var item in UIConfig.Icons)
        {
            tab.AddTab(item.Key, b => BuildContent(b, item.Key));
        }
        tab.AddTab(KeyCustom, b => BuildContent(b, KeyCustom));
        icons = UIConfig.Icons.ToDictionary(k => k.Key, v => v.Value.Select(x => new IconInfo { Icon = x }).ToList());
    }

    protected override void BuildTextBox(RenderTreeBuilder builder)
    {
        if (!string.IsNullOrWhiteSpace(Value))
            builder.Div("kui-module-icon", () => builder.Icon(Value));
        base.BuildTextBox(builder);
    }

    protected override void BuildContent(RenderTreeBuilder builder) => UI.BuildTabs(builder, tab);

    private void BuildContent(RenderTreeBuilder builder, string key)
    {
        var value = SelectedItems.Count == 0 ? "" : SelectedItems[0].Icon;
        builder.Div("kui-icon-picker", () =>
        {
            if (key == KeyCustom)
            {
                UI.BuildText(builder, new InputModel<string>
                {
                    Value = value,
                    ValueChanged = this.Callback<string>(value =>
                    {
                        SelectedItems.Clear();
                        SelectedItems.Add(new IconInfo { Icon = value });
                    })
                });
            }
            else
            {
                BuildSearch(builder);
                BuildIconList(builder, key);
            }
        });
    }

    private void BuildIconList(RenderTreeBuilder builder, string key)
    {
        var items = icons[key];
        if (!string.IsNullOrWhiteSpace(searchKey))
            items = items.Where(i => i.Icon.Contains(searchKey)).ToList();

        builder.Div("items", () =>
        {
            foreach (var item in items)
            {
                var className = "item";
                if (SelectedItems.Contains(item))
                    className += " active";
                builder.Div().Class(className)
                       .OnClick(this.Callback(() => OnSelectItem(item)))
                       .Children(() =>
                       {
                           //if (key == "FontAwesome")
                           //    builder.Span(item.Icon, "");
                           //else
                           builder.Icon(item.Icon);
                           builder.Span("name", item.Icon);
                       })
                       .Close();
            }
        });
    }

    private void BuildSearch(RenderTreeBuilder builder)
    {
        builder.Div("search", () =>
        {
            UI.BuildSearch(builder, new InputModel<string>
            {
                Placeholder = "Search",
                Value = searchKey,
                ValueChanged = this.Callback<string>(value =>
                {
                    searchKey = value;
                    StateChanged();
                })
            });
        });
    }

    private void OnSelectItem(IconInfo item)
    {
        if (!SelectedItems.Remove(item))
            SelectedItems.Add(item);
        StateChanged();
    }
}