using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ProjectService
{
    [Authorize]
    public class ProjectAppService : ProjectServiceAppService, IProjectAppService
    {
        private readonly IRepository<Project, Guid> projectRepository;

        public ProjectAppService(IRepository<Project, Guid> projectRepository)
        {
            this.projectRepository = projectRepository;
        }

        public async Task<ProjectDto> CreateAsync(string text)
        {
            var projectItem = await projectRepository.InsertAsync(
                                new Project { Name = text }
                                );

            return new ProjectDto
            {
                Id = projectItem.Id,
                Name = projectItem.Name
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            await projectRepository.DeleteAsync(id);
        }

        public async Task<List<ProjectDto>> GetListAsync()
        {
            var items = await projectRepository.GetListAsync();
            return items
                .Select(item => new ProjectDto
                {
                    Id = item.Id,
                    Name = item.Name
                }).ToList();
        }
    }
}
