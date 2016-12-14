
$(document).ready(function () {

    calcfrontScore(),
    calcbackScore(),
    calctotalScore(),
    calctotalPoints()

    $('.frontScore, .backScore').numeric();
    jQuery('.frontScore, .backScore').keyup(function () {
        this.value = this.value.replace(/-/, '0');
    });

    $(".frontScore").on("keydown keyup", function () {
        calcfrontScore(),
        calctotalScore(),
        calctotalPoints()
    });

    $(".backScore").on("keydown keyup", function () {
        calcbackScore(),
        calctotalScore(),
        calctotalPoints()
    });

});

function calcfrontScore() {
    var sum = 0;
    var frontPointsSum = 0;
    var playingHcap = parseInt($('#pHcap').text());

    // iterate through each td based on class and add the values
    $(".frontScore").each(function () {

        // add only if the value is number
        var $nel = $(this).closest('td').next();
        if (!isNaN(this.value) && this.value.length != 0) {
            sum += parseInt(this.value);

            // Calculate and display the Points score for this hole
            var $pel = $(this).closest('td').prev();
            var $par = $pel.closest('td').prev();
            var strokes = parseInt(this.value);

            // ********** PASS IN THE VARIABLES AND DISPLAY THE RESULTING POINTS *********
            $.get('/Scores/PointsScore/?Hcap=' + playingHcap + "&SI=" + $pel.text() + "&strokes=" + strokes + "&par=" + $par.text(), function (data) {
                $($nel).text(data.newPoints.points);
                var thisPts = data.newPoints.point;
                frontPointsSum += thisPts;
            })

        }
        else {
            $($nel).text(0);
            frontPointsSum += 0;
        }
    })

    // ** NEED TO RETHINK THE TOTALS **
    $('#frontScoreTotal').text(sum);
    $('#frontPointsTotal').text(frontPointsSum);
    $('#outPointsTotal').text(frontPointsSum);
    $('#outScoreTotal').text(sum);
};

function calcbackScore() {
    var sum = 0;
    var backPointsSum = 0;
    var playingHcap = parseInt($('#pHcap').text());

    // iterate through each td based on class and add the values
    $(".backScore").each(function () {

        // add only if the value is number
        var $nel = $(this).closest('td').next();
        if (!isNaN(this.value) && this.value.length != 0) {
            sum += parseInt(this.value);

            // Calculate and display the Points score for this hole
            var $pel = $(this).closest('td').prev();
            var $par = $pel.closest('td').prev();
            var strokes = parseInt(this.value);

            // ********** PASS IN THE VARIABLES AND DISPLAY THE RESULTING POINTS *********
            $.get('/Scores/PointsScore/?Hcap=' + playingHcap + "&SI=" + $pel.text() + "&strokes=" + strokes + "&par=" + $par.text(), function (data) {
                $($nel).text(data.newPoints.points);
            });
        }
        else {
            $($nel).text(0);
        }

        // Get the sum of the points for each hole.
        backPointsSum += parseInt($($nel).text());
    });
   
   // ** NEED TO RETHINK THE TOTALS **
    $('#backPointsTotal').text(backPointsSum);
    $('#backScoreTotal').text(sum);
};

function calctotalScore() {
    // Calc and display the Total Score
    var totalScore = parseInt($('#frontScoreTotal').text()) + parseInt($('#backScoreTotal').text());
    $("#totalScore").text(totalScore);
};

function calctotalPoints() {
    // Calc and display the Total Points
    var totalPoints = parseInt($('#frontPointsTotal').text()) + parseInt($('#backPointsTotal').text())
    $("#totalPoints").text(totalPoints)
    $("#dbtotalPoints").val(totalPoints)
};
