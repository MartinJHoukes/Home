$(document).ready(function () {
    formatfrontScore()
    formatbackScore()
});

function formatfrontScore() {

    // iterate through each td based on class and format the score depending on par, birdie , bogey etc..
    $('.frontScore').each(function () {

        var holePar = parseInt($(this).closest('tr').children('.par').text());
        var holeScore = parseInt($(this).text());

        if (holeScore !== 0)
        {
            if (holeScore == holePar) {
                $(this).addClass('ScorePar');
            }
            else if (holePar - holeScore == 1)
            {
                $(this).addClass('ScoreBirdie');
            }
            else if (holePar - holeScore == 2)
            {
                $(this).addClass('ScoreEagle');
            }
            else if (holePar - holeScore == -1)
            {
                $(this).addClass('ScoreBogey');
            }
            else if (holePar - holeScore <= -2)
            {
                $(this).addClass('ScoreDblBogey');
            }
        }
    })
};

function formatbackScore() {

    // iterate through each td based on class and format the score depending on par, birdie , bogey etc..
    $('.backScore').each(function () {

        var holePar = parseInt($(this).closest('tr').children('.par').text());
        var holeScore = parseInt($(this).text());

        if (holeScore !== 0) {
            if (holeScore == holePar) {
                $(this).addClass('ScorePar');
            }
            else if (holePar - holeScore == 1) {
                $(this).addClass('ScoreBirdie');
            }
            else if (holePar - holeScore == 2) {
                $(this).addClass('ScoreEagle');
            }
            else if (holePar - holeScore == -1) {
                $(this).addClass('ScoreBogey');
            }
            else if (holePar - holeScore <= -2) {
                $(this).addClass('ScoreDblBogey');
            }
        }
    })
};