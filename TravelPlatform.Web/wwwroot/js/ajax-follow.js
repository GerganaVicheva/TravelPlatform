$(document).ready(function () {
    $('#followBtn').on('click', function () {
        const button = $(this);
        const followedUserId = $(this).data('user-id');

        $.ajax({
            url: '/UserAccount/Follow',
            type: 'POST',
            data: { followedUserId: followedUserId },
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success) {
                    const isFollowing = response.isFollowing;
                    button.data('following', isFollowing);
                    button.text(isFollowing ? 'Unfollow' : 'Follow');

                    // Update follower count (on viewed user's profile)
                    const $followerCount = $('#followerCount');
                    if ($followerCount.length) {
                        let current = parseInt($followerCount.text(), 10);
                        $followerCount.text(isFollowing ? current + 1 : current - 1);
                    }
                } else {
                    alert(response.message || "Unable to follow/unfollow.");
                }
            },
            error: function (xhr, status, error) {
                alert("An error occurred:", xhr.responseText || error);
            }
        });
    });
});