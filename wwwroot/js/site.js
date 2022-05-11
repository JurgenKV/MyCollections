// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(".five li ul").hide();
$(".five li:has('.submenu')").hover(
    function () {
        $(".five li ul").stop().fadeToggle(300);
    }
);

var myModal = document.getElementById('myModal')
var myInput = document.getElementById('myInput')

myModal.addEventListener('shown.bs.modal', function () {
    myInput.focus()
})