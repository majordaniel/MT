Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting MTMiddleware Background Service v1");

try
{
    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        ApplicationName = typeof(Program).Assembly.FullName,
        ContentRootPath = Directory.GetCurrentDirectory()
    });

    builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
       .WriteTo.Console()
       .ReadFrom.Configuration(hostingContext.Configuration));
    builder.Services.AddHealthChecks();
    builder.Services.AddLogging();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    IConfigurationSection appSettingsSection = builder.Configuration.GetSection(ApplicationConstants.AppSettingsKey);

    builder.Services.Configure<AppSettings>(appSettingsSection);

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "MTMiddleware Background Service", Version = "v1" });
        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    });

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString, o =>
        {
            o.EnableRetryOnFailure();
        }));

    builder.Services.AddUnitOfWork<AppDbContext>();

    builder.Services.AddSharedServices();

    //builder.Services.AddScoped<IBusinessOptionService, BusinessOptionService>();
    //builder.Services.AddScoped<IFBNQuestBankInterestRateService, FBNQuestBankInterestRateService>();
    //builder.Services.AddScoped<IPrincipalBandService, PrincipalBandService>();
    //builder.Services.AddScoped<ITenorService, TenorService>();

    //builder.Services.AddScoped<IBookingRollOverInstructionService, BookingRollOverInstructionService>();
    //builder.Services.AddScoped<IInvestmentBookingRequestService, InvestmentBookingRequestService>();
    //builder.Services.AddScoped<IInvestmentLiquidationRequestService, InvestmentLiquidationRequestService>();

    //builder.Services.AddScoped<IRolloverJobService, RolloverJobService>();
    //builder.Services.AddScoped<IInvestmentBookingJobService, InvestmentBookingJobService>();
    //builder.Services.AddScoped<IInvestmentLiquidationJobService, InvestmentLiquidationJobService>();
    //builder.Services.AddScoped<IHollidayService, HollidayService>();

    var mappingConfig = new MapperConfiguration(mc =>
    {
        mc.AddProfile(new AutoMapperProfile());
    });
    IMapper mapper = mappingConfig.CreateMapper();
    builder.Services.AddSingleton(mapper);

    builder.Services.AddHangfire(hangfire =>
    {
        hangfire.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
        hangfire.UseSimpleAssemblyNameTypeSerializer();
        hangfire.UseRecommendedSerializerSettings();
        hangfire.UseColouredConsoleLogProvider();
        hangfire.UseSqlServerStorage(connectionString,
            new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            });

        var server = new BackgroundJobServer(new BackgroundJobServerOptions
        {
            ServerName = "MTMiddleware-hangfire-test",
        });
    });

    builder.Services.AddHangfireServer();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseSwagger(c => c.SerializeAsV2 = true);
    app.UseSwaggerUI(c => c.SwaggerEndpoint("./v1/swagger.json", "MTMiddleware Background Service v1"));
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapHealthChecks("/health");
    });

    app.UseHangfireDashboard("/dashboard", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorization() }
    });

    GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 2 }  );

    JobRunner.Schedule();

    app.Run();

    Log.Information("MTMiddleware Background Service v1 started successfully");
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
    {
        throw;
    }

    Log.Fatal(ex, "MTMiddleware Background Service v1: Unhandled exception");
}
finally
{
    Log.Information("MTMiddleware Background Service v1: Shut down complete");
    Log.CloseAndFlush();
}