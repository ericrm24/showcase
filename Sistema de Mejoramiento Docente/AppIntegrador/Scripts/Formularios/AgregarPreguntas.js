agregarPreguntas = almacenador();

function selectPregunta(element) {
    if (element.checked) {
        agregarPreguntas.add(element.value);
    }
    else {
        agregarPreguntas.remove(element.value);
    }
}

function addPreguntaToSeccion() {
    var codPreguntas = agregarPreguntas.getArray();
    var codSeccion = SeccionModalActual;
    var result = {codSeccion, codPreguntas};

    $.ajax({
        contentType: "application/json; charset=utf-8",
        type: "POST",
        url: "/Seccion/AgregarPreguntas",
        data: JSON.stringify(result),
        dataType: "json",
        traditional: true,
        success: function (data) {
            console.log("todo bien")
            if (data.insertadoExitoso) {
                console.log("todo bien x2")
                $('#ModalAgregarPregunta').modal('hide');$('body').removeClass('modal-open');$('.modal-backdrop').remove();
                ActualizarSecciones();
                agregarPreguntas.removeAll();
            }
        }
    })
}

function DesactivarPreguntasAgregadas() {
    var preguntas = document.getElementsByClassName("PreguntasAgregadas(" + SeccionModalActual + ")");
    Array.prototype.forEach.call(preguntas, function (pregunta) {
        var preguntasCheckbox = document.getElementById("pregch(" + pregunta.value + ")");
        preguntasCheckbox.checked = true;
        preguntasCheckbox.disabled = true;
    });
}

function HabilitarPreguntasEnBanco(codigoPregunta) {
    var preguntaCheckbox = document.getElementById("pregch(" + codigoPregunta + ")");
    preguntaCheckbox.checked = false;
    preguntaCheckbox.disabled = false;
}

function MarcarPreguntasSeleccionadas() {
    var arr = agregarPreguntas.getArray();
    var preguntasEliminadasDelBanco = []
    Array.prototype.forEach.call(arr, function (codigo) {
        var seccionCheckbox = document.getElementById("pregch(" + codigo + ")");
        if (seccionCheckbox) {
            seccionCheckbox.checked = true;
        }
        else {
            preguntasEliminadasDelBanco.push(codigo);
        }
    });

    Array.prototype.forEach.call(preguntasEliminadasDelBanco, function (codigo) {
        agregarPreguntas.remove(codigo);
    });
}


