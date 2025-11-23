namespace MedIA.Api.Domain;

public class Atendimento
{
    public int Id { get; set; }

    public int TriagemId { get; set; }
    public Triagem Triagem { get; set; } = null!;

    public DateTime InicioAtendimento { get; set; }
    public DateTime? FimAtendimento { get; set; }
    public string? Observacoes { get; set; }
}
