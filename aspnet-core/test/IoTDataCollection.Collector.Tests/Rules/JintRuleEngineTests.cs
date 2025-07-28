using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using IoTDataCollection.Collector.Core.Models;
using IoTDataCollection.Collector.Rules;
using Xunit;

namespace IoTDataCollection.Collector.Tests.Rules;

public class JintRuleEngineTests
{
    private readonly Mock<ILogger<JintRuleEngine>> _loggerMock;
    private readonly JintRuleEngine _ruleEngine;

    public JintRuleEngineTests()
    {
        _loggerMock = new Mock<ILogger<JintRuleEngine>>();
        _ruleEngine = new JintRuleEngine(_loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteRule_With_Simple_Calculation_Should_Return_Correct_Result()
    {
        // Arrange
        var dataPoints = new List<DataPoint>
        {
            new() { DeviceCode = "TEST001", PointCode = "TEMP", NumericValue = 25.5, DataType = DataType.Float },
            new() { DeviceCode = "TEST001", PointCode = "HUMIDITY", NumericValue = 60.0, DataType = DataType.Float }
        };

        var ruleCode = @"
            var temp = input.find(x => x.pointCode === 'TEMP').numericValue;
            var humidity = input.find(x => x.pointCode === 'HUMIDITY').numericValue;
            var heatIndex = temp + humidity * 0.1;
            
            output.push({
                deviceCode: 'TEST001',
                pointCode: 'HEAT_INDEX',
                pointName: 'Heat Index',
                dataType: 3,
                value: heatIndex,
                unit: '°C'
            });
        ";

        // Act
        var result = await _ruleEngine.ExecuteRuleAsync(ruleCode, dataPoints);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.OutputData.Should().HaveCount(1);
        result.OutputData[0].PointCode.Should().Be("HEAT_INDEX");
        result.OutputData[0].NumericValue.Should().Be(31.5); // 25.5 + 60.0 * 0.1
    }

    [Fact]
    public async Task ExecuteRule_With_Invalid_JavaScript_Should_Return_Error()
    {
        // Arrange
        var dataPoints = new List<DataPoint>();
        var invalidRuleCode = "invalid javascript code {";

        // Act
        var result = await _ruleEngine.ExecuteRuleAsync(invalidRuleCode, dataPoints);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ValidateRule_With_Valid_Code_Should_Return_True()
    {
        // Arrange
        var validRuleCode = @"
            var temp = input[0].numericValue;
            output.push({
                deviceCode: 'TEST001',
                pointCode: 'PROCESSED',
                value: temp * 2
            });
        ";

        // Act
        var result = await _ruleEngine.ValidateRuleAsync(validRuleCode);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateRule_With_Invalid_Code_Should_Return_False()
    {
        // Arrange
        var invalidRuleCode = "invalid javascript {";

        // Act
        var result = await _ruleEngine.ValidateRuleAsync(invalidRuleCode);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteRule_With_Empty_Input_Should_Return_Empty_Output()
    {
        // Arrange
        var dataPoints = new List<DataPoint>();
        var ruleCode = "output.push({ deviceCode: 'TEST', pointCode: 'EMPTY', value: 0 });";

        // Act
        var result = await _ruleEngine.ExecuteRuleAsync(ruleCode, dataPoints);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.OutputData.Should().HaveCount(1);
    }
} 