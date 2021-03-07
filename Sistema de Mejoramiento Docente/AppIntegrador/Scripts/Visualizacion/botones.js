class Botones {

    constructor() {
    }

    /* COD-64: Yo como administrador quiero poder visualizar la información detallada
    de los resultados de una pregunta*/
    crearBoton(listaPreguntas, codigoPregunta, textoPregunta, idPreg) {

        var boton = document.createElement("button");
        boton.className = "btn-azulUCR";
        boton.id = codigoPregunta;
        boton.innerText = "Ver más";

        boton.addEventListener("click", (e) => {
            this.rellenarModal(listaPreguntas, idPreg);
        });

        return boton;
    }

    // Método encargado de rellenar el modal de la vista con los gráficos, estadísticas
    // y respuestas de una pregunta específica.
    rellenarModal(listaPreguntas, idPreg) {
        
        var insertaContenidos = new InsertaContenidos();

        var codigoSeccion = listaPreguntas[idPreg].codigoSeccion;
        var codigoPregunta = listaPreguntas[idPreg].codigoPregunta;
        var textoPregunta = String(idPreg + 1) + ". " + listaPreguntas[idPreg].textoPregunta;
        var tipoPregunta = listaPreguntas[idPreg].tipoPregunta;

        var base;

        switch (tipoPregunta) {

            case "texto_abierto":
                base = new BaseTexto(tipoPregunta);
                insertaContenidos.insertarTextoAbierto(base, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta);
                break;

            case "escala":
                base = new BaseConEstadisticas(tipoPregunta);
                insertaContenidos.insertarGraficoEscala(base, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta);
                insertaContenidos.insertarEstadisticas(base, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta);
                break;

            case "seleccion_unica":
                base = new BaseDosCol(tipoPregunta);
                insertaContenidos.insertarGraficoSeleccionUnica(base, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta);
                break;

            case "seleccion_multiple":
                base = new BaseDosCol(tipoPregunta);
                insertaContenidos.insertarGraficoSeleccionMultiple(base, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta);
                break;

            case "seleccion_cerrada":
                base = new BaseDosCol(tipoPregunta);
                insertaContenidos.insertarGraficoSeleccionCerrada(base, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta);
                break;

        }

        if (tipoPregunta != "texto_abierto") {

            insertaContenidos.insertarJustificaciones(base, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta);
        }

        // Borra del modal el viejo componente e introduce el nuevo
        var modalEncabezado = document.getElementById("Encabezado");
        var modalTemp = document.getElementById("Ejemplo");

        // Agrega el título de la pregunta como encabezado al modal
        modalEncabezado.innerText = textoPregunta;

        // Esto se usaría para dejar el modal en blanco
        modalTemp.firstChild.remove();

        // Esto se usaría para hacerle append de la respuesta de la pregunta
        modalTemp.appendChild(base.getBase());
        
        //document.getElementById("Ejemplo").innerHTML = idPreg;
        $("#ModalResultados").modal();
    }

    rellenarCanvas(canvas, codigoSeccion, codigoPregunta, tipoPregunta) {

        var graficos = new Graficos();

        switch (tipoPregunta) {

            case "texto_abierto":
                var img = document.getElementById("icono_ta");
                var ctx = canvas.getContext("2d");
                var scale = Math.min(canvas.width / img.width, canvas.height / img.height);
                var x = (canvas.width / 2) - (img.width / 2) * scale;
                var y = (canvas.height / 2) - (img.height / 2) * scale;
                ctx.drawImage(img, x, y, img.width * scale, img.height * scale);
                break;

            case "escala":
                graficos.generarGraficoEscala(canvas, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta, false);
                break;

            case "seleccion_unica":
                graficos.generarGraficoSeleccionUnica(canvas, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta, false);
                break;

            case "seleccion_multiple":
                graficos.generarGraficoSeleccionMultiple(canvas, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta, false);
                break;

            case "seleccion_cerrada":
                graficos.generarGraficoSeleccionCerrada(canvas, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta, false);
                break;

        }

    }

    /* COD-66: Yo como administrador quiero visualizar las preguntas de una sección
     * en forma de cuadrícula para facilitar la visualización de las mismas
     * Tarea: - Incluir en cada campo de la cuadrícula un botón para mas detalles acerca de la pregunta.
    */
    // Método encargado de crear los flexboxes, labels y botones para cada
    // una de las preguntas
    crearBotones(idSeccion, listaPreguntas) {

        var cuerpoPrincipal = document.getElementById("contenedorPreguntas");

        cuerpoPrincipal.innerHTML = "";

        for (var i = 0; i < listaPreguntas.length; ++i) {

            if (listaPreguntas[i].codigoSeccion == idSeccion) {

                var contenedorParcial = document.createElement("div");
                contenedorParcial.style.textAlign = "center";

                var flexBonito = document.createElement("p-2");
                flexBonito.className += "col-md-4";
                flexBonito.className += " border";
                flexBonito.className += " flexPregunta";
                flexBonito.id = "contenedor" + listaPreguntas[i].codigoPregunta;

                var codigoPregunta = listaPreguntas[i].codigoPregunta;
                var textoPregunta = String(i + 1) + ". " + listaPreguntas[i].textoPregunta;

                var filaLabel = document.createElement("div");
                filaLabel.className = "row justify-content-center";

                var filaCanvas = document.createElement("div");
                filaCanvas.className = "row justify-content-center";

                var filaBoton = document.createElement("div");
                filaBoton.className = "row justify-content-center";

                // Incluir un label!!
                var labelcito = document.createElement("p");
                labelcito.className = "labelcito";
                labelcito.innerText = textoPregunta;

                //Canvas
                var canvas = document.createElement("canvas");
                canvas.setAttribute("width", "250vw"); //Ancho canvas
                canvas.setAttribute("height", "180vh"); //Largo canvas

                this.rellenarCanvas(canvas, listaPreguntas[i].codigoSeccion, listaPreguntas[i].codigoPregunta, listaPreguntas[i].tipoPregunta);

                //Botón
                var boton = this.crearBoton(listaPreguntas, codigoPregunta, textoPregunta, i);



                // Appends
                filaLabel.appendChild(labelcito);
                filaCanvas.appendChild(canvas);
                filaBoton.appendChild(boton);
                contenedorParcial.appendChild(filaLabel);
                contenedorParcial.appendChild(filaCanvas);
                contenedorParcial.appendChild(filaBoton);
                flexBonito.appendChild(contenedorParcial);
                cuerpoPrincipal.appendChild(flexBonito);
            }
        }

    }

}

function seccion_cambiada(seccion, listaPreguntas) {
    var botones = new Botones();
    botones.crearBotones(seccion, listaPreguntas);
}

