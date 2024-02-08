$(document).ready(function () {
    // Autocomplete for brand input
    $('#BrandId').on('input', function () {
        var brandName = $(this).val();
        if (brandName) {
            // Perform AJAX request to fetch matching brands based on input brandName
            $.ajax({
                url: '/Car/GetBrandsByName',
                method: 'GET',
                data: { brandName: brandName },
                success: function (response) {
                    // Clear existing options
                    $('#BrandId').empty();
                    // Populate brand dropdown with fetched brands
                    $.each(response, function (index, brand) {
                        $('#BrandId').append($('<option>', {
                            value: brand.BrandID,
                            text: brand.BrandName
                        }));
                    });
                },
                error: function (xhr, status, error) {
                    console.error(error);
                }
            });
        } else {
            // Clear brand dropdown if brandName is empty
            $('#BrandId').empty();
        }
    });

    // Autocomplete for model input
    $('#ModelId').on('input', function () {
        var modelName = $(this).val();
        if (modelName) {
            // Perform AJAX request to fetch matching models based on input modelName
            $.ajax({
                url: '/Car/GetModelsByName',
                method: 'GET',
                data: { modelName: modelName },
                success: function (response) {
                    // Clear existing options
                    $('#ModelId').empty();
                    // Populate model dropdown with fetched models
                    $.each(response, function (index, model) {
                        $('#ModelId').append($('<option>', {
                            value: model.ModelID,
                            text: model.ModelName
                        }));
                    });
                },
                error: function (xhr, status, error) {
                    console.error(error);
                }
            });
        } else {
            // Clear model dropdown if modelName is empty
            $('#ModelId').empty();
        }
    });
});