using System.ComponentModel.DataAnnotations;

namespace MedIA.Api.Domain;

public class UnidadeSaude
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = null!;

    [Required]
    [StringLength(250)]
    public string Endereco { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Cidade { get; set; } = null!;

    [Required]
    [StringLength(10)]
    public string Ocupacao { get; set; } = "baixa"; // baixa | media | alta

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public ICollection<Triagem> Triagens { get; set; } = new List<Triagem>();
}
