using System.ComponentModel.DataAnnotations;

namespace HonestAuto.Models
{
    public class Model
    {
        public int ModelId { get; set; }

        [Required]
        public string Name { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }
    }
}