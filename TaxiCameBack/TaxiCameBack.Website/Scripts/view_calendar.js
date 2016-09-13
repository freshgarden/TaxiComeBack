/* Wait for DOM to load etc */
$(document).ready(function () {

    //Initialisations
    initialise_calendar();

});

/* Initialises calendar */
function initialise_calendar() {
    //Initialise calendar
    $("#calendar").fullCalendar({
        locale: 'vi',
        firstDay: 1,
        header: {
            left: "today prev,next",
            center: "title",
            right: "month,agendaWeek,listWeek"
        },
        defaultView: "listWeek",
        eventSources: [
        {
            url: "/Admin/Schedule/GetScheduleEvents",
            backgroundColor: "#3c8dbc",
            borderColor: "#3c8dbc"
        }
        ],
        eventClick: function (calEvent, jsEvent, view) {
            window.location.href = "/Admin/Schedule/Edit/" + calEvent.id;
        },
        editable: false
    });
}

function UpdateEvent(calEvent) {
    $("#editModal").modal("show");
    var res = calEvent.title.split(" - ");
    $("#BeginLocation").val(res[0]);
    $("#EndLocation").val(res[1]);
}