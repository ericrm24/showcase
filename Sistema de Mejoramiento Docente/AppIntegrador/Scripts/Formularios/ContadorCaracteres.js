// Cuenta los caracteres que hay en un cuadro de entrada y los despliega en un div con texto
// Para que funcione el script, se necesita el atributo maxlength y que tenga cierta estructura (ver ejemplo).
// El div que dice cuántos caracteres tiene debe tener la clase .contador definida
//
// Para llamar el método, se usa el atributo onkeyup: onkeyup="contarCaracteres(this)"
//
// Ejemplo de la estructura, debe haber un div que envuelva el cuadro de entrada y el texto de información:
//
// <div>
//     <input type="text" onkeyup="contarCaracteres(this)" maxlength="50"></input>
//     <div class="contador">0/50 caracteres usados</div>
// </div>

function contarCaracteres(entrada) {
    // Sacar la cantidad de caracteres de el cuadro de entrada
    var caracteres = entrada.value.length;
    // Sacar el valor que está definido en el atributo maxlength
    var max = $(entrada).attr("maxlength")
    // Ir al div padre y buscar el div con clase .contador
    var textoContador = $(entrada).parent().find(".contador");
    // Al div encontrado asígnele el texto con la cantidad de caracteres y el máximo
    textoContador.text(caracteres + "/" + max + " caracteres usados");
}