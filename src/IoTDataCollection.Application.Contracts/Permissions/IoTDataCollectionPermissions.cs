namespace IoTDataCollection.Permissions;

public static class IoTDataCollectionPermissions
{
    public const string GroupName = "IoTDataCollection";

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

    public static class Enterprises
    {
        public const string Default = GroupName + ".Enterprises";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Sites
    {
        public const string Default = GroupName + ".Sites";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Devices
    {
        public const string Default = GroupName + ".Devices";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Debug = Default + ".Debug";
    }

    public static class DataPoints
    {
        public const string Default = GroupName + ".DataPoints";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Rules
    {
        public const string Default = GroupName + ".Rules";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Execute = Default + ".Execute";
    }

    public static class Monitoring
    {
        public const string Default = GroupName + ".Monitoring";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string ViewStats = Default + ".ViewStats";
        public const string HealthCheck = Default + ".HealthCheck";
        public const string NodeControl = Default + ".NodeControl";
    }
} 