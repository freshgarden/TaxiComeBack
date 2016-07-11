(function ($) {    
    $(document).ready(function () {
        //$("body").jqsDialog();
        $(window).resize(function () {
            centerVdc();
        });
    });

    //Functions for checking browser type and version
    var browserDetect =
    {
        init: function () {
            this.browser = this.searchString(this.dataBrowser) || "Other";
            this.version = this.searchVersion(navigator.userAgent) || this.searchVersion(navigator.appVersion) || "Unknown";
        },
        searchString: function (data) {
            for (var i = 0 ; i < data.length ; i++) {
                var dataString = data[i].string;
                this.versionSearchString = data[i].subString;

                if (dataString.indexOf(data[i].subString) != -1) {
                    return data[i].identity;
                }
            }
        },
        searchVersion: function (dataString) {
            var index = dataString.indexOf(this.versionSearchString);
            if (index == -1) return;
            return parseFloat(dataString.substring(index + this.versionSearchString.length + 1));
        },
        dataBrowser:
        [
            { string: navigator.userAgent, subString: "Chrome", identity: "Chrome" },
            { string: navigator.userAgent, subString: "MSIE", identity: "IE" },
            { string: navigator.userAgent, subString: "Firefox", identity: "Firefox" },
            { string: navigator.userAgent, subString: "Safari", identity: "Safari" },
            { string: navigator.userAgent, subString: "Opera", identity: "Opera" }
        ]
    };
    browserDetect.init();    
        
    var centerVdc = function () {
        var vdc = $('.vdc-ctr');
        //IE 8 needs special care...
        if (browserDetect.browser == "IE" && (browserDetect.version <= 8)) {
            vdc.css("position", "fixed");
            vdc.position({
                of: $(window)
            });
        }
        else {
            vdc.css('position', 'absolute').css({
                left: ($(window).width() - vdc.width()) / 2,
                top: ($(window).height() - vdc.height()) / 2
            });
        }
    };

    //Set plugin-level virtual constants
    var constants = {
        //Virtual constances
        //Progress bar image
        DEF_LOAD_PROGRESS_IMAGE: $("<img src='" + location.protocol + '//' + location.host + '/' + "/Content/jqsDialog/Images/ajax-loader.gif' alt='Loading...'/>"),
        //Css classes from bootstrap
        BS_HEADER_CSS: "modal-header",
        BS_BODY_CSS: "modal-body",
        BS_FOOTER_CSS: "modal-footer",
        //New or customizing css classes by default 
        CM_DEF_DIALOG_CSS: "jqs-dialog-main",
        CM_DEF_HEADER_CSS: "jqs-dialog-header",
        CM_DEF_BODY_CSS: "jqs-dialog-body",
        CM_DEF_FOOTER_CSS: "jqs-dialog-footer",
        //Icon css classes
        ICON_INFO_CSS: "dialog-icon-info",
        ICON_WARNING_CSS: "dialog-icon-warning",
        ICON_ERROR_CSS: "dialog-icon-error",
        ICON_QUESTION_CSS: "dialog-icon-question",
        //Others
        DEF_DIALOG_WIDTH: 250,
        DEF_MAX_HEIGHT: $(window).height() - 120,
        DEF_TITIE_TEXT: "Message",
        DEF_TITIE_TAG: "h4",
        DEF_ACTION_BUTTON_TYPE: "button",
        DEF_CLOSE_BUTTON_LABEL: "OK"
    };

    var methods = {
        init: function (args) {
            return this;
        },

        /**************************            
        popupDialog Function Specs
        --------------------------
        //Input
        args.message: String.
        args.id: String. Unique ID of instantiated dialog. Default = GUID.
        args.width: Numeric. Sets visible dialog width in pixels. Default = DEF_DIALOG_WIDTH.
        args.maxHeight: Numeric. Maximum height in pixels to which the dialog can grow in order to accommodate for content. 
                        If content exceeds identified maximum height, a vertical scrolling feature is engaged.
                        Default = DEF_MAX_HEIGHT.
        args.title: String. Default = DEF_TITIE_TEXT.
        args.titleTag: String. Default = DEF_TITIE_TAG.             
        args.icon: String. Shows icon to the left of the content. Valid values: "info", "warning", "error", "question".
        args.actionButtonLabel: String. If has a value, shows built-in action button.
        args.closeButtonLabel: String. If has a value, shows built-in close button. Default = DEF_CLOSE_BUTTON_LABEL.            
        args.actionButtonType: String. Value is "button" or "submit". Default = DEF_ACTION_BUTTON_TYPE if actionButtonLabel has value.
        args.mainCssClass: String. Custom CSS class name for main element. Default = CM_DEF_DIALOG_CSS.            
        args.headerCssClass: String. Custom CSS class name for header element. Default = CM_DEF_HEADER_CSS.
        args.bodyCssClass: String. Custom CSS class name for body element. Default = CM_DEF_BODY_CSS.
        args.footerCssClass: String. Custom CSS class name for footer element. Default = CM_DEF_FOOTER_CSS.
        args.animation: string. Set slide animation. Value is "left,(milliscond)" or "top,(milliscond)". 
        args.clickOutside: bool. Set click-outside to close dialog. Disabled by default for base dialog. 
        args.modal: bool. true for modal and false for non-modal. Default is true.
        args.fadeIn: bool. true for modal and false for non-modal. Default is true.
        args.parentResetClickOutside: parent instance for resuming click-outside after closing this dialog (as child).
        args.parentsToClose: parent instance object or array for closing together with current child.
        args.stopClose: bool. Flag to disable auto close.
        --------------------
        //Return: the dialog instance
        --------------------
        //Members published as JQuery DOM elements:
        mainDialog:	Outmost visible dialog div element.
        header:	Header bar div element.         
        body	All: body area div element including both icon and content.
        iconElement: Icon area div element. 
        contentElement:	Content area div element.
        footer:	footer bar div element.
        closeButton:  pre-defined button element for closing or cancelling the dialog.
        actionButton: pre-defined button element performing submit actions.        
        --------------------
        //Members published as functions:
        close(): Close the dialog.        
        setId(string): Overwrite default GUID for the dialog ID.
        setCloseBtn(label, stopClose): Add a new close button or update the existing button label text. Clicking button will automatically close the dialog if stopClose argument is undefined.
        setActionBtn(label, type): Add a new or update the existing action button with buttonLable text and/or buttonType “button” or “submit”.
        setOtherBtn(label, type, existingBtn): Add and return any button to the left side or update existing button label. No event is raised.  
        maxHeight(pxNumber): Set the maximum dialog height with the number in pixels. If the content exceeds the height, a vertical scroll bar will appear.
        width(pxNumber): Set the dialog width height with the number in pixels. If the fixed content exceeds the width, a horizontal scroll bar will appear.
        title(text): Set the title text in the header.
        titleTag(text): Set the title element as string: “h3”, “h4”, or “h5”.
        setHeader(string): Set header with x button (input undefined) and append title tag element. Must be called if titleTag changed in updating mode.
        animation(string): Set the sliding animation with string "left,(milliscond)" or "top,(milliscond)". For example “top,200”.
        icon(mode): Set icon with the mode string as “info”, warning”, “error”, “question”.
        content(htmlOrText): Set the body content with HTML or pure text.
        mainCssClass(text): Set name of custom css class for main dialog.
        headerCssClass(text)	: Set name of custom css class for dialog header.
        bodyCssClass(text): Set name of custom css class for dialog body.
        footerCssClass(text)	: Set name of custom css class for dialog footer.
        clickOutside(bool): Click outside to close dialog.
        fadeIn(bool): Set fade in.
        ---------------------- -          
        //Properties set before calling vdc close function
        stopClose: bool. 
        parentsToClose: parent vdc instance object or array
        -----------------------
        //Events: 
        "action": Raised by clicking the action button.
        "closing": Raised by clicking the close button.
        **********************/
        popupDialog: function (args) {            
            //Set default args to message string or empty object
            var message;
            if (typeof args == 'string' || args instanceof String) message = args;            
            if (!args) args = {};
            
            //Message passed in args object (args.message could also be undefined)
            if (message == undefined) message = args.message;

            //Argument fields initialization  
            var uniqueId = args.id == undefined ? "jqsd-" + methods.guidGenerator() : args.id;
            var width = Number(args.width);
            if (args.width == undefined || isNaN(width)) width = constants.DEF_DIALOG_WIDTH;
            var maxHeight = args.maxHeight == undefined || isNaN(args.maxHeight) ? constants.DEF_MAX_HEIGHT : args.maxHeight;
            var titleTag = args.titleTag == undefined ? constants.DEF_TITIE_TAG : args.titleTag;
            var titleText = args.title == undefined || args.Title == "" ? constants.DEF_TITIE_TEXT : args.title;
            var closeButtonLabel = args.closeButtonLabel == undefined ? constants.DEF_CLOSE_BUTTON_LABEL : args.closeButtonLabel;
            if (args.actionButtonLabel != undefined && args.actionButtonType == undefined) args.actionButtonType = constants.DEF_ACTION_BUTTON_TYPE;
            var mainCssClass = args.mainCssClass == undefined ? constants.CM_DEF_DIALOG_CSS : args.mainCssClass;
            var headerCssClass = args.headerCssClass == undefined ? constants.CM_DEF_HEADER_CSS : args.headerCssClass;
            var bodyCssClass = args.bodyCssClass == undefined ? constants.CM_DEF_BODY_CSS : args.bodyCssClass;
            var footerCssClass = args.footerCssClass == undefined ? constants.CM_DEF_FOOTER_CSS : args.footerCssClass;
            var container = args.container == undefined ? $(this) : args.container;
            if (args.clickOutside == undefined) args.clickOutside = false;
            if (args.modal == undefined) args.modal = true;
            if (args.fadeIn == undefined) {
                if (args.modal == true) args.fadeIn = true;
                else args.fadeIn = false;
            }
            if (args.stopClose == undefined) args.stopClose == false;
            //No default values or no variable set (use passed arg items directly later)
            //args.actionButtonLabel, args.actionButtonType, args.icon, args.animation, 
            //args.parentResetClickOutside, args.parentsToClose           

            //Never create two same instances
            var newVdc = $("#" + uniqueId);
            if (newVdc.length > 0) {
                return false;
            }

            //Set main dialog element
            var dialogMain = $("<div>").css("text-align", "left");            
            var setMainCssClass = function (mainCssClass) {
                dialogMain.removeClass();
                dialogMain.addClass(mainCssClass);
            }
            setMainCssClass(mainCssClass);
            
            var setWidth = function (number) {
                dialogMain.width(number);
                width = number;
            }
            setWidth(width);

            //Set dialog header
            var dialogHeader = $("<div>");
            var setHeaderCssClass = function (headerCssClass) {
                dialogHeader.removeClass();
                dialogHeader.addClass(constants.BS_HEADER_CSS).addClass(headerCssClass);
            }
            setHeaderCssClass(headerCssClass);

            //x button will be set after closing funtion
            //Set title tag 
            //Note: Must call instaince.setHearder if changing tag in update mode)           
            var dialogTitle;
            var setTitleTag = function (tag) {                
                dialogTitle = $("<" + tag + ">");  
            }
            setTitleTag(titleTag);

            //Set title text
            var setTitleText = function (text) {
                dialogTitle.empty().append(text);
            }
            setTitleText(titleText);            
            dialogMain.append(dialogHeader);

            //Set dialog body
            var dialogBody = $("<div>");
            var setBodyCssClass = function (bodyCssClass) {
                dialogBody.removeClass();
                dialogBody.addClass(constants.BS_BODY_CSS).addClass(bodyCssClass);
            }
            setBodyCssClass(bodyCssClass);
            //Icons
            var iconElem = $("<div>");
            var setIcon = function (mode) {
                iconElem.removeClass();
                switch (mode) {
                    case "info":
                        iconElem.addClass(constants.ICON_INFO_CSS);
                        break;
                    case "warning":
                        iconElem.addClass(constants.ICON_WARNING_CSS);
                        break;
                    case "error":
                        iconElem.addClass(constants.ICON_ERROR_CSS);
                        break;
                    case "question":
                        iconElem.addClass(constants.ICON_QUESTION_CSS);
                        break;
                    default:
                }               
            }
            if (args.icon != undefined) setIcon(args.icon);
            //content
            var contentElem = $("<div>");
            var bodyContent = function (contentText) {
                contentElem.empty().append(contentText);
            }
            if (message != undefined) bodyContent(message);

            //Adjust dialog height if context height > maxHeight
            var setMaxHeight = function (maxHeightArg) {
                if (maxHeightArg == undefined) maxHeightArg = maxHeight;
                if (dialogBody.height() > maxHeightArg) {
                    dialogBody.height(maxHeightArg);
                    dialogBody.css("overflow-y", "scroll");
                    //dialogBody.width(dialogBody.width + 30);
                }
            }
            setMaxHeight(maxHeight);

            //Append all items to body
            var iconTd = $("<td>").append(iconElem);
            var contentTd = $("<td>").append(contentElem);
            var rowElem = $("<tr>").append(iconTd).append(contentTd);
            var tblElem = $("<table>").append(rowElem);
            dialogBody.append(tblElem);
            dialogMain.append(dialogBody);

            //Set dialog footer
            var dialogFooter = $("<div>");
            var setFooterCssClass = function (footerCssClass) {
                dialogFooter.removeClass();
                dialogFooter.addClass(constants.BS_FOOTER_CSS).addClass(footerCssClass);
            }
            setFooterCssClass(footerCssClass);
            dialogMain.append(dialogFooter);
            
            //Set virtual dialog container 
            var vdc = $("<table class='vdc-ctr'><tr><td align='center'>");

            //Append dialog box to virtual dialog container 
            vdc.find("td").append(dialogMain);
            
            var setVdcId = function (vdcId) {
                vdc.attr("id", vdcId);
            }
            setVdcId(uniqueId);

            //Modal and non-modal is always set for create new, not update
            if (args.modal) {
                //Append virtual dialog container to document.body
                $("body").append(vdc);
                vdc.css("width", "100%").css("height", "100%").css("position", "absolute")
                    .css("top", $(document).scrollTop()).css("left", "0px");                    
                vdc.isModal = true;
            }
            else {
                //Append virtual dialog container to selector
                container.append(vdc);
                centerVdc();
                vdc.isModal = false;
            }

            var setFadeIn = function (isFadeIn) {
                if (isFadeIn) {
                    methods.fadeIn(vdc, uniqueId);
                }   
                else {
                    methods.fadeOut(uniqueId);                        
                }
            }
            setFadeIn(args.fadeIn);

            //Animation for open
            var animationOpen = function (arg) {
                var animateArgs = arg.split(",");
                var position = $.trim(animateArgs[0]);
                var millisecond = parseInt($.trim(animateArgs[1]));
                switch (position) {
                    case "left":
                        vdc.css("margin-left", "-" + $(window).width() + "px")
                        vdc.animate({ "margin-left": $(window).scrollLeft() + "px" }, millisecond, "easeOutCirc");
                        break;
                    case "top":
                        vdc.css("margin-top", "-" + $(window).height() + "px")
                        vdc.animate({ "margin-top": $(window).scrollTop() + "px" }, millisecond, "easeOutCirc");
                        break;
                }                
                //Saving arg value from outside call
                args.animation = arg;
            }
            if (args.animation != undefined) animationOpen(args.animation);
            
            //acting function with event raised and keeking dialog unclosed by default.
            //In caller event function, returning explicit "close" for closing dialog.
            //Usually use actionButton click event from caller instead
            var acting = function () {                
                var rtn = vdc.triggerHandler("action");
                if (rtn == "close") close();
            } 

            //set vdc properties for close process. Used by calls from both inside and outside 
            vdc.stopClose = args.stopClose;
            vdc.parentsToClose = args.parentsToClose;

            //closing function with event raised and to close dialog by default.
            var closing = function () {                
                var rtn = vdc.triggerHandler("closing");
                if (!vdc.stopClose) {
                    if (rtn == undefined) close();
                    if (vdc.parentsToClose != undefined) {
                        if ($.isArray(vdc.parentsToClose)) {
                            $.each(vdc.parentsToClose, function () {
                                this.close();
                            });
                        }
                        else {
                            vdc.parentsToClose.close();
                        }
                    }
                }                
            }            
            
            //close function (no event raised)
            var close = function () {                
                //Resuming click-outside after closing this dialog (as child)
                //if passing the args.parentResetClickOutside
                if (args.parentResetClickOutside != undefined) {
                    args.parentResetClickOutside.clickOutside(true);                    
                }

                //Removing all buttons event handlers to avoid memory leak;
                vdc.footer.find("input").off();
                vdc.footer.find("button").off();
                vdc.off();

                //Animation for closing
                var position;
                var millisecond;
                if (args.animation != undefined) {
                    var animateArgs = args.animation.split(",");
                    position = $.trim(animateArgs[0]);
                    millisecond = parseInt($.trim(animateArgs[1]));
                }
                switch (position) {
                    case "left":
                        vdc.animate({ "margin-left": "-" + $(window).width() + "px" }, millisecond, "easeInCirc", removeVdc);
                        break;
                    case "top":
                        vdc.animate({ "margin-top": "-" + $(window).height() + "px" }, millisecond, "easeInCirc", removeVdc);
                        break;
                    default:
                        removeVdc();
                }
                //Remove fade
                methods.fadeOut(uniqueId);
            };
            //Remove dialog container when closing
            var removeVdc = function () {
                window.setTimeout(
                function () { vdc.remove(); }, 0);
            }

            //Key press for focused button
            vdc.on("keypress", function (event, keypressArgs) {
                event.stopPropagation();
                if (actionButton.is(':focus') && event.keyCode == 13) {
                    acting();
                }
                if (closeButton.is(':focus') && event.keyCode == 13) {
                    closing();                    
                }               
            });            
            
            //Set x button which only close itself
            var xClose = $("<button type='button' class='close'>×</button>");
            var setDialogHeader = function (button) {
                if (button == undefined) {
                    dialogHeader.empty().append(xClose).append(dialogTitle);
                    xClose.on("click", function (event) {
                        event.stopPropagation();
                        close();
                    });
                }
                else {
                    dialogHeader.empty().append(dialogTitle);
                }                
            }
            setDialogHeader();            
                  
            var closeButton;
            var actionButton;
            //For close button            
            var setCloseButton = function (label) {
                //For new dialog
                if (closeButton == undefined) {
                    closeButton = $("<button class='btn' type='button'>").css("margin-right", "5px");
                }
                //For new and existing dialog
                if (label != undefined && label != null && label != "") {
                    if (label.length < 4) label = "&#160;" + label + "&#160;";
                    closeButton.empty().append(label);
                }                    
                closeButton.on("click", function (event) {
                    event.stopPropagation();
                    closing();
                });
                var te = dialogFooter.find(closeButton).length;
                if (!dialogFooter.find(closeButton).length) dialogFooter.append(closeButton);
                closeButton.focus();
                vdc.closeButton = closeButton;                
            }
            setCloseButton(closeButtonLabel);

            //action button
            var setActionButton = function (label, type) {
                //Create new if not exist or change type
                if (actionButton == undefined || (type != undefined && type != null &&
                    type != "" && type != args.actionButtonType)) {
                    if (actionButton == undefined && type == undefined) type = "button";
                    switch (type) {
                        case "button":
                            actionButton = $("<button class='btn'>")
                            break;
                        case "submit":
                            actionButton = $("<input type='submit' class='btn btn-primary'>")
                            break;
                        default:
                    }
                    actionButton.css("margin-right", "10px");                                                              
                }
                if (label != undefined && label != null && label != "") {
                    if (label.length < 4) label = "&#160;" + label + "&#160;";
                    actionButton.empty().append(label);
                }
                if (!dialogFooter.find(actionButton).length) dialogFooter.prepend(actionButton);
                actionButton.on("click", function (event) {
                    event.stopPropagation();
                    acting();
                });
                args.actionButtonType = type;
                vdc.actionButton = actionButton;                
            }
            if (args.actionButtonLabel != undefined) setActionButton(args.actionButtonLabel, args.actionButtonType);
           
            //Any additional button
            var setOtherButton = function (label, type, existingBtn) {
                //Cannot add a new button without two predefined buttons
                if (!actionButton && !closeButton) return undefined;

                var otherButton;
                if (!existingBtn) {
                    otherButton = $("<button class='btn' type='button'>")
                    if (type == "submit") {                        
                        otherButton = $("<input type='submit' class='btn btn-primary'>")                          
                    }
                }
                otherButton.css("margin-right", "10px");
                if (label.length < 4) label = "&#160;" + label + "&#160;";
                otherButton.empty().append(label);
                dialogFooter.prepend(otherButton);                                
                return otherButton;
            }
                        
            //Click-outside to close dialog
            var setClickOutside = function (isEnabled) {
                if (vdc.isModal) {
                    if (isEnabled) {
                        $("#" + uniqueId).bind("click", function (event) {
                            //var te = $(event.target).closest($(dialogMain)).length;
                            if ($(event.target).closest($(dialogMain)).length == 0) {
                                closing();
                            }
                        });
                    }
                    else {
                        $("#" + uniqueId).off("click");
                    }
                }
                else {
                    if (isEnabled) {
                        $("body").on("click", function (event) {                            
                            //var ta = $(event.target).closest(container).length;
                            //var tb = $(event.target).closest("#" + uniqueId).length;
                            //var te = $(event.target).closest($(dialogMain)).length;                            
                            if ($(event.target).closest($(dialogMain)).length == 0 &&
                                $(event.target).closest("#" + uniqueId).length == 0) {
                                closing();
                            }
                        });
                    }
                    else {
                        $("body").off("click");
                    }
                }
                if (vdc.typeStatus != "progress") vdc.isClickOutside = isEnabled;
            }           
            setClickOutside(args.clickOutside);
            
            //Publish elements
            vdc.mainDialog = dialogMain;
            vdc.header = dialogHeader;
            vdc.body = dialogBody;
            vdc.iconElement = iconElem;
            vdc.contentElement = contentElem;            
            vdc.footer = dialogFooter;
            //Published already in functions
            //vdc.actionButton = actionButton;
            //vdc.closeButton = closeButton;
            
            //button methods with events 
            vdc.action = acting;
            vdc.close = closing;
            //methods to add or update buttons            
            vdc.setActionBtn = setActionButton;
            vdc.setCloseBtn = setCloseButton;
            vdc.setOtherBtn = setOtherButton;
            
            //Other methods
            vdc.clickOutside = setClickOutside;
            vdc.setId = setVdcId;
            vdc.maxHeight = setMaxHeight;
            vdc.width = setWidth;           
            vdc.title = setTitleText;
            vdc.titleTag = setTitleTag;
            vdc.setHeader = setDialogHeader;            
            //Example - animation: "top,200"
            vdc.animation = animationOpen;
            vdc.icon = setIcon;
            vdc.mainCssClass = setMainCssClass;
            vdc.headerCssClass = setHeaderCssClass;
            vdc.bodyCssClass = setBodyCssClass;
            vdc.footerCssClass = setFooterCssClass;
            //Html or text content (Note: Vdc.body is published as <div> element for content)
            vdc.content = bodyContent;
            
            //Hold existing click-outside setting
            vdc.isClickOutside = args.clickOutside;
            vdc.typeStatus = "base";
            vdc.fadeIn = setFadeIn;
            
            //Return dialog container
            return vdc;
        },

        /******************************
        showCommonPopup Function Specs
        -------------------------------
        //Input
        objOrMsg: string text or args object
        --If args is an object, all properties are the same as "popupDialog" method
        --plus additional properties: 
        args.popup: this dialog instance if for updating exising
        -------------------------------
        //Return: dialog instance if new
        ******************************/
        showCommonPopup: function (objOrMsg, title, icon, popup, parentsToClose) {            
            var container = $(this);
            var args = {};
            //Overloading handling
            if (typeof objOrMsg == 'string' || objOrMsg instanceof String) {
                args.message = objOrMsg;
                args.title = title;
                args.icon = icon;
                args.popup = popup;
                args.parentsToClose = parentsToClose;
            }
            else {
                args = objOrMsg;
            }

            var isNew = args.popup == undefined ? true : false;
            if (!isNew) {            
                if (args.title == undefined) args.title = constants.DEF_TITIE_TEXT;
                if (args.closeButtonLabel == undefined) {                    
                    args.closeButtonLabel = constants.DEF_CLOSE_BUTTON_LABEL;
                }
                //Clear footer when from two buttons to one button
                if (args.actionButtonLabel == undefined && args.popup.actionButton != undefined) {
                    args.popup.footer.empty();
                }
                //Reset click-outside if from progress bar 
                if (args.popup.typeStatus == "progress" && args.clickOutside == undefined) {                  
                    args.popup.clickOutside(args.popup.isClickOutside);
                }
            }            
            //Call for updating common items
            methods.createUpdatePopup(container, args);           

            //Update type status
            args.popup.typeStatus = "common";

            //test object equal
            //var te = _.isEqual(args.popup, popup);

            if (isNew)
                return args.popup;
        },

        /********************************
        showDataPopup Function Specs
        ---------------------------------
        //Input for Ajax call: 
        (The same args properties as in callForJsonView function)
        ---------------------------------
        //input Arguments for dialog 
        (The same args properties as in popupDialog function)
        --Added into this function for updatable dialog:
        args.stopProgress: Boolean. Disable built-in progress bar. Default is undefined or false.
        args.popup: this dialog instance if for updating exising
        --------------------------------
        //Return: dialog instance if new
        --------------------------------
        //Events (the same as in callForJsonView function)                    
        *********************************/
        showDataPopup: function (args) {
            var container = $(this);
            var isNew = args.popup == undefined ? true : false;
            
            //Set Ajax call for data or use successCallback for message, error, or status.
            container.jqsDialog("callForJsonView", {
                url: args.url,
                type: args.type,
                data: args.data,
                contentType: args.contentType,
                cache: args.cache,
                async: args.async,
                beforeSend: args.beforeSend != undefined ? function () {
                    args.beforeSend();
                } : undefined,
                success: args.success != undefined ? function (successArgs) {
                    popup.content(successArgs.html);
                    args.success(successArgs);
                } : undefined,
                error: args.error != undefined ? function (XMLHttpRequest, textStatus, errorThrown) {
                    args.error(XMLHttpRequest, textStatus, errorThrown);
                } : undefined,
                complete: args.complete != undefined ? function (xhr, status) {
                    args.complete(xhr, status);
                } : undefined                
            });
            
            //Tile and Buttons will always be specified explicitly for data dialog
            if (!isNew) {
                //Reset click-outside if from progress bar 
                if (args.popup.typeStatus == "progress" && args.clickOutside == undefined) {
                    args.popup.clickOutside(args.popup.isClickOutside);
                }
            }            
            
            //Set dialog box and provide both new and updating options
            methods.createUpdatePopup(container, args);
            
            //Remove event handler and append partial view code
            if (args.success == undefined) {
                container.on("jsonViewSuccess", function (event, successArgs) {
                    container.off("jsonViewSuccess");
                    args.popup.content(successArgs.html);
                    if (args.popup.actionButton) {
                        args.popup.actionButton.prop("disabled", false);
                    }                    
                });
            }

            //By default, show prograss bar if data process is slow            
            if (!args.stopProgress) {
                //Pass keepParts = true to keep title and buttons
                methods.showProgressBar(args.popup, true);
                if (args.popup.actionButton) {
                    args.popup.actionButton.prop("disabled", true);
                }
            }

            //Update type status
            args.popup.typeStatus = "data";

            if (isNew)
                return args.popup;
        },

        /**********************************
        callForJsonView Function Specs
        ----------------------------------
        //Input
        args.url: string.
        args.type: string. Ajax() defaults to "GET";            
        args.contentType: string. Data sent to server. Ajax() defaults to 'application/x-www-form-urlencoded; charset=UTF-8'.
        args.data: PlainObject or String. Data sent to server.            
        (Note: dataType "json" is hard-coded in this function for data returned from server ) 
        args.cache: Boolean. Ajax() defaults to true.
        args.async: Boolean. Ajax() defaults to true.
        args.beforeSend: callback function. 
        args.success: delegate to method, called when response is received successfully. Setting this parameter will bypass raising jsonViewSuccess event.
        args.error: delegate to method, called when response returns error. Setting this parameter will bypass raising jsonViewError event.
        args.complete: delegate to method, called when response completes. Setting this parameter will bypass raising jsonViewComplete event.
        -----------------------------------
        //Return: JQuery element container
        -----------------------------------
        //Events:
        - "jsonViewSuccess" {html,data}
        - "jsonViewError" {ajax,status,error}
        - "jsonViewComplete" {ajax,status}
        ************************************/
        callForJsonView: function (args) {            
            if (args == undefined) args = {};
            //If contextType is json, reset default to "POST".
            var data;
            if (args.contentType && args.contentType.toLowerCase().indexOf("application/json") >= 0) {
                data = $.toJSON(args.data);
                args.type = "POST";
            }
            else {
                data = args.data;
            }
            var container = $(this);

            $.ajax({
                url: args.url + "?jsoncallback=?",
                type: args.type,
                //Always have json type for returned result.data
                dataType: "json",
                data: data,
                contentType: args.contentType,
                cache: args.cache,
                async: args.async,
                beforeSend: function () {
                    if (args.beforeSend != undefined)
                          args.beforeSend();
                },
                success: function (result) {
                    if (result.Code == -10000) return;
                    if (args.success == undefined)
                        container.trigger("jsonViewSuccess", { html: result.html, data: result.data });
                    else
                        args.success(result);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    if (args.error == undefined)
                        container.trigger("jsonViewError", { ajax: XMLHttpRequest, status: textStatus, error: errorThrown });
                    else
                        args.error(XMLHttpRequest, textStatus, errorThrown);
                },
                complete: function (xhr, status) {
                    if (args.complete == undefined)
                        container.trigger("jsonViewComplete", { ajax: xhr, status: status });
                    else
                        args.complete(xhr, status);
                }
            });
            return container;
        },

        /**********************************
        showProgressBar Function Specs
        ----------------------------------
        //Input
        popup: existing dialog instance. Undefined or null for new.
        replace: undefined for using default settings ("Please wait...", no footer button)
                 "content" for replacing content with image bar (keeping title and buttons)
        -----------------------------------
        Return: the dialog instance if new
        **********************************/
        showProgressBar: function (popup, keepParts) {
            //replace: "content" (no title and footer changes)
            var container = $(this);
            var isNew = false;
            if (!popup) {
                //Create a default dialog without message text.
                popup = container.jqsDialog("popupDialog");
                isNew = true;
            }
            var progress = $("<div style='padding:15px;text-align:center;'>");
            progress.append(constants.DEF_LOAD_PROGRESS_IMAGE);
            popup.icon("");            
            popup.content(progress);
            if (!keepParts) {
                popup.title("Please wait...");                
                popup.footer.empty();
            }

            //Set type status used for re-setting click-outside for next updated dialog
            popup.typeStatus = "progress";

            //Always disable click-outside but doesn't change isClickOutside value when calling VDC's clickOutside function.
            //Thus isClickOutside can be used to resume setting after progress bar dialog is switched to other type.
            popup.clickOutside(false);
            
            if (isNew)
                return popup;
        },

        //Functions called internally        
        createUpdatePopup: function (container, args) {
            if (args.popup == undefined) {
                args.popup = container.jqsDialog("popupDialog", {
                    message: args.message,
                    id: args.id,
                    width: args.width,
                    maxHeight: args.maxHeight,
                    title: args.title,
                    titleTag: args.titleTag,
                    icon: args.icon,
                    closeButtonLabel: args.closeButtonLabel,                    
                    actionButtonLabel: args.actionButtonLabel,
                    actionButtonType: args.actionButtonType,                    
                    mainCssClass: args.mainCssClass,
                    headerCssClass: args.headerCssClass,
                    bodyCssClass: args.bodyCssClass,
                    footerCssClass: args.footerCssClass,
                    animation: args.animation,
                    clickOutside: args.clickOutside,
                    modal: args.modal,
                    fadeIn: args.fadeIn,
                    parentResetClickOutside: args.parentResetClickOutside,
                    parentsToClose: args.parentsToClose,
                    stopClose: args.stopClose
                });
            }
            else {
                //Updating values
                args.popup.content(args.message);
                if (args.icon != undefined && args.icon != null && args.icon != "") args.popup.icon(args.icon);
                if (args.width != undefined) args.popup.width(args.width);
                if (args.Id != undefined) args.popup.setId(args.Id);
                if (args.maxHeight != undefined) args.popup.maxHeight(args.maxHeight);
                if (args.titleTag != undefined) {
                    args.popup.titleTag(args.titleTag);
                    args.popup.setHeader();
                }
                if (args.title != undefined && args.title != "") args.popup.title(args.title);
                if (args.mainCssClass != undefined) args.popup.mainCssClass(args.mainCssClass);
                if (args.headerCssClass != undefined) args.popup.headerCssClass(args.headerCssClass);
                if (args.bodyCssClass != undefined) args.popup.bodyCssClass(args.bodyCssClass);
                if (args.footerCssClass != undefined) args.popup.footerCssClass(args.footerCssClass);

                // Any stopping auto close needs to explicitly set to true
                args.popup.stopClose = args.stopClose == undefined ? false: true;
                args.popup.parentsToClose = args.parentsToClose;   
                if (args.closeButtonLabel != undefined) {
                    args.popup.setCloseBtn(args.closeButtonLabel);
                }
                else {
                    if (args.popup.parentsToClose != undefined) {
                        args.popup.closeButton.on("click", function (event) {
                            event.stopPropagation();
                            args.popup.closing();
                        });
                    }
                }
                
                if (args.actionButtonLabel != undefined) {
                    var buttonType = args.actionButtonType == undefined ? constants.DEF_ACTION_BUTTON_TYPE : args.actionButtonType;
                    args.popup.setActionBtn(args.actionButtonLabel, buttonType);
                } 
                
                if (args.animation != undefined) args.popup.animation(args.animation);
                if (args.clickOutside != undefined) args.popup.clickOutside(args.clickOutside);
                if (args.fadeIn != undefined) args.popup.fadeIn(args.fadeIn);
            }
        },

        //Fade for dialog container - need "overlay" css class
        fadeIn: function (container, id) {
            var existingInstance = $('[aria-describedby="overlay-' + id + '"]');
            if (existingInstance.length == 0) {
                var overLay = $('<div id="ov-' + id + '" aria-describedby="overlay-' + id + '" class="overlay" style=""/>');
                $(container).before(overLay);
                overLay.fadeIn('fast', function () {
                });                
            }
        },
        //Remove fade for dialog container - need "overlay" css class
        fadeOut: function (id) {
            var existingInstance = $('[aria-describedby="overlay-' + id + '"]');
            if (existingInstance.length > 0) {
                $('[aria-describedby="overlay-' + id + '"]').fadeOut('fast');
                $('[aria-describedby="overlay-' + id + '"]').remove();
            }
        },
        //Get GUI for uniqueId
        guidGenerator: function() {
            var S4 = function() {
                return (((1+Math.random())*0x10000)|0).toString(16).substring(1);
            };
            return (S4()+S4()+"-"+S4()+"-"+S4()+"-"+S4()+"-"+S4()+S4()+S4());
        }
    };

    $.fn.jqsDialog = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist in jqsDialog.js plug-in');
        }
    };
})(jQuery);

