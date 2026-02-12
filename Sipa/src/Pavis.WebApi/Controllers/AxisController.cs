using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pavis.Domain.Entities;
using Pavis.Domain.Interfaces;

namespace Pavis.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AxisController : ControllerBase
{
    private readonly IAxisRepository _repository;
    private readonly ILogger<AxisController> _logger;

    public AxisController(IAxisRepository repository, ILogger<AxisController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _repository.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AxisCreateRequest request)
    {
        var existing = await _repository.FindAsync(a => a.Code == request.Code);
        if (existing.Any())
        {
            return Conflict("Axis with same code already exists");
        }

        var axis = new Axis(request.Code, request.Name, request.Status ?? "ACTIVE");
        await _repository.AddAsync(axis);
        return CreatedAtAction(nameof(GetById), new { id = axis.Id }, axis);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AxisUpdateRequest request)
    {
        var axis = await _repository.GetByIdAsync(id);
        if (axis == null) return NotFound();

        axis.Update(request.Name, request.Status);
        await _repository.UpdateAsync(axis);
        return Ok(axis);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var axis = await _repository.GetByIdAsync(id);
        if (axis == null) return NotFound();

        await _repository.DeleteAsync(axis);
        return NoContent();
    }
}

public class AxisCreateRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Status { get; set; }
}

public class AxisUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Status { get; set; }
}
