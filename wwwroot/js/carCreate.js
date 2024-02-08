$(document).ready(function () {
    // Fetch and initialize the Select2 brand dropdown with all brands
    $.ajax({
        url: '/Car/GetAllBrands', // Endpoint to fetch all brands
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            var brandSelect = $('.js-select2-brand');
            brandSelect.empty().append(new Option('Select Brand', ''));
            data.forEach(function (brand) {
                brandSelect.append(new Option(brand.brandName, brand.brandID)); // Ensure these match your JSON keys
            });
            brandSelect.select2(); // Initialize Select2
        }
    });

    // Inside your document ready function
    $('.js-select2-brand').on('change', function () {
        var selectedBrandId = $(this).val();
        updateModelDropdown(selectedBrandId);
    });

    // Function to update the model dropdown
    function updateModelDropdown(brandId) {
        if (brandId) {
            $.ajax({
                url: '/Car/GetModelsByBrand',
                type: 'GET',
                data: { selectedBrandId: brandId },
                success: function (models) {
                    var modelSelect = $('.js-select2-model');
                    modelSelect.empty().append(new Option('Select Model', ''));
                    models.forEach(function (model) {
                        // Corrected to match the casing of your JSON keys
                        modelSelect.append(new Option(model.modelName, model.modelId));
                    });
                    modelSelect.trigger('change'); // Notify Select2 to update the dropdown
                },
                error: function (xhr, status, error) {
                    console.error('Error fetching models:', error);
                }
            });
        } else {
            // If no brand is selected, clear the model dropdown
            $('.js-select2-model').empty().append(new Option('Select Model', '')).trigger('change');
        }
    }
}); 