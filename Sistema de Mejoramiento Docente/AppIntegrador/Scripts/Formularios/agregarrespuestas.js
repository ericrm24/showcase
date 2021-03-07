agregarrespuestas = function () {

    // Contador de la pregunta actual
    var current = null;

    // Inicia con la pregunta 0
    function init() {
        current = 0;
    }

    // Una pregunta más ha sido respondida
    function add() {
        current++;
    }

    return {
        init: init,
        add: add,
        current: function () { return current; }
    }
}();

function seleccionarRespuesta(element) {
    if (element.checked) {
        element.id = "respuestas[" + agregarrespuestas.current() + "].Codigo";
        agregarrespuestas.add();
    }
    else {
        deseleccionarRespuesta(element.value);
        agregarrespuestas.remove();
    }
}

function deseleccionarRespuesta(codigo) {
    var index = 0;
    var beforeElement = true;
    while (index < agregarrespuestas.current() && beforeElement) {
        var respuesta = document.getElementById("respuestas[" + index + "].Codigo");
        if (respuesta.value == codigo) {
            respuesta.setAttribute("name", "");
            respuesta.id = null;
            beforeElement = false;
        }
        ++index;
    }

    while (index < agregarrespuestas.current()) {
        var newIndex = index - 1;
        var respuesta = document.getElementById("respuestas[" + index + "].Codigo");
        respuesta.id = "respuestas[" + newIndex + "].Codigo";
        ++index;
    }
}

function addSeccionToFormulario() {
    var index = 0;

    while (index < agregarrespuestas.current()) {
        var respuesta = document.getElementById("respuestas[" + index + "].Codigo");
        respuesta.setAttribute("name", respuesta.id);
        var tupla = document.getElementById("sec(" + respuesta.value + ")");
        tupla.setAttribute("hidden", "hidden");
        ++index;
    }
}