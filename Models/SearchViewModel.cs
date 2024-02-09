using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HonestAuto.Models
{
    public class SearchViewModel
    {
        public string SelectedBrand { get; set; }

        public List<string> BrandList { get; set; }

        public string SelectedModel { get; set; }

        public List<string> ModelList { get; set; }

        [Display(Name = "Year")]
        public int? SelectedYear { get; set; }

        public List<int> YearList { get; set; }

        [Display(Name = "Colour")]
        public string SelectedColour { get; set; }

        public List<string> ColourList { get; set; }

        [Display(Name = "Min Mileage")]
        public int? MinMileage { get; set; }

        [Display(Name = "Max Mileage")]
        public int? MaxMileage { get; set; }

        public List<CarViewModel> SearchResults { get; set; }
    }
}