using System.ComponentModel.DataAnnotations;
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
    public async Task<IActionResult> Create([FromBody] CreateScheduleRequest request)
    {
        var result = await _service.CreateSchedule(request);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([Required] Guid id)
    {
        var result = await _service.GetSchedule(id);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value.ToResponse());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> ChangeStatus(Guid id)
    {
        var result = await _service.ChangeStatus(id);
        if (result.IsFailure)
            return BadRequest(result.Error);
            
        return Ok();
    }
}
