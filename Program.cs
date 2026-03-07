using Endpoints;
using dotenv.net;
using Services;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "RDMP API";
    settings.Version = "v1";
    settings.Description = "API for RDMP application";
    settings.DocumentName = "v1";
});
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/health", () => "OK");
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.Run();
