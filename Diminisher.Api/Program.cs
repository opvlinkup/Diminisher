using Application;
using DotNetEnv;
using Infrastructure;

var root = Directory.GetCurrentDirectory();

while (!File.Exists(Path.Combine(root, ".env")))
{
    var parent = Directory.GetParent(root);
    if (parent == null)
        break;

    root = parent.FullName;
}

var envFile = Path.Combine(root, ".env");

if (File.Exists(envFile))
{
    Env.Load(envFile);
}


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var port = builder.Configuration["HTTP_PORT"] ?? throw new InvalidOperationException("PORT environment variable is required");

if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .WithMethods("GET", "POST", "PUT", "DELETE")
            .AllowCredentials());
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();


var app = builder.Build();
app.UseCors("DefaultCorsPolicy"); 
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();