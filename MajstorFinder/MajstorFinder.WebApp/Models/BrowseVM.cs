namespace MajstorFinder.WebApp.Models
{
    public class BrowseVm
    {
        public int? LokacijaId { get; set; }
        public int? VrstaRadaId { get; set; }

        public List<LokacijaVm> Lokacije { get; set; } = new();
        public List<VrstaRadaVm> VrsteRada { get; set; } = new();
        public List<TvrtkaVm> Tvrtke { get; set; } = new();
    }
}
