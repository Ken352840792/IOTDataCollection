using Xunit;

namespace IoTDataCollection.EntityFrameworkCore;

[CollectionDefinition(IoTDataCollectionTestConsts.CollectionDefinitionName)]
public class IoTDataCollectionEntityFrameworkCoreCollection : ICollectionFixture<IoTDataCollectionEntityFrameworkCoreFixture>
{

}
