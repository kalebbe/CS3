/*
 * @authors:   Kaleb Eberhart, Sunjil Gahatraj
 * @date:      02/11/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.1
 * @file:      ClickDetector.js
 * @revision:  1.0
 * @synapsis:  This js file is going to be used to detect right clicks. This logic
 *             may not be used, but we're going to keep it here for now.
 */
$('#element').mousedown(function (event) {
    switch (event.which) {
        case 1:
            //left click
            break;
        case 2:
            //Middle mouse click.. don't do anything.
            break;
        case 3:
            //Right click
            break;
        default:
            console.log("You won't be able to use that mouse homie");
    }
});