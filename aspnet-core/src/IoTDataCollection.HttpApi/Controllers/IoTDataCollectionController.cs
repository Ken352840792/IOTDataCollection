using IoTDataCollection.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace IoTDataCollection.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class IoTDataCollectionController : AbpControllerBase
{
    protected IoTDataCollectionController()
    {
        LocalizationResource = typeof(IoTDataCollectionResource);
    }
}
