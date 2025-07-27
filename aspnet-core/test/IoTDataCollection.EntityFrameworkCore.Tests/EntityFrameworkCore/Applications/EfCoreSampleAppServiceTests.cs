using IoTDataCollection.Samples;
using Xunit;

namespace IoTDataCollection.EntityFrameworkCore.Applications;

[Collection(IoTDataCollectionTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<IoTDataCollectionEntityFrameworkCoreTestModule>
{

}
