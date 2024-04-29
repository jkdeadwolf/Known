﻿namespace Known.Blazor;

public class KLayout : BaseComponent
{
    [Parameter] public BaseLayout Layout { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KError>()
               .Set(c => c.OnError, Layout.OnError)
               .Set(c => c.ChildContent, b => b.Cascading(Layout, ChildContent))
               .Build();
    }
}