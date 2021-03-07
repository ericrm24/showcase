// Deshabilita los campos de código, enunciado y el botón de crear si se marca la "opción" de "Seleccionar opción"
function SeleccionarTipoCrearPregunta(tipo) {
    if (tipo.value === '-') {
        deshabilitarCamposBasicos(true)
        $('#Create').prop('disabled', true);
    }
    else {
        deshabilitarCamposBasicos(false)
    }
    console.log (tipo.value)

    cargarVista(tipo.value)
}


// Habilita/deshabilita los campos de código, enunciado y el botón de crear
function deshabilitarCamposBasicos(booleano) {
    $('#Codigo').prop('disabled', booleano);
    $('#Enunciado').prop('disabled', booleano);
}

// Método que carga cada vista según la pregunta que se seleccionó en el combobox
//
// Para cargar la vista, es recomendable que el cshtml esté en la carpeta Views/Preguntas
// Además, se necesita como mínimo un método que retorne View() en el controlador para cada 
// vista que se necesite (como se hace con PreguntaConOpciones y OpcionesDeSeleccion)
// Si se pasan parámetros al constructor, ver ejemplo con las opciones de las preguntas con opciones
//
// Para todos los métodos que se ejecutan con $(document).ready(), hay que hacer un init, como se hace con OU
function cargarVista(valor) {
    // Vaciar el contenedor por si tenía otra vista ya cargada
    $('#contenedor-preguntas').empty();
    // Comparar el tipo de pregunta que se seleccionó 
    // y cargar la vista por medio de Ajax
    if (valor === 'U' || valor === 'M') {
        $.ajax({
            type: "post",
            // url completa: Views/Pregunta/PreguntasConOpciones
            url: '/Preguntas/PreguntaConOpciones',
            success: function (data) {
                $(data).appendTo('#contenedor-preguntas')
                preguntaConOpcionesInit()
            }
        })
    }
    // Comparar el tipo de pregunta que se seleccionó 
    // y cargar la vista por medio de Ajax
    else if (valor === 'S') {
        $.ajax({
            type: "post",
            // url completa: Views/Pregunta/PreguntaSiNo
            url: '/Preguntas/PreguntaSiNo',
            success: function (data) {
                $(data).appendTo('#contenedor-preguntas')
                SiNoInit()
            }
        })
    }
    // Comparar el tipo de pregunta que se seleccionó 
    // y cargar la vista por medio de Ajax
    else if (valor === 'L') {
        $.ajax({
            type: "post",
            // url completa: Views/Pregunta/RespuestaLibre
            url: '/Preguntas/RespuestaLibre',
            success: function (data) {
                $(data).appendTo('#contenedor-preguntas')
            }
        })
    }
    // Comparar el tipo de pregunta que se seleccionó 
    // y cargar la vista por medio de Ajax
    else if (valor === 'E') {
        $.ajax({
            type: "post",
            // url completa: Views/Pregunta/RespuestaLibre
            url: '/Preguntas/PreguntaEscalar',
            success: function (data) {
                $(data).appendTo('#contenedor-preguntas')
                EscalarInit()
            }
        })
    }
}


function EscalarInit() {
    // Esconder campo de justificación por defecto
    $('#justificationField').css('display', 'none');
}

function SiNoInit() {
    // Esconder campo de justificación por defecto
    $('#justificationField').css('display', 'none');
}

function preguntaConOpcionesInit() {
    // Esconder campo de justificación por defecto
    $('#justificationField').css('display', 'none');
    // Método de sortable
    //$("#sortable").disableSelection();
    // Agregar una opción por defecto
    $("#agregar-opcion").trigger('click');
    // Desactivar el botón de crear por defecto
    $('#Create').prop('disabled', true);
    // Aplicarle el sortable al div con id #sortable
    $("#sortable").sortable({
        axis: "y",
        // Fix indexes of each input box when an input box is moved
        update: function () {
            fixIndexes();
        },
    });
}

// Método para preguntas con opciones que agrega nuevas opciones
// se debe usar el selector del documento porque estos campos se manejan dinámicamente
$(document).on("click", "#agregar-opcion", function () {
    var i = fixIndexes();
    $.ajax({
        type: "post",
        url: '/Preguntas/OpcionesDeSeleccion',
        // Este es el parámetro que se le pasa, la i de la izquierda es el nombre
        // del parámetro que recibe el constructor. La i de la derecha es el valor
        // que se manda desde la vista como parámetro (la i viene de fixIndexes())
        data: { i: i, tipo: $("#Tipo").prop("value")},
        success: function (data) {
            $(data).appendTo('#sortable').height(0).animate({ 'height': 75 }, 200);
            $("#sortable").find(".texto").focus();
            disableRemoveButton();
            validarEntradas();
        }
    })
})

function EnableDisableTextBox(justificationField) {
    var txtPassportNumber = document.getElementById("txtJustificationNumber");
    txtJustificationNumber.disabled = justificationField.checked ? false : true;
    if (!txtJustificationNumber.disabled) {
        txtJustificationNumber.focus();
    }
    validarEntradas();
}

// Combobox de Bootstrap, no funciona bien pero se deja el trabajo por
// si se llega a corregir el error de binding que tiene

//$(".dropdown-menu li").on("click", function () {
//    cambiarTipo(this);
//})

//function cambiarTipo(node) {
//    var valorSeleccionado = $(node).attr("value")
//    // Buscar el boton "combobox"
//    var botonCombobox = $(node).parent().parent()
//    // Asignar valor al dropdown
//    $(botonCombobox).find("#Tipo").attr("value", valorSeleccionado)
//    // Cambiar nombre
//    $(botonCombobox).find(".texto-btn").text($(node).text())
//}
