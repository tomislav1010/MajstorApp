create database RWA


-- =========================
-- TVRTKA (primarni entitet)
-- =========================
CREATE TABLE Tvrtka (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500) NOT NULL,
    Phone NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL
);

-- =========================
-- VRSTA RADA (1-N)
-- =========================
CREATE TABLE VrstaRada (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    TvrtkaId INT NOT NULL,
    CONSTRAINT FK_VrstaRada_Tvrtka
        FOREIGN KEY (TvrtkaId) REFERENCES Tvrtka(Id)
        ON DELETE CASCADE
);

-- =========================
-- LOKACIJA (M-N)
-- =========================
CREATE TABLE Lokacija (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

-- =========================
-- TVRTKA - LOKACIJA (bridge)
-- =========================
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

-- =========================
-- KORISNIK
-- =========================
CREATE TABLE Korisnik (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Phone NVARCHAR(50) NOT NULL
);

-- =========================
-- ZAHTJEV (korisnikov M-N)
-- =========================
CREATE TABLE Zahtjev (
    Id INT IDENTITY PRIMARY KEY,
    Description NVARCHAR(500) NOT NULL,
    DateCreated DATETIME NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(50) NOT NULL,
    KorisnikId INT NOT NULL,
    TvrtkaId INT NOT NULL,
    VrstaRadaId INT NOT NULL,
    CONSTRAINT FK_Zahtjev_Korisnik
        FOREIGN KEY (KorisnikId) REFERENCES Korisnik(Id),
    CONSTRAINT FK_Zahtjev_Tvrtka
        FOREIGN KEY (TvrtkaId) REFERENCES Tvrtka(Id),
    CONSTRAINT FK_Zahtjev_VrstaRada
        FOREIGN KEY (VrstaRadaId) REFERENCES VrstaRada(Id)
);