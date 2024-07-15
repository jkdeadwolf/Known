﻿namespace Known.Services;

public interface IAuthService : IService
{
    [AllowAnonymous] Task<Result> SignInAsync(LoginFormInfo info);
    Task<Result> SignOutAsync(string token);
    Task<UserInfo> GetUserAsync(string userName);
    Task<AdminInfo> GetAdminAsync();
    Task<Result> UpdatePasswordAsync(PwdFormInfo info);
}

class AuthService(Context context) : ServiceBase(context), IAuthService
{
    //Account
    public async Task<Result> SignInAsync(LoginFormInfo info)
    {
        var userName = info.UserName.ToLower();
        var entity = await UserRepository.GetUserAsync(Database, userName, info.Password);
        if (entity == null)
            return Result.Error(Language["Tip.LoginNoNamePwd"]);

        if (!entity.Enabled)
            return Result.Error(Language["Tip.LoginDisabled"]);

        if (!entity.FirstLoginTime.HasValue)
        {
            entity.FirstLoginTime = DateTime.Now;
            entity.FirstLoginIP = info.IPAddress;
        }
        entity.LastLoginTime = DateTime.Now;
        entity.LastLoginIP = info.IPAddress;

        var user = Utils.MapTo<UserInfo>(entity);
        user.Token = Utils.GetGuid();
        user.Station = info.Station;
        await SetUserInfoAsync(Database, user);
        await SetUserWeixinAsync(Database, user);
        cachedUsers[user.Token] = user;

        var type = LogType.Login.ToString();
        if (info.IsMobile)
            type = "APP" + type;

        var database = Database;
        database.User = user;
        return await database.TransactionAsync(Language["Login"], async db =>
        {
            await db.SaveAsync(entity);
            await Logger.AddLogAsync(db, type, $"{user.UserName}-{user.Name}", $"IP：{user.LastLoginIP}");
        }, user);
    }

    public async Task<Result> SignOutAsync(string token)
    {
        var user = CurrentUser;
        if (string.IsNullOrWhiteSpace(token))
            token = user?.Token;

        if (!string.IsNullOrWhiteSpace(token))
            cachedUsers.TryRemove(token, out UserInfo _);

        if (user != null)
            await Logger.AddLogAsync(Database, LogType.Logout.ToString(), $"{user.UserName}-{user.Name}", $"token: {token}");
        return Result.Success(Language["Tip.ExitSuccess"]);
    }

    private static readonly ConcurrentDictionary<string, UserInfo> cachedUsers = new();

    internal static UserInfo GetUserByToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        return cachedUsers.Values.FirstOrDefault(u => u.Token == token);
    }

    internal static async Task<UserInfo> GetUserAsync(Database db, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return null;

        userName = userName.Split('-')[0];
        var user = cachedUsers.Values.FirstOrDefault(u => u.UserName == userName);
        if (user != null)
            return user;

        user = await UserRepository.GetUserInfoAsync(db, userName);
        await SetUserInfoAsync(db, user);
        return user;
    }

    public Task<UserInfo> GetUserAsync(string userName) => GetUserAsync(Database, userName);

    public async Task<AdminInfo> GetAdminAsync()
    {
        if (CurrentUser == null)
            return new AdminInfo();

        await Database.OpenAsync();
        var modules = await Database.QueryListAsync<SysModule>();
        DataHelper.Initialize(modules);
        var admin = new AdminInfo
        {
            AppName = await UserHelper.GetSystemNameAsync(Database),
            MessageCount = await UserRepository.GetMessageCountAsync(Database),
            UserMenus = await UserHelper.GetUserMenusAsync(Database),
            UserSetting = await SettingService.GetUserSettingAsync<SettingInfo>(Database, SettingInfo.KeyInfo),
            Codes = await DictionaryService.GetDictionariesAsync(Database)
        };
        await Database.CloseAsync();
        return admin;
    }

    public async Task<Result> UpdatePasswordAsync(PwdFormInfo info)
    {
        var user = CurrentUser;
        if (user == null)
            return Result.Error(Language["Tip.NoLogin"]);

        var errors = new List<string>();
        if (string.IsNullOrEmpty(info.OldPwd))
            errors.Add(Language["Tip.CurPwdRequired"]);
        if (string.IsNullOrEmpty(info.NewPwd))
            errors.Add(Language["Tip.NewPwdRequired"]);
        if (string.IsNullOrEmpty(info.NewPwd1))
            errors.Add(Language["Tip.ConPwdRequired"]);
        if (info.NewPwd != info.NewPwd1)
            errors.Add(Language["Tip.PwdNotEqual"]);

        if (errors.Count > 0)
            return Result.Error(string.Join(Environment.NewLine, errors.ToArray()));

        var entity = await UserRepository.GetUserAsync(Database, user.UserName, info.OldPwd);
        if (entity == null)
            return Result.Error(Language["Tip.CurPwdInvalid"]);

        entity.Password = Utils.ToMd5(info.NewPwd);
        await Database.SaveAsync(entity);
        return Result.Success(Language.Success(Language["Button.Update"]), entity.Id);
    }

    private static async Task SetUserInfoAsync(Database db, UserInfo user)
    {
        var sys = await SystemService.GetConfigAsync<SystemInfo>(db, SystemService.KeySystem);
        user.AvatarUrl = user.Gender == "Female" ? "img/face2.png" : "img/face1.png";
        user.IsTenant = user.CompNo != sys.CompNo;
        user.AppName = Config.App.Name;
        if (user.IsAdmin)
            user.AppId = Config.App.Id;

        var info = await SystemService.GetSystemAsync(db);
        user.AppName = info.AppName;
        user.CompName = info.CompName;
        if (!string.IsNullOrEmpty(user.OrgNo))
        {
            var orgName = await UserRepository.GetOrgNameAsync(db, user.AppId, user.CompNo, user.OrgNo);
            if (string.IsNullOrEmpty(orgName))
                orgName = user.CompName;
            user.OrgName = orgName;
            if (string.IsNullOrEmpty(user.CompName))
                user.CompName = orgName;
        }
    }

    private static async Task SetUserWeixinAsync(Database db, UserInfo user)
    {
        var weixin = await WeixinRepository.GetWeixinByUserIdAsync(db, user.Id);
        if (weixin == null)
            return;

        user.OpenId = weixin.OpenId;
        user.AvatarUrl = weixin.HeadImgUrl;
    }
}