using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jint;
using Jint.Native;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using IoTDataCollection.Collector.Core.Interfaces;
using IoTDataCollection.Collector.Core.Models;

namespace IoTDataCollection.Collector.Rules;

/// <summary>
/// Jint JavaScript规则引擎
/// </summary>
public class JintRuleEngine : IRuleEngine
{
    private readonly ILogger<JintRuleEngine> _logger;
    private readonly Dictionary<string, Engine> _ruleEngines;
    private readonly Dictionary<string, RuleInfo> _rules;

    public JintRuleEngine(ILogger<JintRuleEngine> logger)
    {
        _logger = logger;
        _ruleEngines = new Dictionary<string, Engine>();
        _rules = new Dictionary<string, RuleInfo>();
    }

    /// <summary>
    /// 执行规则
    /// </summary>
    public async Task<RuleResult> ExecuteRuleAsync(string ruleCode, List<DataPoint> data, CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = new RuleResult();

        try
        {
            _logger.LogDebug("开始执行规则，数据点数量: {Count}", data.Count);

            // 创建JavaScript引擎
            var engine = new Engine(cfg => cfg
                .LimitMemory(4_000_000) // 4MB内存限制
                .LimitRecursion(100)    // 递归深度限制
                .TimeoutInterval(TimeSpan.FromSeconds(30)) // 30秒超时
            );

            // 注入全局变量和函数
            InjectGlobalObjects(engine, data);

            // 执行规则代码
            var jsResult = engine.Evaluate(ruleCode);

            // 获取输出数据
            result.OutputData = ExtractOutputData(engine, data);
            result.Success = true;

            stopwatch.Stop();
            result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;

            _logger.LogDebug("规则执行成功，输出数据点数量: {Count}, 执行时间: {Time}ms", 
                result.OutputData.Count, result.ExecutionTimeMs);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.Success = false;
            result.ErrorMessage = ex.Message;
            result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;

            _logger.LogError(ex, "规则执行失败，执行时间: {Time}ms", result.ExecutionTimeMs);
            return result;
        }
    }

    /// <summary>
    /// 验证规则
    /// </summary>
    public async Task<bool> ValidateRuleAsync(string ruleCode, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("开始验证规则代码");

            // 创建临时引擎进行语法检查
            var engine = new Engine(cfg => cfg
                .LimitMemory(1_000_000)
                .LimitRecursion(10)
                .TimeoutInterval(TimeSpan.FromSeconds(5))
            );

            // 注入基本对象
            engine.SetValue("input", new List<DataPoint>());
            engine.SetValue("output", new List<DataPoint>());
            engine.SetValue("console", new ConsoleProxy(_logger));

            // 尝试解析代码
            engine.Evaluate(ruleCode);

            _logger.LogDebug("规则代码验证成功");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "规则代码验证失败");
            return false;
        }
    }

    /// <summary>
    /// 更新规则
    /// </summary>
    public async Task<bool> UpdateRuleAsync(string ruleId, string ruleCode, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("更新规则: {RuleId}", ruleId);

            // 验证规则代码
            if (!await ValidateRuleAsync(ruleCode, cancellationToken))
            {
                return false;
            }

            // 更新规则信息
            if (_rules.ContainsKey(ruleId))
            {
                _rules[ruleId].Code = ruleCode;
                _rules[ruleId].UpdatedAt = DateTime.UtcNow;
                _rules[ruleId].Version++;
            }

            // 清除缓存的引擎
            _ruleEngines.Remove(ruleId);

            _logger.LogInformation("规则更新成功: {RuleId}", ruleId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "规则更新失败: {RuleId}", ruleId);
            return false;
        }
    }

    /// <summary>
    /// 获取规则列表
    /// </summary>
    public async Task<List<RuleInfo>> GetRulesAsync(CancellationToken cancellationToken = default)
    {
        return _rules.Values.ToList();
    }

    /// <summary>
    /// 删除规则
    /// </summary>
    public async Task<bool> DeleteRuleAsync(string ruleId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("删除规则: {RuleId}", ruleId);

            _rules.Remove(ruleId);
            _ruleEngines.Remove(ruleId);

            _logger.LogInformation("规则删除成功: {RuleId}", ruleId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "规则删除失败: {RuleId}", ruleId);
            return false;
        }
    }

    /// <summary>
    /// 注入全局对象
    /// </summary>
    private void InjectGlobalObjects(Engine engine, List<DataPoint> data)
    {
        // 注入输入数据
        engine.SetValue("input", data);

        // 注入输出数据列表
        var outputData = new List<DataPoint>();
        engine.SetValue("output", outputData);

        // 注入控制台对象
        engine.SetValue("console", new ConsoleProxy(_logger));

        // 注入工具函数
        engine.SetValue("utils", new UtilsProxy());

        // 注入数据点创建函数
        engine.SetValue("createDataPoint", new Func<string, string, string, DataType, object, string?, DataPoint>((deviceCode, pointCode, pointName, dataType, value, unit) =>
        {
            return new DataPoint
            {
                Id = Guid.NewGuid().ToString(),
                DeviceCode = deviceCode,
                PointCode = pointCode,
                PointName = pointName,
                Address = string.Empty,
                DataType = dataType,
                Unit = unit,
                Timestamp = DateTime.UtcNow,
                Quality = 192
            };
        }));

        // 注入数据类型枚举
        engine.SetValue("DataType", new
        {
            Bool = DataType.Bool,
            Int16 = DataType.Int16,
            Int32 = DataType.Int32,
            Float = DataType.Float,
            String = DataType.String
        });
    }

    /// <summary>
    /// 提取输出数据
    /// </summary>
    private List<DataPoint> ExtractOutputData(Engine engine, List<DataPoint> inputData)
    {
        try
        {
            var outputValue = engine.GetValue("output");
            if (outputValue.IsArray())
            {
                var array = outputValue.AsArray();
                var result = new List<DataPoint>();

                for (int i = 0; i < array.Length; i++)
                {
                    var item = array.Get(i.ToString());
                    if (item.IsObject())
                    {
                        var dataPoint = ConvertJsObjectToDataPoint(item);
                        if (dataPoint != null)
                        {
                            result.Add(dataPoint);
                        }
                    }
                }

                return result;
            }

            return new List<DataPoint>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "提取输出数据失败");
            return new List<DataPoint>();
        }
    }

    /// <summary>
    /// 转换JavaScript对象为数据点
    /// </summary>
    private DataPoint? ConvertJsObjectToDataPoint(JsValue jsObject)
    {
        try
        {
            var dataPoint = new DataPoint
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                Quality = 192
            };

            // 提取属性
            try
            {
                var deviceCode = jsObject.Get("deviceCode");
                if (!deviceCode.IsUndefined())
                    dataPoint.DeviceCode = deviceCode.AsString();
                
                var pointCode = jsObject.Get("pointCode");
                if (!pointCode.IsUndefined())
                    dataPoint.PointCode = pointCode.AsString();
                
                var pointName = jsObject.Get("pointName");
                if (!pointName.IsUndefined())
                    dataPoint.PointName = pointName.AsString();
                
                var address = jsObject.Get("address");
                if (!address.IsUndefined())
                    dataPoint.Address = address.AsString();
                
                var dataType = jsObject.Get("dataType");
                if (!dataType.IsUndefined())
                    dataPoint.DataType = (DataType)dataType.AsNumber();
                
                var unit = jsObject.Get("unit");
                if (!unit.IsUndefined())
                    dataPoint.Unit = unit.AsString();
            }
            catch
            {
                // 忽略属性访问错误
            }

            // 设置值
            try
            {
                var value = jsObject.Get("value");
                if (!value.IsUndefined())
                {
                    switch (dataPoint.DataType)
                    {
                        case DataType.Bool:
                            dataPoint.BooleanValue = value.AsBoolean();
                            break;
                        case DataType.Int16:
                        case DataType.Int32:
                        case DataType.Float:
                            dataPoint.NumericValue = value.AsNumber();
                            break;
                        case DataType.String:
                            dataPoint.StringValue = value.AsString();
                            break;
                    }
                    dataPoint.RawValue = value.ToString();
                }
            }
            catch
            {
                // 忽略值设置错误
            }

            return dataPoint;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "转换JavaScript对象为数据点失败");
            return null;
        }
    }
}

/// <summary>
/// 控制台代理
/// </summary>
public class ConsoleProxy
{
    private readonly ILogger _logger;

    public ConsoleProxy(ILogger logger)
    {
        _logger = logger;
    }

    public void log(object message) => _logger.LogInformation("Rule: {Message}", message);
    public void info(object message) => _logger.LogInformation("Rule: {Message}", message);
    public void warn(object message) => _logger.LogWarning("Rule: {Message}", message);
    public void error(object message) => _logger.LogError("Rule: {Message}", message);
    public void debug(object message) => _logger.LogDebug("Rule: {Message}", message);
}

/// <summary>
/// 工具函数代理
/// </summary>
public class UtilsProxy
{
    public double round(double value, int decimals = 2) => Math.Round(value, decimals);
    public double abs(double value) => Math.Abs(value);
    public double min(params double[] values) => values.Min();
    public double max(params double[] values) => values.Max();
    public double avg(params double[] values) => values.Average();
    public double sum(params double[] values) => values.Sum();
    public string formatNumber(double value, string format = "F2") => value.ToString(format);
    public DateTime now() => DateTime.UtcNow;
    public string toJson(object obj) => JsonConvert.SerializeObject(obj);
} 