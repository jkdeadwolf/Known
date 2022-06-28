﻿namespace KApp;

public class LayoutOption
{
    public string Style { get; set; }
    public string Title { get; set; }
    public bool ShowBack { get; set; }
    public bool ShowTopbar { get; set; }
    public bool ShowTabbar { get; set; }

    public static LayoutOption Main(string title, string style = "", bool showTopbar = true)
    {
        return new LayoutOption
        {
            Style = style,
            Title = title,
            ShowTabbar = true,
            ShowTopbar = showTopbar
        };
    }

    public static LayoutOption Sub(string title, string style = "")
    {
        return new LayoutOption
        {
            Style = style,
            Title = title,
            ShowTabbar = false,
            ShowTopbar = true,
            ShowBack = true
        };
    }
}
