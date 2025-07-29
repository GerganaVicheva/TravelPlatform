//$(document).ready(function () {

//    // Open modal when followers/following button clicked
//    $(document).on('click', '#showFollowersBtn, #showFollowingBtn', function (e) {
//        e.preventDefault();

//        const userId = $(this).data('user-id');
//        const type = $(this).attr('id') === 'showFollowersBtn' ? 'followers' : 'following';
//        const modal = $('#followModal');

//        // Set modal title
//        $('#customModalTitle').text(type.charAt(0).toUpperCase() + type.slice(1));

//        // Show modal (remove hidden class)
//        modal.removeClass('hidden');

//        // Load partial view into modal body
//        $.get(`/UserAccount/GetFollowersPartial?userId=${userId}`, function (data) {
//            $('#customModalBody').html(data);
//        }).fail(function () {
//            alert(`Failed to load ${type}`);
//            modal.addClass('hidden');  // Hide modal if failed
//        });
//    });

//    // Close modal when close button clicked
//    $(document).on('click', '#closeFollowModal', function () {
//        $('#followModal').addClass('hidden');
//        $('#customModalBody').empty(); // clear modal content
//    });

//});


$(document).ready(function () {
    // Open modal when followers or following button is clicked
    $(document).on('click', '#showFollowersBtn, #showFollowingBtn', function (e) {
        e.preventDefault();

        const userId = $(this).data('user-id');
        const isFollowers = $(this).attr('id') === 'showFollowersBtn';
        const type = isFollowers ? 'followers' : 'following';
        const modal = $('#followModal');

        // Set modal title
        $('#customModalTitle').text(type.charAt(0).toUpperCase() + type.slice(1));

        // Show modal
        modal.removeClass('hidden');

        // Construct correct endpoint
        const endpoint = isFollowers
            ? `/UserAccount/GetFollowersPartial?userId=${userId}`
            : `/UserAccount/GetFollowingPartial?userId=${userId}`;

        // Load partial view into modal
        $.get(endpoint, function (data) {
            $('#customModalBody').html(data);
        }).fail(function () {
            alert(`Failed to load ${type}`);
            modal.addClass('hidden'); // Hide modal on failure
        });
    });

    // Close modal on close button click
    $(document).on('click', '#closeFollowModal', function () {
        $('#followModal').addClass('hidden');
        $('#customModalBody').empty(); // Clear previous content
    });
});

