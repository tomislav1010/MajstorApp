USE RWA;
GO

/* 1) LOKACIJE */
IF NOT EXISTS (SELECT 1 FROM Lokacija WHERE Name = N'Zagreb')
BEGIN
    INSERT INTO Lokacija (Name) VALUES
    (N'Zagreb'),
    (N'Split'),
    (N'Rijeka'),
    (N'Osijek'),
    (N'Zadar');
END
GO

/* 2) TVRTKE */
IF NOT EXISTS (SELECT 1 FROM Tvrtka WHERE Name = N'Majstor Plus')
BEGIN
    INSERT INTO Tvrtka (Name, Description, Phone, Email) VALUES
    (N'Majstor Plus', N'Zidarski i fasaderski radovi.', N'0911111111', N'info@majstorplus.hr'),
    (N'Brzi Kist',    N'Ličenje i gletanje stanova.',  N'0912222222', N'kontakt@brzikist.hr'),
    (N'Okućnica Pro', N'Uređenje okućnice i vrtova.',  N'0913333333', N'hello@okucnicapro.hr');
END
GO

/* 3) VRSTE RADA (1-N po tvrtki) */
DECLARE @TvrtkaMajstorPlus INT = (SELECT TOP 1 Id FROM Tvrtka WHERE Name = N'Majstor Plus');
DECLARE @TvrtkaBrziKist    INT = (SELECT TOP 1 Id FROM Tvrtka WHERE Name = N'Brzi Kist');
DECLARE @TvrtkaOkucnicaPro INT = (SELECT TOP 1 Id FROM Tvrtka WHERE Name = N'Okućnica Pro');

IF NOT EXISTS (SELECT 1 FROM VrstaRada WHERE Name = N'Zidarija' AND TvrtkaId = @TvrtkaMajstorPlus)
BEGIN
    INSERT INTO VrstaRada (Name, TvrtkaId) VALUES
    (N'Zidarija', @TvrtkaMajstorPlus),
    (N'Fasada',   @TvrtkaMajstorPlus),
    (N'Ličenje',  @TvrtkaBrziKist),
    (N'Gletanje', @TvrtkaBrziKist),
    (N'Košnja',   @TvrtkaOkucnicaPro),
    (N'Orezivanje', @TvrtkaOkucnicaPro);
END
GO

/* 4) VEZE TVRTKA-LOKACIJA (M:N) */
DECLARE @LokZg INT = (SELECT TOP 1 Id FROM Lokacija WHERE Name = N'Zagreb');
DECLARE @LokSt INT = (SELECT TOP 1 Id FROM Lokacija WHERE Name = N'Split');
DECLARE @LokRi INT = (SELECT TOP 1 Id FROM Lokacija WHERE Name = N'Rijeka');
DECLARE @LokOs INT = (SELECT TOP 1 Id FROM Lokacija WHERE Name = N'Osijek');
DECLARE @LokZd INT = (SELECT TOP 1 Id FROM Lokacija WHERE Name = N'Zadar');

-- Majstor Plus: Zagreb, Rijeka
IF NOT EXISTS (SELECT 1 FROM TvrtkaLokacija WHERE TvrtkaId=@TvrtkaMajstorPlus AND LokacijaId=@LokZg)
    INSERT INTO TvrtkaLokacija (TvrtkaId, LokacijaId) VALUES (@TvrtkaMajstorPlus, @LokZg);
IF NOT EXISTS (SELECT 1 FROM TvrtkaLokacija WHERE TvrtkaId=@TvrtkaMajstorPlus AND LokacijaId=@LokRi)
    INSERT INTO TvrtkaLokacija (TvrtkaId, LokacijaId) VALUES (@TvrtkaMajstorPlus, @LokRi);

-- Brzi Kist: Zagreb, Split, Zadar
IF NOT EXISTS (SELECT 1 FROM TvrtkaLokacija WHERE TvrtkaId=@TvrtkaBrziKist AND LokacijaId=@LokZg)
    INSERT INTO TvrtkaLokacija (TvrtkaId, LokacijaId) VALUES (@TvrtkaBrziKist, @LokZg);
IF NOT EXISTS (SELECT 1 FROM TvrtkaLokacija WHERE TvrtkaId=@TvrtkaBrziKist AND LokacijaId=@LokSt)
    INSERT INTO TvrtkaLokacija (TvrtkaId, LokacijaId) VALUES (@TvrtkaBrziKist, @LokSt);
IF NOT EXISTS (SELECT 1 FROM TvrtkaLokacija WHERE TvrtkaId=@TvrtkaBrziKist AND LokacijaId=@LokZd)
    INSERT INTO TvrtkaLokacija (TvrtkaId, LokacijaId) VALUES (@TvrtkaBrziKist, @LokZd);

-- Okućnica Pro: Osijek, Zagreb
IF NOT EXISTS (SELECT 1 FROM TvrtkaLokacija WHERE TvrtkaId=@TvrtkaOkucnicaPro AND LokacijaId=@LokOs)
    INSERT INTO TvrtkaLokacija (TvrtkaId, LokacijaId) VALUES (@TvrtkaOkucnicaPro, @LokOs);
IF NOT EXISTS (SELECT 1 FROM TvrtkaLokacija WHERE TvrtkaId=@TvrtkaOkucnicaPro AND LokacijaId=@LokZg)
    INSERT INTO TvrtkaLokacija (TvrtkaId, LokacijaId) VALUES (@TvrtkaOkucnicaPro, @LokZg);
GO

/* 5) KORISNICI (za zahtjeve) */
IF NOT EXISTS (SELECT 1 FROM Korisnik WHERE Email = N'klijent1@test.hr')
BEGIN
    INSERT INTO Korisnik (Name, Email, Phone) VALUES
    (N'Klijent 1', N'klijent1@test.hr', N'0991111111'),
    (N'Klijent 2', N'klijent2@test.hr', N'0992222222');
END
GO

DECLARE @K1 INT = (SELECT TOP 1 Id FROM Korisnik WHERE Email = N'klijent1@test.hr');
DECLARE @K2 INT = (SELECT TOP 1 Id FROM Korisnik WHERE Email = N'klijent2@test.hr');

DECLARE @VrZidarija INT = (SELECT TOP 1 Id FROM VrstaRada WHERE Name = N'Zidarija' AND TvrtkaId = @TvrtkaMajstorPlus);
DECLARE @VrLicenje  INT = (SELECT TOP 1 Id FROM VrstaRada WHERE Name = N'Ličenje' AND TvrtkaId = @TvrtkaBrziKist);
DECLARE @VrKosnja   INT = (SELECT TOP 1 Id FROM VrstaRada WHERE Name = N'Košnja' AND TvrtkaId = @TvrtkaOkucnicaPro);

/* 6) ZAHTJEVI */
IF NOT EXISTS (SELECT 1 FROM Zahtjev WHERE Description = N'Popravak zida u hodniku')
BEGIN
    INSERT INTO Zahtjev (Description, Status, KorisnikId, TvrtkaId, VrstaRadaId)
    VALUES
    (N'Popravak zida u hodniku', N'Poslano', @K1, @TvrtkaMajstorPlus, @VrZidarija),
    (N'Ličenje dnevnog boravka 25m2', N'U obradi', @K2, @TvrtkaBrziKist, @VrLicenje),
    (N'Košnja trave i odvoz otpada', N'Poslano', @K1, @TvrtkaOkucnicaPro, @VrKosnja);
END
GO