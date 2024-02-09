using System.ComponentModel.DataAnnotations;

namespace HonestAuto.Models
{
    public class SearchViewModel
    {
        // SelectedBrand property holds the ID of the selected brand from the dropdown list
        public string SelectedBrand { get; set; }

        // BrandList property contains the list of available brands for the dropdown list
        public List<string> BrandList { get; set; }

        // SelectedModel property holds the ID of the selected model from the dropdown list
        public string SelectedModel { get; set; }

        // ModelList property contains the list of available models for the dropdown list
        public List<string> ModelList { get; set; }

        // SelectedYear property holds the selected year for filtering
        [Display(Name = "Year")]
        public int? SelectedYear { get; set; }

        // YearList property contains the list of available years for the dropdown list
        public List<int> YearList { get; set; }

        // SelectedColour property holds the selected colour for filtering
        [Display(Name = "Colour")]
        public string SelectedColour { get; set; }

        // ColourList property contains the list of available colours for the dropdown list
        public List<string> ColourList { get; set; }

        // MinMileage property holds the minimum mileage for filtering
        [Display(Name = "Min Mileage")]
        public int? MinMileage { get; set; }

        // MaxMileage property holds the maximum mileage for filtering
        [Display(Name = "Max Mileage")]
        public int? MaxMileage { get; set; }

        // SearchResults property holds the list of car view models returned from the search
        public List<CarViewModel> SearchResults { get; set; }
    }
}