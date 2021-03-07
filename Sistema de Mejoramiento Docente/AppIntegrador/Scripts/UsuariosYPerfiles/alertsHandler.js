function customAlert(type, message) {
    if (type === "confirm") {
        //sweetalerts
    } else {
        $.notify({
            // options
            message: message
        }, {
            // settings
                type: type,
                placement: {
                    from: 'bottom',
                    align: 'center'
                },
                animate: {
                    enter: "animated fadeInUp",
                    exit: "animated fadeOutDown"
                }


            //    },
            //position: absolute
        });
    }
}