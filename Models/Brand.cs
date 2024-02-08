using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HonestAuto.Models
{
    public class Brand
    {
        public int BrandId { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Model> Models { get; set; }
    }
}