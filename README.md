# Add a new Module and convert it to a microservice in ABP

In this post we will see how to create a modular abp application and convert it to microservice. We will add a new module to tiered abp app and then use the separate database to store the modules data and then convert the module to a microservice.

In this sample we will create Tiered app which is called `MainApp`. Then we will add a module called `ProjectService`.

## Creating the abp application and run migrations

```bash
abp new MainApp -t app -u mvc --tiered
```

## Run Migrations

Change directory to src/MainApp.DbMigrator and run the migration project

```bash
dotnet run
```

This will apply the migrations to the db and we can run the `MainApp.Web` project. This will host the UI and API..

## Add a new Module

Now we will add a new module to our MainApp. Move back to the `root` folder of your project.

```bash
abp add-module ProjectService --new
```

This will create a ProjectService and it will be available in the modules folder.

## Add and configure the host to the module

We need to add a host for our module. first we have to navigate to the the src folder of the Module.

```bash
cd .\modules\ProjectService\src\
```

Now lets create a Web Api project to host our module.

```bash
dotnet new webapi -n ProjectService.HttpApi.Host
```

Now open the ProjectService solution and the newly created Host project to the solution.

Update the `appsettings.json` with the following

```json
{
  "App": {
    "SelfUrl": "{Host Url}",
    "CorsOrigins": "https://*.MainApp.com"
  },
  "AuthServer": {
    "Authority": "{ Identity Server Url }",
    "RequireHttpsMetadata": "true",
    "SwaggerClientId": "ProjectService_Swagger",
    "SwaggerClientSecret": "1q2w3e*"
  },
  "ConnectionStrings": {
    "ProjectService": "Server=(LocalDb)\\MSSQLLocalDB;Database=ProjectService;Trusted_Connection=True"
  },
  "Redis": {
    "Configuration": "127.0.0.1"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

Add the following packages to the `ProjectService.HttpApi.Host` project.

```xml
<ItemGroup>
    <PackageReference Include="Serilog.AspNetCore" Version="4.1.0" />
    <PackageReference Include="Volo.Abp.EntityFrameworkCore" Version="5.0.0-rc.1" />
    <PackageReference Include="Volo.Abp.AspNetCore.MultiTenancy" Version="5.0.0-rc.1" />
    <PackageReference Include="Volo.Abp.Autofac" Version="5.0.0-rc.1" />
    <PackageReference Include="Volo.Abp.Core" Version="5.0.0-rc.1" />
    <PackageReference Include="Volo.Abp.EntityFrameworkCore.SqlServer" Version="5.0.0-rc.1" />
    <PackageReference Include="Volo.Abp.Swashbuckle" Version="5.0.0-rc.1" />
    <PackageReference Include="Volo.Abp.AspNetCore.Authentication.JwtBearer" Version="5.0.0-rc.1" />
    <PackageReference Include="Volo.Abp.AspNetCore.Serilog" Version="5.0.0-rc.1" />
    <PackageReference Include="Serilog.Extensions.Logging" Version="3.0.1" />
    <PackageReference Include="Serilog.Sinks.Async" Version="1.4.0" />
    <PackageReference Include="Serilog.Sinks.File" Version="4.1.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="3.1.1" />
</ItemGroup>
```

These are the essential packages for our host.

Add `ProjectService.Application`, `ProjectService.EntityFrameworkCore`, `ProjectService.HttpApi` projects as a reference to your `ProjectService.HttpApi.Host`

We are adding the projects reference because the host project module will depend on the module from these projects.

Create a `ProjectServiceHostModule` in the newly created `ProjectService.HttpApi.Host`. This will be a abp module where we will setup the host.

Here is the sample for the `ProductService`.

```cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ProjectService.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace ProjectService.HttpApi.Host
{

    [DependsOn(
    typeof(ProjectServiceHttpApiModule),
    typeof(ProjectServiceApplicationModule),
    typeof(ProjectServiceEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
    )]
    public class ProjectServiceHostModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                    options.Audience = "ProjectService";
                });

            context.Services.AddAbpSwaggerGenWithOAuth(
                configuration["AuthServer:Authority"],
                new Dictionary<string, string>
                {
                    {"ProjectService", "ProjectService API"}
                },
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjectService API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                });

            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(ProjectServiceApplicationModule).Assembly);
            });

            context.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .WithOrigins(
                            configuration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.Trim().RemovePostFix("/"))
                                .ToArray()
                        )
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCorrelationId();
            app.UseCors();
            app.UseAbpRequestLocalization();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAbpClaimsMap();
            app.UseMultiTenancy();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseAbpSwaggerUI(options => {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectService Service API");
                var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
                options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]); 
                options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]); 
            });
            app.UseAbpSerilogEnrichers();
            app.UseAuditing();
            app.UseUnitOfWork();
            app.UseConfiguredEndpoints();
        }
    }
}
```

In this module the depends on section configures the modules that it depends on in the `DependsOn` section and we will configure the JWT auth and the swagger UI for the host.

Update the `Program.cs`

```cs
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace ProjectService
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File("Logs/logs.txt"))
#if DEBUG
                .WriteTo.Async(c => c.Console())
#endif
                .CreateLogger();

            try
            {
                Log.Information("Starting ProjectService.Host.");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(build =>
                {
                    build.AddJsonFile("appsettings.secrets.json", optional: true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseAutofac()
                .UseSerilog();
    }

}
```

Program file is updated to use the serilog with enrichers.

Update the `Startup.cs`

```cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectService.HttpApi.Host;

namespace ProjectService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<ProjectServiceHostModule>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.InitializeApplication();
        }
    }
}
```

In the startup file we are configuring our newly created Host module.

Create a `HomeController.cs` in the `Controller` folder and update wit the following text.

```cs
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace ProjectService.HttpApi.Host.Controllers
{
    public class HomeController : AbpController
    {
        public ActionResult Index()
        {
            return Redirect("~/swagger");
        }
    }
}
```

Now our default route will be swagger.

We are mostly done with the host and now we can create the AppService.

## Add new Entity to the ProjectService

We are going to create a simple `Project` entity and create a AppService for that project. This section of the tutorial is based on the [Quick Start Guide](https://docs.abp.io/en/abp/latest/Tutorials/Todo/Index?UI=MVC&DB=EF)

We will create a new Entity inside the `ProjectService.Domain` called `Project`.

## Create an Entity

Learn more about the [Entity](https://docs.abp.io/en/abp/latest/Entities) in the abp docs.

First step is to create an Entity. Create the Entity in the `ProjectService.Domain` project.

```cs
public class Project : Entity<Guid>
{
    public string Name { get; set; }
}
```

## Add Entity to EfCore

Learn more about the [ef core](https://docs.abp.io/en/abp/latest/Entity-Framework-Core) in the abp docs.

Next is to add Entity to the EF Core. you will find the DbContext in the `ProjectService.EntityFrameworkCore` project. Add the DbSet to the DbContext

```cs
public DbSet<Project> Projects { get; set; }
```

## Configure Entity in EfCore

Make sure you have the updated the ef core global tool

Configuration is done in the `DbContextModelCreatingExtensions` class. This should be available in the `ProjectService.EntityFrameworkCore` project

```cs
builder.Entity<Project>(b =>
{
    //Configure table & schema name
    b.ToTable(ProjectServiceDbProperties.DbTablePrefix + "Projects", ProjectServiceDbProperties.Schema);

    b.ConfigureByConvention();
});
```

## Prepare for the migration

```bash
dotnet tool update --global dotnet-ef
```

Add necessary packages to the `ProjectService.EntityFrameworkCore` project.

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="6.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
<PackageReference Include="Volo.Abp.EntityFrameworkCore" Version="5.0.0-rc.1" />
<PackageReference Include="Volo.Abp.EntityFrameworkCore.SqlServer" Version="5.0.0-rc.1" />
```

We need to create a `ProjectServiceDbContextFactory` to support migrations. [Check here for more info about this](https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli)

```cs
public class ProjectServiceDbContextFactory : IDesignTimeDbContextFactory<ProjectServiceDbContext>
{
    public ProjectServiceDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ProjectServiceDbContext>()
            .UseSqlServer(GetConnectionStringFromConfiguration());
        return new ProjectServiceDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration()
            .GetConnectionString(ProjectServiceDbProperties.ConnectionStringName);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    $"..{Path.DirectorySeparatorChar}ProjectService.HttpApi.Host"
                )
            )
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
```

This is needed for ef core migration to work. We are building the configuration by taking the `appsetting.json` from the `ProjectService.HttpApi.Host`.

## Adding Migrations

Now the `DbContextFactory` is configured we can add the migrations.

Go the `ProjectService.EntityFrameworkCore` project in the terminal and create migrations.

To create migration run this command:

```bash
dotnet ef migrations add created_projects
```

Verify the migrations created in the migrations folder.

To update the database run this command

```bash
dotnet ef database update
```

## Create a Entity Dto

Dto are placed in `ProjectService.Application.Contracts` project

```cs
public class ProjectDto: EntityDto<Guid>
{
    public string Name { get; set; }
}
```

## Create a AppService interface

create `IProjectAppService` interface in the `ProjectService.Application.Contracts` project

```cs
public interface IProjectAppService: IApplicationService
{
    Task<List<ProjectDto>> GetListAsync();
    Task<ProjectDto> CreateAsync(string text);
    Task DeleteAsync(Guid id);
}
```

## Create an Application Services

Learn more about the [Application Services](https://docs.abp.io/en/abp/latest/Application-Services) in the abp docs.

Application service are created in the `ProjectService.Application` project

```cs
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
```

## Update the EntityFrameworkCoreModule

Update the `ConfigureServices` method in the `ProjectServiceEntityFrameworkCoreModule` file.

```cs
context.Services.AddAbpDbContext<ProjectServiceDbContext>(options =>
{
    options.AddDefaultRepositories(includeAllEntities: true);
});

Configure<AbpDbContextOptions>(options =>
{
    options.UseSqlServer();
});
```

## Create API Scope, API Resource and Swagger Client in IdentityServer

We need to do this in the `MainApp`. We have to update the `IdentityServerDataSeedContributor` in the `MainApp.Domain`.

```cs
private async Task CreateApiScopesAsync()
{
    await CreateApiScopeAsync("MainApp");
    await CreateApiScopeAsync("ProjectService");
}

private async Task CreateApiResourcesAsync()
{
    var commonApiUserClaims = new[]
    {
        "email",
        "email_verified",
        "name",
        "phone_number",
        "phone_number_verified",
        "role"
    };

    await CreateApiResourceAsync("MainApp", commonApiUserClaims);
    await CreateApiResourceAsync("ProjectService", commonApiUserClaims);
}
```

Now lets update the create the swagger client for the new service.

In the `MainApp.DbMigrator` project update the `appsettings.json` with the new swagger client.

```cs
"ProjectService_Swagger": {
    "ClientId": "ProjectService_Swagger",
    "ClientSecret": "1q2w3e*",
    "RootUrl": "{ Your Service url }"
}
```

Update the `commonScopes` in the `CreateClientsAsync` method

```cs
var commonScopes = new[]
{
    "email",
    "openid",
    "profile",
    "role",
    "phone",
    "address",
    "MainApp",
    "ProjectService"
};
```

This will add the newly created scope to all the clients.

Update the `CreateClientsAsync` method to create the swagger client in the `IdentityServerDataSeedContributor` in the `MainApp.Domain`.

```cs
var swaggerClientIdProjectService = configurationSection["ProjectService_Swagger:ClientId"];
if (!swaggerClientIdProjectService.IsNullOrWhiteSpace())
{
    var swaggerRootUrl = configurationSection["ProjectService_Swagger:RootUrl"].TrimEnd('/');

    await CreateClientAsync(
        name: swaggerClientIdProjectService,
        scopes: commonScopes,
        grantTypes: new[] { "authorization_code" },
        secret: configurationSection["ProjectService_Swagger:ClientSecret"]?.Sha256(),
        requireClientSecret: false,
        redirectUri: $"{swaggerRootUrl}/swagger/oauth2-redirect.html",
        corsOrigins: new[] { swaggerRootUrl.RemovePostFix("/") }
    );
}
```

## Run Migrations again for the MainApp

Change directory to `MainApp.DbMigrator` and run the migration project

```bash
dotnet run
```

This will run the `DbMigrator` project. The `DbMigrator` will seed the database with the New Scope, API and Client in our Identity Server.

Once the migration is done lets update the `CorsOrigins` in the IdentityServer.

```json
"App": {
    "SelfUrl": "https://localhost:44373",
    "ClientUrl": "http://localhost:4200",
    "CorsOrigins": "https://*.MainApp.com,http://localhost:4200,https://localhost:44307,https://localhost:44358,https://localhost:44372,{ Project Service url }", // update the entry here
    "RedirectAllowedUrls": "http://localhost:4200,https://localhost:44307"
},
```

## Communicating with the Microservice

Now we have the application running lets see how we can communicate between services.

To communicate with the we need to use the `Client Proxy` [Check docs here](https://docs.abp.io/en/abp/latest/API/Dynamic-CSharp-API-Clients)

### Add the Contract project reference

We need to add `ProjectService.Application.Contracts` project as project reference to `MainApp.HttpApi.Client`

```xml
<ProjectReference Include="..\..\modules\ProjectService\src\ProjectService.Application.Contracts\ProjectService.Application.Contracts.csproj" />
```

Update the `MainAppHttpApiClientModule` dependency and add the `ProjectService` as a client proxy.

```cs
typeof(ProjectServiceApplicationContractsModule)
```

Update the `ConfigureServices` service in the `MainAppHttpApiClientModule.cs` file in the `MainApp.HttpApi.Client` project.

```cs
//Create dynamic client proxies
context.Services.AddHttpClientProxies(
    typeof(ProjectServiceApplicationContractsModule).Assembly,
    "ProjectService"
);
```

Now lets add Remote Service Endpoints to the `appsettings.json` in `MainApp.Web`

```json
"RemoteServices": {
    "Default": {
        "BaseUrl": "https://localhost:44358/"
    },
    "ProjectService": {
        "BaseUrl": "https://localhost:44372/"
    }
}
```

### Update the scope of the Web App

To connect to the project service we need to add the `ProjectService` scope as a scope to the `AddAbpOpenIdConnect` method inside the `ConfigureAuthentication` method in the `MainAppWebModule`

```cs
.AddAbpOpenIdConnect("oidc", options =>
{
    options.Authority = configuration["AuthServer:Authority"];
    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

    options.ClientId = configuration["AuthServer:ClientId"];
    options.ClientSecret = configuration["AuthServer:ClientSecret"];

    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;

    options.Scope.Add("role");
    options.Scope.Add("email");
    options.Scope.Add("phone");
    options.Scope.Add("MainApp");
    options.Scope.Add("ProjectService"); // This is for the new project service
});
```

## Create a UI to display the projects

Create a project page to display the project list in the `MainApp.Web`.

Create a `Projects.cshtml`

```html
@page
@model MainApp.Web.Pages.ProjectsModel
<div class="container">
    <H1>List of projects</H1>
    @if(Model.Projects != null && Model.Projects.Count > 0){
        @foreach (var item in Model?.Projects) {
            <div class="card">
                <div class="card-body">
                    <h5 class="card-title">@item.Name</h5>
                </div>
            </div>
        }  
    }
</div>
```

Create a `Projects.cshtml.cs`

```cs
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
    }
```

Lets add the newly created page to the Main menu.

Update the `ConfigureMainMenuAsync` method in the `MainAppMenuContributor.cs` file.

```cs
context.Menu.Items.Insert(
    3,
    new ApplicationMenuItem(
        MainAppMenus.Home,
        l["Projects"],
        "~/Projects",
        icon: "fas fa-list",
        order: 0
    )
);
```

Now lets run the application.

## Running the application

Since we have run 4 services lets init `tye` so that it will be easier.

```bash
tye init
```

This command will create the `tye.yaml` file which will have the project without out ports binding. Update the ports from your solution. Here is the sample `tye.yaml`.

```yaml
name: mainapp
services:
- name: mainapp-web
  project: src/MainApp.Web/MainApp.Web.csproj
  bindings:
  - port: 44343 //update your ports here
    protocol: https
- name: mainapp-identityserver
  project: src/MainApp.IdentityServer/MainApp.IdentityServer.csproj
  bindings:
  - port: 44373 //update your ports here
    protocol: https
- name: mainapp-httpapi-host
  project: src/MainApp.HttpApi.Host/MainApp.HttpApi.Host.csproj
  bindings:
  - port: 44358 //update your ports here
    protocol: https
- name: project-service
  project: modules/ProjectService/src/ProjectService.HttpApi.Host/ProjectService.HttpApi.Host.csproj
  bindings:
  - port: 44372 //update your ports here
    protocol: https
```

Now run the tye command.

```bash
tye run
```

This will run all the projects. Navigate to the `ProjectService` and do the swagger authorization to login and call the project api to create new project.

To View the project visit the `MainApp.Web` project and click `Projects` menu to see the list of the project you just created.
