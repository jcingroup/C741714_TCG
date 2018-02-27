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

        changeTitle();

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

    changeTitle();

    function changeTitle(){
        var title = $('title').text();
        var str = $('.tabs .tab-nav.current').text();
        $('title').html(str + " - " + title);
    }

});