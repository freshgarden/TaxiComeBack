/* Wait for DOM to load etc */
$(document).ready(function(){

	//Initialisations
	initialise_calendar();
	initialise_update_event();
	
});




/* Initialises calendar */
function initialise_calendar(){
	//Initialise calendar
	$('#calendar').fullCalendar({
		firstDay: 1,
		header: {
			left: 'today prev,next',
			center: 'title',
			right: 'month,agendaWeek'
		},
		defaultView: 'month',
		//minTime: '6:00am',
		//maxTime: '6:00pm',
		allDaySlot: false,
		columnFormat: {
			month: 'ddd',
			week: 'ddd dd/MM',
			day: 'dddd M/d'
		},
		eventSources: [
			{
			    url: '/Admin/Schedule/GetScheduleEvents',
			    backgroundColor: "#f56954",
			    borderColor: "#f56954",
			    editable: true
			}
		],
		droppable: false,
		eventClick: function (cal_event, js_event, view) {
		    window.location.href = "/Admin/Schedule/Create/" + cal_event.id;
		    calendar_event_clicked(cal_event, js_event, view);
//		    UpdateEvent(cal_event);
		},
		dayClick: function (date, allDay, jsEvent, view) {
		    window.location.href = "/Admin/Schedule/Create";
		},
		editable: false,
		selectable: true,
		select: function(){
		},
	});	
}


/* Initialise event clicks */
function calendar_event_clicked(cal_event, js_event, view){

	//Set generation values
	set_event_generation_values(cal_event.id, cal_event.backgroundColor, cal_event.borderColor, cal_event.textColor, cal_event.title, cal_event.description, cal_event.price, cal_event.available);
}


/* Set event generation values */
function set_event_generation_values(event_id, bg_color, border_color, text_color, title, description, price, available){

	//Set values
	$('#txt_title').val(title);
	$('#txt_description').val(description);
	$('#txt_price').val(price);
	$('#txt_available').val(available);
	$('#txt_current_event').val(event_id);
}


/* Generate unique id */
function get_uni_id(){

	//Generate unique id
	return new Date().getTime() + Math.floor(Math.random()) * 500;
}


/* Initialise update event button */
function initialise_update_event(){
	var test = $('#calendar').fullCalendar( 'clientEvents');
	//Bind event
	$('#btn_update_event').bind('click', function(){

		//Create vars
		var current_event_id = $('#txt_current_event').val();

		//Check if value found
		if(current_event_id){

			//Retrieve current event
			var current_event = $('#calendar').fullCalendar('clientEvents', current_event_id);

			//Check if found
			if(current_event && current_event.length == 1){

				//Retrieve current event from array
				current_event = current_event[0];

				//Set values
				current_event.backgroundColor = $('#txt_background_color').val();
				current_event.textColor = $('#txt_text_color').val();
				current_event.borderColor = $('#txt_border_color').val();
				current_event.title = $('#txt_title').val();
				current_event.description = $('#txt_description').val();
				current_event.price = $('#txt_price').val();
				current_event.available = $('#txt_available').val();

				//Update event
				$('#calendar').fullCalendar('updateEvent', current_event);
			}
		}
	});
}



function UpdateEvent(cal_event) {
	
    $("#editModal").modal('show');
    var res = cal_event.title.split(" - ")
    $("#BeginLocation").val(res[0]);
    $("#EndLocation").val(res[1]);
}

function ClearPopupFormValues() {
	$('#eventID').val("");
	$('#eventTitle').val("");
	$('#eventDateTime').val("");
	$('#eventDuration').val("");
}