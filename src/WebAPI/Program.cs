using Infrastructure.Extensions;

#pragma warning disable CA1852

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwagger();

var app = builder.Build();

app.UseSwaggerWithUi();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();