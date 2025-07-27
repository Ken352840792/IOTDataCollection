using Microsoft.Extensions.Localization;
using IoTDataCollection.Localization;
using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace IoTDataCollection;

[Dependency(ReplaceServices = true)]
public class IoTDataCollectionBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<IoTDataCollectionResource> _localizer;

    public IoTDataCollectionBrandingProvider(IStringLocalizer<IoTDataCollectionResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
