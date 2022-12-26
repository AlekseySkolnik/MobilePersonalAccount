using Infrastructure.Extensions;
using Serilog;

#pragma warning disable CA1852

var builder = WebApplication
    .CreateBuilder(args)
    .AddSerilog();

builder.Services.AddControllers();
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwagger();

var app = builder.Build();

app.UseSwaggerWithUi();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();