function showSlide(index) {
    const currentSlide = slides.eq(currentIndex);
    const nextSlide = slides.eq(index);

    // Animate current slide out
    currentSlide.removeClass('active').addClass('out');

    // After animation ends, hide it
    setTimeout(() => {
        currentSlide.removeClass('out').hide();
    }, 500); // match animation duration

    // Show next slide
    nextSlide.show().addClass('active');

    // Update dots
    $('.carousel-nav button').removeClass('active').eq(index).addClass('active');

    currentIndex = index;
}

$(document).ready(function () {
    let currentIndex = 0;
    const slides = $('.carousel-slide');
    const totalSlides = slides.length;

    // Show the first slide initially
    slides.hide().eq(currentIndex).show();
    $('.carousel-nav button').eq(currentIndex).addClass('active');

    function showSlide(index) {
        slides.fadeOut(300).eq(index).fadeIn(300);
        $('.carousel-nav button').removeClass('active').eq(index).addClass('active');
        currentIndex = index;
    }

    // Navigation dots click
    $('.carousel-nav button').click(function () {
        const index = $(this).data('slide');
        showSlide(index);
    });

    // Optional: Auto-play
    setInterval(function () {
        let nextIndex = (currentIndex + 1) % totalSlides;
        showSlide(nextIndex);
    }, 5000); // Change slide every 5s
});