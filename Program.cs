using Endpoints;
using dotenv.net;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "RDMP API";
    settings.Version = "v1";
    settings.Description = "API for RDMP application";
    settings.DocumentName = "v1";
});

var app = builder.Build();

app.MapAIEndpoints();
app.MapGet("/health", () => "OK");

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.Run();
