var FormDropzone = {
    init: function (id, imageId, fileSize, msgBig, msgExc, msgRem, msgDef, btnSaveId, btnCancelId) {
        $(id).dropzone({
            url: "/Upload/Upload",
            maxFiles: 1,
            maxFilesize: fileSize,
            acceptedFiles: 'image/jpeg,image/png',
            dictFileTooBig: msgBig,
            dictMaxFilesExceeded: msgExc,
            dictDefaultMessage: msgDef,
            init: function () {
                var image = $("#" + imageId).val();

                var pathOld = null;

                if (image !== undefined && image != null && image != "") {
                    var thisDropzone = this;
                    var removeButton = Dropzone.createElement("<a class='btn default btn-block'>" + msgRem + "</a>");
                    //Create new file with logo path
                    var mockFile = {
                    };
                    thisDropzone.emit("addedfile", mockFile);
                    thisDropzone.emit("thumbnail", mockFile, image);
                    mockFile.previewElement.appendChild(removeButton);

                    removeButton.addEventListener("click", function (e) {
                        // Make sure the button click doesn't submit the form:
                        e.preventDefault();
                        e.stopPropagation();
                        // Remove the file preview.
                        thisDropzone.removeFile(mockFile);
                        // If you want to the delete the file on the server as well,
                        // you can do the AJAX request here.
                        var id = $(thisDropzone.element).attr('id');
                        pathOld = $('input:hidden:last', '#' + id).val();

                        $('input:hidden:last', '#' + id).val("");

                    });
                }

                var btnSave = "#" + btnSaveId;
                $(btnSave).click(function () {
                    if (pathOld != null) {
                        $.ajax({
                            type: 'POST',
                            url: '/Upload/Delete',
                            contentType: 'application/json; charset=utf-8',
                            data: JSON.stringify({ 'path': pathOld }),
                            dataType: 'json',
                            success: function (result) {
                                // nothing
                            },
                            error: function (result) {
                                //nothing
                            }
                        });
                    }
                });


                this.on("complete", function (data) {
                    $('input:hidden:last', '#' + id).val(null);
                    var res = JSON.parse(data.xhr.responseText);
                    if (res == null || !res.isSuccess) {
                        alert("Failed to upload file");
                        return;
                    }
                    var id = $(this.element).attr('id');
                    $('input:hidden:last', '#' + id).val(res.path);
                });

                this.on("maxfilesexceeded", function (file) {
                    this.removeFile(file);
                });
                //Declare flag for change image
                var changedImage = false;

                this.on("addedfile", function (file) {
                    changedImage = true;

                    var id = $(this.element).attr('id');
                    var existImg = $('input:hidden:last', '#' + id).val();
                    if (existImg !== undefined && existImg != null && existImg != "") {
                        // delete old image
                        var myDropzone = '#' + id + " .dz-preview button.btn-sm.btn-block";
                        $(myDropzone).click();
                    }
                    // Create the remove button
                    var removeButton = Dropzone.createElement("<a class='btn default btn-block'>" + msgRem + "</a>");

                    // Capture the Dropzone instance as closure.
                    var _this = this;

                    // Listen to the click event
                    removeButton.addEventListener("click", function (e) {
                        // Make sure the button click doesn't submit the form:
                        e.preventDefault();
                        e.stopPropagation();

                        // Remove the file preview.
                        _this.removeFile(file);
                        // If you want to the delete the file on the server as well,
                        // you can do the AJAX request here.
                        var id = $(_this.element).attr('id');
                        var data = $('input:hidden:last', '#' + id).val();

                        $.ajax({
                            type: 'POST',
                            url: '/Upload/Delete',
                            contentType: 'application/json; charset=utf-8',
                            data: JSON.stringify({ 'path': data }),
                            dataType: 'json',
                            success: function (result) {
                                // Remove data when click remove button
                                $('input:hidden:last', '#' + id).val("");
                            },
                            error: function (result) {
                                //nothing
                            }
                        });
                    });

                    // Add the button to the file preview element.
                    file.previewElement.appendChild(removeButton);

                });

                var btnCancel = "#" + btnCancelId;
                $(btnCancel).click(function () {
                    if (changedImage) {
                        var removeImage = id + ' .dz-preview button.btn-sm.btn-block';
                        $(removeImage).click();
                    }
                });
            }
        });

    }
};