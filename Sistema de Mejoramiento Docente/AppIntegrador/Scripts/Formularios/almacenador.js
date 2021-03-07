function almacenador() {
    var seccionesSeleccionadas = null;
    // Inicializador que crea el arreglo
    function init() {
        seccionesSeleccionadas = [];
    }
    // Agrega el elemento especificado por id
    function add(id) {
        seccionesSeleccionadas.push(id);
    }
    // Remueve la primera aparición de id en el arreglo
    // (En buena teoría solo debería existir un elemento)
    function remove(id) {
        seccionesSeleccionadas.splice(seccionesSeleccionadas.indexOf(id), 1);
    }
    // Borra todo el arreglo
    function removeAll() {
        seccionesSeleccionadas = [];
    }
    // Verifica si el elemento está contenido en el arreglo
    function containsElement(id) {
        return seccionesSeleccionadas.find(function (element) {
            return element == id;
        });
    }

    return {
        init: init,
        add: add,
        remove: remove,
        removeAll: removeAll,
        containsElement: containsElement,
        // Función para obtener el arreglo. Se usa para poder pasárselo al controlador
        getArray: function () { return seccionesSeleccionadas; }
    }
};
