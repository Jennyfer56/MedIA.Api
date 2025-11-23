using MedIA.Api.Domain;
using MedIA.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedIA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnidadesSaudeController : ControllerBase
{
    private readonly MedIaDbContext _db;

    public UnidadesSaudeController(MedIaDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnidadeSaude>>> GetAll()
        => await _db.UnidadesSaude.ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UnidadeSaude>> GetById(int id)
    {
        var unidade = await _db.UnidadesSaude.FindAsync(id);
        return unidade is null ? NotFound() : Ok(unidade);
    }

    [HttpPost]
    public async Task<ActionResult<UnidadeSaude>> Create([FromBody] UnidadeSaude unidade)
    {
        _db.UnidadesSaude.Add(unidade);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = unidade.Id }, unidade);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UnidadeSaude unidade)
    {
        if (id != unidade.Id)
            return BadRequest();

        _db.Entry(unidade).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Atualizar ocupação via IoT / Node-RED
    public record AtualizarOcupacaoRequest(string Ocupacao);

    [HttpPatch("{id:int}/ocupacao")]
    public async Task<IActionResult> AtualizarOcupacao(int id, [FromBody] AtualizarOcupacaoRequest req)
    {
        var unidade = await _db.UnidadesSaude.FindAsync(id);
        if (unidade is null)
            return NotFound();

        unidade.Ocupacao = req.Ocupacao;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var unidade = await _db.UnidadesSaude.FindAsync(id);
        if (unidade is null)
            return NotFound();

        _db.UnidadesSaude.Remove(unidade);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
