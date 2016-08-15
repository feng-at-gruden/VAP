jQuery(function ($) {
    $.fancybox.defaults.iframe.preload = false;
    $.fancybox.defaults.iframe.autoSize = true;
    $('.fancybox:not(.disabled)').fancybox({
        minWidth: 800,
        minHeight: 500,
        beforeLoad: function () {
            var width, height;
            this.width = this.minWidth;
            this.height = this.minHeight;
            // alert("aa");
            /*if (width = parseInt(this.element.data('fancybox-width'))) {
                this.width = this.minWidth = width;
            }
            if (height = parseInt(this.element.data('fancybox-height'))) {
                this.height = this.minHeight = height;
            }*/
        }
    });

    $('.js-fancybox-close').click(function (e) {
        e.preventDefault();
        var win = $('body').is('.popup-window')
            ? top
            : window;
        win.jQuery.fancybox.close();
    });
    
});