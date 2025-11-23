using System.ComponentModel.DataAnnotations;

namespace MedIA.Api.Application.DTOs;

public class CriarTriagemRequest
{
    [Required]
    public int PacienteId { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 5)]
    public string SintomasDescricao { get; set; } = null!;
}
