﻿using System.Net;
using System.Net.Http.Json;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using Known.Extensions;

namespace Known.Weixins;

static class WeixinApi
{
    private static string AccessToken = "";
    internal static string GZHId { get; set; }
    internal static string AppId { get; set; }
    internal static string AppSecret { get; set; }
    internal static string RedirectUri { get; set; }

    #region 初始化接口
    internal static void Initialize()
    {
        ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;
        Task.Run(async () =>
        {
            while (true)
            {
                if (!string.IsNullOrWhiteSpace(AppId) && !string.IsNullOrWhiteSpace(AppSecret))
                    AccessToken = await GetAccessTokenAsync(AppId, AppSecret);
                Thread.Sleep(7000 * 1000);
            }
        });
    }

    //获取ACCESS_TOKEN，7200秒过期，需要定时刷新才能调用接口
    //{"access_token":"ACCESS_TOKEN","expires_in":7200}
    private static async Task<string> GetAccessTokenAsync(string appId, string appSecret)
    {
        try
        {
            using var http = new HttpClient();
            var url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appId}&secret={appSecret}";
            var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
            return result.GetValue<string>("access_token");
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return null;
        }
    }

    private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
    {
        //为了通过证书验证，总是返回true
        return true;
    }
    #endregion

    #region 账号管理
    //1.创建二维码ticket
    public static async Task<TicketInfo> CreateTicketAsync(this HttpClient http, string sceneId)
    {
        if (string.IsNullOrWhiteSpace(AccessToken))
        {
            Logger.Error("AccessToken is null.");
            return null;
        }

        try
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={AccessToken}";
            var data = new
            {
                action_name = "QR_LIMIT_STR_SCENE",
                action_info = new { scene = new { scene_str = sceneId } }
            };
            var result = await http.PostDataAsync(url, data);
            if (result == null)
                return null;

            return new TicketInfo
            {
                Ticket = result.GetValue<string>("ticket"),
                ExpireSeconds = result.GetValue<int>("expire_seconds"),
                Url = result.GetValue<string>("url")
            };
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return null;
        }
    }

    //2.通过ticket换取二维码
    public static string GetQRCodeUrl(string ticket)
    {
        ticket = HttpUtility.UrlEncode(ticket);
        return $"https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={ticket}";
    }
    #endregion

    #region 网页授权
    //1.用户同意授权，获取code
    //  同意返回redirect_uri/?code=CODE&state=STATE
    public static string GetAuthorizeUrl(string state, string scope = "snsapi_userinfo")
    {
        var redirectUri = HttpUtility.UrlEncode(RedirectUri);
        state = HttpUtility.UrlEncode(state);
        return $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={AppId}&redirect_uri={redirectUri}&response_type=code&scope={scope}&state={state}#wechat_redirect";
    }

    //2.通过code换取网页授权access_token
    public static async Task<AuthorizeToken> GetAuthorizeTokenAsync(this HttpClient http, string code)
    {
        try
        {
            var url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={AppId}&secret={AppSecret}&code={code}&grant_type=authorization_code";
            var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
            return new AuthorizeToken
            {
                AccessToken = result.GetValue<string>("access_token"),
                ExpiresIn = result.GetValue<int>("expires_in"),
                RefreshToken = result.GetValue<string>("refresh_token"),
                OpenId = result.GetValue<string>("openid"),
                Scope = result.GetValue<string>("scope"),
                IsSnapshotUser = result.GetValue<int>("is_snapshotuser"),
                UnionId = result.GetValue<string>("unionid")
            };
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return null;
        }
    }

    //3.刷新access_token（如果需要）
    public static async Task<AuthorizeRefreshToken> GetAuthorizeRefreshTokenAsync(this HttpClient http, string refreshToken)
    {
        try
        {
            var url = $"https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={AppId}&grant_type=refresh_token&refresh_token={refreshToken}";
            var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
            return new AuthorizeRefreshToken
            {
                AccessToken = result.GetValue<string>("access_token"),
                ExpiresIn = result.GetValue<int>("expires_in"),
                RefreshToken = result.GetValue<string>("refresh_token"),
                OpenId = result.GetValue<string>("openid"),
                Scope = result.GetValue<string>("scope")
            };
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return null;
        }
    }

    //4.拉取用户信息(需scope为 snsapi_userinfo)
    public static async Task<SysWeixin> GetUserInfoAsync(this HttpClient http, string accessToken, string openId)
    {
        try
        {
            var url = $"https://api.weixin.qq.com/sns/userinfo?access_token={accessToken}&openid={openId}&lang=zh_CN";
            var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
            var privileges = result.GetValue<List<string>>("privilege");
            var info = new SysWeixin
            {
                OpenId = result.GetValue<string>("openid"),
                NickName = result.GetValue<string>("nickname"),
                Sex = result.GetValue<string>("sex"),
                Province = result.GetValue<string>("province"),
                City = result.GetValue<string>("city"),
                Country = result.GetValue<string>("country"),
                HeadImgUrl = result.GetValue<string>("headimgurl"),
                UnionId = result.GetValue<string>("unionid")
            };
            if (privileges != null)
                info.Privilege = string.Join(",", privileges);
            return info;
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return null;
        }
    }

    //附：检验授权凭证（access_token）是否有效
    //正确返回：{ "errcode":0,"errmsg":"ok"}
    //错误返回：{ "errcode":40003,"errmsg":"invalid openid"}
    public static async Task<Result> CheckAccessTokenAsync(this HttpClient http, string accessToken, string openId)
    {
        try
        {
            var url = $"https://api.weixin.qq.com/sns/auth?access_token={accessToken}&openid={openId}";
            var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
            var errCode = result.GetValue<int>("errcode");
            var errMsg = result.GetValue<string>("errmsg");
            if (errCode != 0)
                return Result.Error(errMsg);

            return Result.Success(errMsg);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return null;
        }
    }
    #endregion

    #region 模板消息
    //发送模板消息
    public static async Task<Result> SendTemplateMessageAsync(this HttpClient http, TemplateInfo info)
    {
        if (string.IsNullOrWhiteSpace(AccessToken))
            return Result.Error("AccessToken is null.");

        try
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={AccessToken}";
            var result = await http.PostDataAsync(url, info);
            var errCode = result?.GetValue<int>("errcode");
            var errMsg = result?.GetValue<string>("errmsg");
            if (errCode != 0)
                return Result.Error(errMsg);

            return Result.Success(errMsg);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            Logger.Error(Utils.ToJson(info));
            return Result.Error(ex.Message);
        }
    }
    #endregion

    private static async Task<Dictionary<string, object>> PostDataAsync(this HttpClient http, string url, object data)
    {
        var json = Utils.ToJson(data);
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await http.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        var result = Utils.FromJson<Dictionary<string, object>>(content);
        if (result == null)
            Logger.Error($"PostData={content}");
        return result;
    }
}