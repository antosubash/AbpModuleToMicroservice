﻿using Volo.Abp.Studio;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace MyCompany.ProjectService
{
    [DependsOn(
        typeof(AbpStudioModuleInstallerModule),
        typeof(AbpVirtualFileSystemModule)
        )]
    public class ProjectServiceInstallerModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<ProjectServiceInstallerModule>();
            });
        }
    }
}
