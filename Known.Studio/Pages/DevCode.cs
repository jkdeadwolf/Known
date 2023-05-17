﻿using Known.Studio.Services;

namespace Known.Studio.Pages;

class DevCode : BasePage
{
    private string domain;
    private string curItem = "SQL";
    private string codeString;

    protected override async void OnAfterRender(bool firstRender)
    {
        if (firstRender)
            await SetCode(curItem);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildLeft(builder);
        BuildRight(builder);
    }

    private void BuildLeft(RenderTreeBuilder builder)
    {
        builder.Div("dc-left", attr =>
        {
            builder.Pre("tips", @"说明：
工程：名称|表前缀
实体：名称|代码
字段：名称|代码|类型|长度|必填|查询
字段类型：CheckBox,CheckList,Date,Number,RadioList,Select,Text,TextArea</pre>
<pre class=""demo"">示例：
Demo|Dm
测试|Test
文本|Field1|Text|50|Y|Y
日期|Field2|Date");
            builder.Field<TextArea>("领域模型", "Domain").ValueChanged(v => domain = v)
                   .Set(f => f.Height, 320)
                   .Build();
            builder.Div("dc-btn", attr =>
            {
                builder.Button("生成", "fa fa-download", Callback(OnGenerate));
            });
        });
    }

    private void BuildRight(RenderTreeBuilder builder)
    {
        builder.Div("dc-right", attr =>
        {
            builder.Component<Tab>()
                   .Set(c => c.Codes, "SQL,Entity,Service,ListCS,FormCS")
                   .Set(c => c.CurItem, curItem)
                   .Set(c => c.OnChanged, OnTabChanged)
                   .Build();
            builder.Element("pre", attr => attr.Id("code").Class("code prettyprint source linenums"));
        });
    }

    private async void OnGenerate()
    {
        codeString = CodeService.GetCode(curItem, domain);
        await JS.InvokeAsync<string>("printCode", new object[] { codeString });
    }

    private async void OnTabChanged(MenuItem item)
    {
        await SetCode(item.Code);
    }

    private async Task SetCode(string code)
    {
        curItem = code;
        codeString = CodeService.GetCode(code, domain);
        await JS.InvokeAsync<string>("printCode", new object[] { codeString });
    }
}