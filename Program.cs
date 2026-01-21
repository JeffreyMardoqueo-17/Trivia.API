using TriviaGame.Api.Data;
using TriviaGame.Api.Middleware;
using TriviaGame.Api.Services.Interfaces;
using TriviaGame.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TriviaGame.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));

// DAPPER
builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<SpExecutor>();

// SERVICIOS
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();

builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// los cors para que solo me acpte los del puerto del frontendn
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendLocal", policy =>
    {
        policy
            .WithOrigins("https://localhost:5227")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
// CONFIGURAICON DE JWT
var jwtKey = builder.Configuration["Jwt:Key"];
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new Exception("JWT Key no configurada");

if (string.IsNullOrWhiteSpace(issuer))
    throw new Exception("JWT Issuer no configurado");

if (string.IsNullOrWhiteSpace(audience))
    throw new Exception("JWT Audience no configurado");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = issuer,
            ValidAudience = audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ClockSkew = TimeSpan.Zero
        };

        // 🔥 ESTO ES LO QUE TE FALTABA
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/gamehub"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddSignalR();

Console.WriteLine($"JWT KEY: {jwtKey}");
Console.WriteLine($"ISSUER: {issuer}");
Console.WriteLine($"AUDIENCE: {audience}");
Console.WriteLine($"ConnectionString: {builder.Configuration.GetConnectionString("DefaultConnection")}");
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors("FrontendLocal");

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<GameHub>("/gamehub");

app.MapControllers();
app.Run();
