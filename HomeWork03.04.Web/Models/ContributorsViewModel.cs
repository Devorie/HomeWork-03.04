using HomeWork03._04.Data.wwwroot;

namespace HomeWork03._04.Web.Models
{
    public class ContributorsViewModel
    {
        public List<Contributor> Contributors { get; set; }
        public Simcha Simcha { get; set; }
        public decimal TotalAvailable { get; set; }
    }
}
