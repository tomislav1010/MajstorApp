namespace MajstorFinder.WebApp.Models
{
    public class TvrtkaLokacijeVm
    {
        public int TvrtkaId { get; set; }
        public string TvrtkaName { get; set; } = "";

        public List<LokacijaCheckVm> Lokacije { get; set; } = new();
    }

    public class LokacijaCheckVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool Selected { get; set; }
    }
}
