using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class LogsController : ControllerBase
{
    private readonly ILogger<LogsController> _logger;

    public LogsController(ILogger<LogsController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult Levels()
    {
        var ex = new InvalidCastException("Не смог привести тип int к типу string");

        _logger.LogInformation("Обычный лог");
        _logger.LogWarning("Внимание! Подозрительное значение");
        _logger.LogError(ex, "Произошла ошибка. Смотри exception");
        
        return Ok();
    }

    [HttpGet]
    public IActionResult Test()
    {
        var json = new
        {
            Text = $"Text_{DateTime.UtcNow.Millisecond}",
            Date = DateTime.UtcNow,
            Amount = DateTime.UtcNow.Millisecond,
            NestedObject = new
            {
                Text = $"NestedObject_{DateTime.UtcNow.Date}",
                Count = DateTime.UtcNow.Millisecond
            }
        };

        _logger.LogInformation(
#pragma warning disable CA2254
            $"Обычное логирование Text = {json.Text},  Date = {json.Date}, Amount = {json.Amount}");
#pragma warning restore CA2254
      
        _logger.LogInformation(
            "Структурное логирование Text = {Text}, Date = {Date}, Amount = {Amount}", json.Text,
            json.Date, json.Amount);

        return Ok();
    }
}