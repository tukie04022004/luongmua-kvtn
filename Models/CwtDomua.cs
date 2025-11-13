using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KttvKvtnWeb.Models
{
    [Table("cwt_domua")]
    public class CwtDomua
    {
        [Key]
        public long Id { get; set; }

        public string Matram { get; set; } = null!;

        public string Thoigian { get; set; } = null!;

        public long Demmua { get; set; }

        public double Luongmua { get; set; }

        public double Pe { get; set; }

        [Column("date_time")]  // <<< THÊM DÒNG NÀY – MAP ĐÚNG TÊN CỘT TRONG DB
        public DateTime DateTime { get; set; }

        public double Luuluong { get; set; }
    }
}