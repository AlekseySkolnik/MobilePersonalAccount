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
    public IActionResult TestLogs()
    {
        var ex = new InvalidCastException("Не смог привести тип int к типу string");

        // Обычные логи
        _logger.LogInformation( "Обычный лог");
        _logger.LogWarning("Внимание! Подозрительное значение");
        _logger.LogError("Произошла ошибка");
        _logger.LogError(ex, "Произошла ошибка. Смотри exception");

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

        _logger.LogInformation($"Обычное логирование с записью значения Text = '{json.Text}'");
        _logger.LogInformation(
            "Структурное логирование лог с записью значения Text = {Text}, Date = {Date}, Amount = {Amount}", json.Text,
            json.Date, json.Amount);

        _logger.LogInformation($"Обычное логирование вложенного объекта json.NestedObject = '{json.NestedObject}'");
        _logger.LogInformation("Структурное логирование вложенного объекта json.NestedObject = '{NestedObject}'",
            json.NestedObject);

        return Ok();
    }
}