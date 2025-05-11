using HumanResource.Grpc.Database;
using HumanResource.Grpc.Interceptors;
using HumanResource.Grpc.Mapping;
using HumanResource.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"!));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;

    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddSignInManager<SignInManager<ApplicationUser>>()
.AddRoleManager<RoleManager<IdentityRole<int>>>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

TypeAdapterConfig.GlobalSettings.Scan(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<IRegister, MapsterConfig>();

builder.Services.AddSingleton<ICurrentUserHelper, CurrentUserHelper>();
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<GrpcUserInterceptor>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
await app.UseMigrationAsync(builder.Environment);

app.MapGrpcService<DepartmentService>();
app.MapGrpcService<ApplicationUserService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
