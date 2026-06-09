using library_management.Data;
using library_management.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

// services
builder.Services.AddScoped<IBooksDataService, BooksDataService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

// jobs
builder.Services.AddHostedService<BookDeptReminder>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6380";
    options.InstanceName = "library-management:";
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
