using TriviaGame.Api.Data;
using TriviaGame.Api.Middleware;
using TriviaGame.Api.Services.Interfaces;
using TriviaGame.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(
    builder.Configuration.GetConnectionString("DefaultConnection")
);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));


// Dapper
builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<SpExecutor>();

//registro del services
builder.Services.AddScoped<IUserService, UserService>();

//lo del jwt
builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendLocal", policy =>
    {
        policy
            .WithOrigins("http://localhost:5227")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var jwtKey = builder.Configuration["Jwt:Key"];
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey!)
        ),
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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
app.MapControllers();
app.Run();

