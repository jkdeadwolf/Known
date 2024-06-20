﻿namespace Known;

public abstract class HttpInterceptor<T>(IServiceProvider provider) where T : class
{
    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

    protected IServiceProvider ServiceProvider { get; } = provider;
    protected abstract HttpClient CreateClient();

    protected async Task<object> SendAsync(MethodInfo method, object[] arguments)
    {
        using var client = CreateClient();
        var request = await CreateRequestMessageAsync(method, arguments);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        if (stream == null || stream.Length == 0)
            return default;

        return await JsonSerializer.DeserializeAsync(stream, method.ReturnType, jsonOptions);
    }

    protected async Task<TResult> SendAsync<TResult>(MethodInfo method, object[] arguments)
    {
        using var client = CreateClient();
        var request = await CreateRequestMessageAsync(method, arguments);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        if (stream == null || stream.Length == 0)
            return default;

        return await JsonSerializer.DeserializeAsync<TResult>(stream, jsonOptions);
    }

    private async Task<HttpRequestMessage> CreateRequestMessageAsync(MethodInfo method, object[] arguments)
    {
        var request = new HttpRequestMessage();
        var info = Config.ApiMethods.FirstOrDefault(m => m.Id == $"{typeof(T).Name}.{method.Name}");
        if (info == null)
            return request;

        request.Method = info.HttpMethod;

        var auth = ServiceProvider.GetRequiredService<IAuthStateProvider>();
        var user = await auth.GetUserAsync();
        request.Headers.Add(Constants.KeyToken, user?.Token);
        
        var queries = new List<string>();
        foreach (var param in info.Parameters)
        {
            var value = arguments[param.Position] ?? default;
            if (info.HttpMethod == HttpMethod.Get)
            {
                queries.Add($"{param.Name}={value}");
            }
            else
            {
                if (param.ParameterType.IsClass)
                {
                    var json = JsonSerializer.Serialize(value);
                    request.Content = new StringContent(json, Encoding.Default, "application/json");
                }
            }
        }

        var url = info.Route;
        if (queries.Count > 0)
            url += $"?{string.Join('&', queries)}";
        request.RequestUri = new(url, UriKind.Relative);
        return request;
    }
}