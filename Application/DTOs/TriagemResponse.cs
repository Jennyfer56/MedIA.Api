using MedIA.Api.Domain;

namespace MedIA.Api.Application.DTOs;

public class TriagemResponse
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public int UnidadeSaudeId { get; set; }
    public string SintomasDescricao { get; set; } = null!;
    public NivelUrgencia NivelUrgencia { get; set; }
    public StatusTriagem Status { get; set; }
    public DateTime DataCriacao { get; set; }
    public string QrCodeBase64 { get; set; } = null!;

    public IDictionary<string, string> Links { get; set; } = new Dictionary<string, string>();
}
