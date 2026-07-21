using System.IdentityModel.Tokens.Jwt;
using System.Text;
using library_management.Data;
using library_management.Filters;
using library_management.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExecutionTimeFilter>();
});
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// services
builder.Services.AddScoped<IBooksDataService, BooksDataService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
builder.Services.AddSingleton<IHybridCacheService, HybridCacheService>();
builder.Services.AddScoped<ExecutionTimeFilter>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher<LibraryUser>, PasswordHasher<LibraryUser>>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserClaimsService, UserClaimsService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// authentiacation
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwt = builder.Configuration.GetSection("Jwt");
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwt["Issuer"],

        ValidateAudience = true,
        ValidAudience = jwt["Audience"],

        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"])),

        ClockSkew = TimeSpan.Zero,

    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AppAuthorizationPolicies.IsLibraryUser, policy =>
        policy.AddRequirements(new LibraryUserAccessRequirment())
    );
});

builder.Services.AddScoped<IAuthorizationHandler, LibraryUserAccessHandler>();

// jobs
// builder.Services.AddHostedService<BookDeptReminder>();

//
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6380";
    options.InstanceName = "library-management:";
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseMiddleware<MaintenanceMiddleware>();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<ApiLoggerMiddleware>();
app.UseMiddleware<IdempotencyMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
