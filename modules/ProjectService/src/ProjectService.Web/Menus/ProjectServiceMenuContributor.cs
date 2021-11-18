using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace ProjectService.Web.Menus
{
    public class ProjectServiceMenuContributor : IMenuContributor
    {
        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenuAsync(context);
            }
        }

        private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            //Add main menu items.
            context.Menu.AddItem(new ApplicationMenuItem(ProjectServiceMenus.Prefix, displayName: "ProjectService", "~/ProjectService", icon: "fa fa-globe"));

            return Task.CompletedTask;
        }
    }
}