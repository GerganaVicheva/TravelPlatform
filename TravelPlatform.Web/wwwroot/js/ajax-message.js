$(document).ready(function () {

    // Form submission
    $('#contact-form').on('submit', function (e) {
        e.preventDefault();

        // Get form data
        var formData = $(this).serialize();

        // Disable submit button and show loading
        $('#submit-btn').prop('disabled', true).html('<i class="bi bi-hourglass-split me-2"></i>Sending...');
        $('#response-message').hide();

        // Make AJAX request
        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: formData,
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    showMessage(response.message, 'success');
                    // Clear form on success
                    $('#contact-form')[0].reset();
                } else {
                    showMessage(response.message, 'danger');
                }
            },
            error: function (xhr, status, error) {
                var errorMessage = 'An error occurred while submitting the form.';
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage = xhr.responseJSON.message;
                }
                showMessage(errorMessage, 'danger');
            },
            complete: function () {
                // Re-enable submit button
                $('#submit-btn').prop('disabled', false).html('<i class="bi bi-send me-2"></i>Send Message');
            }
        });
    });

    function showMessage(message, type) {
        var $responseDiv = $('#response-message');
        $responseDiv.removeClass().addClass('alert alert-' + type);
        $responseDiv.html('<i class="bi bi-' + (type === 'success' ? 'check-circle' : 'exclamation-triangle') + ' me-2"></i>' + message).show();

        // Auto-hide success messages after 5 seconds
        if (type === 'success') {
            setTimeout(function () {
                $responseDiv.fadeOut();
            }, 5000);
        }
    }
});