# SPESIFIKASI KEBUTUHAN FUNGSIONAL SISTEM (FUNCTIONAL PRD)
**Sistem Informasi Pendukung Keputusan Pemilihan Supplier Berbasis SRM Menggunakan Metode AHP**

Dokumen ini menjelaskan secara rinci **apa yang bisa dilakukan pengguna** dan **bagaimana sistem merespons** di setiap halaman/menu aplikasi.

---

## 1. Modul 1: Autentikasi & Navigasi Pengguna

### 1.1 Alur Login & Hak Akses
* **Input Pengguna:** Email dan Password.
* **Proses Sistem:** 
  1. Memverifikasi kredensial pengguna ke database.
  2. Memeriksa status akun (`IsActive`).
  3. Mengenerate JWT Bearer Token yang memuat identitas dan Role pengguna.
  4. Memuat menu sidebar dinamis dari tabel `Menu_Roles` sesuai role pengguna.
* **Output Sistem:** 
  * Jika valid: Pengguna dialihkan ke halaman **Dashboard**.
  * Jika tidak valid: Muncul pesan kesalahan *"Email atau kata sandi tidak valid"*.

### 1.2 Alur Logout
* Pengguna menekan tombol **"Keluar"** di bagian bawah sidebar.
* Sistem menghapus token otentikasi dari *session/cookies* dan mengembalikan pengguna ke halaman Login.

### 1.3 Alur Lupa Password (*Forgot Password & Reset via OTP Email*)
Fitur bawaan sistem untuk pemulihan kata sandi pengguna yang lupa:
* **Tahap 1: Permintaan Reset Password**
  * Pengguna mengklik tautan *"Lupa Password?"* pada halaman Login.
  * Pengguna menginputkan alamat **Email** terdaftar.
  * Sistem memverifikasi keberadaan email di database. Jika terdaftar, sistem men-generate **Kode OTP 6 Digit** unik yang memiliki masa berlaku (*expiry*) selama **5 menit**.
  * Sistem mengirimkan email otomatis berformat HTML resmi ke email pengguna menggunakan SMTP Gmail (*MailKit*).
* **Tahap 2: Verifikasi Kode OTP**
  * Pengguna menginputkan 6 digit kode OTP yang diterima melalui email.
  * Sistem memvalidasi kesesuaian kode OTP dan memastikan masa aktifnya belum kedaluwarsa.
* **Tahap 3: Pembaruan Kata Sandi Baru**
  * Setelah OTP terverifikasi valid, pengguna memasukkan **Password Baru** dan **Konfirmasi Password**.
  * Sistem mengenkripsi (*hash*) password baru menggunakan algoritma **BCrypt**.
  * Sistem memperbarui data pengguna di database dan menghapus/mengosongkan token OTP.
  * Muncul pesan sukses dan pengguna dapat langsung login menggunakan password baru.

---

## 2. Modul 2: Dashboard Eksekutif

Halaman pertama yang dilihat manajer pengadaan setelah login untuk mendapatkan gambaran menyeluruh.

```text
+-----------------------------------------------------------------------------------+
|  [Total Supplier: 12]  |  [Aspek Aktif: 5]  |  [Parameter: 18]  |  [Sesi SPK: 8]  |
+-----------------------------------------------------------------------------------+
| [KARTU SOROTAN KEPUTUSAN TERAKHIR]               | [GRAFIK PERFORMA SUPPLIER]     |
| Sesi: Pengadaan Bahan Baku Q4 (26 Sept 2026)     |                                |
| Pemenang: PT Sumber Makmur (Skor: 0.865)         | [Grafik Radar / Bar Chart      |
| Keunggulan: OTD 98%, Cacat 0.5%, Tempo 30 Hari   |  Kualitas vs Harga vs Waktu]   |
+-----------------------------------------------------------------------------------+
| TABEL DAFTAR REKANAN SUPPLIER TERBAIK (TOP RANKING)                               |
| No | Kode | Nama Supplier | Status | Kategori Utama | Rating | Aksi               |
+-----------------------------------------------------------------------------------+
```

### 2.1 Fitur Fungsional Dashboard
1. **Statistik Instan:**
   * Menampilkan jumlah supplier aktif, jumlah kriteria evaluasi, dan total keputusan yang telah dibuat.
2. **Kartu Keputusan Terakhir (*Decision Highlight*):**
   * Mengambil sesi keputusan terbaru secara otomatis.
   * Menampilkan nama kegiatan, tanggal penetapan, nama supplier pemenang (Juara 1), dan skor kelayakannya.
3. **Pintasan Cepat (*Quick Actions*):**
   * Tombol pintas untuk langsung menjalankan *"Buat Keputusan Pengadaan Baru"*.

---

## 3. Modul 3: Manajemen Data Supplier (SRM Master)

Tempat mengelola seluruh profil perusahaan rekanan pemasok.

### 3.1 Daftar Supplier (Tabel Interaktif)
* Menampilkan seluruh supplier dengan kolom:
  * `No`, `Kode Supplier`, `Nama Perusahaan`, `Kontak Person (PIC)`, `Telepon/WA`, `Email`, `Status` (*Aktif/Percobaan/Tidak Aktif*), dan `Aksi`.
* Dilengkapi fitur pencarian cepat (*search*), pengurutan kolom (*sort*), dan paginasi standar bahasa Indonesia.

### 3.2 Tambah & Edit Supplier (Modal Form)
* **Field Input:**
  * Kode Supplier (otomatis digenerate sistem jika dikosongkan, format: `SPL-YYYYMMDD-XXX`).
  * Nama Perusahaan/Supplier (*Wajib diisi*).
  * Nama Kontak Person / Sales (*Wajib diisi*).
  * Nomor Telepon / WhatsApp (*Wajib diisi*).
  * Alamat Email resmi.
  * Alamat Lengkap Kantor/Gudang.
  * Status Kemitraan (*Pilihan dropdown: Aktif, Percobaan, Tidak Aktif*).
* **Validasi Sistem:** Mencegah duplikasi Kode Supplier dan memastikan format email valid.

### 3.3 Hapus Supplier
* Menampilkan konfirmasi dialog sebelum menghapus.
* *Aturan Bisnis:* Supplier yang sudah pernah memiliki riwayat pada sesi keputusan tidak boleh dihapus keras (*hard delete*), melainkan statusnya diubah menjadi *"Tidak Aktif"* agar integritas data riwayat masa lalu tidak rusak.

---

## 4. Modul 4: Kriteria Evaluasi (Hierarki Aspek & Parameter)

Sistem mengelola kriteria penilaian secara berjenjang 2 tingkat (**Aspek $\rightarrow$ Parameter**).

```text
[ASPEK 1: Komersial & Finansial]  ───> [Parameter 1.1: Harga Satuan (Rp) - Cost]
                                 ───> [Parameter 1.2: Biaya Pengiriman (Rp) - Cost]
                                 ───> [Parameter 1.3: Tempo Pembayaran (Hari) - Benefit]

[ASPEK 2: Kualitas & Mutu]       ───> [Parameter 2.1: Tingkat Cacat (%) - Cost]
                                 ───> [Parameter 2.2: Masa Simpan (Bulan) - Benefit]

[ASPEK 3: Operasional Logistik]  ───> [Parameter 3.1: On-Time Delivery (%) - Benefit]
                                 ───> [Parameter 3.2: Order Lead Time (Hari) - Cost]
```

### 4.1 Tingkat 1: Master Aspek
* Halaman utama menampilkan tabel daftar Aspek (No, Kode Aspek, Nama Aspek, Urutan, Jumlah Parameter, Aksi).
* Fitur Tambah/Edit/Hapus Aspek.
* Tombol aksi utama: **`[Kelola Parameter]`** untuk masuk ke sub-parameter milik aspek tersebut.

### 4.2 Tingkat 2: Master Parameter di Bawah Aspek
* Halaman/Modal menampilkan seluruh parameter di bawah aspek yang dipilih.
* **Field Input Parameter:**
  * Kode Parameter (misal: `PRM-001`).
  * Nama Parameter (misal: *Harga Satuan*, *On-Time Delivery*).
  * Satuan Ukur (Rupiah, Persen, Hari, Skala 1-5).
  * Sifat Kriteria (*Pilihan Radio Button*):
    * **Cost**: Nilai semakin kecil semakin disukai (misal: harga murah, cacat sedikit, waktu tunggu singkat).
    * **Benefit**: Nilai semakin besar semakin disukai (misal: tempo bayar lama, persentase tepat waktu tinggi, skor kualitas tinggi).
  * Keterangan/Panduan penilaian.

---

## 5. Modul 5: Keputusan (Fitur Inti SPK - Proses Pemilihan & AHP)

Dirancang sebagai **1 Halaman Terpadu Berurutan (*Step-by-Step Flow*)** agar manajer tidak bingung:

### Tahap 1: Inisialisasi Kegiatan Pengadaan
* Pengguna mengisi:
  * Kode Keputusan (otomatis: `KPTS-YYYYMMDD-XXX`).
  * Nama Sesi Pengadaan (misal: *"Pengadaan Kain Katun Periode Oktober 2026"*).
  * Tanggal Keputusan.
  * Keterangan/Tujuan Pengadaan.

### Tahap 2: Pemilihan Kandidat Supplier & Parameter Penilaian
* **Pilih Supplier:** Kotak centang (*checkbox*) untuk memilih supplier mana saja yang diundang/dibandingkan (minimal 2 supplier).
* **Pilih Parameter:** Kotak centang memilih parameter apa saja yang relevan untuk sesi ini (misal memilih 4 parameter: *Harga Satuan*, *Tingkat Cacat*, *On-Time Delivery*, *Tempo Bayar*).

### Tahap 3: Pengisian Nilai Penawaran Riil (Fakta Lapangan)
* Sistem otomatis merender tabel input sesuai supplier dan parameter yang dicentang di Tahap 2:

| Kandidat Supplier | Harga Satuan (Rp) | Tingkat Cacat (%) | On-Time Delivery (%) | Tempo Bayar (Hari) |
| :--- | :---: | :---: | :---: | :---: |
| **PT Sumber Rejeki** | `[ 12.000 ]` | `[ 0.8 ]` | `[ 96 ]` | `[ 30 ]` |
| **CV Makmur Mandiri** | `[ 10.500 ]` | `[ 2.5 ]` | `[ 85 ]` | `[ 14 ]` |
| **Toko Berkah Abadi** | `[ 9.800 ]` | `[ 4.0 ]` | `[ 75 ]` | `[ 0 ]` |

### Tahap 4: Pembobotan Prioritas AHP (Pairwise Comparison)
* Sistem menampilkan matriks perbandingan berpasangan antar parameter terpilih menggunakan skala standar Saaty (1 sampai 9):
  * *Contoh pilihan pengguna:* 
    * "Kualitas (Cacat)" dibanding "Harga" $\rightarrow$ **Kualitas 3x lebih penting**.
    * "On-Time Delivery" dibanding "Harga" $\rightarrow$ **OTD 3x lebih penting**.
    * "Tempo Bayar" dibanding "Harga" $\rightarrow$ **Sama penting (skala 1)**.
* Tombol **"Validasi Konsistensi"**:
  * Sistem menghitung nilai **$CR$ (Consistency Ratio)** secara instan.
  * Jika $CR \le 0.10$: Muncul lencana hijau $\checkmark$ **"Konsistensi Valid ($CR = 0.042$)"**.
  * Jika $CR > 0.10$: Muncul peringatan merah $\triangle$ *"Penilaian tidak konsisten ($CR = 0.15$), silakan sesuaikan kembali nilai perbandingan"*.

### Tahap 5: Eksekusi Perhitungan & Tampilan Hasil Pemenang
Ketika pengguna menekan tombol **"Hitung Rekomendasi Keputusan"**:
1. Komputer menghitung normalisasi nilai riil (Cost/Benefit) dan mengalikannya dengan bobot prioritas AHP di memori.
2. Hasil langsung muncul di bawah layar secara elegan:

```text
+------------------------------------------------------------------------------------+
| HASIL PERANGKINGAN KELAYAKAN SUPPLIER                                              |
+------------------------------------------------------------------------------------+
| PERINGKAT 1 (PEMENANG UTAMA): PT SUMBER REJEKI                                     |
| Skor Akhir: 0.8425  |  Status: SANGAT DIREKOMENDASIKAN                             |
| Alasan Kemenangan: Unggul mutlak pada Kualitas (Cacat 0.8%) dan OTD (96%),        |
| serta tempo pembayaran 30 hari yang aman bagi arus kas perusahaan.                 |
+------------------------------------------------------------------------------------+
| TABEL HASIL LENGKAP:                                                               |
| Peringkat | Nama Supplier       | Nilai Akhir | Status Rekomendasi                 |
|    1      | PT Sumber Rejeki    |   0.8425    | Pilihan Utama                      |
|    2      | CV Makmur Mandiri   |   0.6310    | Alternatif Cadangan                |
|    3      | Toko Berkah Abadi   |   0.4520    | Tidak Disarankan (Risiko Tinggi)   |
+------------------------------------------------------------------------------------+
```

### Tahap 6: Simpan Keputusan
* Pengguna menekan tombol **"Simpan Hasil Keputusan"**.
* Seluruh data disimpan ke tabel `Keputusans`, `KeputusanDetails`, dan `KeputusanNilais`.
* Pengguna diarahkan ke halaman **Riwayat Keputusan**.

---

## 6. Modul 6: Riwayat Keputusan (Arsip & Pelaporan)

### 6.1 Daftar Arsip Keputusan
* Menampilkan tabel riwayat sesi pemilihan yang pernah disimpan:
  * `No`, `Kode Keputusan`, `Nama Pengadaan`, `Tanggal`, `Supplier Pemenang (Juara 1)`, `Skor Tertinggi`, `Status`, dan `Aksi`.

### 6.2 Modal Detail Riwayat (Audit & Transparansi)
* Ketika tombol **"Lihat Detail"** diklik, muncul modal besar yang menampilkan:
  1. Identitas pengadaan.
  2. Parameter yang digunakan beserta bobot AHP-nya saat itu.
  3. Tabel nilai penawaran riil masing-masing peserta.
  4. Tabel ranking pemenang juara 1, 2, dan 3.

### 6.3 Cetak Lembar Rekomendasi (*Print / Export*)
* Pengguna dapat menekan tombol **"Cetak Lembar Rekomendasi Pengadaan"**.
* Sistem membuka jendela cetak (*print-friendly view*) yang memuat kop surat perusahaan, tabel ranking, dan kolom tanda tangan persetujuan:
  * Dibuat oleh (*Staff Purchasing*)
  * Disetujui oleh (*Manajer / Direktur Pengadaan*)

---

## 7. Rangkuman Input, Proses, Output Sistem

| Modul | Input Utama | Proses Sistem | Output Utama |
| :--- | :--- | :--- | :--- |
| **Supplier** | Profil & kontak rekanan | Simpan, validasi unik, update status | Database master supplier terdaftar |
| **Kriteria** | Aspek & Parameter (Satuan, Cost/Benefit) | Strukturisasi hierarki master-detail | Daftar tolok ukur evaluasi baku |
| **Keputusan** | - Pilihan supplier & parameter<br>- Nilai penawaran riil<br>- Skala perbandingan AHP (1-9) | - Hitung bobot AHP & uji CR<br>- Normalisasi matriks Cost/Benefit<br>- Akumulasi skor kelayakan | - Peringkat Juara 1, 2, 3<br>- Penjelasan alasan kemenangan<br>- Rekomendasi keputusan belanja |
| **Riwayat** | Pencarian / filter tanggal sesi | Query data relasi keputusan lampau | Tampilan detail audit & cetak lembar persetujuan |
