using IoTDataCollection.Samples;
using Xunit;

namespace IoTDataCollection.EntityFrameworkCore.Domains;

[Collection(IoTDataCollectionTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<IoTDataCollectionEntityFrameworkCoreTestModule>
{

}
