
using MTMiddleware.Api;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting MTMiddleware API");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host
        .UseDefaultServiceProvider((context, options) =>
        {
            var isDevelopment = context.HostingEnvironment.IsDevelopment();
            options.ValidateScopes = !isDevelopment;
            options.ValidateOnBuild = !isDevelopment;
        });

    builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    {
        loggerConfiguration
        .ReadFrom.Configuration(hostingContext.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProperty("ApplicationName", hostingContext.HostingEnvironment.ApplicationName)
        .Enrich.WithMachineName()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
    });

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    IConfigurationSection appSettingsSection = builder.Configuration.GetSection(ApplicationConstants.AppSettingsKey);

    //string[] AllowedCORSOrigins = builder.Configuration.GetSection(ApplicationConstants.AllowedCORSOrigins).Get<string[]>();

    var appSettings = builder.Configuration.GetSection("AppSettings");
    var jwtSettings = appSettings.GetSection("JwtSettings");

    builder.Services.Configure<AppSettings>(appSettingsSection);
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString, o =>
        {
            o.EnableRetryOnFailure();
        }));

    builder.Services.AddUnitOfWork<AppDbContext>();

    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
    {
        opt.Password.RequiredLength = 7;
        opt.Password.RequireDigit = false;
        opt.Password.RequireUppercase = false;
        opt.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        var Key = Encoding.UTF8.GetBytes(jwtSettings.GetSection("secretKey").Value);

        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["issuer"],
            ValidAudience = jwtSettings["audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetSection("secretKey").Value))
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    });


    builder.Services.AddScoped<JwtHandler>();

    builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
        options.TokenLifespan = TimeSpan.FromHours(2));
    
    builder.Services.AddDbInit();
    builder.Services.AddCoreServices();
    builder.Services.AddSharedServices();
    builder.Services.AddSalesforceCustomerServices();

    var mappingConfig = new MapperConfiguration(mc =>
    {
        mc.AddProfile(new AutoMapperProfile());
    });
    IMapper mapper = mappingConfig.CreateMapper();
    builder.Services.AddSingleton(mapper);

    builder.Services.AddDataProtection();

    builder.Services.AddAutoMapperX();
    builder.Services.AddSwagger();

    builder.Services.AddControllers(config =>
    {
        //config.Filters.Add<BasicAuthenticationFilter>();
    });

    // Register Middleware to intercept exception
    builder.Services.AddTransient<ExceptionLoggingMiddleware>();
    builder.Services.AddTransient<BasicAuthenticationFilter>();

    // Add MediatR
    //builder.Services.AddMediatR(typeof(MediatREntrypoint).Assembly);

    var app = builder.Build();

    // Configure the HTTP request pipeline.

   // app.UseElasticApm(builder.Configuration, new SqlClientDiagnosticSubscriber());

    if (app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
        DbInit.Run(app);
    }
    else
    {

    }

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseMiddleware<ExceptionLoggingMiddleware>();
    app.UseNWebSecurity();

    app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    app.UseSerilogRequestLogging();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information("Shut down completed");
    Log.CloseAndFlush();
}