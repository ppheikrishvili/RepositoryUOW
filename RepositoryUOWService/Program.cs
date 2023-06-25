using RepositoryUOWInfrastructure.Extension;
using RepositoryUOWInfrastructure.Middleware;
//using RepositoryUOWService.Extensions;

//private AppSettings AppSettings { get; set; }

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<GlobalExceptionHandler>();

IConfigurationRoot configRoot = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json").Build();
builder.Services.AddDbContext(builder.Configuration, configRoot);

builder.AddSerLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//app.UseRequestResponseLogging();

app.UseMiddleware<GlobalExceptionHandler>();

app.Run();
