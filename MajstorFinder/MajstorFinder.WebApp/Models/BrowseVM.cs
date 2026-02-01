namespace MajstorFinder.WebApp.Models
{
    public class BrowseVm
    {
        public int? LokacijaId { get; set; }
        public int? VrstaRadaId { get; set; }

        public List<LokacijaVm> Lokacije { get; set; } = new();
        public List<VrstaRadaVm> VrsteRada { get; set; } = new();
        public List<TvrtkaVm> Tvrtke { get; set; } = new();

        public int Page { get; set; } 
        public int PageSize { get; set; }
        public int TotalPages { get; set; } 
    }
}
