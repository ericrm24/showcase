$(function () {
    var includes = $('[data-include]');
    jQuery.each(includes, function () {
        var file = 'views/' + $(this).data('include') + '.html';
        $(this).load(file);
    });
});


var currentEnabled = null;
function enableElement(elem) {
    if (currentEnabled) {
        currentEnabled.disabled = true;
    }
    elem.disabled = false;
    currentEnabled = elem;
}


/* Historia RIP - CF5.Llama al método del controlador que genera la vista parcial filtrada y
vuelve a llamar al diálogo modal para mostrar los resultados.*/
function ActualizarVistaFiltros() {
    var input0 = document.getElementById("section-other-option").value;
    var input1 = document.getElementById("section-codigo-option").value;
    var input2 = document.getElementById("section-enunciado-option").value;

    var resultado = { input0, input1, input2 };
    $.ajax({
        contentType: "application/json; charset=utf-8",
        type: "POST",
        url: "/Seccion/ActualizarBancoSecciones",
        data: JSON.stringify(resultado),
        dataType: "html",
        traditional: true,
        success: function (result) {
            $("#ModalAgregarSecciones").html(result);
        }
    });
}

function checkSeccion(elem) {
    // use one of possible conditions
    // if (elem.value == 'Other')
    if (elem.selectedIndex == 0) {
        document.getElementById("section-other-option").style.display = 'inline';

        document.getElementById("section-codigo-option").style.display = 'none';
        document.getElementById("section-codigo-option").value = "";

        document.getElementById("section-enunciado-option").style.display = 'none';
        document.getElementById("section-enunciado-option").value = "";
    }
    else if (elem.selectedIndex == 1) {
        document.getElementById("section-other-option").style.display = 'none';
        document.getElementById("section-other-option").value = "";

        document.getElementById("section-codigo-option").style.display = 'inline';

        document.getElementById("section-enunciado-option").style.display = 'none';
        document.getElementById("section-enunciado-option").value = "";
    }
    else if (elem.selectedIndex == 2) {
        document.getElementById("section-other-option").style.display = 'none';
        document.getElementById("section-other-option").value = "";

        document.getElementById("section-codigo-option").style.display = 'none';
        document.getElementById("section-codigo-option").value = "";

        document.getElementById("section-enunciado-option").style.display = 'inline';
    }
}

function BorrarPregunta(Scod, Pcod) {
    GuardandoCambios();
    var SCodigo = Scod
    var PCodigo = Pcod
    var resultado = { SCodigo, PCodigo };
    $.ajax({
        contentType: "application/json; charset=utf-8",
        type: "POST",
        url: "/Formularios/EliminarPregunta",
        data: JSON.stringify(resultado),
        dataType: "json",
        traditional: true,
        success: function (data) {
            if (data.eliminadoExitoso) {
                ActualizarSecciones();
                CambiosGuardadosExitosamente();
            }
            else {
                CambiosSinGuardar();
            }
        },
        error: function () {

        }
    });
}

function BorrarSeccion(Scod) {
    GuardandoCambios();
    // Arregla la vista cuando se deselecciona
    var seccionBanco = document.getElementById("sec(" + Scod + ")");
    $(seccionBanco).removeAttr("hidden");
    var checkboxBanco = $(seccionBanco).children().children();
    checkboxBanco = $(checkboxBanco).children();
    $(checkboxBanco).trigger("click");


    var FCodigo = document.getElementById("textCode").value
    var SCodigo = Scod
    var resultado = { FCodigo, SCodigo };

    $.ajax({
        contentType: "application/json; charset=utf-8",
        type: "POST",
        url: "/Formularios/EliminarSeccion",
        data: JSON.stringify(resultado),
        dataType: "json",
        traditional: true,
        success: function (data) {
            if (data.eliminadoExitoso) {
                ActualizarSecciones();
                HabilitarSeccionEnBanco(SCodigo);
                CambiosGuardadosExitosamente();
            }
            else {
                CambiosSinGuardar();
            }
        },
        error: function () {

        }
    });
}

function ActualizarSecciones() {
    var id = document.getElementById("textCode").value;
    var result = { id };
    $.ajax({
        contentType: "application/json; charset=utf-8",
        type: "POST",
        url: "/Formularios/DesplegarFormulario",
        data: JSON.stringify(result),
        dataType: "html",
        traditional: true,
        success: function (data) {
            resultado = [];
            $('#seccionesActuales').html(data);
            DesactivarSeccionesAgregadas();
        },
        error: function () {

        }
    });
}

var guardandoCambios = 0;

function ValidarCodigo() {
    var Codigo = document.getElementById("textCode").value;
    var Nombre = document.getElementById("textName").value;

    if (Codigo.length > 0 && Nombre.length > 0) {
        guardandoCambios = 1;
        GuardandoCambios();
        resultado = { Codigo, Nombre };

        if (document.getElementById("formularioCreado").value == 0) {
            console.log("Validar Codigo 0 ", Codigo, " ", Nombre);
            $.ajax({
                contentType: "application/json; charset=utf-8",
                type: "POST",
                url: "/Formularios/AgregarFormulario",
                data: JSON.stringify(resultado),
                dataType: "json",
                traditional: true,
                success: function (data) {
                    if (data.guardadoExitoso) {
                        /*$(".validar-agregar-secciones").each(function () {
                            this.disabled = "disabled";
                            return true;
                        })*/
                        document.getElementById("validacion-codigo").textContent = "";
                        $("#textCode").removeClass("error");
                        document.getElementById("formularioCreado").setAttribute("value", "1");
                        document.getElementById("codigoViejo").setAttribute("value", Codigo);

                        CambiosGuardadosExitosamente();
                        document.getElementById("titulo").innerHTML = "Editar formulario";
                    }
                    else {
                        document.getElementById("validacion-codigo").textContent = "Código en uso";
                        $("#textCode").addClass("error");
                        CambiosSinGuardar();
                    }
                }
            });
        }
        else {
            ModificarFormulario();
        }
        guardandoCambios = 0;
    }
}

function AbrirSeccionesModal() {
    var Codigo = document.getElementById("textCode").value;
    var Nombre = document.getElementById("textName").value;

    if (Codigo.length > 0 && Nombre.length > 0) {
        while (guardandoCambios == 1) {
            ;
        }
        CrearModal();
    }
}

function ModificarFormulario() {
    GuardandoCambios();
    var codViejo = document.getElementById("codigoViejo").value;
    var codNuevo = document.getElementById("textCode").value;
    var nombre = document.getElementById("textName").value;

    console.log("ModificarFormulario ", codViejo, " ", codNuevo, " ", nombre);

    resultado = { codViejo, codNuevo, nombre };
    $.ajax({
        contentType: "application/json; charset=utf-8",
        type: "POST",
        url: "/Formularios/ModificarFormulario",
        data: JSON.stringify(resultado),
        dataType: "json",
        traditional: true,
        success: function (data) {
            if (data.modificacionExitosa) {
                document.getElementById("validacion-codigo").textContent = "";
                $("#textCode").removeClass("error");
                document.getElementById("codigoViejo").setAttribute("value", codNuevo);

                CambiosGuardadosExitosamente();
            }
            else {
                document.getElementById("validacion-codigo").textContent = "Código en uso";
                $("#textCode").addClass("error");
                CambiosSinGuardar();
            }
        }
    });
}

function CambiosSinGuardar() {
    document.getElementById("cambiosGuardados").innerHTML = "Cambios sin guardar";
}

function CambiosGuardadosExitosamente() {
    document.getElementById("cambiosGuardados").innerHTML = "Cambios guardados exitosamente";
}

function GuardandoCambios() {
    document.getElementById("cambiosGuardados").innerHTML = "Guardando cambios...";
}