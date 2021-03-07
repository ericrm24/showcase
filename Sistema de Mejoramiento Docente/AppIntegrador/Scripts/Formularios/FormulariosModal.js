function CrearModal() {
    $('#ExampleModal').modal();
    $('#BancoDeSecciones').show();
    $('#ModalCrearSeccion').hide();
}
function CerrarModalSecciones() {
    $('ExampleModal').modal('hide')
}

$('.CrearSeccionModal').click(function () {
    $('#ModalCrearSeccion').show("fast");
    $('#BancoDeSecciones').hide("fast");
});

function CerrarCrearSeccion() {
    $('#ModalCrearSeccion').hide("fast");
    $('#BancoDeSecciones').show("fast");
}

function GuardarSeccion() {
    var Codigo = document.getElementById("sectionCode").value;
    var Nombre = document.getElementById("sectionName").value;

    resultado = { Codigo, Nombre };

    $.ajax({
        contentType: "application/json; charset=utf-8",
        type: "POST",
        url: "/Formularios/AgregarSeccion",
        data: JSON.stringify(resultado),
        dataType: "json",
        traditional: true,
        success: function (data) {
            if (data.guardadoExitoso) {
                $.ajax({
                    url: "/Seccion/ActualizarBancoSecciones",
                    type: "post",
                    dataType: "html",
                    success: function (result) {
                        $("#ModalAgregarSecciones").html(result);
                        CerrarCrearSeccion();
                        DesactivarSeccionesAgregadas();
                        MarcarSeccionesSeleccionadas();
                    }
                })
                $.ajax({
                    url: "/Seccion/ActualizarCrearSeccion",
                    type: "post",
                    dataType: "html",
                    success: function (result) {
                        $("#ModalCrearSeccion").html(result);
                    }
                })
            }
            else {
                document.getElementById("validacion-codigo-seccion").textContent = "Código en uso";
                $("#sectionCode").addClass("error");
            }
        }
    });
}

/* Permite que los checkboxes sean persistentes a través del refrescamiento de la página.*/

var checkboxValues = JSON.parse(localStorage.getItem('checkboxValues')) || {},
    $checkboxes = $("#checkbox-container :checkbox");

$checkboxes.on("change", function () {
    $checkboxes.each(function () {
        checkboxValues[this.id] = this.checked;
        //console.log(this.id + "Checked")
    });

    localStorage.setItem("checkboxValues", JSON.stringify(checkboxValues));
});

// On page load
$(document).ready(function () {
    $(checkboxValues).each(function (key, value) {
        $("#" + key).prop('checked', value);
    });
});

