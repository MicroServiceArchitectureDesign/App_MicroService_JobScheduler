using App_MicroService_JobScheduler.Middlewares;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddScoped<ISyncService, SyncService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TPAPP_JOB_SERVICE",
        Version = "1.0.1",
        Description = "<a href='/'> Back to home page </a>"
    });
});

builder.Services.AddScoped<ITestRepo, TestRepo>();

builder.Services.AddHangfire(configuration => configuration
       .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
       .UseSimpleAssemblyNameTypeSerializer()
       .UseRecommendedSerializerSettings()
       .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer(p => p.SchedulePollingInterval = TimeSpan.FromSeconds(5));

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen();
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
// })
//         .AddCookie(options =>
//         {
//             options.ExpireTimeSpan = TimeSpan.FromDays(1);
//             options.LoginPath = "/Auth/Login";
//             options.LogoutPath = "/Auth/Logout";
//             options.AccessDeniedPath = options.LoginPath;
//             options.ReturnUrlParameter = "returnUrl";
//         });
//

builder.Services.AddOptionsSetups();
builder.Services.AddScoped<IRecurringExecutableJob, UserJob1>();
// builder.Services.AddScoped<IRecurringExecutableJob, UserJob2>();
// builder.Services.AddScoped<IRecurringExecutableJob, UserJob3>();

builder.Services.AddHealthChecks();
builder.Services.AddConsuleService();

var app = builder.Build();


using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
context.Database.Migrate();

// app.UseMiddleware<UnAuthorizedPathMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HangfireApplication v1"));
}

await InitJobs(app);

app.MapHealthChecks("/hc");
app.UseStaticFiles();
// app.UseHttpsRedirection();
app.UseRouting();
// app.UseAuthorization();
app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    IsReadOnlyFunc = p => true, //IsReadOnly(),
    // Authorization = [new HangfireAuthorizationFilter()],
    DarkModeEnabled = true,
    DashboardTitle = "Microservices job scheduler service.",
    StatsPollingInterval = 3600,
});
app.MapHangfireDashboard();
app.MapDefaultControllerRoute();
app.Run();

//static Func<DashboardContext, bool> IsReadOnly()
//{
//    return (DashboardContext context) => !context.GetHttpContext().User.Identity!.IsAuthenticated;
//}

static async Task InitJobs(WebApplication app)
{
    var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

    using var scope = app.Services.CreateScope();

    var jobs = scope.ServiceProvider.GetRequiredService<IEnumerable<IRecurringExecutableJob>>();
    var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();

    await syncService.SyncJobsInDatabaseAsync(jobs);


    var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

    var allAvailableJobDetails = context.JobsDetails.ToList();

    var activeDbJobs = allAvailableJobDetails.Where(p => p.IsActive).Select(p => p.Name).ToList();
    var deActiveDbJobs = allAvailableJobDetails.Where(p => !p.IsActive).Select(p => p.Name).ToList();
    var shouldRunJobs = jobs.Where(p => activeDbJobs.Contains(p.JobName)).ToList();
    var shouldRemovedJobs = jobs.Where(p => deActiveDbJobs.Contains(p.JobName)).ToList();

    foreach (var item in shouldRunJobs)
    {
        var cronExpression = allAvailableJobDetails.Where(p => p.IsActive && p.Name == item.JobName)
            .Select(p => p.CronExpression)
            .SingleOrDefault();

        if (cronExpression != null)
        {
            recurringJobManager.RemoveIfExists(item.JobName);
            item.Execute(cronExpression);
        }
    }

    foreach (var item in shouldRemovedJobs)
    {
        recurringJobManager.RemoveIfExists(item.JobName);
    }
}