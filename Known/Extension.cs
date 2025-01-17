﻿namespace Known;

public static class Extension
{
    public static void AddKnown(this IServiceCollection services, Action<AppInfo> action = null)
    {
        Config.StartTime = DateTime.Now;
        Logger.Level = LogLevel.Info;
        action?.Invoke(Config.App);

        if (Config.App.Type == AppType.WebApi)
            return;

        Config.AddApp();
        services.AddScoped<Context>();
        services.AddScoped<JSService>();
        services.AddSingleton<ICodeGenerator, CodeGenerator>();

        var content = Utils.GetResource(typeof(Extension).Assembly, "IconFA");
        if (!string.IsNullOrWhiteSpace(content))
        {
            var lines = content.Split([.. Environment.NewLine]);
            UIConfig.Icons["FontAwesome"] = lines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => $"fa fa-{l}").ToList();
        }

        UIConfig.FillHeightScript = @"
var page = $('.kui-page').outerHeight(true) || 0;
var tab = $('.kui-page > .ant-tabs > .ant-tabs-nav').outerHeight(true) || 0;
var tabs = $('.kui-page .ant-tabs-nav').outerHeight(true) || 0;
var query = $('.kui-page .kui-query').outerHeight(true) || 0;
var toolbar = $('.kui-table .kui-toolbar').outerHeight(true) || 0;
var tableHead = $('.kui-table .ant-table-header').outerHeight(true) || 0;
var pagination = $('.kui-table .ant-table-pagination').outerHeight(true) || 0;
//console.log('page='+page+',tab='+tab+',tabs='+tabs+',query='+query+',toolbar='+toolbar+',tableHead='+tableHead+',pagination='+pagination);
var cardDiff = tab > 0 ? 40 : 10;
var cardHeight = page-tab-tabs-cardDiff;
var tableDiff = tab > 0 ? 60 : 20;
var tableHeight = page-tab-query-tabs-toolbar-tableHead-pagination-tableDiff;
$('.kui-card .ant-tabs-content-holder').css('height', cardHeight+'px');
$('.kui-table .ant-table-body').not('.form-list .ant-table-body').css('height', tableHeight+'px');";
    }

    public static void AddKnownCore(this IServiceCollection services, Action<AppInfo> action = null)
    {
        action?.Invoke(Config.App);
        DBUtils.RegisterConnections();
        FileLogger.Start();

        if (Config.App.Type == AppType.WebApi)
            return;

        Config.Version?.LoadBuildTime();
        Config.CoreAssemblies.Add(typeof(Extension).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAutoService, AutoService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IDictionaryService, DictionaryService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IFlowService, FlowService>();
        services.AddScoped<ISystemService, SystemService>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<IModuleService, ModuleService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWeixinService, WeixinService>();
    }

    public static void AddKnownClient(this IServiceCollection services, Action<ClientInfo> action = null)
    {
        Config.IsClient = true;
        var info = new ClientInfo();
        action?.Invoke(info);

        foreach (var type in Config.ApiTypes)
        {
            //Console.WriteLine(type.Name);
            var interceptorType = info.InterceptorType?.Invoke(type);
            services.AddScoped(interceptorType);
            services.AddScoped(type, provider =>
            {
                var interceptor = provider.GetRequiredService(interceptorType);
                return info.InterceptorProvider?.Invoke(type, interceptor);
            });
        }
    }
}