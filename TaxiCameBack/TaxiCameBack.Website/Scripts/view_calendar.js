/* Wait for DOM to load etc */
$(document).ready(function(){

	//Initialisations
	initialise_calendar();
	
});

/* Initialises calendar */
function initialise_calendar(){
	//Initialise calendar
	$("#calendar").fullCalendar({
		firstDay: 1,
		header: {
			left: "today prev,next",
			center: "title",
			right: "month,agendaWeek"
		},
		defaultView: "month",
		//minTime: '6:00am',
		//maxTime: '6:00pm',
		allDaySlot: false,
		eventLimit: true,
		columnFormat: {
			month: "ddd",
			week: "ddd dd/MM",
			day: "dddd M/d"
		},
		eventSources: [
			{
			    url: "/Admin/Schedule/GetScheduleEvents",
			    backgroundColor: "#f56954",
			    borderColor: "#f56954",
			    editable: true
			}
		],
		droppable: false,
		eventClick: function (calEvent, jsEvent, view) {
		    window.location.href = "/Admin/Schedule/Edit/" + calEvent.id;
		},
		dayClick: function (date, allDay, jsEvent, view) {
		    window.location.href = "/Admin/Schedule/Create";
		},
		editable: false,
		selectable: true,
		select: function(){
		}
	});	
}


function UpdateEvent(calEvent) {
	
    $("#editModal").modal("show");
    var res = calEvent.title.split(" - ");
    $("#BeginLocation").val(res[0]);
    $("#EndLocation").val(res[1]);
}