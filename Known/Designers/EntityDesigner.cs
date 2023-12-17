﻿using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class EntityDesigner : BaseComponent
{
    private readonly List<CodeInfo> AddTypes =
    [
        new CodeInfo("新建"),
        new CodeInfo("从实体库中选择")
    ];

    private string addType;
    private EntityInfo entity = new();
    private EntityView view;

    private bool IsNew => addType == AddTypes[0].Code;

    [Parameter] public string Model { get; set; }
    [Parameter] public Action<string> OnChanged { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        addType = !string.IsNullOrWhiteSpace(Model) && Model.Contains('|') 
                ? AddTypes[0].Code : AddTypes[1].Code;
        entity = GetEntity(Model);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Cascading(this, this.BuildTree(b =>
        {
            b.Div("kui-designer entity", () =>
            {
                b.Div("panel-model", () =>
                {
                    b.Div("caption", () =>
                    {
                        b.Div("title", "实体模型");
                        BuildModelType(b);
                    });
                    BuildNewModel(b);
                });
                b.Div("panel-view", () =>
                {
                    b.Component<EntityView>().Set(c => c.Model, entity).Build(value => view = value);
                });
            });
        }));
    }

    private void BuildModelType(RenderTreeBuilder builder)
    {
        builder.Div(() =>
        {
            UI.BuildRadioList(builder, new InputModel<string>
            {
                Disabled = ReadOnly,
                Codes = AddTypes,
                Value = addType,
                ValueChanged = this.Callback<string>(OnTypeChanged)
            });
        });

        if (!IsNew)
        {
            builder.Div(() =>
            {
                UI.BuildSelect(builder, new InputModel<string>
                {
                    Disabled = ReadOnly,
                    Codes = Cache.GetCodes("TbApply"),
                    Value = Model,
                    ValueChanged = this.Callback<string>(OnModelChanged)
                });
            });
        }
    }

    private void BuildNewModel(RenderTreeBuilder builder)
    {
        builder.Markup(@"<pre>说明：
实体：名称|代码|流程类
字段：名称|代码|类型|长度|必填
字段类型：CheckBox,CheckList,Date,Number,RadioList,Select,Text,TextArea
示例：
测试|KmTest
文本|Field1|Text|50|Y
数值|Field2|Number|18,5
日期|Field3|Date</pre>");
        UI.BuildTextArea(builder, new InputModel<string>
        {
            Disabled = ReadOnly || !IsNew,
            Rows = 10,
            Value = Model,
            ValueChanged = this.Callback<string>(OnModelChanged)
        });
    }

    private void OnTypeChanged(string type) => addType = type;

    private async void OnModelChanged(string model)
    {
        Model = model;
        entity = GetEntity(model);
        await view?.SetModelAsync(entity);
        OnChanged?.Invoke(model);
    }

    private static EntityInfo GetEntity(string model)
    {
        var info = new EntityInfo();
        if (string.IsNullOrWhiteSpace(model))
            return info;

        var lines = model.Split(Environment.NewLine.ToArray())
                         .Where(s => !string.IsNullOrWhiteSpace(s))
                         .ToArray();

        if (lines.Length > 0)
        {
            var values = lines[0].Split('|');
            if (values.Length > 0)
                info.Name = values[0];
            if (values.Length > 1)
                info.Id = values[1];
            if (values.Length > 2)
                info.IsFlow = values[2] == "Y";
        }

        if (lines.Length > 1)
        {
            for (int i = 1; i < lines.Length; i++)
            {
                var field = new FieldInfo();
                var values = lines[i].Split('|');
                if (values.Length > 0)
                    field.Name = values[0];
                if (values.Length > 1)
                    field.Id = values[1];
                if (values.Length > 2)
                    field.Type = values[2];
                if (values.Length > 3)
                    field.Length = values[3];
                if (values.Length > 4)
                    field.Required = values[4] == "Y";

                if (field.Type == "CheckBox")
                {
                    field.Length = "50";
                    field.Required = true;
                }

                info.Fields.Add(field);
            }
        }

        return info;
    }
}