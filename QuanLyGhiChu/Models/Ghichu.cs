using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyGhiChu.Models
{
    public partial class Ghichu
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string HashCode { get; set; }

        [MaxLength(50)]
        public string Token { get; set; }

        [Display(Name = "Tiêu đề ghi chú")]
        [Required(ErrorMessage = "Tiêu đề ghi chú bị thiếu")]
        [MaxLength(500, ErrorMessage = "Tiêu đề ghi chú tối đa 500 kí tự")]
        public string Title { get; set; }

        [Display(Name = "Nội dung ghi chú")]
        public string Context { get; set; }

        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        [MaxLength(30, ErrorMessage = "Mật khẩu tối đa 30 kí tự")]
        public string Password { get; set; }

        [Display(Name = "Ngày tạo")]
        [DataType(DataType.DateTime)]
        public DateTime TimeCreated { get; set; }

        [Display(Name = "Ngày chỉnh sửa")]
        [DataType(DataType.DateTime)]
        public DateTime? TimeUpdated { get; set; }

        [Range(0, 1)]
        public byte HienAn { get; set; }
    }
}
