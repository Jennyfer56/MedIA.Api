using MedIA.Api.Domain;
using MedIA.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedIA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly MedIaDbContext _db;

    public PacientesController(MedIaDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Paciente>>> GetAll()
        => await _db.Pacientes.ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Paciente>> GetById(int id)
    {
        var paciente = await _db.Pacientes.FindAsync(id);
        return paciente is null ? NotFound() : Ok(paciente);
    }

    [HttpPost]
    public async Task<ActionResult<Paciente>> Create([FromBody] Paciente paciente)
    {
        _db.Pacientes.Add(paciente);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = paciente.Id }, paciente);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Paciente paciente)
    {
        if (id != paciente.Id)
            return BadRequest();

        _db.Entry(paciente).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var paciente = await _db.Pacientes.FindAsync(id);
        if (paciente is null)
            return NotFound();

        _db.Pacientes.Remove(paciente);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
