using Microsoft.EntityFrameworkCore;
using RestAPISistemSPK.Models.Entities;

namespace RestAPISistemSPK.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Otorisasi Bawaan
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<MenuRole> MenuRoles { get; set; }

        // Master Data SRM & Kriteria
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Aspek> Aspeks { get; set; }
        public DbSet<Parameter> Parameters { get; set; }

        // Transaksi Keputusan
        public DbSet<Keputusan> Keputusans { get; set; }
        public DbSet<KeputusanDetail> KeputusanDetails { get; set; }
        public DbSet<KeputusanNilai> KeputusanNilais { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relasi Aspek -> Parameters (Cascade Delete)
            modelBuilder.Entity<Parameter>()
                .HasOne(p => p.Aspek)
                .WithMany(a => a.Parameters)
                .HasForeignKey(p => p.AspekId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relasi Keputusan -> KeputusanDetails (Cascade Delete)
            modelBuilder.Entity<KeputusanDetail>()
                .HasOne(kd => kd.Keputusan)
                .WithMany(k => k.KeputusanDetails)
                .HasForeignKey(kd => kd.KeputusanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relasi KeputusanDetail -> KeputusanNilais (Cascade Delete)
            modelBuilder.Entity<KeputusanNilai>()
                .HasOne(kn => kn.KeputusanDetail)
                .WithMany(kd => kd.KeputusanNilais)
                .HasForeignKey(kn => kn.KeputusanDetailId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relasi KeputusanDetail -> Supplier (Restrict Delete)
            modelBuilder.Entity<KeputusanDetail>()
                .HasOne(kd => kd.Supplier)
                .WithMany(s => s.KeputusanDetails)
                .HasForeignKey(kd => kd.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relasi KeputusanNilai -> Parameter (Restrict Delete)
            modelBuilder.Entity<KeputusanNilai>()
                .HasOne(kn => kn.Parameter)
                .WithMany(p => p.KeputusanNilais)
                .HasForeignKey(kn => kn.ParameterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relasi Keputusan -> SupplierPemenang (Set Null on Delete)
            modelBuilder.Entity<Keputusan>()
                .HasOne(k => k.SupplierPemenang)
                .WithMany()
                .HasForeignKey(k => k.SupplierPemenangId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relasi Keputusan -> Aspek (Set Null on Delete)
            modelBuilder.Entity<Keputusan>()
                .HasOne(k => k.Aspek)
                .WithMany()
                .HasForeignKey(k => k.AspekId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seeding Data Default Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, NamaRole = "Administrator SPK", Deskripsi = "Administrator sistem informasi dengan akses penuh" },
                new Role { Id = 2, NamaRole = "Tim Pengambil Keputusan", Deskripsi = "Penilai dan penentu prioritas alternatif terbaik" }
            );

            // Seeding Menu Sidebar Dinamis
            modelBuilder.Entity<MenuRole>().HasData(
                new MenuRole { Id = 1, RoleId = 1, NamaMenu = "Dashboard", Icon = "fas fa-chart-pie", Url = "/Dashboard/Index", Urutan = 1 },
                new MenuRole { Id = 3, RoleId = 1, NamaMenu = "Kriteria & Aspek", Icon = "fas fa-layer-group", Url = "/Kriteria/Index", Urutan = 2 },
                new MenuRole { Id = 4, RoleId = 1, NamaMenu = "Penentuan Prioritas", Icon = "fas fa-scale-balanced", Url = "/Keputusan/Index", Urutan = 3 },
                new MenuRole { Id = 5, RoleId = 1, NamaMenu = "Riwayat Penilaian", Icon = "fas fa-clock-rotate-left", Url = "/Riwayat/Index", Urutan = 4 },
                
                // Role 2: Tim Pengambil Keputusan
                new MenuRole { Id = 6, RoleId = 2, NamaMenu = "Dashboard", Icon = "fas fa-chart-pie", Url = "/Dashboard/Index", Urutan = 1 },
                new MenuRole { Id = 8, RoleId = 2, NamaMenu = "Kriteria & Aspek", Icon = "fas fa-layer-group", Url = "/Kriteria/Index", Urutan = 2 },
                new MenuRole { Id = 9, RoleId = 2, NamaMenu = "Penentuan Prioritas", Icon = "fas fa-scale-balanced", Url = "/Keputusan/Index", Urutan = 3 },
                new MenuRole { Id = 10, RoleId = 2, NamaMenu = "Riwayat Penilaian", Icon = "fas fa-clock-rotate-left", Url = "/Riwayat/Index", Urutan = 4 }
            );
        }
    }
}
