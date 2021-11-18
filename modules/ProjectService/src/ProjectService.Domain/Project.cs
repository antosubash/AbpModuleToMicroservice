using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace ProjectService
{
    public class Project : Entity<Guid>
    {
        public string Name { get; set; }
    }
}
