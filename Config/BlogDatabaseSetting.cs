namespace tangti.Configs;

public class BlogDatabaseSetting
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string BlogCollectionName { get; set; } = null!;

    public string UserCollectionName { get; set; } = null!;
}