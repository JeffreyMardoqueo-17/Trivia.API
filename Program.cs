using TriviaGame.Api.Data;
using TriviaGame.Api.Middleware;
using TriviaGame.Api.Middleware;
using AutoMapper;
using AutoMapper.Execution;
using TriviaGame.Api.Services.Interfaces;
using TriviaGame.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));


// Registramos DapperContext y Services
builder.Services.AddSingleton<SpExecutor>();
builder.Services.AddSingleton<DapperContext>(sp =>
    new DapperContext(new ConfigurationBuilder()
     
    )
);
//registro del services
builder.Services.AddScoped<IUserService, UserService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
