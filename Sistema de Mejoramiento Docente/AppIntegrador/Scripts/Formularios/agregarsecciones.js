agregarsecciones = almacenador();

// Cuando se marca o desmarca un checkbox del banco,
// se agrega o elimina el elemento del arreglo de secciones.
function selectSeccion(element) {
    if (element.checked) {
        agregarsecciones.add(element.value);
    }
    else {
        agregarsecciones.remove(element.value);
    }
}

// Función que llama al controlador para agregar las secciones selecionadas al formulario
function addSeccionToFormulario() {
    // Obtener el nombre y el codigo del formulario
    var codigo = document.getElementById("textCode").value
    var nombre = document.getElementById("textName").value
    var seccionesAsociadas = agregarsecciones.getArray();
    // Construir el objeto que recibe la vista
    var resultado = { codigo, nombre, seccionesAsociadas };
    // Llamado al método del controlador
    $.ajax({
        contentType: "application/json; charset=utf-8",
        type: "POST",
        url: "/Formularios/AsociarSeccionesAFormulario",
        data: JSON.stringify(resultado),
        dataType: "html",
        traditional: true,
        success: function (data) {
            resultado = [];
            // La vista es actualizada con los datos recibidos del controlador  
            $('#seccionesActuales').html(data);
            agregarsecciones.removeAll();
            DesactivarSeccionesAgregadas();
        }
    });
}

function DesactivarSeccionesAgregadas() {
    var secciones = document.getElementsByClassName("SeccionesAgregadasAFormulario");
    Array.prototype.forEach.call(secciones, function (seccion) {
        var seccionCheckbox = document.getElementById("ch(" + seccion.value + ")");
        seccionCheckbox.checked = true;
        seccionCheckbox.disabled = true;
    });
}

function HabilitarSeccionEnBanco(codigoSeccion) {
    var seccionCheckbox = document.getElementById("ch(" + codigoSeccion + ")");
    seccionCheckbox.checked = false;
    seccionCheckbox.disabled = false;
}

function MarcarSeccionesSeleccionadas() {
    var arr = agregarsecciones.getArray();
    var seccionesEliminadasDelBanco = []
    Array.prototype.forEach.call(arr, function (codigo) {
        var seccionCheckbox = document.getElementById("ch(" + codigo + ")");
        if (seccionCheckbox) {
            seccionCheckbox.checked = true;
        }
        else {
            seccionesEliminadasDelBanco.push(codigo);
        }
    });

    Array.prototype.forEach.call(seccionesEliminadasDelBanco, function (codigo) {
        agregarsecciones.remove(codigo);
    });
}