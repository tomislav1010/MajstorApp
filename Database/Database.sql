/* =========================
   MAJSTORFINDER - FINAL DB SCRIPT
   (validator friendly)
   ========================= */
   create database Applikacija
/* =========================
   TVRTKA
   ========================= */
CREATE TABLE Tvrtka (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Tvrtka PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL CONSTRAINT UQ_Tvrtka_Name UNIQUE,
    Description NVARCHAR(500) NOT NULL,
    Phone NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL
);

/* =========================
   VRSTA RADA (1-N prema Tvrtka)
   ========================= */
CREATE TABLE VrstaRada (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_VrstaRada PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    TvrtkaId INT NOT NULL,
    CONSTRAINT FK_VrstaRada_Tvrtka
        FOREIGN KEY (TvrtkaId) REFERENCES Tvrtka(Id)
        ON DELETE CASCADE
);

/* =========================
   LOKACIJA
   ========================= */
CREATE TABLE Lokacija (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Lokacija PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL CONSTRAINT UQ_Lokacija_Name UNIQUE
);

/* =========================
   TVRTKA - LOKACIJA (M-N)
   ========================= */
CREATE TABLE TvrtkaLokacija (
    TvrtkaId INT NOT NULL,
    LokacijaId INT NOT NULL,
    CONSTRAINT PK_TvrtkaLokacija PRIMARY KEY (TvrtkaId, LokacijaId),
    CONSTRAINT FK_TvrtkaLokacija_Tvrtka
        FOREIGN KEY (TvrtkaId) REFERENCES Tvrtka(Id)
        ON DELETE CASCADE,
    CONSTRAINT FK_TvrtkaLokacija_Lokacija
        FOREIGN KEY (LokacijaId) REFERENCES Lokacija(Id)
        ON DELETE CASCADE
);

/* =========================
   APP USER (login/registracija)
   PasswordHash 32 bytes (SHA-256)
   PasswordSalt 16 bytes
   ========================= */
CREATE TABLE AppUser (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppUser PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL CONSTRAINT UQ_AppUser_Username UNIQUE,
    Email NVARCHAR(100) NOT NULL CONSTRAINT UQ_AppUser_Email UNIQUE,
    PasswordHash VARBINARY(32) NOT NULL,
    PasswordSalt VARBINARY(16) NOT NULL,
    Iterations INT NOT NULL,
    CreatedAt DATETIME NOT NULL CONSTRAINT DF_AppUser_CreatedAt DEFAULT (GETDATE()),
    Isadmin Bit not null default 0
);

/* =========================
   ZAHTJEV
   (KorisnikId je FK na AppUser.Id
    da ti ne puca postojeæi C#)
   ========================= */
CREATE TABLE Zahtjev (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Zahtjev PRIMARY KEY,
    Description NVARCHAR(500) NOT NULL,
    DateCreated DATETIME NOT NULL CONSTRAINT DF_Zahtjev_DateCreated DEFAULT (GETDATE()),
    Status NVARCHAR(50) NOT NULL CONSTRAINT DF_Zahtjev_Status DEFAULT ('Poslano'),
    KorisnikId INT NOT NULL,
    TvrtkaId INT NOT NULL,
    VrstaRadaId INT NOT NULL,
    CONSTRAINT FK_Zahtjev_AppUser
        FOREIGN KEY (KorisnikId) REFERENCES AppUser(Id),
    CONSTRAINT FK_Zahtjev_Tvrtka
        FOREIGN KEY (TvrtkaId) REFERENCES Tvrtka(Id),
    CONSTRAINT FK_Zahtjev_VrstaRada
        FOREIGN KEY (VrstaRadaId) REFERENCES VrstaRada(Id)
);

/* =========================
   LOGS
   ========================= */
CREATE TABLE Logs (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Logs PRIMARY KEY,
    Timestamp DATETIME NOT NULL CONSTRAINT DF_Logs_Timestamp DEFAULT (GETDATE()),
    Level NVARCHAR(50) NOT NULL,
    Message NVARCHAR(500) NOT NULL
);


/* =========================
   Korisnik
   ========================= */
create table Korisnik(
Id int identity(1,1) primary key,
Name nvarchar(100) not null,
Email Nvarchar(100) not null unique,
Phone nvarchar(50) not null
);

