using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectService.EntityFrameworkCore
{
    public class ProjectServiceDbContextFactory : IDesignTimeDbContextFactory<ProjectServiceDbContext>
    {
        public ProjectServiceDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseSqlServer(GetConnectionStringFromConfiguration());
            return new ProjectServiceDbContext(builder.Options);
        }

        private static string GetConnectionStringFromConfiguration()
        {
            return BuildConfiguration()
                .GetConnectionString(ProjectServiceDbProperties.ConnectionStringName);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        $"..{Path.DirectorySeparatorChar}ProjectService.HttpApi.Host"
                    )
                )
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
