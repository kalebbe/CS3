/*
 * @authors:   Kaleb Eberhart
 * @date:      03/11/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.3
 * @file:      Click.js
 * @revision:  1.0
 * @synapsis:  The purpose of this script is to block the contextmenu
 *             from appearing when the user right clicks and then to
 *             post a form with the right click.
 */

//This is for blocking the contextmenu. Does not work < IE 9, but who uses that anyways.
document.addEventListener('contextmenu', function (e) {
    e.preventDefault();
}, false);

function Post(x, y) {
    //This creates a form with ajax.
    var form = $('<form></form>');

    //Setting the post and controller action.
    form.attr("method", "post");
    form.attr("action", "RightClick");

    //This adds the hidden input fields that hold the coordinates of the cell.
    $(form).append("<input type='hidden' name='xcoor' value='" + x + "' />");
    $(form).append("<input type='hidden' name='ycoor' value='" + y + "' />")

    //This is required because it attaches this invisible form to the view.
    $(document.body).append(form);
    form.submit();
}