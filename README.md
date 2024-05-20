# Supplier Maintenance Web

Aplikasi web untuk memaintain data Supplier dari sebuah perusahaan  

## Requirements
- ASP.NET 
- SQL Server

## Konfigurasi Proyek

### 1. Clone Repository
```sh
git clone https://github.com/username/supplier-maintenance-web.git
cd supplier-maintenance-web
```
### 2. Restore Database
- Download file backup database dari [link ini](https://drive.google.com/file/d/17FJAFWaSS6Mk1XDU8bcATxDJKs6k2mmh/view?usp=sharing).
- Buka SQL Server Management Studio (SSMS).
- Kik kanan pada "Databases" dan pilih "Restore Database...".
- Pilih "Device" dan cari file backup (.bak) yang sudah diunduh.
- Klik "OK" untuk memulai proses restore.
### 3. Konfigurasi Koneksi
Pastikan string koneksi database di appsettings.json sudah diatur dengan benar. Contoh untuk appsettings.json:
```sh
{
  "ConnectionStrings": {
    "SupplierMaintenance": "Server=your_server;Database=your_database;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```
### 4. Running Project
Untuk menjalankan proyek, ikuti langkah-langkah berikut:
- Buka folder proyek dengan Visual Studio.
- Pulihkan dependensi NuGet dengan klik kanan pada Solution dan pilih "Restore NuGet Packages".
- Jalankan proyek dengan menekan F5 atau klik tombol "Run".
