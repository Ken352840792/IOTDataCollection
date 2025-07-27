using System;
using System.Collections.Generic;
using System.Text;
using IoTDataCollection.Localization;
using Volo.Abp.Application.Services;

namespace IoTDataCollection;

/* Inherit your application services from this class.
 */
public abstract class IoTDataCollectionAppService : ApplicationService
{
    protected IoTDataCollectionAppService()
    {
        LocalizationResource = typeof(IoTDataCollectionResource);
    }
}
