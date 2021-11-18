using ProjectService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace ProjectService.Permissions
{
    public class ProjectServicePermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(ProjectServicePermissions.GroupName, L("Permission:ProjectService"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<ProjectServiceResource>(name);
        }
    }
}