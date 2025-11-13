using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KttvKvtnWeb.Models
{
    [Table("users")]  // Đúng tên bảng
    public class User
    {
        [Key]  // <<< THÊM DÒNG NÀY
        public string Userid { get; set; } = null!;

        public string Hoten { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Matkhau { get; set; } = null!;

        public string Hienthi { get; set; } = null!;

        public string? Lastlogin { get; set; }
    }
}