﻿namespace Known.Razor.Pages;

public interface IMyFlow
{
    UIService UI { get; }
    SysFlow Flow { get; set; }
    void Refresh();
}