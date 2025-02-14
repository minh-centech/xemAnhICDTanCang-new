using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace GetImagesFromDatabase.Models
{
    public class getTenThuMuc
    {
        [Required(ErrorMessage = "Vui lòng nhập tên thư mục")]
        // Validation: Bắt buộc nhập
        public string ID { get; set; }
       
        public List<string> ImageData { get; set; } // Thêm property ImageData
    }
}