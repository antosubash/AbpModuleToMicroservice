using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace ProjectService.EntityFrameworkCore
{
    public static class ProjectServiceDbContextModelCreatingExtensions
    {
        public static void ConfigureProjectService(
            this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure all entities here. Example:

            builder.Entity<Question>(b =>
            {
                //Configure table & schema name
                b.ToTable(ProjectServiceDbProperties.DbTablePrefix + "Questions", ProjectServiceDbProperties.DbSchema);

                b.ConfigureByConvention();

                //Properties
                b.Property(q => q.Title).IsRequired().HasMaxLength(QuestionConsts.MaxTitleLength);

                //Relations
                b.HasMany(question => question.Tags).WithOne().HasForeignKey(qt => qt.QuestionId);

                //Indexes
                b.HasIndex(q => q.CreationTime);
            });
            */

            builder.Entity<Project>(b =>
            {
                //Configure table & schema name
                b.ToTable(ProjectServiceDbProperties.DbTablePrefix + "Projects", ProjectServiceDbProperties.DbSchema);

                b.ConfigureByConvention();
            });
        }
    }
}
