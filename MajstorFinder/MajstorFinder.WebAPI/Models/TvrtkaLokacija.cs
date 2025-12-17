namespace MajstorFinder.WebAPI.Models
{
    public partial class TvrtkaLokacija
    {
        public int TvrtkaId { get; set; }
        public int LokacijaId { get; set; }

        public virtual Tvrtka Tvrtka { get; set; } = null!;
        public virtual Lokacija Lokacija { get; set; } = null!;
    }

}
