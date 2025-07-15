using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models
{
    public class HeThongPhanPhoi
    {
        [Key]
        public required string MaHTPP { get; set; }
        public required string TenHTPP { get; set; }
        public ICollection<DaiLy>? CacDaiLy { get; set; }
    }
}