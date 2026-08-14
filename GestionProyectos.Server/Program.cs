using Blazored.SessionStorage;
using GestionProyectos.Data;
using GestionProyectos.Engine.Catalog;
using GestionProyectos.Engine.Excel.Download;
using GestionProyectos.Engine.Feature.Dashboard;
using GestionProyectos.Engine.Feature.Customer.Read;
using GestionProyectos.Engine.Feature.Performance;
using GestionProyectos.Engine.Feature.Billing;
using GestionProyectos.Engine.Feature.HoursReport;
using GestionProyectos.Engine.Feature.Requirement.Burndown;
using GestionProyectos.Engine.Feature.Project.Burndown;
using GestionProyectos.Engine.Feature.Project.Create;
using GestionProyectos.Engine.Feature.Project.Delete;
using GestionProyectos.Engine.Feature.Project.Detail;
using GestionProyectos.Engine.Feature.Project.Read;
using GestionProyectos.Engine.Feature.Project.Update;
using GestionProyectos.Engine.Feature.Requirement.Create;
using GestionProyectos.Engine.Feature.Requirement.Delete;
using GestionProyectos.Engine.Feature.Requirement.DownloadFile;
using GestionProyectos.Engine.Feature.Requirement.Read;
using GestionProyectos.Engine.Feature.Requirement.Update;
using GestionProyectos.Engine.Feature.Task.TaskCreation;
using GestionProyectos.Engine.Feature.Task.TaskDeletion;
using GestionProyectos.Engine.Feature.Task.TaskList;
using GestionProyectos.Engine.Feature.Task.TaskUpdate;
using GestionProyectos.Engine.Feature.Task.Detail;
using GestionProyectos.Engine.Feature.Backlog;

using GestionProyectos.Engine.Feature.Task.BlockReport;
using GestionProyectos.Engine.Feature.Task.Bug;
using GestionProyectos.Engine.Feature.Task.Note;
using GestionProyectos.Engine.Feature.Task.TimeLogList;
using GestionProyectos.Engine.Feature.Task.TimeLogRegistration;
using GestionProyectos.Engine.Security;
using GestionProyectos.Engine.Security.Admin;
using GestionProyectos.Engine.Security.Users;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Engine.Utility.S3DownloadFile;
using GestionProyectos.Engine.Utility.S3UploadFile;
using GestionProyectos.Engine.Utility.SendEmail;
using GestionProyectos.Server.Data;
using GestionProyectos.Server.Extensions;
using GestionProyectos.Shared.AWSManager;
using GestionProyectos.Shared.Configurations;
using GestionProyectos.Shared.Enums;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
AppSettingsManagerBase appSettingsManager;

// Solo aceptar un nombre de entorno real (Development/Staging/Production).
// Las herramientas EF pasan args como --applicationName; no deben sobrescribir el entorno.
if (args is { Length: > 0 } &&
    !args[0].StartsWith("-", StringComparison.Ordinal) &&
    !string.IsNullOrWhiteSpace(args[0]))
{
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", args[0]);
}

var environmentName = string.IsNullOrWhiteSpace(Constants.EnvironmentName)
    ? "Development"
    : Constants.EnvironmentName;

IConfigurationRoot config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.{environmentName}.json")
    .AddEnvironmentVariables()
.Build();

appSettingsManager = config.Get<AppSettingsManagerBase>();

if (!environmentName.Equals("Development", StringComparison.OrdinalIgnoreCase))
    appSettingsManager = SecretManagerHelper.GetSecret<AppSettingsManagerBase>(appSettingsManager.Configurations.AwsSecretManager, appSettingsManager.Configurations.AwsVersionStage, appSettingsManager.Configurations.AwsRegion);

// APPSETTINGS
builder.Services.Configure<AppSettingsManagerBase>(appSettings =>
{
    appSettings.Configurations = appSettingsManager.Configurations;
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<DataDbContext>(x =>
{
    if (appSettingsManager.Configurations.UseInMemoryDatabase)
        x.UseInMemoryDatabase("GestionProyectosDev").UseLazyLoadingProxies();
    else
        x.UseNpgsql(appSettingsManager.Configurations.ConnectionStrings.GestionProyectos).UseLazyLoadingProxies();
}
);

builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; }); ;
builder.Services.AddTransient<ISecurityEngine, SecurityEngine>();
builder.Services.AddTransient<IExcelDownloadEngine, ExcelDownloadEngine>();
builder.Services.AddTransient<ISecurityEngine, SecurityEngine>();
builder.Services.AddTransient<ICatalogEngine, CatalogEngine>();
builder.Services.AddTransient<ISecurityAdminEngine, SecurityAdminEngine>();
builder.Services.AddTransient<IUserManagementEngine, UserManagementEngine>();
builder.Services.AddTransient<IRequirementCreateEngine, RequirementCreateEngine>();
builder.Services.AddTransient<IRequirementReadEngine, RequirementReadEngine>();
builder.Services.AddTransient<ICustomerReadEngine, CustomerReadEngine>();
builder.Services.AddTransient<IProjectReadEngine, ProjectReadEngine>();
builder.Services.AddTransient<IProjectDetailReadEngine, ProjectDetailReadEngine>();
builder.Services.AddTransient<IS3UploadFileEngine, S3UploadFileEngine>();
builder.Services.AddTransient<ISendEmailEngine, SendEmailEngine>();
builder.Services.AddTransient<ITaskListEngine, TaskListEngine>();
builder.Services.AddTransient<ITaskCreationEngine, TaskCreationEngine>();
builder.Services.AddTransient<ITaskDeletionEngine, TaskDeletionEngine>();
builder.Services.AddTransient<ITaskUpdateEngine, TaskUpdateEngine>();
builder.Services.AddTransient<ITaskDetailReadEngine, TaskDetailReadEngine>();
builder.Services.AddTransient<ITaskBugEngine, TaskBugEngine>();
builder.Services.AddTransient<ITaskBlockReportEngine, TaskBlockReportEngine>();
builder.Services.AddTransient<IBacklogEngine, BacklogEngine>();
builder.Services.AddTransient<ITaskNoteEngine, TaskNoteEngine>();
builder.Services.AddTransient<ITimeLogRegistrationEngine, TimeLogRegistrationEngine>();
builder.Services.AddTransient<ITaskTimeLogListEngine, TaskTimeLogListEngine>();
builder.Services.AddTransient<IRequirementUpdateEngine, RequirementUpdateEngine>();
builder.Services.AddTransient<IRequirementDeletionEngine, RequirementDeletionEngine>();
builder.Services.AddTransient<IProjectCreateEngine, ProjectCreateEngine>();
builder.Services.AddTransient<IS3DownloadFileEngine, S3DownloadFileEngine>();
builder.Services.AddTransient<IDownloadFileEngine, DownloadFileEngine>();
builder.Services.AddTransient<IProjectDeleteEngine, ProjectDeleteEngine>();
builder.Services.AddTransient<IProjectBurndownEngine, ProjectBurndownEngine>();
builder.Services.AddTransient<IRequirementBurndownEngine, RequirementBurndownEngine>();
builder.Services.AddTransient<IPerformanceEngine, PerformanceEngine>();
builder.Services.AddTransient<IBillingReportEngine, BillingReportEngine>();
builder.Services.AddTransient<IHoursReportEngine, HoursReportEngine>();
builder.Services.AddTransient<IDashboardReadEngine, DashboardReadEngine>();
builder.Services.AddTransient<IProjectUpdateEngine, ProjectUpdateEngine>();

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationExtension>();
builder.Services.AddAuthorizationCore();

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Seed/Ensure desactivados: la data la controlas tú (staging/dev).
// Solo se sincronizan secuencias identity tras inserts con Id explícito.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataDbContext>();
    DevDataSeeder.SyncPostgreSqlIdentitySequences(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
