using Microsoft.EntityFrameworkCore;
using SothbeysKillerApi.Context;
using SothbeysKillerApi.ExceptionHandlers;
using SothbeysKillerApi.Repository;
using SothbeysKillerApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Services.AddDbContext<UserDBContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DB"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddTransient<IAuctionService, DbAuctionService>();
builder.Services.AddTransient<IUserService, DbUserService>();

builder.Services.AddScoped<IUnitOfWork, EFUnitOfWork>();


builder.Services.AddExceptionHandler<UserValidationExceptionHandler>();
builder.Services.AddExceptionHandler<UserNotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<UserUnautorizedExceptionHandler>();
builder.Services.AddExceptionHandler<ServerExceptionsHandler>();

builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.Run();