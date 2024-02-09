$(document).ready(function () {
    // Fetch and initialize the Select2 brand dropdown with all brands
    $.ajax({
        url: '/Car/GetAllBrands', // Endpoint to fetch all brands
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log('Data received:', data); // Log the received data for debugging
            var brandSelect = $('#SelectedBrand');
            brandSelect.empty().append(new Option('Select Brand', ''));
            data.forEach(function (brand) {
                brandSelect.append(new Option(brand.brandName, brand.brandID));
            });
            brandSelect.select2();
        },
        error: function (xhr, status, error) {
            console.error('Error fetching brands:', error); // Log any errors for debugging
        }
    });

    // Inside your document ready function
    $('#SelectedBrand').on('change', function () {
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
                    var modelSelect = $('#SelectedModel');
                    modelSelect.empty().append(new Option('Select Model', ''));
                    models.forEach(function (model) {
                        modelSelect.append(new Option(model.modelName, model.modelId));
                    });
                    modelSelect.trigger('change');
                },
                error: function (xhr, status, error) {
                    console.error('Error fetching models:', error);
                }
            });
        } else {
            $('#SelectedModel').empty().append(new Option('Select Model', '')).trigger('change');
        }
    }
});