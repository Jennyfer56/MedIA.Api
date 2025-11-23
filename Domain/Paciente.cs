using System.ComponentModel.DataAnnotations;

namespace MedIA.Api.Domain;

public class Paciente
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string NomeCompleto { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string Documento { get; set; } = null!; // CPF/RG

    [Required]
    public DateTime DataNascimento { get; set; }

    [Required]
    [StringLength(20)]
    public string Telefone { get; set; } = null!;

    [StringLength(250)]
    public string? Endereco { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public ICollection<Triagem> Triagens { get; set; } = new List<Triagem>();
}
