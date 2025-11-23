namespace MedIA.Api.Domain;

public class Triagem
{
    public int Id { get; set; }

    public int PacienteId { get; set; }
    public Paciente Paciente { get; set; } = null!;

    public int UnidadeSaudeId { get; set; }
    public UnidadeSaude UnidadeSaude { get; set; } = null!;

    public string SintomasDescricao { get; set; } = null!;
    public NivelUrgencia NivelUrgencia { get; set; }
    public StatusTriagem Status { get; set; } = StatusTriagem.Aberta;

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public string QrCodeHash { get; set; } = null!;
}
