
    //Despliega el dialogo modal de la creacion exitosa
function formulario() {
    swal({
        title: "Formulario creada con éxito!",
        text: "",
        type: "success",
        showConfirmButton: true
    },
        function () {
            window.location.href = "Create";
        })
}

    //  Se encarga de verificar si se realizo con exito la operación de creación, para mostrar el mensaje exitoso*@
    $(document).ready(function () {
        validarEntradas();
        var msg = '@ViewBag.Message';
        if (msg && msg.length > 0) {
            formulario();
        }
        $("#Add").trigger('click');
    });

    // Función encargada de desaparecer el mensaje de confirmación automáticamente luego de un tiempo
    $(function () {
        $('#myModal').on('show.bs.modal', function () {
            var myModal = $(this);
            clearTimeout(myModal.data('hideInterval'));
            myModal.data('hideInterval', setTimeout(function () {
                myModal.modal('hide');
            }, 3000));
        });
    });