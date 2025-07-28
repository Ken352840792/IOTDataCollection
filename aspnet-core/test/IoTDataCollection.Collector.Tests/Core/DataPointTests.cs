using FluentAssertions;
using IoTDataCollection.Collector.Core.Models;
using Xunit;

namespace IoTDataCollection.Collector.Tests.Core;

public class DataPointTests
{
    [Fact]
    public void DataPoint_Should_Create_With_Default_Values()
    {
        // Arrange & Act
        var dataPoint = new DataPoint();

        // Assert
        dataPoint.Should().NotBeNull();
        dataPoint.Id.Should().NotBeEmpty();
        dataPoint.DeviceCode.Should().BeEmpty();
        dataPoint.PointCode.Should().BeEmpty();
        dataPoint.PointName.Should().BeEmpty();
        dataPoint.Address.Should().BeEmpty();
        dataPoint.DataType.Should().Be(DataType.Bool);
        dataPoint.Quality.Should().Be(192);
        dataPoint.IsEnabled.Should().BeTrue();
        dataPoint.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void DataPoint_Should_Set_Numeric_Value_Correctly()
    {
        // Arrange
        var dataPoint = new DataPoint
        {
            DataType = DataType.Float,
            NumericValue = 25.5
        };

        // Act & Assert
        dataPoint.NumericValue.Should().Be(25.5);
        dataPoint.RawValue.Should().Be("25.5");
    }

    [Fact]
    public void DataPoint_Should_Set_String_Value_Correctly()
    {
        // Arrange
        var dataPoint = new DataPoint
        {
            DataType = DataType.String,
            StringValue = "Test Value"
        };

        // Act & Assert
        dataPoint.StringValue.Should().Be("Test Value");
        dataPoint.RawValue.Should().Be("Test Value");
    }

    [Fact]
    public void DataPoint_Should_Set_Boolean_Value_Correctly()
    {
        // Arrange
        var dataPoint = new DataPoint
        {
            DataType = DataType.Bool,
            BooleanValue = true
        };

        // Act & Assert
        dataPoint.BooleanValue.Should().BeTrue();
        dataPoint.RawValue.Should().Be("True");
    }
} 