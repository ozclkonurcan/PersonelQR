using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Barkod.MVC.Models
{
    public class AhlPersonel
    {
        [Key]
        public int id { get; set; }
        public string tc { get; set; } = string.Empty;
        public string ad_soyad { get; set; } = string.Empty;
        public string unvan { get; set; } = string.Empty;
        public string isyeri { get; set; } = string.Empty;
        public string bolum { get; set; } = string.Empty;
        public string sirket { get; set; } = string.Empty;
        public string QRCode { get; set; } = string.Empty;
    }
}