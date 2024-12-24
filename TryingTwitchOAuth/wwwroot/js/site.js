// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function fadeOutAlert() {
    var alert = document.getElementById('alert');
    if (alert) {
        var alertHeight = alert.offsetHeight;
        var style = window.getComputedStyle(alert);
        var marginTop = parseInt(style.marginTop);
        var marginBottom = parseInt(style.marginBottom);
        var totalHeight = alertHeight + marginTop + marginBottom;

        setTimeout(function () {
            alert.style.transition = 'opacity 0.5s, margin-top 0.5s';
            alert.style.opacity = '0';
            alert.style.marginTop = `-${totalHeight}px`;
            setTimeout(function () {
                alert.remove();
            }, 500);
        }, 2000);
    }
}

document.addEventListener('DOMContentLoaded', function () {
    fadeOutAlert();
});