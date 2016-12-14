$(document).ready(function () {

    calcfrontScore(),
    calcbackScore(),
    calctotalScore()

    $('.frontScore, .backScore').numeric();
    jQuery('.frontScore, .backScore').keyup(function () {
        this.value = this.value.replace(/-/, '0');
    });

    $(".frontScore").on("keydown keyup", function () {
        calcfrontScore(),
        calctotalScore()
    });

    $(".backScore").on("keydown keyup", function () {
        calcbackScore(),
        calctotalScore()
    });

});

function calcfrontScore() {
    var sum = 0;
    var playingHcap = parseInt($('#pHcap').text());

    // iterate through each td based on class and add the values
    $(".frontScore").each(function () {
        if (!isNaN(this.value) && this.value.length != 0) {
            sum += parseInt(this.value);
        }
    })
    $('#frontScoreTotal').text(sum);
    $('#outScoreTotal').text(sum);
};

function calcbackScore() {
    var sum = 0;
    var backPointsSum = 0;
    var playingHcap = parseInt($('#pHcap').text());

    // iterate through each td based on class and add the values
    $(".backScore").each(function () {
        if (!isNaN(this.value) && this.value.length != 0) {
            sum += parseInt(this.value);
        }
    });

    $('#backScoreTotal').text(sum);
};

function calctotalScore() {
    // Calc and display the Total Score
    var totalScore = parseInt($('#frontScoreTotal').text()) + parseInt($('#backScoreTotal').text());
    $("#totalScore").text(totalScore);
};
