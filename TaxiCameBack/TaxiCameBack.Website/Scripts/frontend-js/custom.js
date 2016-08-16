$(document).ready(function() {
    "use strict";

    if ($.fn.cssOriginal!==undefined){
        $.fn.css = $.fn.cssOriginal;
    }
    $('.fullwidthbanner').show().revolution({
        delay:9000,
        startwidth:960,
        startheight:357,
        onHoverStop:"on",                       // Stop Banner Timet at Hover on Slide on/off
        thumbWidth:100,                         // Thumb With and Height and Amount (only if navigation Tyope set to thumb !)
        thumbHeight:50,
        thumbAmount:3,
        hideThumbs:1,
        navigationType:"bullet",                // bullet, thumb, none
        navigationArrows:"none",                // nexttobullets, solo (old name verticalcentered), none
        navigationStyle:"round",                // round,square,navbar,round-old,square-old,navbar-old, or any from the list in the docu (choose between 50+ different item), custom
        navigationHAlign:"center",                // Vertical Align top,center,bottom
        navigationVAlign:"bottom",                  // Horizontal Align left,center,right
        navigationHOffset:30,
        navigationVOffset:30,
        soloArrowLeftHalign:"left",
        soloArrowLeftValign:"center",
        soloArrowLeftHOffset:20,
        soloArrowLeftVOffset:0,
        soloArrowRightHalign:"right",
        soloArrowRightValign:"center",
        soloArrowRightHOffset:20,
        soloArrowRightVOffset:0,
        touchenabled:"on",                      // Enable Swipe Function : on/off
        stopAtSlide:-1,                         // Stop Timer if Slide "x" has been Reached. If stopAfterLoops set to 0, then it stops already in the first Loop at slide X which defined. -1 means do not stop at any slide. stopAfterLoops has no sinn in this case.
        stopAfterLoops:-1,                      // Stop Timer if All slides has been played "x" times. IT will stop at THe slide which is defined via stopAtSlide:x, if set to -1 slide never stop automatic
        hideCaptionAtLimit:0,                   // It Defines if a caption should be shown under a Screen Resolution ( Basod on The Width of Browser)
        hideAllCaptionAtLilmit:0,               // Hide all The Captions if Width of Browser is less then this value
        hideSliderAtLimit:0,                    // Hide the whole slider, and stop also functions if Width of Browser is less than this value
        fullWidth:"on",
        shadow:0                                //0 = no Shadow, 1,2,3 = 3 Different Art of Shadows -  (No Shadow in Fullwidth Version !)
    });
    
    $('input, textarea').placeholder();

    $('select.custom_select').customSelect().show();

    $('.nav-responsive').on('change', function() {
      window.location = $(this).val();
    });

    $('.custom_checkbox, .custom_radio').click(function(){
        setupLabel();
    });
    setupLabel();

    $.fn.stars = function() {
        return $(this).each(function() {
            $(this).html($('<span />').width(Math.max(0, (Math.min(5, parseFloat($(this).html())))) * 14));
        });
    };

    $(function() {
        $('span.item_rating').stars();
    });

    $(".fancybox").fancybox({
        'transitionIn'      : 'elastic',
        'transitionOut'     : 'elastic',
        'titlePosition'     : 'over',
        'cyclic'            : true,
        'overlayShow'       : true,
        'titleFormat'       : function(title, currentArray, currentIndex, currentOpts) {
            return '<span id="fancybox-title-over">Image ' + (currentIndex + 1) + ' / ' + currentArray.length + (title.length ? ' &nbsp; ' + title : '') + '</span>';
        }
    });

    $(".show_login_form").fancybox({
        'scrolling'     : 'no',
        'titleShow'     : false,
        'transitionIn'  : 'elastic',
        'transitionOut' : 'elastic',
        'padding'       : 0,
        'centerOnScroll': true,
    });

    $(function(){
        $('#main_menu').superfish({
            delay:          300,
            animation:      {opacity:'show',height:'show'},
            animationOut:   {height:'hide'},
            speed:          'fast',
            speedOut:       'fast',            
            cssArrows:      false, 
            disableHI:      true /*load hoverIntent.js in header to use this option*/
        });
    });

    // Google map
    $('#map').gMap({
        maptype: 'TERRAIN',
        zoom: 12,
        scrollwheel: false,
        markers: [
            {
                address: 'paris',
                html: '<img src="images/logo.png" alt="" /><br><br><p style="text-align: center;"><strong>Automotive Retail and Services</strong><br>Our Street 555, Our City, State</p>',
                icon: {
                    image: "images/gmap-pin.png",
                    iconsize: [52, 64],
                    iconanchor: [12,46]
                }
            }
        ]
    });


    $('#language_button').click(function() {
        var drop = $('#language_selection');
        if(drop.css('display') === 'none'){
            drop.slideDown(100);
        }
        else{
            drop.slideUp(150);
        }
    });

    header_menu_line();
    footer_menu_line();

    $(window).resize(function(){
        "use strict";
        $('select.custom_select').trigger('update').show();
        header_menu_line();
        footer_menu_line();
    });


});


function header_menu_line() {
    "use strict";
    var $el, leftPos, newWidth;
    $("#magic-line").remove();
    $("#main_menu").append("<li id='magic-line'></li>");
    var $magicLine = $("#magic-line");
    $magicLine
        .width($(".current_menu_ancestor").width())
        .css("left", $(".current_menu_ancestor a").position().left)
        .data("origLeft", $magicLine.position().left)
        .data("origWidth", $magicLine.width());
    $("#main_menu > li").hover(function() {
        $el = $(this).find("a");
        leftPos = $el.position().left;
        newWidth = $el.parent().width();
        $magicLine.stop().animate({
            left: leftPos,
            width: newWidth
        }, 300);
    }, function() {
        $magicLine.stop().animate({
            left: $magicLine.data("origLeft"),
            width: $magicLine.data("origWidth")
        }, 300);    
    });
}


function footer_menu_line() {
    "use strict";
    var $el, leftPos, newWidth;
    $("#footer-magic-line").remove();
    $("#footer_menu").append("<li id='footer-magic-line'></li>");
    var $magicLine = $("#footer-magic-line");
    $magicLine
        .width($("#footer_menu .current_menu_ancestor").width())
        .css("left", $("#footer_menu .current_menu_ancestor a").position().left)
        .data("origLeft", $magicLine.position().left)
        .data("origWidth", $magicLine.width());
    $("#footer_menu li").find("a").hover(function() {
        $el = $(this);
        leftPos = $el.position().left;
        newWidth = $el.parent().width();
        $magicLine.stop().animate({
            left: leftPos,
            width: newWidth
        }, 300);
    }, function() {
        $magicLine.stop().animate({
            left: $magicLine.data("origLeft"),
            width: $magicLine.data("origWidth")
        }, 300);    
    });
}


//Contact form
jQuery("#form").submit(function() {
    var str = jQuery(this).serialize();                  
    jQuery.ajax({
        type: "POST",
        url: "http://ab-themes.com/motor/php/sendmail.php",
        data: str,
        success: function(msg){
            var result;
                if(msg === "OK"){
                    result = "<div class='alert-success'>Your message has been successfully send! Thank you!</div>";
                    jQuery("#form").animate({ height: '0px' }, 1000, function() {
                        jQuery(this).hide();
                    });
                }
                else {
                    $('.field_error').removeClass('field_error');
                    var errors = msg.split(",");
                    var i;
                    for (i = 0; i < errors.length; ++i) {
                        $('#' + errors[i]).addClass('field_error');
                    }
                    Recaptcha.reload();
                    result = "<div class='alert-warning'>Correct input is required in fields marked with red border!</div>";
                }
            jQuery('.submit_note').html(result);
        }                    
    });                  
    return false;
});         