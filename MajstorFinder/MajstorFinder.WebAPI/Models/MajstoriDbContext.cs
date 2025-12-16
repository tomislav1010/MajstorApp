using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MajstorFinder.WebAPI.Models;

public partial class MajstoriDbContext : DbContext
{
    public MajstoriDbContext()
    {
    }

    public MajstoriDbContext(DbContextOptions<MajstoriDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Korisnik> Korisniks { get; set; }

    public virtual DbSet<Lokacija> Lokacijas { get; set; }

    public virtual DbSet<Tvrtka> Tvrtkas { get; set; }

    public virtual DbSet<VrstaRadum> VrstaRada { get; set; }

    public virtual DbSet<Zahtjev> Zahtjevs { get; set; }

    public DbSet<Log> Logs { get; set; }

    public DbSet<AppUser> AppUsers { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=RWA;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False");

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

        modelBuilder.Entity<VrstaRadum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VrstaRad__3214EC079E864058");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Tvrtka).WithMany(p => p.VrstaRada)
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
