using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectService;

namespace MainApp.Web.Pages
{
    public class ProjectsModel : MainAppPageModel
    {
        private readonly ILogger<ProjectsModel> logger;

        public List<ProjectDto> Projects { get; set; }
        private IProjectAppService projectAppService { get; set; }
        public ProjectsModel(IProjectAppService projectAppService, ILogger<ProjectsModel> logger)
        {
            this.projectAppService = projectAppService;
            this.logger = logger;
            Projects = new List<ProjectDto>();
        }

        public void OnGet()
        {
            try
            {
                var projects = projectAppService.GetListAsync().Result;
                Projects = projects;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        public async Task OnPostLoginAsync()
        {

        }
    }
}