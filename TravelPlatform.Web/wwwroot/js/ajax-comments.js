$(document).ready(function () {

    // Open modal when comment icon is clicked
    $(document).on('click', '.open-comments-modal', function (e) {
        e.preventDefault();

        const postId = $(this).data('post-id');
        const imageUrl = $(this).data('post-image');
        const username = $(this).data('username');

        const finalImageUrl = imageUrl || '/images/place.jpg';

        $('#modal-post-image').attr('src', finalImageUrl);
        $('#modal-username').text(username);
        $('#modalPostId').val(postId); // set hidden input in the form if needed

        $('#commentsModal').removeClass('d-none');

        // Load comments
        $.get(`/Post/GetComments?postId=${postId}`, function (data) {
            $('#commentsList').html(data);

            const $commentsList = $('#commentsList');
            $commentsList.scrollTop($commentsList[0].scrollHeight);
        });
    });

    // Close modal
    $(document).on('click', '.close-modal', function () {
        $('#commentsModal').addClass('d-none');
    });

    // Submit new comment
    $(document).on('submit', '#addCommentForm', function (e) {
        e.preventDefault();

        const postId = $('#modalPostId').val();
        const content = $(this).find('textarea[name="content"]').val();
        const token = $(this).find('input[name="__RequestVerificationToken"]').val();

        $.post('/Post/AddComment', { postId, content, "__RequestVerificationToken": token }, function (response) {
            if (response.success) {
                const comment = response.comment;

                $('#no-comments-message').remove();

                const commentHtml = `
            <div class="comment">
                <img src="${comment.userProfilePictureUrl || '/images/user.png'}" class="comment-avatar me-2 mt-1 border rounded-circle" />
                <div class="comment-content">
                    <strong>${comment.username}</strong>
                    <p>${comment.content}</p>
                </div>
                <div class="comment-meta">
                    <span class="comment-date">${formatDateOnly(comment.createdAt)}</span>
                </div>
            </div>
        `;
                $('#commentsList').append(commentHtml);
                $('#addCommentForm textarea[name="content"]').val('');

                const $commentLink = $(`.open-comments-modal[data-post-id="${postId}"]`);
                if ($commentLink.length) {
                    const currentText = $commentLink.text().trim();
                    const match = currentText.match(/\d+/);
                    const currentCount = match ? parseInt(match[0], 10) : 0;
                    const newCount = currentCount + 1;
                    $commentLink.html(`<i class="bi bi-chat-right-dots"></i> ${newCount}`);
                }
            } else {
                alert('Failed to add comment.');
            }
        });

    });

});

function formatDateOnly(dateString) {
    const date = new Date(dateString);

    return new Intl.DateTimeFormat(navigator.language, {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    }).format(date);
}
