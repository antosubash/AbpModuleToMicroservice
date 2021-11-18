using MainApp.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace MainApp.Permissions
{
    public class MainAppPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(MainAppPermissions.GroupName);
            //Define your own permissions here. Example:
            //myGroup.AddPermission(MainAppPermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<MainAppResource>(name);
        }
    }
}
