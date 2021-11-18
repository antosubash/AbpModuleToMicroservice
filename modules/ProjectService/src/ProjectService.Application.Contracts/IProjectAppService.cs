using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectService
{
    public interface IProjectAppService
    {
        Task<List<ProjectDto>> GetListAsync();
        Task<ProjectDto> CreateAsync(string text);
        Task DeleteAsync(Guid id);
    }
}
