﻿using AntDesign;
using Microsoft.AspNetCore.Components;

namespace Known.AntBlazor.Components;

class AntCheckboxGroup : CheckboxGroup
{
    [Parameter] public List<CodeInfo> Codes { get; set; }

    protected override void OnInitialized()
    {
        Options = Codes.ToCheckboxOptions();
        base.OnInitialized();
    }
}