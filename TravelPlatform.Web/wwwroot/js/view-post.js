window.onload = function () {
    const urlParams = new URLSearchParams(window.location.search);
    const postId = urlParams.get('highlightPost');
    if (postId) {
        const target = document.getElementById(`post-${postId}`);
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'center' // <-- center the element vertically
            });

            target.classList.add('highlighted-post');
            setTimeout(() => target.classList.remove('highlighted-post'), 2000);
        }
    }
};
