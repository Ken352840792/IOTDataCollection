using AutoMapper;
using IoTDataCollection.Enterprises;
using IoTDataCollection.Devices;

namespace IoTDataCollection;

public class IoTDataCollectionApplicationAutoMapperProfile : Profile
{
    public IoTDataCollectionApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        // 企业映射配置
        CreateMap<Enterprise, EnterpriseDto>()
            .ForMember(dest => dest.EnterpriseCode, opt => opt.MapFrom(src => src.F_EnterpriseCode))
            .ForMember(dest => dest.EnterpriseName, opt => opt.MapFrom(src => src.F_EnterpriseName))
            .ForMember(dest => dest.ShortName, opt => opt.MapFrom(src => src.F_ShortName))
            .ForMember(dest => dest.EnterpriseType, opt => opt.MapFrom(src => src.F_EnterpriseType))
            .ForMember(dest => dest.ContactPerson, opt => opt.MapFrom(src => src.F_ContactPerson))
            .ForMember(dest => dest.ContactPhone, opt => opt.MapFrom(src => src.F_ContactPhone))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.F_Address))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.F_Description))
            .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.F_SortOrder))
            .ForMember(dest => dest.IsEnabled, opt => opt.MapFrom(src => src.F_IsEnabled));

        CreateMap<CreateEnterpriseDto, Enterprise>()
            .ForMember(dest => dest.F_EnterpriseCode, opt => opt.MapFrom(src => src.EnterpriseCode))
            .ForMember(dest => dest.F_EnterpriseName, opt => opt.MapFrom(src => src.EnterpriseName))
            .ForMember(dest => dest.F_ShortName, opt => opt.MapFrom(src => src.ShortName))
            .ForMember(dest => dest.F_EnterpriseType, opt => opt.MapFrom(src => src.EnterpriseType))
            .ForMember(dest => dest.F_ContactPerson, opt => opt.MapFrom(src => src.ContactPerson))
            .ForMember(dest => dest.F_ContactPhone, opt => opt.MapFrom(src => src.ContactPhone))
            .ForMember(dest => dest.F_Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.F_Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.F_SortOrder, opt => opt.MapFrom(src => src.SortOrder))
            .ForMember(dest => dest.F_IsEnabled, opt => opt.MapFrom(src => src.IsEnabled));

        CreateMap<UpdateEnterpriseDto, Enterprise>()
            .ForMember(dest => dest.F_EnterpriseCode, opt => opt.MapFrom(src => src.EnterpriseCode))
            .ForMember(dest => dest.F_EnterpriseName, opt => opt.MapFrom(src => src.EnterpriseName))
            .ForMember(dest => dest.F_ShortName, opt => opt.MapFrom(src => src.ShortName))
            .ForMember(dest => dest.F_EnterpriseType, opt => opt.MapFrom(src => src.EnterpriseType))
            .ForMember(dest => dest.F_ContactPerson, opt => opt.MapFrom(src => src.ContactPerson))
            .ForMember(dest => dest.F_ContactPhone, opt => opt.MapFrom(src => src.ContactPhone))
            .ForMember(dest => dest.F_Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.F_Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.F_SortOrder, opt => opt.MapFrom(src => src.SortOrder))
            .ForMember(dest => dest.F_IsEnabled, opt => opt.MapFrom(src => src.IsEnabled));

        // 设备映射配置
        CreateMap<Device, DeviceDto>()
            .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.F_SiteId))
            .ForMember(dest => dest.DeviceCode, opt => opt.MapFrom(src => src.F_DeviceCode))
            .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.F_DeviceName))
            .ForMember(dest => dest.DeviceType, opt => opt.MapFrom(src => src.F_DeviceType))
            .ForMember(dest => dest.DeviceModel, opt => opt.MapFrom(src => src.F_DeviceModel))
            .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.F_Manufacturer))
            .ForMember(dest => dest.CommunicationProtocol, opt => opt.MapFrom(src => src.F_CommunicationProtocol))
            .ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.F_IpAddress))
            .ForMember(dest => dest.Port, opt => opt.MapFrom(src => src.F_Port))
            .ForMember(dest => dest.SlaveAddress, opt => opt.MapFrom(src => src.F_SlaveAddress))
            .ForMember(dest => dest.CollectionInterval, opt => opt.MapFrom(src => src.F_CollectionInterval))
            .ForMember(dest => dest.Timeout, opt => opt.MapFrom(src => src.F_Timeout))
            .ForMember(dest => dest.RetryCount, opt => opt.MapFrom(src => src.F_RetryCount))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.F_Location))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.F_Description))
            .ForMember(dest => dest.ConnectionStatus, opt => opt.MapFrom(src => src.F_ConnectionStatus))
            .ForMember(dest => dest.LastCommunicationTime, opt => opt.MapFrom(src => src.F_LastCommunicationTime))
            .ForMember(dest => dest.IsCollectionEnabled, opt => opt.MapFrom(src => src.F_IsCollectionEnabled))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.F_Status))
            .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.F_SortOrder));

        CreateMap<CreateDeviceDto, Device>()
            .ForMember(dest => dest.F_SiteId, opt => opt.MapFrom(src => src.SiteId))
            .ForMember(dest => dest.F_DeviceCode, opt => opt.MapFrom(src => src.DeviceCode))
            .ForMember(dest => dest.F_DeviceName, opt => opt.MapFrom(src => src.DeviceName))
            .ForMember(dest => dest.F_DeviceType, opt => opt.MapFrom(src => src.DeviceType))
            .ForMember(dest => dest.F_DeviceModel, opt => opt.MapFrom(src => src.DeviceModel))
            .ForMember(dest => dest.F_Manufacturer, opt => opt.MapFrom(src => src.Manufacturer))
            .ForMember(dest => dest.F_CommunicationProtocol, opt => opt.MapFrom(src => src.CommunicationProtocol))
            .ForMember(dest => dest.F_IpAddress, opt => opt.MapFrom(src => src.IpAddress))
            .ForMember(dest => dest.F_Port, opt => opt.MapFrom(src => src.Port))
            .ForMember(dest => dest.F_SlaveAddress, opt => opt.MapFrom(src => src.SlaveAddress))
            .ForMember(dest => dest.F_CollectionInterval, opt => opt.MapFrom(src => src.CollectionInterval))
            .ForMember(dest => dest.F_Timeout, opt => opt.MapFrom(src => src.Timeout))
            .ForMember(dest => dest.F_RetryCount, opt => opt.MapFrom(src => src.RetryCount))
            .ForMember(dest => dest.F_Location, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.F_Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.F_IsCollectionEnabled, opt => opt.MapFrom(src => src.IsCollectionEnabled))
            .ForMember(dest => dest.F_Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.F_SortOrder, opt => opt.MapFrom(src => src.SortOrder));

        CreateMap<UpdateDeviceDto, Device>()
            .ForMember(dest => dest.F_SiteId, opt => opt.MapFrom(src => src.SiteId))
            .ForMember(dest => dest.F_DeviceCode, opt => opt.MapFrom(src => src.DeviceCode))
            .ForMember(dest => dest.F_DeviceName, opt => opt.MapFrom(src => src.DeviceName))
            .ForMember(dest => dest.F_DeviceType, opt => opt.MapFrom(src => src.DeviceType))
            .ForMember(dest => dest.F_DeviceModel, opt => opt.MapFrom(src => src.DeviceModel))
            .ForMember(dest => dest.F_Manufacturer, opt => opt.MapFrom(src => src.Manufacturer))
            .ForMember(dest => dest.F_CommunicationProtocol, opt => opt.MapFrom(src => src.CommunicationProtocol))
            .ForMember(dest => dest.F_IpAddress, opt => opt.MapFrom(src => src.IpAddress))
            .ForMember(dest => dest.F_Port, opt => opt.MapFrom(src => src.Port))
            .ForMember(dest => dest.F_SlaveAddress, opt => opt.MapFrom(src => src.SlaveAddress))
            .ForMember(dest => dest.F_CollectionInterval, opt => opt.MapFrom(src => src.CollectionInterval))
            .ForMember(dest => dest.F_Timeout, opt => opt.MapFrom(src => src.Timeout))
            .ForMember(dest => dest.F_RetryCount, opt => opt.MapFrom(src => src.RetryCount))
            .ForMember(dest => dest.F_Location, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.F_Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.F_IsCollectionEnabled, opt => opt.MapFrom(src => src.IsCollectionEnabled))
            .ForMember(dest => dest.F_Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.F_SortOrder, opt => opt.MapFrom(src => src.SortOrder));
    }
} 