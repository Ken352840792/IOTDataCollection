using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using IoTDataCollection.Collector.Core.Interfaces;
using IoTDataCollection.Collector.Core.Models;
using IoTDataCollection.Collector.Protocols;
using IoTDataCollection.Collector.Protocols.PLC;
using Xunit;

namespace IoTDataCollection.Collector.Tests.Protocols;

public class ProtocolFactoryTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ILogger<ProtocolFactory>> _loggerMock;
    private readonly Mock<PlcProtocolAdapter> _plcProtocolMock;
    private readonly ProtocolFactory _factory;

    public ProtocolFactoryTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _loggerMock = new Mock<ILogger<ProtocolFactory>>();
        _plcProtocolMock = new Mock<PlcProtocolAdapter>(_loggerMock.Object);
        
        _factory = new ProtocolFactory(_serviceProviderMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void CreateProtocol_With_PLC_DeviceType_Should_Return_PlcProtocolAdapter()
    {
        // Arrange
        _serviceProviderMock.Setup(x => x.GetService(typeof(PlcProtocolAdapter)))
            .Returns(_plcProtocolMock.Object);

        // Act
        var result = _factory.CreateProtocol(DeviceType.PLC);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PlcProtocolAdapter>();
    }

    [Fact]
    public void CreateProtocol_With_DeviceConfig_Should_Return_Correct_Protocol()
    {
        // Arrange
        var deviceConfig = new DeviceConfig
        {
            DeviceCode = "TEST001",
            DeviceName = "Test Device",
            DeviceType = DeviceType.PLC
        };

        _serviceProviderMock.Setup(x => x.GetService(typeof(PlcProtocolAdapter)))
            .Returns(_plcProtocolMock.Object);

        // Act
        var result = _factory.CreateProtocol(deviceConfig);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PlcProtocolAdapter>();
    }

    [Fact]
    public void CreateProtocol_With_Unsupported_DeviceType_Should_Throw_Exception()
    {
        // Act & Assert
        var action = () => _factory.CreateProtocol(DeviceType.Modbus);
        action.Should().Throw<NotSupportedException>()
            .WithMessage("*Modbus*暂不支持*");
    }

    [Fact]
    public void CreateProtocol_With_Invalid_DeviceType_Should_Throw_Exception()
    {
        // Act & Assert
        var action = () => _factory.CreateProtocol((DeviceType)999);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*未知的设备类型*");
    }
} 