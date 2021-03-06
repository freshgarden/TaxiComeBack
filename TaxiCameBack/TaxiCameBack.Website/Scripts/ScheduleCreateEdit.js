﻿var urlSchedule = "/admin/schedule";
var url = window.location.pathname;
var scheduleId = url.substring(url.lastIndexOf('/') + 1);
var scheduleGeolocations = [];
var startDate;
var scheduleGuid;

$(function () {
    function getFormattedDate(date)
    {
        var year = date.getFullYear();
        var month = (date.getMonth() + 1).toString();
        month = month.length > 1 ? month : '0' + month;
        var day = date.getDate().toString();
        day = day.length > 1 ? day : '0' + day;
        var hours = date.getHours().toString();
        hours = hours.length > 1 ? hours : '0' + hours;
        var minutes = date.getMinutes().toString();
        minutes = minutes.length > 1 ? minutes : '0' + minutes;
        var formDate = day + '-' + month + '-' + year + " " + hours + ":" + minutes;
        startDate = date;//new Date(year, month, day, date.getHours(), date.getMinutes());
        return formDate;
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
    function generateUUID() {
        var d = new Date().getTime();
        if (window.performance && typeof window.performance.now === "function") {
            d += performance.now();; //use high-precision timer if available
        }
        var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (d + Math.random() * 16) % 16 | 0;
            d = Math.floor(d / 16);
            return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });
        return uuid;
    };
    function isGuid(stringToTest) {
        if (stringToTest[0] === "{") {
            stringToTest = stringToTest.substring(1, stringToTest.length - 1);
        }
        var regexGuid = /^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$/gi;
        return regexGuid.test(stringToTest);
    }
    var container = $(this);
    var Schedule = function(schedule) {
        var self = this;
        self.Id = ko.observable(schedule ? schedule.Id : scheduleGuid).extend({required: true});
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

        if (!isGuid(scheduleId)) {
            scheduleGuid = generateUUID();
            self.schedule = ko.observable(new Schedule());
            self.scheduleGeolocation = ko.observableArray([new ScheduleGeolocation()]);
        } else {
            $.ajax({
                url: urlSchedule + '/GetScheduleById/' + scheduleId,
                async: false,
                dataType: 'json',
                success: function (json) {
                    var buttonUpdate = document.getElementById("btnUpdateSchedule");
                    var buttonDelete = document.getElementById("btnDeleteSchedule");
                    if (buttonUpdate) {
                        if (json.CanUpdate === 0) {
                            document.getElementById("BeginLocation").disabled = true;
                            document.getElementById("EndLocation").disabled = true;
                            document.getElementById("StartDate").disabled = true;
                            buttonUpdate.style.visibility = 'hidden';
                        } else {
                            buttonUpdate.style.visibility = 'visible';
                        }
                    }
                    if (buttonDelete) {
                        if (json.CanUpdate === 1) {
                            buttonDelete.style.visibility = 'visible';
                        } else {
                            buttonDelete.style.visibility = 'hidden';
                        }
                    }
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
                    type: (ko.toJS(self.schedule().Id) !== scheduleGuid ? "PUT" : "POST"),
                    cache: false,
                    dataType: "json",
                    url: urlSchedule + (ko.toJS(self.schedule().Id) !== scheduleGuid ? "/UpdateScheduleInfomation?id=" + ko.toJS(self.schedule().Id) : '/SaveScheduleInfomation'),
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
                                if (data.messenge[0].Value[0]) {
                                    self.showErrorPopup(container, popup, data.messenge[0].Value[0]);
                                    return;
                                }
                                self.showErrorPopup(container, popup, data.messenge[0]);
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

        self.deleteSchedule = function () {
            if (ko.toJS(self.schedule().Id) === scheduleGuid) {
                return;
            }
            var popup;
            $.ajaxAntiForgery({
                type: "POST",
                cache: false,
                dataType: "json",
                url: urlSchedule + "/DeleteSchedule?id=" + ko.toJS(self.schedule().Id),
                data: ko.toJS(self.schedule().Id),
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
                            if (data.messenge[0].Value[0]) {
                                self.showErrorPopup(container, popup, data.messenge[0].Value[0]);
                                return;
                            }
                            self.showErrorPopup(container, popup, data.messenge[0]);
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