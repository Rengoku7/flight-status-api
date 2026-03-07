using System.Text;
using FlightStatus.Application;
using FlightStatus.Application.Auth;
using FlightStatus.Infrastructure;
using FlightStatus.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day);
});

builder.Services.AddHttpContextAccessor();
builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection(JwtConfiguration.SectionName));
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var config = builder.Configuration.GetSection(JwtConfiguration.SectionName).Get<JwtConfiguration>()
            ?? throw new InvalidOperationException("Jwt section not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret));

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidIssuer = config.Issuer,
            ValidAudience = config.Audience,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers(o => o.Filters.Add<FlightStatus.Api.Filters.ExceptionFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Flight Status API", Version = "v1" });
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "FlightStatus.Api.xml");
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT. Укажи: Bearer {token}",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    try
    {
        await next(context);
    }
    catch (ValidationException ex)
    {
        logger.LogWarning("Валидация: {Errors}", string.Join("; ", ex.Errors.Select(e => e.ErrorMessage)));
        context.Response.StatusCode = 400;
        var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
        await context.Response.WriteAsJsonAsync(new { success = false, title = "Ошибка валидации", errors });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Необработанное исключение");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { success = false, title = "Ошибка сервера" });
    }
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbSeeder.SeedAsync(db);
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flight Status API v1"));
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
