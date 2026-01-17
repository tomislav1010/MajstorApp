namespace MajstorFinder.WebApp.Models
{
    public class ZahtjevVm
    {
        public int Id { get; set; }
        public string Description { get; set; } = "";
        public DateTime DateCreated { get; set; }
        public string Status { get; set; } = "";

        public int TvrtkaId { get; set; }
        public int VrstaRadaId { get; set; }
        public int KorisnikId { get; set; }
    }
}
