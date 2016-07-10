var urlSchedule = "/admin/schedule";
var url = window.location.pathname;
var scheduleId = url.substring(url.lastIndexOf('/') + 1);
var scheduleGeolocations = [];
var startDate;

$(function () {
    function getFormattedDate(date)
    {
        var year = date.getFullYear();
        var month = (1 + date.getMonth()).toString();
        month = month.length > 1 ? month : '0' + month;
        var day = date.getDate().toString();
        day = day.length > 1 ? day : '0' + day;
        return day + '-' + month + '-' + year + " " + date.getHours() + ":" + date.getMinutes();
    }
    function toJavaScriptDate(value) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));
        return getFormattedDate(dt);
    }
    function getScheduleGeolocations(json) {
        var object = [];
        scheduleGeolocations = [];
        scheduleGeolocations = json;
        for (var i = 0; i < json.length; i++) {
            object.push(new ScheduleGeolocation(json[i]));
        }
        return object;
    }
    var container = $(this);
    var Schedule = function(schedule) {
        var self = this;
        self.Id = ko.observable(schedule ? schedule.ScheduleId : 0).extend({required: true});
        self.BeginLocation = ko.observable(schedule ? schedule.BeginLocation : '').extend({ required: true });
        self.EndLocation = ko.observable(schedule ? schedule.EndLocation : '').extend({ required: true });
        self.StartDate = ko.observable(schedule ? toJavaScriptDate(schedule.StartDate) : '').extend({ required: true });
        self.ScheduleGeolocations = ko.observableArray(schedule ? getScheduleGeolocations(schedule.ScheduleGeolocations) : []);
    }

    var ScheduleGeolocation = function(scheduleGeolocation) {
        var self = this;
        self.Latitude = ko.observable(scheduleGeolocation ? scheduleGeolocation.Latitude : 0).extend({ required: true });
        self.Longitude = ko.observable(scheduleGeolocation ? scheduleGeolocation.Longitude : 0).extend({ required: true });
    }

    var ScheduleCollection = function() {
        var self = this;

        if (scheduleId == 0 || isNaN(scheduleId)) {
            self.schedule = ko.observable(new Schedule());
            self.scheduleGeolocation = ko.observableArray([new ScheduleGeolocation()]);
        } else {
            $.ajax({
                url: urlSchedule + '/GetScheduleById/' + scheduleId,
                async: false,
                dataType: 'json',
                success: function (json) {
                    self.schedule = ko.observable(new Schedule(json));
                }
            });
        }

        self.addScheduleGeolocation = function (data) {
            if (!self.scheduleGeolocation)
                self.scheduleGeolocation = ko.observableArray([new ScheduleGeolocation()]);
            self.scheduleGeolocation.push({ "Latitude": data[0], "Longitude": data[1] });
        }
        
        self.clearScheduleGeolocation = function() {
            self.scheduleGeolocation = [];
        }

        self.scheduleErrors = ko.validation.group(self.schedule());

//        self.scheduleGeolocationErrors = ko.validation.group(self.scheduleGeolocation, { deep: true });

        self.getScheduleGeolocation = ko.computed(function() {
            var geo = [];
            for (var key in scheduleGeolocations) {
                if (scheduleGeolocations.hasOwnProperty(key)) {
                    if (scheduleGeolocations[key].Latitude != 0 && scheduleGeolocations[key].Longitude != 0)
                        geo.push([scheduleGeolocations[key].Latitude, scheduleGeolocations[key].Longitude]);
                }
            }
            return geo;
        });

        self.setStartDate = function (date) {
            startDate = date;
        }

        self.showErrorPopup=function (container, popup, message) {
            container.jqsDialog("showCommonPopup", {
                message: message ? message : "Error saving change!",
                title: "Error",
                icon: "error",
                popup: popup
            });
            return;
        }

        self.saveSchedule = function () {

            var isValid = true;
            if (self.scheduleErrors().length != 0) {
                self.scheduleErrors.showAllMessages();
                isValid = false;
            }

//            if (self.scheduleGeolocationErrors().length != 0) {
//                isValid = false;
//            }

            if (isValid) {
                self.schedule().ScheduleGeolocations = self.scheduleGeolocation;
                var monthsName = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                self.schedule().StartDate = ko.observable(startDate.getDate() + "-" + monthsName[startDate.getMonth()] + "-" + startDate.getFullYear() + " " + startDate.getHours() + ":" + startDate.getMinutes());
                var popup;
                $.ajaxAntiForgery({
                    type: (self.schedule().ScheduleId > 0 ? "PUT" : "POST"),
                    cache: false,
                    dataType: "json",
                    url: urlSchedule + (self.schedule().ScheduleId > 0 ? "/UpdateScheduleInfomation?id=" + self.schedule().ScheduleId : '/SaveScheduleInfomation'),
                    data: ko.toJS(self.schedule()),
                    contentType: "application/x-www-form-urlencoded",
                    async: false,
                    beforeSend: function () {
                        popup = container.jqsDialog("showProgressBar");
                        popup.setHeader("no-x");
                    },
                    success: function (data) {
                        if (data.status === 'OK') {
                            popup.close();
                            window.location.href = urlSchedule;
                        }
                        else if (data.status === "ERROR") {
                            if (data.messenge) {
                                self.showErrorPopup(container, popup, data.messenge[0].Value[0]);
                                return;
                            }
                            self.showErrorPopup(container, popup, data.messenge);
                        }
                    },
                    error: function(err) {
                        var err = JSON.parse(err.responseText);
                        var errors = '';
                        for (var key in err) {
                            if (err.hasOwnProperty(key)) {
                                errors += key.replace("schedule.", "") + " : " + err[key];
                            }
                        }
                        $("<div></div>").html(errors).dialog({
                            modal: true,
                            title: JSON.parse(err.responseText).Message,
                            buttons: {
                                "Ok": function() {
                                    $(this).dialog("close");
                                }
                            }
                        }).show();

                        self.showErrorPopup(container, popup);
                    },
                    complete: function() {}
                });
            }
        }
    }
    window.vm = new ScheduleCollection();
    ko.applyBindings(vm);
});

var clone = (function () {
    return function (obj) {
        Clone.prototype = obj;
        return new Clone();
    };
    function Clone() { }
}());