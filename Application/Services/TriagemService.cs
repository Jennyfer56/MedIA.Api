using System.Security.Cryptography;
using System.Text;
using MedIA.Api.Application.DTOs;
using MedIA.Api.Domain;
using MedIA.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MedIA.Api.Application.Services;

public class TriagemService
{
    private readonly MedIaDbContext _db;

    public TriagemService(MedIaDbContext db)
    {
        _db = db;
    }

    public async Task<TriagemResponse> CriarTriagemAsync(CriarTriagemRequest request)
    {
        var paciente = await _db.Pacientes.FindAsync(request.PacienteId);
        if (paciente is null)
            throw new InvalidOperationException("Paciente não encontrado.");

        var unidade = await EscolherUnidadePorLocalizacaoAsync(paciente);
        if (unidade is null)
            throw new InvalidOperationException("Nenhuma unidade de saúde configurada.");

        var nivel = CalcularUrgencia(request.SintomasDescricao);

        var triagem = new Triagem
        {
            PacienteId = paciente.Id,
            UnidadeSaudeId = unidade.Id,
            SintomasDescricao = request.SintomasDescricao,
            NivelUrgencia = nivel,
            Status = StatusTriagem.Aberta,
            DataCriacao = DateTime.UtcNow,
            QrCodeHash = GerarHashQr(paciente.Id, unidade.Id, DateTime.UtcNow)
        };

        _db.Triagens.Add(triagem);
        await _db.SaveChangesAsync();

        return MapToResponse(triagem);
    }

    public async Task<(IEnumerable<TriagemResponse> Itens, int Total)> SearchAsync(
    int? pacienteId,
    StatusTriagem? status,
    int page,
    int size,
    string? sort)
    {
        var query = _db.Triagens.AsQueryable();

        if (pacienteId.HasValue)
            query = query.Where(t => t.PacienteId == pacienteId.Value);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        // ordenação
        sort = (sort ?? "dataDesc").ToLowerInvariant();

        query = sort switch
        {
            "dataasc" => query.OrderBy(t => t.DataCriacao),
            "datadesc" => query.OrderByDescending(t => t.DataCriacao),
            _ => query.OrderByDescending(t => t.DataCriacao)
        };

        var total = await query.CountAsync();

        var itens = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return (itens.Select(MapToResponse), total);
    }


    // -------- regras de negócio --------

    private NivelUrgencia CalcularUrgencia(string sintomas)
    {
        var s = sintomas.ToLowerInvariant();
        int score = 0;

        if (s.Contains("dor no peito") || s.Contains("peito"))
            score += 3;
        if (s.Contains("falta de ar") || s.Contains("respirar") || s.Contains("respiração"))
            score += 3;
        if (s.Contains("febre"))
            score += 2;
        if (s.Contains("vomito") || s.Contains("vômito"))
            score += 1;
        if (s.Contains("desmaio") || s.Contains("inconsciente"))
            score += 4;

        return score switch
        {
            >= 7 => NivelUrgencia.Critica,
            >= 5 => NivelUrgencia.Alta,
            >= 3 => NivelUrgencia.Media,
            _ => NivelUrgencia.Baixa
        };
    }

    private async Task<UnidadeSaude?> EscolherUnidadePorLocalizacaoAsync(Paciente paciente)
    {
        var unidades = await _db.UnidadesSaude.ToListAsync();
        if (!unidades.Any())
            return null;

        return unidades
            .OrderBy(u => CalcularDistancia(
                paciente.Latitude, paciente.Longitude,
                u.Latitude, u.Longitude))
            .First();
    }

    private static double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = lat1 - lat2;
        var dLon = lon1 - lon2;
        return Math.Sqrt(dLat * dLat + dLon * dLon);
    }

    private static string GerarHashQr(int pacienteId, int unidadeId, DateTime data)
    {
        var texto = $"{pacienteId}:{unidadeId}:{data:O}";
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(texto));
        return Convert.ToBase64String(bytes);
    }

    private TriagemResponse MapToResponse(Triagem t)
    {
        return new TriagemResponse
        {
            Id = t.Id,
            PacienteId = t.PacienteId,
            UnidadeSaudeId = t.UnidadeSaudeId,
            SintomasDescricao = t.SintomasDescricao,
            NivelUrgencia = t.NivelUrgencia,
            Status = t.Status,
            DataCriacao = t.DataCriacao,
            QrCodeBase64 = t.QrCodeHash,
            Links = new Dictionary<string, string>
            {
                ["self"] = $"/api/triagens/{t.Id}",
                ["cancelar"] = $"/api/triagens/{t.Id}/cancelar"
            }
        };
    }
}
