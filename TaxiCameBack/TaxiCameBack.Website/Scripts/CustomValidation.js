$(function () {
    $.validator.addMethod("validimage", function (value, element, param) {
        if (value == "" || value == null || value == undefined) {
            return true;
        }
        var rulesErrormessages = param["errormessages"];
        var maxSize = param.filesize;
        var $element = $(element);

        /// valid extension
        var extension = getFileExtension(value);
        var validExtension = $.inArray(extension, param.fileextensions) !== -1;
        if (!validExtension) {
            $.validator.messages.validimage = rulesErrormessages[0];
            return false;
        }

        // valid file size
        var files = $element.closest('form').find(':file[name=' + $element.attr('name') + ']');
        var totalFileSize = 0;
        files.each(function () {
            var file = $(this)[0].files[0];
            if (file && file.size) {
                totalFileSize += file.size;
            }
        });
        if (totalFileSize > maxSize)
        {
            $.validator.messages.validimage = rulesErrormessages[1];
            return false;
        }
        return true;
    }, "This is not a valid image"//defualt error message
);

    $.validator.unobtrusive.adapters.add('validimage', ['fileextensions', 'filesize', 'errormessages'], function (options) {
        options.rules['validimage'] = {
            fileextensions: options.params.fileextensions.split(','),
            filesize: options.params.filesize,
            errormessages: options.params['errormessages'].split(',')
        };
    });

    function getFileExtension(fileName) {
        var extension = (/[.]/.exec(fileName)) ? /[^.]+$/.exec(fileName) : undefined;
        if (extension != undefined) {
            return extension[0];
        }
        return extension;
    };
}(jQuery))