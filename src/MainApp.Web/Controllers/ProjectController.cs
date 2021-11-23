using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectService;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Authentication;
using Volo.Abp.Http.Client.DynamicProxying;

namespace MainApp.Web.Controllers
{
    public class ProjectController : AbpController
    {
        private readonly IHttpClientProxy<IProjectAppService> projectAppService;
        private readonly ILogger<ProjectController> logger;

        public ProjectController(IHttpClientProxy<IProjectAppService> projectAppService)
        {
            this.projectAppService = projectAppService;
        }

        [HttpGet]
        [Route("api/projects")]
        public async Task<IActionResult> GetListAsync()
        {
            var result = await projectAppService.Service.GetListAsync();
            return Ok(result);
        }
    }
}