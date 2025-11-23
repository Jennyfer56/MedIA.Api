using MedIA.Api.Application.DTOs;
using MedIA.Api.Application.Services;
using MedIA.Api.Domain;
using MedIA.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace MedIA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TriagensController : ControllerBase
{
    private readonly TriagemService _service;
    private readonly MedIaDbContext _db;

    public TriagensController(TriagemService service, MedIaDbContext db)
    {
        _service = service;
        _db = db;
    }

    [HttpPost("analisar")]
    public async Task<ActionResult<TriagemResponse>> Analisar([FromBody] CriarTriagemRequest request)
    {
        try
        {
            var resultado = await _service.CriarTriagemAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
        }
        catch (InvalidOperationException ex)
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Erro de negócio",
                Detail = ex.Message
            };
            return BadRequest(problem);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TriagemResponse>> ObterPorId(int id)
    {
        var triagem = await _db.Triagens.FindAsync(id);
        if (triagem is null)
            return NotFound();

        var resp = new TriagemResponse
        {
            Id = triagem.Id,
            PacienteId = triagem.PacienteId,
            UnidadeSaudeId = triagem.UnidadeSaudeId,
            SintomasDescricao = triagem.SintomasDescricao,
            NivelUrgencia = triagem.NivelUrgencia,
            Status = triagem.Status,
            DataCriacao = triagem.DataCriacao,
            QrCodeBase64 = triagem.QrCodeHash
        };

        return Ok(resp);
    }

    [HttpGet("search")]
    public async Task<ActionResult<object>> Search(
     [FromQuery] int? pacienteId,
     [FromQuery] StatusTriagem? status,
     [FromQuery] int page = 1,
     [FromQuery] int size = 10,
     [FromQuery] string? sort = "dataDesc")
    {
        if (page <= 0 || size <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Parâmetros de paginação inválidos"
            });
        }

        var (itens, total) = await _service.SearchAsync(pacienteId, status, page, size, sort);

        return Ok(new
        {
            total,
            page,
            size,
            sort,
            items = itens
        });
    }

}
