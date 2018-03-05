$(document).ready(function () {

    $(".tabs .tab-nav").click(function(e){
        $(this).parent().siblings().children(".tab-nav").removeClass("current");
        $(this).addClass("current");

        var target = $(this).attr("href");
        $(target).siblings(".tab").hide().removeClass("current");
        $(target).fadeIn().addClass("current");

        // change url hash without reloading page
        if(history.pushState) {
            history.pushState(null, null, target);
        }
        else {
            location.hash = target;
        }
        
        // changeTitle();
        
        e.preventDefault();
    });
    
    var hash = window.location.hash;
    if (hash) {
        var hash_root = hash.split('-')[0];
        $(".tabs .tab-nav").each(function(){
            var target = $(this).attr("href");
            if (target == hash || target == hash_root) {
                $(this).parent().siblings().children(".tab-nav").removeClass("current");
                $(this).addClass("current");
            }
        });
        $(hash).add(hash_root).siblings(".tab").hide().removeClass("current");
        $(hash).add(hash_root).fadeIn().addClass("current");
    }

    // changeTitle();

    // function changeTitle(){
    //     var siteName = $('#title').text().split('|')[1];
    //     var title = $('.title .underline');
    //     var subtitle = $('.editor .tabs .tab-nav.current');
    //     var titleText = "";
    //     var subtitleText = "";
    //     title.each(function() {
    //         if ($(this).is(":visible")) {
    //             titleText = $(this).text();
    //         }
    //     });
    //     subtitle.each(function() {
    //         if ($(this).is(":visible")) {
    //             subtitleText = $(this).text();
    //         }
    //     });
    //     if (subtitleText == "") {
    //         $('#title').html(titleText + " | " + siteName);
    //         $('#metaTitle').attr('content', titleText + " | " + siteName);
    //     } else {
    //         $('#title').html(subtitleText + " - " + titleText + " | " + siteName);
    //         $('#metaTitle').attr('content', subtitleText + " - " + titleText + " | " + siteName);
    //     }
    // }

});