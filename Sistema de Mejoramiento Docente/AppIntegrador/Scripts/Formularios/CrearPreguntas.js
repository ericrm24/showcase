function disableRemoveButton() {
    var nodes = document.getElementsByClassName("remove-button");
    console.log(nodes.length);
    var disable = nodes.length <= 1;

    Array.prototype.forEach.call(nodes, function (node) {
        node.disabled = disable;
    });
}

// Set index starting from 0 for each option
function fixIndexes() {
    var i = 0;
    // Fix order value and name
    $(".orden").each(function () {
        $(this).val(i);
        this.name = "[" + i + "].Orden";
        ++i;
    });

    // Fix text name
    i = 0;
    $(".texto").each(function () {
        this.name = "[" + i + "].Texto";
        ++i;
    });

    return i;
}

// Delete option input box(es)
function removeOption(deleteNode) {
    // Get the parent div that contains all the option modules (text, inputbox, and button)
    var target = deleteNode.parentNode;

    $(target).animate({ 'height': 0, 'opacity': 0 }, 200, function () {
        // Remove div from DOM
        $(this).remove();
        // Fix indexes when an input box is deleted
        fixIndexes();
        // Check if the first button needs to be disabled
        disableRemoveButton();
        validarEntradas();
    })

}


function myFunction() {
    if ($('#showField').prop('checked')) {
        $('#justificationField').css('display', 'block');
    } else {
        $('#justificationField').css('display', 'none');
    }
}

function pregunta() {
    swal({
        title: "Pregunta creada con éxito!",
        text: "Puede ser vista en el banco de preguntas",
        type: "success",
        showConfirmButton: true
    },
        function () {
            window.location.href = "Create";
        });
}