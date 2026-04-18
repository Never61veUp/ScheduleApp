using Microsoft.AspNetCore.Mvc;
using ScheduleApp.API.Contracts;
using ScheduleApp.Application.Extension;
using ScheduleApp.Application.Services;
using ScheduleApp.Contracts.Contracts;

namespace ScheduleApp.API.Controllers;

[ApiController]
[Route("api/schedule")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _service;

    public ScheduleController(IScheduleService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateScheduleRequest request)
    {
        var result = _service.CreateSchedule(request);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value.ToResponse());
    }

    [HttpGet]
    public IActionResult Get()
    {
        var result = _service.GetSchedule();

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value.ToResponse());
    }
}
