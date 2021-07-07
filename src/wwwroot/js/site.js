// Write your Javascript code.
var dh = dh || {};
(function ($) {
    function inithome() {
        $(document).ready(function () {
            $("body").backstretch([
                "/images/bg1.jpg",
                "/images/bg4.jpg",
                "/images/bg5.jpg"
            ], { duration: 4000, fade: 500 });
        });
    }
    dh.inithome = inithome;
}(jQuery));