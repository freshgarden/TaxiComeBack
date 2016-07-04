﻿var urlSchedule = "/schedule";
var url = window.location.pathname;
var scheduleId = url.substring(url.lastIndexOf('/') + 1);


$(function () {
    ko.validation.rules['simpleDate'] = {
        validator: function (val, validate) {
            return ko.validation.utils.isEmptyVal(val) || moment(val, 'MM/DD/YYYY').isValid();
        },
        message: 'Invalid date'
    };
    ko.validation.registerExtenders();
    var Schedule = function(schedule) {
        var self = this;
        self.ScheduleId = ko.observable(schedule ? schedule.ScheduleId : 0).extend({required: true});
        self.BeginLocation = ko.observable(schedule ? schedule.BeginLocation : '').extend({ required: true });
        self.EndLocation = ko.observable(schedule ? schedule.EndLocation : '').extend({ required: true });
        self.StartDate = ko.observable(schedule ? schedule.StartDate : '').extend({ required: true });
        self.ScheduleGeolocations = ko.observableArray(schedule ? schedule.ScheduleGeolocations : []);
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
                success: function(json) {
                    self.schedule = ko.observable(new Schedule(json));
                }
            });
        }

        self.backToScheduleList = function () { window.location.href = '/schedule'; };

        self.addScheduleGeolocation = function(data) {
            self.scheduleGeolocation.push({ "Latitude": data[0], "Longitude": data[1] });
        }
        
        self.scheduleErrors = ko.validation.group(self.schedule());

        self.scheduleGeolocationErrors = ko.validation.group(self.scheduleGeolocation(), { deep: true });

        self.saveSchedule = function () {

            var isValid = true;
            if (self.scheduleErrors().length != 0) {
                self.scheduleErrors.showAllMessages();
                isValid = false;
            }

            if (self.scheduleGeolocationErrors().length != 0) {
                isValid = false;
            }

            if (isValid) {
                self.schedule().ScheduleGeolocations = self.scheduleGeolocation;
                $.ajax({
                    type: (self.schedule().ScheduleId > 0 ? 'PUT' : 'POST'),
                    cache: false,
                    dataType: 'json',
                    url: urlSchedule + (self.schedule().ScheduleId > 0 ? '/UpdateScheduleInfomation?id=' + self.schedule().ScheduleId : '/SaveScheduleInfomation'),
                    data: JSON.stringify(ko.toJS(self.schedule())),
                    contentType: 'application/json; charset=utf-8',
                    async: false,
                    success: function(data) {
                        window.location.href = '/schedule';
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