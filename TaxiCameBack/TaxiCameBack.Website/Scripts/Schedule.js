var source, destination;
var autoComplete1, autoComplete2;
var directionsDisplay;
var conta = $(this);
var directionsService = new google.maps.DirectionsService();
var options = {
    componentRestrictions: { country: "vn" }
};
google.maps.event.addDomListener(window, 'load', function () {
    autoComplete1 = new google.maps.places.Autocomplete(document.getElementById('BeginLocation'), options);
    autoComplete2 = new google.maps.places.Autocomplete(document.getElementById('EndLocation'), options);
    directionsDisplay = new google.maps.DirectionsRenderer({
        //                 'draggable': true,
        //                 suppressMarkers: true,
    });
    var hanoi = new google.maps.LatLng(21.0031201, 105.82013870000003);
    var mapOptions = {
        zoom: 7,
        center: hanoi
    };
    map = new google.maps.Map(document.getElementById('dvMap'), mapOptions);
    directionsDisplay.setMap(map);

//    var locations = window.vm.getScheduleGeolocation();
    //            for (var i = 0; i < locations.length; i++) {
    //                if (i > 7) break;
    //                wps.push({ location: new google.maps.LatLng(locations[i][0], locations[i][1]), stopover: false });
    //            }

//    var batches = [];
//    var itemsPerBatch = 10; // google API max - 1 start, 1 stop, and 8 waypoints
//    var itemsCounter = 0;
//    var wayptsExist = locations.length > 0;
//
//    while (wayptsExist) {
//        var subBatch = [];
//        var subitemsCounter = 0;
//
//        for (var j = itemsCounter; j < locations.length; j++) {
//            subitemsCounter++;
//            subBatch.push({
//                location: new google.maps.LatLng(locations[j][0], locations[j][1]),
//                stopover: false
//            });
//            if (subitemsCounter == itemsPerBatch)
//                break;
//        }
//
//        itemsCounter += subitemsCounter;
//        batches.push(subBatch);
//        wayptsExist = itemsCounter < locations.length;
//        // If it runs again there are still points. Minus 1 before continuing to 
//        // start up with end of previous tour leg
//        itemsCounter--;
//    }
//    calcRoute(batches, directionsService, directionsDisplay);

    //            GetRoute();

    google.maps.event.addListener(autoComplete1, 'place_changed', function () {
        getSearchChange();
    });
    google.maps.event.addListener(autoComplete2, 'place_changed', function () {
        getSearchChange();
    });
});

function calcRoute(batches, directionsService, directionsDisplay) {
    var combinedResults;
    var unsortedResults = [{}]; // to hold the counter and the results themselves as they come back, to later sort
    var directionsResultsReturned = 0;

    for (var k = 0; k < batches.length; k++) {
        var lastIndex = batches[k].length - 1;
        var start = batches[k][0].location;
        var end = batches[k][lastIndex].location;

        // trim first and last entry from array
        var waypts = [];
        waypts = batches[k];
        waypts.splice(0, 1);
        waypts.splice(waypts.length - 1, 1);

        var request = {
            origin: start,
            destination: end,
            waypoints: waypts,
            optimizeWaypoints: true,
            provideRouteAlternatives: true,
            travelMode: google.maps.DirectionsTravelMode.DRIVING
        };
        (function (kk) {
            directionsService.route(request, function (result, status) {
                if (status == window.google.maps.DirectionsStatus.OK) {

                    var unsortedResult = {
                        order: kk,
                        result: result
                    };
                    unsortedResults.push(unsortedResult);

                    directionsResultsReturned++;

                    if (directionsResultsReturned == batches.length) // we've received all the results. put to map
                    {
                        // sort the returned values into their correct order
                        unsortedResults.sort(function (a, b) {
                            return parseFloat(a.order) - parseFloat(b.order);
                        });
                        var count = 0;
                        for (var key in unsortedResults) {
                            if (unsortedResults[key].result != null) {
                                if (unsortedResults.hasOwnProperty(key)) {
                                    if (count == 0) // first results. new up the combinedResults object
                                        combinedResults = unsortedResults[key].result;
                                    else {
                                        // only building up legs, overview_path, and bounds in my consolidated object. This is not a complete
                                        // directionResults object, but enough to draw a path on the map, which is all I need
                                        combinedResults.routes[0].legs = combinedResults.routes[0].legs.concat(unsortedResults[key].result.routes[0].legs);
                                        combinedResults.routes[0].overview_path = combinedResults.routes[0].overview_path.concat(unsortedResults[key].result.routes[0].overview_path);

                                        combinedResults.routes[0].bounds = combinedResults.routes[0].bounds.extend(unsortedResults[key].result.routes[0].bounds.getNorthEast());
                                        combinedResults.routes[0].bounds = combinedResults.routes[0].bounds.extend(unsortedResults[key].result.routes[0].bounds.getSouthWest());
                                    }
                                    count++;
                                }
                            }
                        }
                        //directionsDisplay.setDirections(combinedResults);
                        if (status == google.maps.DirectionsStatus.OK) {
                            directionsDisplay.setDirections(combinedResults);
                            SetScheduleGeolocation(combinedResults);
                        } else {
                            alert("Sorry! Unable to determine a valid route");
                        }
                    }
                }
            });
        })(k);
    }
    google.maps.event.addListener(directionsDisplay, 'directions_changed', function () {
        SetScheduleGeolocation(directionsDisplay.directions);
    });
}

function getSearchChange() {
    if ($("#BeginLocation").val() != '' && $("#EndLocation").val() != '') {
        $("#BeginLocation").change();
        $("#EndLocation").change();

        GetRoute();
    }
}

$(function () {
    $('#frmCreateSchedule').submit(function (e) {
        if ($("#ScheduleGeolocations").val() === '') {
            e.preventDefault();
            return false;
        }
    });
    $.datetimepicker.setLocale('vi');
    jQuery("#StartDate").datetimepicker({
        format: 'd-m-Y H:i',
        minDate: 0,
        lang: 'vi',
        onChangeDateTime: function (dp, $input) {
            window.vm.setStartDate(dp);
        }
    });
    $('input,textarea').attr('autocomplete', 'on');
    $('input').keydown(function (e) {
        if (e.keyCode === 13) {
            return false;
        }
    });
});


function GetRoute() {
    //*********DIRECTIONS AND ROUTE**********************//
    var sourcePlaceId = autoComplete1.getPlace().place_id;
    var destinationPlaceId = autoComplete2.getPlace().place_id;
    if (!sourcePlaceId && !destinationPlaceId)
        return;
    var request = {
        origin: { 'placeId': sourcePlaceId },
        destination: { 'placeId': destinationPlaceId },
        travelMode: google.maps.TravelMode.DRIVING
    };

    directionsService.route(request, function (response, status) {
        console.log(status)
        if (status === google.maps.DirectionsStatus.OK) {
            directionsDisplay.setDirections(response);
            SetScheduleGeolocation(response);

            google.maps.event.addListener(directionsDisplay, 'directions_changed', function() {
                SetScheduleGeolocation(directionsDisplay.directions);
            });
        } else {
            alert("Could not be found the direction.");
        }
    });
}

function SetScheduleGeolocation(response) {

    var rleg = response.routes[0].legs[0];
    var wp = response.routes[0].overview_path;
    window.vm.clearScheduleGeolocation();
    window.vm.addScheduleGeolocation([rleg.start_location.lat(), rleg.start_location.lng()]);
    for (var i = 0; i < wp.length; i++) {
        window.vm.addScheduleGeolocation([wp[i].lat(), wp[i].lng()]);
    }
    window.vm.addScheduleGeolocation([rleg.end_location.lat(), rleg.end_location.lng()]);
}