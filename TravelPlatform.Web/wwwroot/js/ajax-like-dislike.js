$(document).on("click", ".like-btn", function (e) {
    e.preventDefault();

    const button = $(this);
    const postId = button.data("post-id");

    $.ajax({
        type: "POST",
        url: "/Post/Like",
        data: { postId: postId },
        success: function (response) {
            if (response.success) {
                const icon = button.find("i");
                const countSpan = button.find(".like-count");

                // Toggle icon
                if (response.liked) {
                    icon.removeClass("bi-heart").addClass("bi-heart-fill text-danger");
                } else {
                    icon.removeClass("bi-heart-fill text-danger").addClass("bi-heart");
                }

                // Update count
                countSpan.text(response.likes);
            } else {
                alert("Error: " + response.error);
            }
        },
        error: function () {
            alert("AJAX error occurred.");
        }
    });
});