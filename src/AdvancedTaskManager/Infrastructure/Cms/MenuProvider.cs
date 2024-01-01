using System.Collections.Generic;
using AdvancedTaskManager.Infrastructure.Helpers;
using EPiServer.Shell.Navigation;

namespace AdvancedTaskManager.Infrastructure.Cms
{
    [MenuProvider]
    public class MenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            var url = Extensions.ContainerControllerActionPathsToResource;

            var link = new UrlMenuItem(
                "Adv. Task Manager",
                MenuPaths.Global + "/cms/advancedtask",
                url)
            {
                SortIndex = 100,
                AuthorizationPolicy = Constants.PolicyName
            };

            return new List<MenuItem>
            {
                link
            };
        }
    }
}
