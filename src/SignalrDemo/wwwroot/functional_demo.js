(function () {
    //todo
    var demo = function () {

        var date = new Date();

        var show = function () {
            console.log(new Date());
        };

        var show2 = function (date) {
            console.log(date);
        };

        return {
            date: date,
            show: show,
            show2: show2
        }
    }();


    demo.show();

    demo.show2(demo.date);

    var test = new Date();
    demo.show2(test);

}());