using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ProjectService
{
    public interface IProjectAppService : IApplicationService
    {
        Task<List<ProjectDto>> GetListAsync();
        Task<ProjectDto> CreateAsync(string text);
        Task DeleteAsync(Guid id);
    }
}
