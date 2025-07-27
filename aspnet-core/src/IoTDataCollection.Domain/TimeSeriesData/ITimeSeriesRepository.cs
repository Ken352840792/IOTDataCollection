using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IoTDataCollection.TimeSeriesData;

/// <summary>
/// 时序数据仓储接口
/// </summary>
public interface ITimeSeriesRepository
{
    #region 设备数据点操作

    /// <summary>
    /// 写入单个设备数据点
    /// </summary>
    /// <param name="dataPoint">数据点</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task WriteDeviceDataPointAsync(DeviceDataPoint dataPoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量写入设备数据点
    /// </summary>
    /// <param name="dataPoints">数据点列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task WriteDeviceDataPointsAsync(IEnumerable<DeviceDataPoint> dataPoints, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询设备数据点 - 按时间范围
    /// </summary>
    /// <param name="deviceCode">设备编码</param>
    /// <param name="pointCode">数据点编码</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>数据点列表</returns>
    Task<List<DeviceDataPoint>> QueryDeviceDataAsync(
        string deviceCode, 
        string pointCode, 
        DateTime startTime, 
        DateTime endTime, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询设备最新数据点
    /// </summary>
    /// <param name="deviceCode">设备编码</param>
    /// <param name="pointCode">数据点编码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>最新数据点</returns>
    Task<DeviceDataPoint?> QueryLatestDeviceDataAsync(
        string deviceCode, 
        string pointCode, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询站点下所有设备的最新数据
    /// </summary>
    /// <param name="siteCode">站点编码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>最新数据点列表</returns>
    Task<List<DeviceDataPoint>> QueryLatestSiteDataAsync(
        string siteCode, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询设备数据统计信息
    /// </summary>
    /// <param name="deviceCode">设备编码</param>
    /// <param name="pointCode">数据点编码</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>统计结果</returns>
    Task<DeviceDataStatistics> QueryDeviceDataStatisticsAsync(
        string deviceCode, 
        string pointCode, 
        DateTime startTime, 
        DateTime endTime, 
        CancellationToken cancellationToken = default);

    #endregion

    #region 系统监控数据操作

    /// <summary>
    /// 写入系统监控数据
    /// </summary>
    /// <param name="monitoringData">监控数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task WriteSystemMonitoringDataAsync(SystemMonitoringData monitoringData, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量写入系统监控数据
    /// </summary>
    /// <param name="monitoringDataList">监控数据列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task WriteSystemMonitoringDataAsync(IEnumerable<SystemMonitoringData> monitoringDataList, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询系统监控数据 - 按时间范围
    /// </summary>
    /// <param name="nodeCode">节点编码</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>监控数据列表</returns>
    Task<List<SystemMonitoringData>> QuerySystemMonitoringDataAsync(
        string nodeCode, 
        DateTime startTime, 
        DateTime endTime, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询节点最新监控数据
    /// </summary>
    /// <param name="nodeCode">节点编码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>最新监控数据</returns>
    Task<SystemMonitoringData?> QueryLatestSystemMonitoringDataAsync(
        string nodeCode, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询所有节点最新监控数据
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>所有节点最新监控数据</returns>
    Task<List<SystemMonitoringData>> QueryAllNodesLatestMonitoringDataAsync(
        CancellationToken cancellationToken = default);

    #endregion

    #region 数据库管理

    /// <summary>
    /// 测试连接
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>连接是否成功</returns>
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建数据库和Bucket（如果不存在）
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task EnsureDatabaseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除过期数据
    /// </summary>
    /// <param name="retentionDays">保留天数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task DeleteExpiredDataAsync(int retentionDays, CancellationToken cancellationToken = default);

    #endregion
}

/// <summary>
/// 设备数据统计结果
/// </summary>
public class DeviceDataStatistics
{
    /// <summary>
    /// 数据点数量
    /// </summary>
    public long Count { get; set; }

    /// <summary>
    /// 最小值
    /// </summary>
    public double? MinValue { get; set; }

    /// <summary>
    /// 最大值
    /// </summary>
    public double? MaxValue { get; set; }

    /// <summary>
    /// 平均值
    /// </summary>
    public double? AverageValue { get; set; }

    /// <summary>
    /// 第一个值的时间戳
    /// </summary>
    public DateTime? FirstTime { get; set; }

    /// <summary>
    /// 最后一个值的时间戳
    /// </summary>
    public DateTime? LastTime { get; set; }
} 