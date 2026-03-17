using dotenv.net;
using Authentication;
using Services;
using Data.Database;
using Microsoft.AspNetCore.Authentication;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "RDMP API";
    settings.Version = "v1";
    settings.Description = "API for RDMP application";
    settings.DocumentName = "v1";
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddDbContext<RdmpContext>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddScoped<ICrawlerService, CrawlerService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoadmapService, RoadmapService>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddControllers();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = "Jwt";
        options.DefaultAuthenticateScheme = "Jwt";
        options.DefaultChallengeScheme = "Jwt";
    })
    .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>("Jwt", _ => { });

builder.Services.AddAuthorization();


var app = builder.Build();

app.MapGet("/health", () => "OK");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}
app.UseCors();

app.Run();
