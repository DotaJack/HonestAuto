$(document).ready(function () {
    // Get the selected brand ID and model ID from the index page
    var selectedBrandId = $('#BrandId').val();
    var selectedModelId = $('#ModelId').val();

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
            brandSelect.val(selectedBrandId).trigger('change'); // Pre-select current brand
            brandSelect.select2(); // Initialize Select2
        }
    });

    // Inside your document ready function
    $('.js-select2-brand').on('change', function () {
        var selectedBrandId = $(this).val();
        updateModelDropdown(selectedBrandId, selectedModelId); // Pass selectedModelId here
    });

    // Function to update the model dropdown
    function updateModelDropdown(brandId, selectedModelId = null) {
        if (brandId) {
            $.ajax({
                url: '/Car/GetModelsByBrand',
                type: 'GET',
                data: { selectedBrandId: brandId },
                success: function (models) {
                    var modelSelect = $('.js-select2-model');
                    modelSelect.empty().append(new Option('Select Model', ''));
                    models.forEach(function (model) {
                        modelSelect.append(new Option(model.modelName, model.modelId));
                    });

                    // Pre-select the current model if available
                    if (selectedModelId) {
                        modelSelect.val(selectedModelId).trigger('change');
                    }
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