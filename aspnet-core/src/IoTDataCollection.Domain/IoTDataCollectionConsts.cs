namespace IoTDataCollection;

public static class IoTDataCollectionConsts
{
    /// <summary>
    /// 数据库表名前缀 - 所有表名以T_开头
    /// </summary>
    public const string DbTablePrefix = "T_";

    /// <summary>
    /// 数据库架构名称
    /// </summary>
    public const string DbSchema = null;
    
    /// <summary>
    /// 字段名前缀 - 所有字段名以F_开头
    /// </summary>
    public const string FieldPrefix = "F_";
    
    /// <summary>
    /// 视图名前缀 - 所有视图名以V_开头
    /// </summary>
    public const string ViewPrefix = "V_";
    
    /// <summary>
    /// 存储过程名前缀 - 所有存储过程名以G_开头
    /// </summary>
    public const string ProcedurePrefix = "G_";
    
    /// <summary>
    /// 系统保留扩展字段数量 - 每个表保留10个扩展字段
    /// </summary>
    public const int ExtensionFieldCount = 10;
    
    /// <summary>
    /// 扩展字段名前缀模板 - F_EXP_{01-10}
    /// </summary>
    public const string ExtensionFieldPrefix = "F_EXP_";
}
