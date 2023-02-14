using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Barkod.MVC.Models
{
    public class YetkiliPersonel
    {
        [Key]
        public int id { get; set; }
        public string k_ad { get; set; }
        public string k_sifre { get; set; }
        public string k_durum { get; set; }
    }
}