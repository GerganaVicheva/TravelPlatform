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

        $.post('/Post/AddComment', { postId, content }, function (response) {
            if (response.success) {
                const comment = response.comment;
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
