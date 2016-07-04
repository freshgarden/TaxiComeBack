var ScheduleViewModel = function() {
    var self = this;
    var url = "/schedule/GetAllSchedules";
    var refresh = function() {
        $.getJSON(url, {}, function (data) {
            self.Schedules(data);
        });
    }

    self.Schedules = ko.observableArray([]);

    self.createSchedule = function() {
        window.location.href = '/Schedule/CreateEdit/0';
    };

    self.editSchedule = function(schedule) {
        window.location.href = '/Schedule/CreateEdit/' + schedule.ScheduleId;
    }

    self.removeSchedule = function(schedule) {
        if (confirm("Are you sure you want to delete this schedule?")) {
            var id = schedule.ScheduleId;
            waitingDialog({});
            $.ajax({
                type: 'DELETE',
                url: 'Schedule/DeleteSchedule/' + id,
                success: function() { self.Profiles.remove(profile); },
                error: function(err) {
                    var error = JSON.parse(err.responseText);
                    $("<div></div>").html(error.Message).dialog({
                        modal: true,
                        title: "Error",
                        buttons: {
                             "Ok": function() {
                                  $(this).dialog("close");
                             }
                        }
                    }).show();
                },
                complete: function() { closeWaitingDialog(); }
            });
        }
    }
    refresh();
};
ko.applyBindings(new ScheduleViewModel());