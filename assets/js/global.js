console.log('global.js loaded');
document.addEventListener('DOMContentLoaded', function() {
    const elements = document.querySelectorAll('.svgText');
    // console.log(elements);

    elements.forEach(function(element) {
        element.addEventListener('mouseover', function() {
            element.classList.add('animating');
        });

        element.addEventListener('transitionend', function() {
            element.classList.remove('animating');
        });
    });
});