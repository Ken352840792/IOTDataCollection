using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace IoTDataCollection.Collector.Communication;

/// <summary>
/// HTTP客户端服务
/// </summary>
public class HttpClientService : IDisposable
{
    private readonly ILogger<HttpClientService> _logger;
    private readonly HttpClient _httpClient;
    private readonly HttpConfiguration _config;

    public HttpClientService(
        ILogger<HttpClientService> logger,
        IOptions<HttpConfiguration> config)
    {
        _logger = logger;
        _config = config.Value;
        
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_config.BaseUrl),
            Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds)
        };

        // 设置默认请求头
        if (!string.IsNullOrEmpty(_config.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _config.ApiKey);
        }

        if (!string.IsNullOrEmpty(_config.Authorization))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", _config.Authorization);
        }
    }

    /// <summary>
    /// 发送GET请求
    /// </summary>
    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("发送GET请求: {Endpoint}", endpoint);

            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonConvert.DeserializeObject<T>(content);
                
                _logger.LogDebug("GET请求成功: {Endpoint}", endpoint);
                return result;
            }
            else
            {
                _logger.LogError("GET请求失败: {Endpoint}, 状态码: {StatusCode}", endpoint, response.StatusCode);
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET请求异常: {Endpoint}", endpoint);
            return default;
        }
    }

    /// <summary>
    /// 发送POST请求
    /// </summary>
    public async Task<T?> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("发送POST请求: {Endpoint}", endpoint);

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonConvert.DeserializeObject<T>(responseContent);
                
                _logger.LogDebug("POST请求成功: {Endpoint}", endpoint);
                return result;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("POST请求失败: {Endpoint}, 状态码: {StatusCode}, 错误: {Error}", 
                    endpoint, response.StatusCode, errorContent);
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST请求异常: {Endpoint}", endpoint);
            return default;
        }
    }

    /// <summary>
    /// 发送PUT请求
    /// </summary>
    public async Task<bool> PutAsync(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("发送PUT请求: {Endpoint}", endpoint);

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("PUT请求成功: {Endpoint}", endpoint);
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("PUT请求失败: {Endpoint}, 状态码: {StatusCode}, 错误: {Error}", 
                    endpoint, response.StatusCode, errorContent);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PUT请求异常: {Endpoint}", endpoint);
            return false;
        }
    }

    /// <summary>
    /// 发送DELETE请求
    /// </summary>
    public async Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("发送DELETE请求: {Endpoint}", endpoint);

            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("DELETE请求成功: {Endpoint}", endpoint);
                return true;
            }
            else
            {
                _logger.LogError("DELETE请求失败: {Endpoint}, 状态码: {StatusCode}", endpoint, response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DELETE请求异常: {Endpoint}", endpoint);
            return false;
        }
    }

    /// <summary>
    /// 测试连接
    /// </summary>
    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("测试HTTP连接: {BaseUrl}", _config.BaseUrl);

            var response = await _httpClient.GetAsync("api/health", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("HTTP连接测试成功");
                return true;
            }
            else
            {
                _logger.LogWarning("HTTP连接测试失败，状态码: {StatusCode}", response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HTTP连接测试异常");
            return false;
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

/// <summary>
/// HTTP配置
/// </summary>
public class HttpConfiguration
{
    /// <summary>
    /// 基础URL
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:44330";

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// API密钥
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// 授权头
    /// </summary>
    public string? Authorization { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// 重试间隔（秒）
    /// </summary>
    public int RetryIntervalSeconds { get; set; } = 5;
} 