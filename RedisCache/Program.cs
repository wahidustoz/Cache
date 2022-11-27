var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
});

var app = builder.Build();

app.MapControllers();

app.Run();