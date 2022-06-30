﻿/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-06-28     KnownChen    初始化
 * ------------------------------------------------------------------------------- */

using Known.Razor;

namespace KApp;

public class BasePage : AppComponent
{
    //[Inject] protected DataService Service { get; set; }

    protected bool IsAdmin
    {
        get { return Client.CheckIsAdmin(CurrentUser); }
    }
}