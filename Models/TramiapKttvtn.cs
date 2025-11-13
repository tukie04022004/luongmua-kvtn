using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KttvKvtnWeb.Models
{
    [Table("tram_kttvtn")]  // Đúng tên bảng trong DB của bạn
    public class TramKttvtn
    {
        [Key]  // <<< THÊM DÒNG NÀY – PRIMARY KEY BẮT BUỘC
        public string Matram { get; set; } = null!;

        public string Tentram { get; set; } = null!;

        public string Vitri { get; set; } = null!;

        public string Loaitram { get; set; } = null!;

        public string Diachi { get; set; } = null!;
    }
}