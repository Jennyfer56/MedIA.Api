using MedIA.Api.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MedIA.Api.Infrastructure;

public class MedIaDbContext : DbContext
{
    public MedIaDbContext(DbContextOptions<MedIaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Triagem> Triagens => Set<Triagem>();
    public DbSet<UnidadeSaude> UnidadesSaude => Set<UnidadeSaude>();
    public DbSet<Atendimento> Atendimentos => Set<Atendimento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Paciente>(e =>
        {
            e.Property(p => p.NomeCompleto).IsRequired().HasMaxLength(200);
            e.Property(p => p.Documento).IsRequired().HasMaxLength(20);
            e.HasIndex(p => p.Documento).IsUnique();
        });

        modelBuilder.Entity<UnidadeSaude>(e =>
        {
            e.Property(u => u.Nome).IsRequired().HasMaxLength(150);
            e.Property(u => u.Ocupacao).IsRequired().HasMaxLength(10);
        });

        modelBuilder.Entity<Triagem>(e =>
        {
            e.Property(t => t.SintomasDescricao).IsRequired().HasMaxLength(1000);
            e.Property(t => t.QrCodeHash).IsRequired().HasMaxLength(200);
        });
    }
}

