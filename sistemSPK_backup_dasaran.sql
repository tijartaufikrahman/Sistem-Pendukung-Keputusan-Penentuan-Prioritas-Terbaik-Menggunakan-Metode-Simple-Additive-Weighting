-- =============================================================================
-- BACKUP DASARAN DATABASE SPK: sistemSPK
-- Tanggal Export: 2026-09-27 22:28:57
-- Engine: Microsoft SQL Server
-- =============================================================================

USE [master];
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'sistemSPK')
BEGIN
    CREATE DATABASE [sistemSPK];
END
GO

USE [sistemSPK];
GO

-- -------------------------------------------------------------
-- 1. HAPUS TABEL JIKA SUDAH ADA SEBELUMNYA (CASCADE CLEANUP)
-- -------------------------------------------------------------
IF OBJECT_ID('dbo.KeputusanNilais', 'U') IS NOT NULL DROP TABLE dbo.KeputusanNilais;
IF OBJECT_ID('dbo.KeputusanDetails', 'U') IS NOT NULL DROP TABLE dbo.KeputusanDetails;
IF OBJECT_ID('dbo.Keputusans', 'U') IS NOT NULL DROP TABLE dbo.Keputusans;
IF OBJECT_ID('dbo.Suppliers', 'U') IS NOT NULL DROP TABLE dbo.Suppliers;
IF OBJECT_ID('dbo.Parameters', 'U') IS NOT NULL DROP TABLE dbo.Parameters;
IF OBJECT_ID('dbo.Aspeks', 'U') IS NOT NULL DROP TABLE dbo.Aspeks;
IF OBJECT_ID('dbo.Menu_Roles', 'U') IS NOT NULL DROP TABLE dbo.Menu_Roles;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;
GO

-- -------------------------------------------------------------
-- 2. DEFINISI STRUKTUR TABEL (DDL)
-- -------------------------------------------------------------
CREATE TABLE [dbo].[Roles] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [NamaRole] NVARCHAR(50) NOT NULL,
    [Deskripsi] NVARCHAR(255) NULL
);

CREATE TABLE [dbo].[Users] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [NamaLengkap] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL UNIQUE,
    [PasswordHash] NVARCHAR(255) NOT NULL,
    [RoleId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [ResetPasswordToken] NVARCHAR(255) NULL,
    [ResetPasswordExpiry] DATETIME2(7) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NULL
);

CREATE TABLE [dbo].[Menu_Roles] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [RoleId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE,
    [NamaMenu] NVARCHAR(100) NOT NULL,
    [Icon] NVARCHAR(50) NOT NULL,
    [Url] NVARCHAR(200) NOT NULL,
    [Urutan] INT NOT NULL DEFAULT 0
);

CREATE TABLE [dbo].[Aspeks] (
    [Id] NVARCHAR(450) NOT NULL PRIMARY KEY,
    [KodeAspek] NVARCHAR(50) NOT NULL,
    [NamaAspek] NVARCHAR(100) NOT NULL,
    [Bobot] FLOAT NOT NULL,
    [Keterangan] NVARCHAR(255) NULL
);

CREATE TABLE [dbo].[Parameters] (
    [Id] NVARCHAR(450) NOT NULL PRIMARY KEY,
    [KodeParameter] NVARCHAR(50) NOT NULL,
    [NamaParameter] NVARCHAR(100) NOT NULL,
    [Satuan] NVARCHAR(50) NULL,
    [SifatKriteria] NVARCHAR(50) NOT NULL,
    [Bobot] FLOAT NOT NULL,
    [AspekId] NVARCHAR(450) NOT NULL FOREIGN KEY REFERENCES [dbo].[Aspeks]([Id]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[Suppliers] (
    [Id] NVARCHAR(450) NOT NULL PRIMARY KEY,
    [KodeSupplier] NVARCHAR(50) NOT NULL,
    [NamaSupplier] NVARCHAR(150) NOT NULL,
    [Email] NVARCHAR(100) NULL,
    [Alamat] NVARCHAR(255) NULL,
    [Status] NVARCHAR(50) NOT NULL
);

CREATE TABLE [dbo].[Keputusans] (
    [Id] NVARCHAR(450) NOT NULL PRIMARY KEY,
    [KodeKeputusan] NVARCHAR(50) NOT NULL,
    [NamaKeputusan] NVARCHAR(200) NOT NULL,
    [TanggalKeputusan] DATETIME2(7) NOT NULL,
    [AspekId] NVARCHAR(450) NULL FOREIGN KEY REFERENCES [dbo].[Aspeks]([Id]) ON DELETE SET NULL,
    [Keterangan] NVARCHAR(255) NULL,
    [SupplierPemenangId] NVARCHAR(450) NULL FOREIGN KEY REFERENCES [dbo].[Suppliers]([Id]) ON DELETE SET NULL,
    [SkorTertinggi] DECIMAL(18,4) NULL,
    [Status] NVARCHAR(50) NOT NULL
);

CREATE TABLE [dbo].[KeputusanDetails] (
    [Id] NVARCHAR(450) NOT NULL PRIMARY KEY,
    [KeputusanId] NVARCHAR(450) NOT NULL FOREIGN KEY REFERENCES [dbo].[Keputusans]([Id]) ON DELETE CASCADE,
    [SupplierId] NVARCHAR(450) NULL FOREIGN KEY REFERENCES [dbo].[Suppliers]([Id]) ON DELETE SET NULL,
    [NamaSupplier] NVARCHAR(150) NOT NULL,
    [SkorAkhir] DECIMAL(18,4) NULL,
    [Peringkat] INT NULL,
    [StatusRekomendasi] NVARCHAR(100) NULL,
    [CatatanEvaluasi] NVARCHAR(255) NULL
);

CREATE TABLE [dbo].[KeputusanNilais] (
    [Id] NVARCHAR(450) NOT NULL PRIMARY KEY,
    [KeputusanDetailId] NVARCHAR(450) NOT NULL FOREIGN KEY REFERENCES [dbo].[KeputusanDetails]([Id]) ON DELETE CASCADE,
    [ParameterId] NVARCHAR(450) NOT NULL FOREIGN KEY REFERENCES [dbo].[Parameters]([Id]),
    [NilaiRiil] DECIMAL(18,4) NOT NULL
);
GO

-- -------------------------------------------------------------
-- 3. DATA DASAR DAN SEEDING (DML INSERT)
-- -------------------------------------------------------------
-- Data Tabel: Roles
SET IDENTITY_INSERT [dbo].[Roles] ON;
INSERT INTO [dbo].[Roles] ([Id], [NamaRole], [Deskripsi]) VALUES (1, N'Administrator SPK', N'Administrator sistem informasi dengan akses penuh');
INSERT INTO [dbo].[Roles] ([Id], [NamaRole], [Deskripsi]) VALUES (2, N'Tim Pengambil Keputusan', N'Penilai dan penentu prioritas alternatif terbaik');
SET IDENTITY_INSERT [dbo].[Roles] OFF;
GO

-- Data Tabel: Users
SET IDENTITY_INSERT [dbo].[Users] ON;
INSERT INTO [dbo].[Users] ([Id], [NamaLengkap], [Email], [PasswordHash], [RoleId], [IsActive], [ResetPasswordToken], [ResetPasswordExpiry], [CreatedAt], [UpdatedAt]) VALUES (1, N'Administrator', N'tijartaufikr2@gmail.com', N'$2a$11$gegoZbmvnbfW3rYfJWkVG.Bkq6UHZHi5CRQYMyWGfhg3vDaBYtVuC', 1, 1, NULL, NULL, '2026-09-26 16:22:23.800', NULL);
INSERT INTO [dbo].[Users] ([Id], [NamaLengkap], [Email], [PasswordHash], [RoleId], [IsActive], [ResetPasswordToken], [ResetPasswordExpiry], [CreatedAt], [UpdatedAt]) VALUES (2, N'Manajer Purchasing', N'purchasing@spk.com', N'$2a$11$gegoZbmvnbfW3rYfJWkVG.Bkq6UHZHi5CRQYMyWGfhg3vDaBYtVuC', 2, 1, NULL, NULL, '2026-09-26 16:22:23.800', NULL);
SET IDENTITY_INSERT [dbo].[Users] OFF;
GO

-- Data Tabel: Menu_Roles
SET IDENTITY_INSERT [dbo].[Menu_Roles] ON;
INSERT INTO [dbo].[Menu_Roles] ([Id], [RoleId], [NamaMenu], [Icon], [Url], [Urutan]) VALUES (1, 1, N'Dashboard', N'fas fa-chart-pie', N'/Dashboard/Index', 1);
INSERT INTO [dbo].[Menu_Roles] ([Id], [RoleId], [NamaMenu], [Icon], [Url], [Urutan]) VALUES (3, 1, N'Kriteria & Aspek', N'fas fa-layer-group', N'/Kriteria/Index', 2);
INSERT INTO [dbo].[Menu_Roles] ([Id], [RoleId], [NamaMenu], [Icon], [Url], [Urutan]) VALUES (4, 1, N'Penentuan Prioritas', N'fas fa-scale-balanced', N'/Keputusan/Index', 3);
INSERT INTO [dbo].[Menu_Roles] ([Id], [RoleId], [NamaMenu], [Icon], [Url], [Urutan]) VALUES (5, 1, N'Riwayat Penilaian', N'fas fa-clock-rotate-left', N'/Riwayat/Index', 4);
INSERT INTO [dbo].[Menu_Roles] ([Id], [RoleId], [NamaMenu], [Icon], [Url], [Urutan]) VALUES (6, 2, N'Dashboard', N'fas fa-chart-pie', N'/Dashboard/Index', 1);
INSERT INTO [dbo].[Menu_Roles] ([Id], [RoleId], [NamaMenu], [Icon], [Url], [Urutan]) VALUES (8, 2, N'Kriteria & Aspek', N'fas fa-layer-group', N'/Kriteria/Index', 2);
INSERT INTO [dbo].[Menu_Roles] ([Id], [RoleId], [NamaMenu], [Icon], [Url], [Urutan]) VALUES (9, 2, N'Penentuan Prioritas', N'fas fa-scale-balanced', N'/Keputusan/Index', 3);
INSERT INTO [dbo].[Menu_Roles] ([Id], [RoleId], [NamaMenu], [Icon], [Url], [Urutan]) VALUES (10, 2, N'Riwayat Penilaian', N'fas fa-clock-rotate-left', N'/Riwayat/Index', 4);
SET IDENTITY_INSERT [dbo].[Menu_Roles] OFF;
GO

-- Data Tabel: Aspeks
INSERT INTO [dbo].[Aspeks] ([Id], [KodeAspek], [NamaAspek], [Deskripsi], [Urutan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'570dbb10-4d04-4011-b032-0a8400d96c62', N'ASP-01', N'Komersial & Finansial', N'Penilaian struktur harga, ongkos kirim, dan fleksibilitas tempo pembayaran', 1, '2026-09-26 16:32:53.176', NULL, NULL, NULL);
INSERT INTO [dbo].[Aspeks] ([Id], [KodeAspek], [NamaAspek], [Deskripsi], [Urutan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'ff3666c0-eccb-4e23-bd20-62d1c2baaca1', N'ASP-03', N'Operasional & Logistik', N'Keandalan pengiriman tepat waktu, lead time produksi, dan kelengkapan kiriman', 3, '2026-09-26 16:32:53.176', NULL, NULL, NULL);
INSERT INTO [dbo].[Aspeks] ([Id], [KodeAspek], [NamaAspek], [Deskripsi], [Urutan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'f89ea1d4-c3ab-49ee-b9c4-bbdd66ac7baa', N'ASP-02', N'Kualitas & Mutu Produk', N'Tingkat kesesuaian mutu barang, rasio cacat, dan masa simpan', 2, '2026-09-26 16:32:53.176', NULL, NULL, NULL);
INSERT INTO [dbo].[Aspeks] ([Id], [KodeAspek], [NamaAspek], [Deskripsi], [Urutan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'f825a895-1f41-4a0e-8502-d100508730f4', N'ASP-04', N'Layanan & Hubungan Kemitraan', N'Kecepatan respon sales, penanganan komplain retur, dan fleksibilitas pemesanan', 4, '2026-09-26 16:32:53.176', NULL, NULL, NULL);
INSERT INTO [dbo].[Aspeks] ([Id], [KodeAspek], [NamaAspek], [Deskripsi], [Urutan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'dde04a96-e5ef-4f22-a23f-e7a3d78cecc3', N'ASP-05', N'Kepatuhan & Keberlanjutan', N'Legalitas perizinan, sertifikasi ISO/SNI, dan komitmen keberlanjutan pasokan', 5, '2026-09-26 16:32:53.176', NULL, NULL, NULL);
GO

-- Data Tabel: Parameters
INSERT INTO [dbo].[Parameters] ([Id], [AspekId], [KodeParameter], [NamaParameter], [Satuan], [SifatKriteria], [Bobot], [Keterangan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'e9215903-3226-4c54-8fea-0ef4c473a1f9', N'f89ea1d4-c3ab-49ee-b9c4-bbdd66ac7baa', N'PRM-05', N'Tingkat Cacat Barang (Defect Rate)', N'%', N'Cost', 20.00, N'Persentase barang rusak atau afkir saat inspeksi penerimaan', '2026-09-26 16:32:53.176', NULL, '2026-09-27 06:36:49.464', NULL);
INSERT INTO [dbo].[Parameters] ([Id], [AspekId], [KodeParameter], [NamaParameter], [Satuan], [SifatKriteria], [Bobot], [Keterangan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'1305c194-8f11-4d1f-b11e-11779e40278f', N'570dbb10-4d04-4011-b032-0a8400d96c62', N'PRM-03', N'Ketentuan Pembayaran (Tempo)', N'Hari', N'Benefit', 25.00, N'Lama waktu tempo pelunasan faktur invoice', '2026-09-26 16:32:53.176', NULL, '2026-09-26 18:21:54.624', NULL);
INSERT INTO [dbo].[Parameters] ([Id], [AspekId], [KodeParameter], [NamaParameter], [Satuan], [SifatKriteria], [Bobot], [Keterangan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'009f6882-0c59-4a4d-95fe-26a39341730f', N'f89ea1d4-c3ab-49ee-b9c4-bbdd66ac7baa', N'PRM-06', N'Masa Simpan (Shelf Life)', N'Bulan', N'Benefit', 20.00, N'Sisa durasi masa kedaluwarsa atau keawetan barang', '2026-09-26 16:32:53.176', NULL, '2026-09-27 06:36:49.464', NULL);
INSERT INTO [dbo].[Parameters] ([Id], [AspekId], [KodeParameter], [NamaParameter], [Satuan], [SifatKriteria], [Bobot], [Keterangan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'b019d433-5a28-4d2f-8845-28c997f83194', N'ff3666c0-eccb-4e23-bd20-62d1c2baaca1', N'PRM-09', N'Waktu Tunggu Pesanan (Lead Time)', N'Hari', N'Cost', 33.00, N'Durasi waktu dari penerbitan PO hingga kedatangan barang di gudang', '2026-09-26 16:32:53.176', NULL, NULL, NULL);
INSERT INTO [dbo].[Parameters] ([Id], [AspekId], [KodeParameter], [NamaParameter], [Satuan], [SifatKriteria], [Bobot], [Keterangan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'd590f603-43ce-4ef5-944a-33248d4b9c2a', N'f825a895-1f41-4a0e-8502-d100508730f4', N'PRM-12', N'Kecepatan Penanganan Retur (RMA)', N'Hari', N'Cost', 40.00, N'Lama waktu proses penukaran barang yang rusak/salah kirim', '2026-09-26 16:32:53.176', NULL, '2026-09-26 18:23:02.697', NULL);
INSERT INTO [dbo].[Parameters] ([Id], [AspekId], [KodeParameter], [NamaParameter], [Satuan], [SifatKriteria], [Bobot], [Keterangan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'aa15ea04-ee75-4a72-83f1-6bf6ff18ae3c', N'570dbb10-4d04-4011-b032-0a8400d96c62', N'PRM-02', N'Biaya Pengiriman (Freight Cost)', N'Rp', N'Cost', 25.00, N'Ongkos kirim sampai di gudang pembeli', '2026-09-26 16:32:53.176', NULL, '2026-09-26 18:21:54.625', NULL);
INSERT INTO [dbo].[Parameters] ([Id], [AspekId], [KodeParameter], [NamaParameter], [Satuan], [SifatKriteria], [Bobot], [Keterangan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'e2212f56-0421-4e4a-beda-6dc2c1bd539a', N'ff3666c0-eccb-4e23-bd20-62d1c2baaca1', N'PRM-10', N'Kelengkapan Jumlah Kiriman (In-Full)', N'%', N'Benefit', 33.00, N'Tingkat kesesuaian kuantitas fisik barang dengan Purchase Order', '2026-09-26 16:32:53.176', NULL, NULL, NULL);
INSERT INTO [dbo].[Parameters] ([Id], [AspekId], [KodeParameter], [NamaParameter], [Satuan], [SifatKriteria], [Bobot], [Keterangan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'3d11bd6c-7d68-4408-a43c-77d920cda702', N'570dbb10-4d04-4011-b032-0a8400d96c62', N'PRM-01', N'Harga Satuan (Unit Price)', N'Rp', N'Cost', 25.00, N'Harga penawaran bersih per unit material', '2026-09-26 16:32:53.176', NULL, '2026-09-26 18:21:54.625', NULL);
INSERT INTO [dbo].[Parameters] ([Id], [AspekId], [KodeParameter], [NamaParameter], [Satuan], [SifatKriteria], [Bobot], [Keterangan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'59b7085a-2425-457a-8c1d-8b896de47997', N'570dbb10-4d04-4011-b032-0a8400d96c62', N'PRM-04', N'Diskon Volume Pembelian', N'%', N'Benefit', 25.00, N'Besaran potongan harga untuk pembelian partai besar', '2026-09-26 16:32:53.176', NULL, '2026-09-26 18:21:54.625', NULL);
INSERT INTO [dbo].[Parameters] ([Id], [AspekId], [KodeParameter], [NamaParameter], [Satuan], [SifatKriteria], [Bobot], [Keterangan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'06f9973f-1776-4db6-8834-c92bc99b2321', N'f825a895-1f41-4a0e-8502-d100508730f4', N'PRM-13', N'Fleksibilitas Perubahan Pesanan', N'Skor (1-5)', N'Benefit', 20.00, N'Kemudahan penyesuaian kuantitas atau jadwal kirim darurat', '2026-09-26 16:32:53.176', NULL, '2026-09-26 18:23:02.697', NULL);
INSERT INTO [dbo].[Parameters] ([Id], [AspekId], [KodeParameter], [NamaParameter], [Satuan], [SifatKriteria], [Bobot], [Keterangan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'9a53df7d-dc50-47e4-9855-d67cabc10637', N'ff3666c0-eccb-4e23-bd20-62d1c2baaca1', N'PRM-08', N'Ketepatan Waktu (On-Time Delivery)', N'%', N'Benefit', 34.00, N'Persentase pesanan yang sampai tepat pada tanggal yang dijanjikan', '2026-09-26 16:32:53.176', NULL, NULL, NULL);
INSERT INTO [dbo].[Parameters] ([Id], [AspekId], [KodeParameter], [NamaParameter], [Satuan], [SifatKriteria], [Bobot], [Keterangan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'c5604456-dcd8-4b46-b2d7-d841ee571fc4', N'dde04a96-e5ef-4f22-a23f-e7a3d78cecc3', N'PRM-14', N'Legalitas & Sertifikasi Standar', N'Skor (1-5)', N'Benefit', 100.00, N'Kelengkapan izin usaha resmi, sertifikasi ISO atau standar industri', '2026-09-26 16:32:53.176', NULL, NULL, NULL);
INSERT INTO [dbo].[Parameters] ([Id], [AspekId], [KodeParameter], [NamaParameter], [Satuan], [SifatKriteria], [Bobot], [Keterangan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'3c5a8f29-3998-4c97-a2c0-e5950d9eee7c', N'f825a895-1f41-4a0e-8502-d100508730f4', N'PRM-11', N'Kecepatan Respon Layanan (SLA)', N'Jam', N'Cost', 40.00, N'Waktu tanggap perwakilan supplier membalas pesanan atau kendala', '2026-09-26 16:32:53.176', NULL, '2026-09-26 18:23:02.697', NULL);
INSERT INTO [dbo].[Parameters] ([Id], [AspekId], [KodeParameter], [NamaParameter], [Satuan], [SifatKriteria], [Bobot], [Keterangan], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'bdea00d4-5a33-4294-8a64-f68f74614cec', N'f89ea1d4-c3ab-49ee-b9c4-bbdd66ac7baa', N'PRM-07', N'Konsistensi Spesifikasi Produk', N'Skor (1-5)', N'Benefit', 60.00, N'Keseragaman kualitas antar batch pengiriman barang', '2026-09-26 16:32:53.176', NULL, '2026-09-27 06:36:49.464', NULL);
GO

-- Data Tabel: Suppliers
INSERT INTO [dbo].[Suppliers] ([Id], [KodeSupplier], [NamaSupplier], [Email], [Alamat], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'8f03137e-cd39-4650-b456-04918a9507dd', N'SPL-006', N'CV Sinar Terang Engineering', N'sales@sinarterang.id', N'Jl. Industri Rungkut No. 88, Surabaya', N'Aktif', '2026-09-27 10:47:54.611', NULL, NULL, NULL);
INSERT INTO [dbo].[Suppliers] ([Id], [KodeSupplier], [NamaSupplier], [Email], [Alamat], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'65766494-a4f7-4642-b988-17f745d32b7d', N'SPL-001', N'PT Sumber Makmur Sentosa', N'sales@sumbermakmur.co.id', N'Kawasan Industri Jababeka Blok C-12, Cikarang', N'Aktif', '2026-09-26 16:32:53.176', NULL, NULL, NULL);
INSERT INTO [dbo].[Suppliers] ([Id], [KodeSupplier], [NamaSupplier], [Email], [Alamat], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'f8f0bba9-ece9-423f-9a80-25d7ee1db5ee', N'SPL-003', N'PT Global Mitra Sejahtera', N'kevin@globalmitra.com', N'Ruko Mall Kelapa Gading Blok C, Jakarta Utara', N'Aktif', '2026-09-26 16:32:53.176', NULL, NULL, NULL);
INSERT INTO [dbo].[Suppliers] ([Id], [KodeSupplier], [NamaSupplier], [Email], [Alamat], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'3f1f386b-bcc4-41dc-9207-69181561860f', N'SPL-005', N'PT Berkat Graha Perkasa', N'procurement@berkatgraha.co.id', N'Kawasan Industri MM2100 Blok DD-8, Cikarang Barat', N'Aktif', '2026-09-27 10:47:54.514', NULL, NULL, NULL);
INSERT INTO [dbo].[Suppliers] ([Id], [KodeSupplier], [NamaSupplier], [Email], [Alamat], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'4bc93b5e-5863-4fea-b6bd-ae2d110c99b6', N'SPL-004', N'UD Nusantara Berkah Mandiri', N'nusantaraberkah@gmail.com', N'Jl. Raya Pasar Minggu No. 12, Jakarta Selatan', N'Percobaan', '2026-09-26 16:32:53.176', NULL, NULL, NULL);
INSERT INTO [dbo].[Suppliers] ([Id], [KodeSupplier], [NamaSupplier], [Email], [Alamat], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'bc71a8db-efe5-49b2-9acb-dfca18b81c65', N'SPL-002', N'CV Prima Logistik Abadi', N'info@primalogistik.com', N'Jl. Gatot Subroto No. 45, Jakarta Selatan', N'Aktif', '2026-09-26 16:32:53.176', NULL, NULL, NULL);
GO

-- Data Tabel: Keputusans
INSERT INTO [dbo].[Keputusans] ([Id], [KodeKeputusan], [NamaKeputusan], [TanggalKeputusan], [Keterangan], [AspekId], [SupplierPemenangId], [SkorTertinggi], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'1e13e373-e9c9-4b79-bd40-07a8943bc09e', N'KPTS-20260927-005', N'Evaluasi Pengadaan Perlengkapan Keselamatan Kerja (K3) Proyek 2026', '2026-09-27 08:00:00.000', N'Sesi evaluasi penetapan rekanan penyedia alat pelindung diri (APD) dan helm keselamatan berstandar SNI/ANSI untuk seluruh pekerja proyek.', N'570dbb10-4d04-4011-b032-0a8400d96c62', NULL, NULL, N'Draft', '2026-09-27 10:51:10.040', NULL, NULL, NULL);
INSERT INTO [dbo].[Keputusans] ([Id], [KodeKeputusan], [NamaKeputusan], [TanggalKeputusan], [Keterangan], [AspekId], [SupplierPemenangId], [SkorTertinggi], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'aa6f5e40-369b-4dd3-b408-1f8f19b67c60', N'KPTS-20260918-003', N'Evaluasi Kualitas Bahan Baku Kimia Polimer & Resin Industri', '2026-09-18 10:00:00.000', N'Uji kelayakan mutu sampel material polimer impor untuk lini perakitan presisi tinggi guna menekan angka afkir produksi.', N'f89ea1d4-c3ab-49ee-b9c4-bbdd66ac7baa', N'f8f0bba9-ece9-423f-9a80-25d7ee1db5ee', 0.9520, N'Selesai', '2026-09-27 10:48:39.614', NULL, NULL, NULL);
INSERT INTO [dbo].[Keputusans] ([Id], [KodeKeputusan], [NamaKeputusan], [TanggalKeputusan], [Keterangan], [AspekId], [SupplierPemenangId], [SkorTertinggi], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'079b41cf-3090-45ce-a35c-6ad70249b366', N'KPTS-20260901-001', N'Evaluasi Rekanan Pengadaan Bahan Baku Logam & Baja Konstruksi', '2026-09-01 08:30:00.000', N'Evaluasi penawaran harga satuan, efisiensi ongkos angkut, dan kelonggaran termin tempo pembayaran untuk kebutuhan proyek infrastruktur fase 3.', N'570dbb10-4d04-4011-b032-0a8400d96c62', N'65766494-a4f7-4642-b988-17f745d32b7d', 0.9425, N'Selesai', '2026-09-27 10:48:20.926', NULL, NULL, NULL);
INSERT INTO [dbo].[Keputusans] ([Id], [KodeKeputusan], [NamaKeputusan], [TanggalKeputusan], [Keterangan], [AspekId], [SupplierPemenangId], [SkorTertinggi], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'7b6555ad-06b4-4ab4-935e-92d9907a3963', N'KPTS-20260910-002', N'Seleksi Penyedia Jasa Ekspedisi & Distribusi Antar-Pulau', '2026-09-10 09:15:00.000', N'Penetapan mitra logistik utama untuk rute pengiriman Jawa-Sumatera-Kalimantan dengan tolok ukur ketepatan jadwal dan lead time pemenuhan PO.', N'ff3666c0-eccb-4e23-bd20-62d1c2baaca1', N'bc71a8db-efe5-49b2-9acb-dfca18b81c65', 0.9680, N'Selesai', '2026-09-27 10:48:28.242', NULL, NULL, NULL);
INSERT INTO [dbo].[Keputusans] ([Id], [KodeKeputusan], [NamaKeputusan], [TanggalKeputusan], [Keterangan], [AspekId], [SupplierPemenangId], [SkorTertinggi], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]) VALUES (N'393f2a52-8f95-41d0-9cfe-d71397c0853d', N'KPTS-20260925-004', N'Seleksi Vendor Pengadaan Perangkat Keras Jaringan & Server Enterprise', '2026-09-25 11:00:00.000', N'Evaluasi kapabilitas service level agreement (SLA) respon darurat dan kecepatan resolusi RMA unit server pusat data.', N'f825a895-1f41-4a0e-8502-d100508730f4', N'3f1f386b-bcc4-41dc-9207-69181561860f', 0.9650, N'Selesai', '2026-09-27 10:51:00.183', NULL, NULL, NULL);
GO

-- Data Tabel: KeputusanDetails
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'cea744c1-ee48-41da-bfe1-0000ed69d0e5', N'1e13e373-e9c9-4b79-bd40-07a8943bc09e', N'bc71a8db-efe5-49b2-9acb-dfca18b81c65', NULL, NULL, N'Draft', N'', '2026-09-27 10:51:10.045', NULL);
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'a529e168-2c06-4838-9332-2a1a118b985d', N'079b41cf-3090-45ce-a35c-6ad70249b366', N'3f1f386b-bcc4-41dc-9207-69181561860f', 0.6520, 4, N'Dipertimbangkan', N'Harga penawaran relatif tinggi dan tidak menyediakan potongan diskon pembelian volume besar.', '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'd71f34c8-bf53-4606-9da0-2a44f9b1d386', N'1e13e373-e9c9-4b79-bd40-07a8943bc09e', N'65766494-a4f7-4642-b988-17f745d32b7d', NULL, NULL, N'Draft', N'', '2026-09-27 10:51:10.045', NULL);
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'c09dd635-f878-4c24-a3ae-4c1f0dfdfd13', N'393f2a52-8f95-41d0-9cfe-d71397c0853d', N'8f03137e-cd39-4650-b456-04918a9507dd', 0.7100, 3, N'Dipertimbangkan', N'Waktu respon teknis standar (5.0 jam) dan durasi penanganan unit retur membutuhkan 5 hari kerja.', '2026-09-27 10:51:00.184', NULL);
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'54d5fe80-9ff5-45ee-8074-5918293409e7', N'079b41cf-3090-45ce-a35c-6ad70249b366', N'f8f0bba9-ece9-423f-9a80-25d7ee1db5ee', 0.8160, 2, N'Alternatif Cadangan', N'Pilihan cadangan dengan diskon volume tertinggi (8.5%) namun harga satuan sedikit lebih tinggi.', '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'f1ab709d-f995-4854-bd20-6a7afa089634', N'7b6555ad-06b4-4ab4-935e-92d9907a3963', N'f8f0bba9-ece9-423f-9a80-25d7ee1db5ee', 0.8420, 2, N'Alternatif Cadangan', N'Kesesuaian kuantitas pengiriman sempurna (99.0%) dengan lead time 4 hari kerja.', '2026-09-27 10:48:28.242', NULL);
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'3e63d4c9-6b79-4b41-82c7-6cfbbb063237', N'1e13e373-e9c9-4b79-bd40-07a8943bc09e', N'3f1f386b-bcc4-41dc-9207-69181561860f', NULL, NULL, N'Draft', N'', '2026-09-27 10:51:10.045', NULL);
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'3ca27d49-a461-48da-9e40-725cea235357', N'7b6555ad-06b4-4ab4-935e-92d9907a3963', N'bc71a8db-efe5-49b2-9acb-dfca18b81c65', 0.9680, 1, N'Pilihan Utama', N'Tingkat ketepatan waktu pengiriman mencapai 98.5% dengan lead time tercepat (2 hari) di seluruh rute armada.', '2026-09-27 10:48:28.242', NULL);
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'e14eed6b-f3d3-4caf-ba70-8630eb83421b', N'aa6f5e40-369b-4dd3-b408-1f8f19b67c60', N'4bc93b5e-5863-4fea-b6bd-ae2d110c99b6', 0.6980, 3, N'Dipertimbangkan', N'Spesifikasi cukup memadai namun tingkat cacat relatif lebih tinggi (2.8%).', '2026-09-27 10:48:39.615', NULL);
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'c7f5f6a8-5dac-4804-8412-87097db869cd', N'7b6555ad-06b4-4ab4-935e-92d9907a3963', N'8f03137e-cd39-4650-b456-04918a9507dd', 0.7650, 3, N'Dipertimbangkan', N'Cakupan wilayah cukup baik namun rata-rata lead time pengiriman membutuhkan 5 hari.', '2026-09-27 10:48:28.242', NULL);
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'5af2392d-a7fd-4ebc-b3b4-8c89e505ac11', N'393f2a52-8f95-41d0-9cfe-d71397c0853d', N'f8f0bba9-ece9-423f-9a80-25d7ee1db5ee', 0.8400, 2, N'Alternatif Cadangan', N'Respon layanan cukup sigap (3.0 jam) dengan SLA penggantian unit hardware rata-rata 3 hari kerja.', '2026-09-27 10:51:00.184', NULL);
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'158f9655-94d1-4ea1-9831-b0a5170ebc08', N'393f2a52-8f95-41d0-9cfe-d71397c0853d', N'3f1f386b-bcc4-41dc-9207-69181561860f', 0.9650, 1, N'Pilihan Utama', N'SLA respon darurat tercepat (1.5 jam), proses penanganan garansi/RMA hanya 2 hari, serta fleksibilitas pesanan darurat sangat tinggi.', '2026-09-27 10:51:00.183', NULL);
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'801a191d-2f9f-4f64-b75f-c4d1299b3e51', N'aa6f5e40-369b-4dd3-b408-1f8f19b67c60', N'65766494-a4f7-4642-b988-17f745d32b7d', 0.8350, 2, N'Alternatif Cadangan', N'Masa simpan terpanjang (24 bulan) dengan tingkat cacat terkendali di angka 1.2%.', '2026-09-27 10:48:39.615', NULL);
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'334f73ae-d997-4e0c-a040-cbc33e7a1a11', N'079b41cf-3090-45ce-a35c-6ad70249b366', N'4bc93b5e-5863-4fea-b6bd-ae2d110c99b6', 0.7045, 3, N'Dipertimbangkan', N'Biaya logistik bersaing namun termin pembayaran hanya 14 hari.', '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'ca91b4d8-6dab-4f4c-891c-d637c73a8ed0', N'aa6f5e40-369b-4dd3-b408-1f8f19b67c60', N'f8f0bba9-ece9-423f-9a80-25d7ee1db5ee', 0.9520, 1, N'Pilihan Utama', N'Tingkat cacat sangat rendah (0.4%) dan skor konsistensi spesifikasi laboratorium sempurna (Skor 4.9 dari 5.0).', '2026-09-27 10:48:39.614', NULL);
INSERT INTO [dbo].[KeputusanDetails] ([Id], [KeputusanId], [SupplierId], [SkorAkhir], [Peringkat], [StatusRekomendasi], [CatatanEvaluasi], [CreatedAt], [CreatedBy]) VALUES (N'5fad8ec8-b596-4999-a023-e53ff19f7e48', N'079b41cf-3090-45ce-a35c-6ad70249b366', N'65766494-a4f7-4642-b988-17f745d32b7d', 0.9425, 1, N'Pilihan Utama', N'Unggul mutlak pada efisiensi harga penawaran terendah (Rp 8.500.000/ton) dan termin tempo pembayaran terpanjang (60 hari).', '2026-09-27 10:48:20.927', NULL);
GO

-- Data Tabel: KeputusanNilais
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'54751b44-63b2-47bf-9515-067809371606', N'5fad8ec8-b596-4999-a023-e53ff19f7e48', N'59b7085a-2425-457a-8c1d-8b896de47997', 5.00, '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'e3834adf-5ced-4f7d-abaf-0740a2130dd9', N'5af2392d-a7fd-4ebc-b3b4-8c89e505ac11', N'd590f603-43ce-4ef5-944a-33248d4b9c2a', 3.00, '2026-09-27 10:51:00.184', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'97bb1fa0-51c1-444d-9c37-0f75d7c76bd2', N'c7f5f6a8-5dac-4804-8412-87097db869cd', N'b019d433-5a28-4d2f-8845-28c997f83194', 5.00, '2026-09-27 10:48:28.243', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'b99d8326-4f3b-4e9d-a2ff-117aaaa3fc68', N'3e63d4c9-6b79-4b41-82c7-6cfbbb063237', N'59b7085a-2425-457a-8c1d-8b896de47997', 3.00, '2026-09-27 10:51:10.045', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'14138099-5e95-45c4-a39f-158a52174d27', N'3e63d4c9-6b79-4b41-82c7-6cfbbb063237', N'1305c194-8f11-4d1f-b11e-11779e40278f', 60.00, '2026-09-27 10:51:10.045', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'9366b0f6-a054-4412-bba9-15d61a61b8b4', N'c7f5f6a8-5dac-4804-8412-87097db869cd', N'9a53df7d-dc50-47e4-9855-d67cabc10637', 88.00, '2026-09-27 10:48:28.243', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'08c607d6-1fd6-4c1b-9747-18a5949d60ed', N'a529e168-2c06-4838-9332-2a1a118b985d', N'aa15ea04-ee75-4a72-83f1-6bf6ff18ae3c', 500000.00, '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'a2d4b4fd-3170-4fb4-82b8-2280974b0bd1', N'334f73ae-d997-4e0c-a040-cbc33e7a1a11', N'3d11bd6c-7d68-4408-a43c-77d920cda702', 9800000.00, '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'4962c70b-e4a0-4424-9373-22d319a4e70f', N'c09dd635-f878-4c24-a3ae-4c1f0dfdfd13', N'd590f603-43ce-4ef5-944a-33248d4b9c2a', 5.00, '2026-09-27 10:51:00.184', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'd4f3c36d-9936-497b-8375-299c9d06ba79', N'5af2392d-a7fd-4ebc-b3b4-8c89e505ac11', N'06f9973f-1776-4db6-8834-c92bc99b2321', 4.30, '2026-09-27 10:51:00.184', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'2f75011a-692c-4552-b1b1-2acd05a86378', N'54d5fe80-9ff5-45ee-8074-5918293409e7', N'3d11bd6c-7d68-4408-a43c-77d920cda702', 9200000.00, '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'b6edbc1e-3cfb-4105-a94f-2fc604ef56ac', N'54d5fe80-9ff5-45ee-8074-5918293409e7', N'1305c194-8f11-4d1f-b11e-11779e40278f', 45.00, '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'135f9080-2246-4a44-950e-336ab1dbdb08', N'158f9655-94d1-4ea1-9831-b0a5170ebc08', N'd590f603-43ce-4ef5-944a-33248d4b9c2a', 2.00, '2026-09-27 10:51:00.183', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'7baad472-5e24-4ab8-97ab-34835ed9365b', N'334f73ae-d997-4e0c-a040-cbc33e7a1a11', N'1305c194-8f11-4d1f-b11e-11779e40278f', 14.00, '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'9b41b030-c627-4c2c-b8ed-34ad961ba9a7', N'a529e168-2c06-4838-9332-2a1a118b985d', N'59b7085a-2425-457a-8c1d-8b896de47997', 0.00, '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'5ebf4aae-19b7-4aa1-b69d-384e48c425de', N'ca91b4d8-6dab-4f4c-891c-d637c73a8ed0', N'009f6882-0c59-4a4d-95fe-26a39341730f', 18.00, '2026-09-27 10:48:39.615', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'eafb8ee8-63ee-4fd5-90e1-39d073bad542', N'158f9655-94d1-4ea1-9831-b0a5170ebc08', N'06f9973f-1776-4db6-8834-c92bc99b2321', 4.80, '2026-09-27 10:51:00.183', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'e14faf92-f765-42f2-852b-3ff18ee51f50', N'801a191d-2f9f-4f64-b75f-c4d1299b3e51', N'009f6882-0c59-4a4d-95fe-26a39341730f', 24.00, '2026-09-27 10:48:39.615', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'eacfb9b8-28ed-49a4-87a8-42ccd941ca85', N'c09dd635-f878-4c24-a3ae-4c1f0dfdfd13', N'06f9973f-1776-4db6-8834-c92bc99b2321', 3.70, '2026-09-27 10:51:00.184', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'a4f6259e-690b-4ab6-8f22-432d2a73d075', N'801a191d-2f9f-4f64-b75f-c4d1299b3e51', N'bdea00d4-5a33-4294-8a64-f68f74614cec', 4.20, '2026-09-27 10:48:39.615', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'4fbcd110-605f-42e4-9949-51ad0e58330a', N'c09dd635-f878-4c24-a3ae-4c1f0dfdfd13', N'3c5a8f29-3998-4c97-a2c0-e5950d9eee7c', 5.00, '2026-09-27 10:51:00.184', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'8e8c4f15-51b3-446b-af59-5ae415075e7e', N'cea744c1-ee48-41da-bfe1-0000ed69d0e5', N'59b7085a-2425-457a-8c1d-8b896de47997', 7.50, '2026-09-27 10:51:10.045', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'112c1146-fdaf-48a6-891b-5e801da8f653', N'cea744c1-ee48-41da-bfe1-0000ed69d0e5', N'3d11bd6c-7d68-4408-a43c-77d920cda702', 260000.00, '2026-09-27 10:51:10.045', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'3a954a1f-fdb7-4876-97c9-7129196f0afb', N'd71f34c8-bf53-4606-9da0-2a44f9b1d386', N'aa15ea04-ee75-4a72-83f1-6bf6ff18ae3c', 350000.00, '2026-09-27 10:51:10.045', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'47c8e662-39e6-41d4-8101-71c93d1d8bc4', N'a529e168-2c06-4838-9332-2a1a118b985d', N'1305c194-8f11-4d1f-b11e-11779e40278f', 30.00, '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'4b40ae5f-54e1-4709-8d5c-77594b14a9a8', N'd71f34c8-bf53-4606-9da0-2a44f9b1d386', N'59b7085a-2425-457a-8c1d-8b896de47997', 5.00, '2026-09-27 10:51:10.045', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'9523882e-d9eb-4064-9275-7b519bc7cde0', N'54d5fe80-9ff5-45ee-8074-5918293409e7', N'aa15ea04-ee75-4a72-83f1-6bf6ff18ae3c', 350000.00, '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'0d0a4f70-1982-4888-a2a4-8962106f0d20', N'f1ab709d-f995-4854-bd20-6a7afa089634', N'9a53df7d-dc50-47e4-9855-d67cabc10637', 92.00, '2026-09-27 10:48:28.242', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'bd7b6f85-024d-425e-bcc1-89755a7a0f9f', N'cea744c1-ee48-41da-bfe1-0000ed69d0e5', N'aa15ea04-ee75-4a72-83f1-6bf6ff18ae3c', 250000.00, '2026-09-27 10:51:10.045', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'ba6e1c51-a11b-44f1-8f36-8c3001f5610b', N'5fad8ec8-b596-4999-a023-e53ff19f7e48', N'aa15ea04-ee75-4a72-83f1-6bf6ff18ae3c', 450000.00, '2026-09-27 10:48:20.927', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'9a42b9c1-28c4-4dd5-ad12-8db0183912c2', N'5fad8ec8-b596-4999-a023-e53ff19f7e48', N'3d11bd6c-7d68-4408-a43c-77d920cda702', 8500000.00, '2026-09-27 10:48:20.927', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'11b0e8e6-4a24-49d2-b868-8e1c13d757f6', N'5fad8ec8-b596-4999-a023-e53ff19f7e48', N'1305c194-8f11-4d1f-b11e-11779e40278f', 60.00, '2026-09-27 10:48:20.927', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'ef848e7b-ce56-4422-bf6a-8f51e9cd425c', N'd71f34c8-bf53-4606-9da0-2a44f9b1d386', N'1305c194-8f11-4d1f-b11e-11779e40278f', 45.00, '2026-09-27 10:51:10.045', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'b3cde76c-e970-423e-ae7f-8fb9c3645e43', N'3ca27d49-a461-48da-9e40-725cea235357', N'e2212f56-0421-4e4a-beda-6dc2c1bd539a', 99.20, '2026-09-27 10:48:28.242', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'ae80092c-b2c4-40b2-a3dd-9be41151478d', N'334f73ae-d997-4e0c-a040-cbc33e7a1a11', N'aa15ea04-ee75-4a72-83f1-6bf6ff18ae3c', 300000.00, '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'09c8b992-33ee-4f68-bde7-9e10689ccd9e', N'c7f5f6a8-5dac-4804-8412-87097db869cd', N'e2212f56-0421-4e4a-beda-6dc2c1bd539a', 95.00, '2026-09-27 10:48:28.243', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'73a86944-42bd-46eb-8123-9f59c7a8ddf9', N'd71f34c8-bf53-4606-9da0-2a44f9b1d386', N'3d11bd6c-7d68-4408-a43c-77d920cda702', 245000.00, '2026-09-27 10:51:10.045', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'2847934f-3deb-463d-bbfc-9fafc0def8de', N'801a191d-2f9f-4f64-b75f-c4d1299b3e51', N'e9215903-3226-4c54-8fea-0ef4c473a1f9', 1.20, '2026-09-27 10:48:39.615', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'10fdebd9-9837-4bc3-b470-ad1202492ecb', N'54d5fe80-9ff5-45ee-8074-5918293409e7', N'59b7085a-2425-457a-8c1d-8b896de47997', 8.50, '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'a3c98a37-e075-4a64-a4b1-b391272a76eb', N'a529e168-2c06-4838-9332-2a1a118b985d', N'3d11bd6c-7d68-4408-a43c-77d920cda702', 10500000.00, '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'd2877456-2c76-4fa6-b887-b593552fb2cc', N'3e63d4c9-6b79-4b41-82c7-6cfbbb063237', N'3d11bd6c-7d68-4408-a43c-77d920cda702', 238000.00, '2026-09-27 10:51:10.045', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'5c0a2f2c-2627-41de-b204-b7eeabebb4da', N'ca91b4d8-6dab-4f4c-891c-d637c73a8ed0', N'bdea00d4-5a33-4294-8a64-f68f74614cec', 4.90, '2026-09-27 10:48:39.615', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'496c0d19-70fc-4c7b-a0e9-bf5ce9ba4f2f', N'334f73ae-d997-4e0c-a040-cbc33e7a1a11', N'59b7085a-2425-457a-8c1d-8b896de47997', 3.00, '2026-09-27 10:48:20.928', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'92b70e33-487e-44e1-a480-c48915750b5f', N'3ca27d49-a461-48da-9e40-725cea235357', N'b019d433-5a28-4d2f-8845-28c997f83194', 2.00, '2026-09-27 10:48:28.242', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'e0a6e3be-bd54-49d1-8728-c7d2888e3db9', N'e14eed6b-f3d3-4caf-ba70-8630eb83421b', N'009f6882-0c59-4a4d-95fe-26a39341730f', 12.00, '2026-09-27 10:48:39.615', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'540f2caf-3bf7-40ea-90a1-c92f24289d13', N'f1ab709d-f995-4854-bd20-6a7afa089634', N'e2212f56-0421-4e4a-beda-6dc2c1bd539a', 99.00, '2026-09-27 10:48:28.242', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'99143ab2-e630-4f1a-bd60-c9d1f9eb951c', N'e14eed6b-f3d3-4caf-ba70-8630eb83421b', N'e9215903-3226-4c54-8fea-0ef4c473a1f9', 2.80, '2026-09-27 10:48:39.615', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'9159f0e7-ad79-45b9-b67f-cdf444d591a0', N'cea744c1-ee48-41da-bfe1-0000ed69d0e5', N'1305c194-8f11-4d1f-b11e-11779e40278f', 30.00, '2026-09-27 10:51:10.045', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'efdd5eb5-291c-4b5f-b188-d10987c04e61', N'ca91b4d8-6dab-4f4c-891c-d637c73a8ed0', N'e9215903-3226-4c54-8fea-0ef4c473a1f9', 0.40, '2026-09-27 10:48:39.614', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'63292ea1-5f12-4022-b104-d907ef08f7ad', N'158f9655-94d1-4ea1-9831-b0a5170ebc08', N'3c5a8f29-3998-4c97-a2c0-e5950d9eee7c', 1.50, '2026-09-27 10:51:00.183', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'8a90803c-647c-474f-b21d-e02b9ceb8fce', N'f1ab709d-f995-4854-bd20-6a7afa089634', N'b019d433-5a28-4d2f-8845-28c997f83194', 4.00, '2026-09-27 10:48:28.242', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'31a805ba-f6b6-4f26-9ce3-eaa58a1377e2', N'e14eed6b-f3d3-4caf-ba70-8630eb83421b', N'bdea00d4-5a33-4294-8a64-f68f74614cec', 3.80, '2026-09-27 10:48:39.615', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'ba4d51d5-0cdc-429f-aaf3-f7e1d36f6e4d', N'3e63d4c9-6b79-4b41-82c7-6cfbbb063237', N'aa15ea04-ee75-4a72-83f1-6bf6ff18ae3c', 400000.00, '2026-09-27 10:51:10.045', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'b2a76e77-35c1-4a73-a1ee-f8ec2003b7ec', N'3ca27d49-a461-48da-9e40-725cea235357', N'9a53df7d-dc50-47e4-9855-d67cabc10637', 98.50, '2026-09-27 10:48:28.242', NULL);
INSERT INTO [dbo].[KeputusanNilais] ([Id], [KeputusanDetailId], [ParameterId], [NilaiRiil], [CreatedAt], [CreatedBy]) VALUES (N'1ead95ce-b4ca-47e0-b10c-fc0b79437daf', N'5af2392d-a7fd-4ebc-b3b4-8c89e505ac11', N'3c5a8f29-3998-4c97-a2c0-e5950d9eee7c', 3.00, '2026-09-27 10:51:00.184', NULL);
GO

-- =============================================================
-- SELESAI RESTORE DATABASE sistemSPK
-- =============================================================

