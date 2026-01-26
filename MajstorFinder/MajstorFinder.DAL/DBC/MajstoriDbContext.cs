using System;
using System.Collections.Generic;
using MajstorFinder.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MajstorFinder.DAL.DBC;

public partial class MajstoriDbContext : DbContext
{
    // private DbSet<Lokacija> lokacijas;

    public MajstoriDbContext()
    {
    }

    public MajstoriDbContext(DbContextOptions<MajstoriDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Korisnik> Korisniks { get; set; } = null!;
    public virtual DbSet<Lokacija> Lokacijas { get; set; } = null!;
    public virtual DbSet<Tvrtka> Tvrtkas { get; set; } = null!;
    public virtual DbSet<VrstaRada> VrstaRadas { get; set; } = null!;
    public virtual DbSet<Zahtjev> Zahtjevs { get; set; } = null!;

    // Manually added entities (not scaffolded)
    public DbSet<Log> Logs { get; set; } = null!;
    public DbSet<AppUser> AppUsers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Korisnik>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Korisnik__3214EC070132BD28");

            entity.ToTable("Korisnik");

            entity.HasIndex(e => e.Email, "UQ__Korisnik__A9D10534CDEDA70C").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);
        });

        modelBuilder.Entity<Lokacija>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lokacija__3214EC071D5CC8F4");

            entity.ToTable("Lokacija");

            entity.HasIndex(e => e.Name, "UQ__Lokacija__737584F69576655F").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Tvrtka>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tvrtka__3214EC07F8E9E346");

            entity.ToTable("Tvrtka");

            entity.HasIndex(e => e.Name, "UQ__Tvrtka__737584F65F117E69").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);

            // M:N Tvrtka <-> Lokacija through table TvrtkaLokacija
            entity.HasMany(d => d.Lokacijas).WithMany(p => p.Tvrtkas)
                .UsingEntity<Dictionary<string, object>>(
                    "TvrtkaLokacija",
                    r => r.HasOne<Lokacija>().WithMany()
                        .HasForeignKey("LokacijaId")
                        .HasConstraintName("FK_TvrtkaLokacija_Lokacija"),
                    l => l.HasOne<Tvrtka>().WithMany()
                        .HasForeignKey("TvrtkaId")
                        .HasConstraintName("FK_TvrtkaLokacija_Tvrtka"),
                    j =>
                    {
                        j.HasKey("TvrtkaId", "LokacijaId");
                        j.ToTable("TvrtkaLokacija");
                    });
        });

        modelBuilder.Entity<VrstaRada>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VrstaRad__3214EC079E864058");

            entity.ToTable("VrstaRada");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Tvrtka).WithMany(p => p.VrstaRadas)
                .HasForeignKey(d => d.TvrtkaId)
                .HasConstraintName("FK_VrstaRada_Tvrtka");
        });

        modelBuilder.Entity<Zahtjev>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Zahtjev__3214EC0701684E8D");

            entity.ToTable("Zahtjev");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Korisnik).WithMany(p => p.Zahtjevs)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Zahtjev_Korisnik");

            entity.HasOne(d => d.Tvrtka).WithMany(p => p.Zahtjevs)
                .HasForeignKey(d => d.TvrtkaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Zahtjev_Tvrtka");

            entity.HasOne(d => d.VrstaRada).WithMany(p => p.Zahtjevs)
                .HasForeignKey(d => d.VrstaRadaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Zahtjev_VrstaRada");
        });

        // Manual tables mapping
        modelBuilder.Entity<Log>(entity =>
        {
            entity.ToTable("Logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Level).HasMaxLength(50);
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.Timestamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("AppUser");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(200);
           // entity.Property(e => e.Role).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}