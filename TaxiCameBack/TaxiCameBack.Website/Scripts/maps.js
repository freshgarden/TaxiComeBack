//$(document).ready(function () {

//    //Initialisations
//    initialise_map();

//});

//function initialise_map() {   

//    loadScript('http://maps.googleapis.com/maps/api/js?key=AIzaSyA1naPg1USZOsy3uiEJv0A71GwcGeaOTS8&v=3&libraries=places&sensor=false&callback=initialize', function () { positionFooter(); });
    
//}

//function loadScript(src) {
//    var script = document.createElement("script");
//    script.type = "text/javascript";
//    document.getElementsByTagName("head")[0].appendChild(script);
//    script.src = src;
//}

//function initialize() {

//    var source, destination;
//    var autoComplete1, autoComplete2;
//    var directionsDisplay;
//    var conta = $(this);
//    var wps = [];
//    var directionsService = new google.maps.DirectionsService();
//    var options = {
//        componentRestrictions: { country: "vn" }
//    };

//    google.maps.event.addDomListener(window, 'load', function () {
//        autoComplete1 = new google.maps.places.Autocomplete(document.getElementById('BeginLocation'), options);
//        autoComplete2 = new google.maps.places.Autocomplete(document.getElementById('EndLocation'), options);
//        directionsDisplay = new google.maps.DirectionsRenderer({ 'draggable': true });
//        var hanoi = new google.maps.LatLng(21.0031201, 105.82013870000003);
//        var mapOptions = {
//            center: hanoi
//        };
//        map = new google.maps.Map(document.getElementById('dvMap'), mapOptions);
//        directionsDisplay.setMap(map);
//        var locations = window.vm.getScheduleGeolocation;

//        for (var i = 0; i < locations.length; i++) {
//            console.log(locations[i][0]);
//            wps.push({ location: new google.maps.LatLng(locations[i][0], locations[i][1]) });
//        }

//        GetRoute(wps,directionsService,directionsDisplay);

//        google.maps.event.addListener(autoComplete1, 'place_changed', function () {
//            if ($("#BeginLocation").val() != '' && $("#EndLocation").val() != '') {
//                GetRoute(wps,directionsService,directionsDisplay);
//            }
//        });
//        google.maps.event.addListener(autoComplete2, 'place_changed', function () {
//            if ($("#BeginLocation").val() != '' && $("#EndLocation").val() != '') {
//                GetRoute(wps,directionsService,directionsDisplay);
//            }
//        });
//    });

//    google.maps.event.addDomListener(window, 'load', initialize);

//    google.maps.event.addDomListener(window, "resize", resizingMap())

//}

//$('#editModal').on('show.bs.modal', function () {
//    //Must wait until the render of the modal appear, thats why we use the resizeMap and NOT resizingMap!! ;-)
//    resizeMap();
//})

//function resizeMap() {
//    if (typeof map == "undefined") return;
//    setTimeout(function () { resizingMap(); }, 400);
//}

//function resizingMap() {
//    if (typeof map == "undefined") return;
//    var center = map.getCenter();
//    google.maps.event.trigger(map, "resize");
//    map.setCenter(center);
//}
//$(function () {
//    $('#frmCreateSchedule').submit(function (e) {
//        if ($("#ScheduleGeolocations").val() === '') {
//            e.preventDefault();
//            return false;
//        }
//    });
//    $("#StartDate").datepicker({
//        dateFormat: 'dd-mm-yy',
//        minDate: 0
//    });
//    $('input,textarea').attr('autocomplete', 'on');
//    $('input').keydown(function (e) {
//        if (e.keyCode === 13) {
//            return false;
//        }
//    });
//});


//function GetRoute(wps, directionsService, directionsDisplay) {
//    //*********DIRECTIONS AND ROUTE**********************//
//    source = document.getElementById("BeginLocation").value;
//    destination = document.getElementById("EndLocation").value;

//    var request = {
//        origin: source,
//        waypoints: wps,
//        destination: destination,
//        travelMode: google.maps.TravelMode.DRIVING
//    };
//    directionsService.route(request, function (response, status) {
//        if (status === google.maps.DirectionsStatus.OK) {
//            directionsDisplay.setDirections(response);
//            SetScheduleGeolocation(response);

//            google.maps.event.addListener(directionsDisplay, 'directions_changed', function () {
//                SetScheduleGeolocation(directionsDisplay.directions);
//            });
//        }
//    });
//}

//function SetScheduleGeolocation(response) {
//    var rleg = response.routes[0].legs[0];
//    var wp = response.routes[0].overview_path;
//    window.vm.clearScheduleGeolocation();
//    window.vm.addScheduleGeolocation([rleg.start_location.lat(), rleg.start_location.lng()]);
//    for (var i = 0; i < wp.length; i++) {
//        window.vm.addScheduleGeolocation([wp[i].lat(), wp[i].lng()]);
//    }
//    window.vm.addScheduleGeolocation([rleg.end_location.lat(), rleg.end_location.lng()]);
//}