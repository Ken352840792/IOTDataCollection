using System;
using System.Collections.Generic;

namespace IoTDataCollection.Devices;

/// <summary>
/// 设备调试请求DTO
/// </summary>
public class DeviceDebugRequestDto
{
    /// <summary>
    /// 设备ID
    /// </summary>
    public Guid DeviceId { get; set; }

    /// <summary>
    /// 调试操作类型 - TestConnection: 测试连接, ReadData: 读取数据, WriteData: 写入数据
    /// </summary>
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// 数据点地址（用于读取/写入操作）
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 写入值（用于写入操作）
    /// </summary>
    public string? WriteValue { get; set; }

    /// <summary>
    /// 数据类型（用于读取/写入操作）- Int16, Int32, Float, Boolean, String
    /// </summary>
    public string? DataType { get; set; }

    /// <summary>
    /// 超时时间（毫秒）
    /// </summary>
    public int Timeout { get; set; } = 5000;
}

/// <summary>
/// 设备调试响应DTO
/// </summary>
public class DeviceDebugResponseDto
{
    /// <summary>
    /// 调试是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// 执行时间（毫秒）
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// 读取的数据值
    /// </summary>
    public string? ReadValue { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 调试详细信息
    /// </summary>
    public List<string> DebugDetails { get; set; } = new();

    /// <summary>
    /// 调试时间
    /// </summary>
    public DateTime DebugTime { get; set; } = DateTime.Now;
}

/// <summary>
/// 设备连接状态DTO
/// </summary>
public class DeviceConnectionStatusDto
{
    /// <summary>
    /// 设备ID
    /// </summary>
    public Guid DeviceId { get; set; }

    /// <summary>
    /// 连接状态 - 0:离线 1:在线 2:故障
    /// </summary>
    public int ConnectionStatus { get; set; }

    /// <summary>
    /// 最后通信时间
    /// </summary>
    public DateTime? LastCommunicationTime { get; set; }

    /// <summary>
    /// 连接状态文本
    /// </summary>
    public string ConnectionStatusText => ConnectionStatus switch
    {
        0 => "离线",
        1 => "在线",
        2 => "故障",
        _ => "未知"
    };
} 