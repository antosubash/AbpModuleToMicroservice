namespace ProjectService
{
    public static class ProjectServiceDbProperties
    {
        public static string DbTablePrefix { get; set; } = "ProjectService";

        public static string DbSchema { get; set; } = null;

        public const string ConnectionStringName = "ProjectService";
    }
}
