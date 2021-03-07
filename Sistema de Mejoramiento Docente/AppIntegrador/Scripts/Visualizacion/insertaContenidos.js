class InsertaContenidos {

    constructor() {

        this.graficos = new Graficos();
        this.textoLibre = new TextoLibre();
        this.estadisticas = new Estadisticas();

    }

    limpiarBase(base) {

        while (base.base.firstChild) {

            base.base.removeChild(base.base.firstChild);

        }

    }

    insertarMensajeNoEncontrado(base) {

        var columna = document.createElement("div");
        columna.className = "col";
        var mensaje = document.createElement("h3");
        mensaje.className = "mensajeError";
        mensaje.innerText = "No se encontraron respuestas para esta pregunta.";

        columna.appendChild(mensaje);
        base.base.appendChild(columna);

    }

    insertarGraficoEscala(baseDosCol, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta) {

        var resultado = this.graficos.generarGraficoEscala(baseDosCol.getCanvas(), codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta, true);

        if (resultado <= 0) {

            this.limpiarBase(baseDosCol);
            this.insertarMensajeNoEncontrado(baseDosCol);

        }

    }

    insertarGraficoSeleccionUnica(baseDosCol, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta) {

        var resultado = this.graficos.generarGraficoSeleccionUnica(baseDosCol, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta, true);

        if (resultado <= 0) {

            this.limpiarBase(baseDosCol);
            this.insertarMensajeNoEncontrado(baseDosCol);

        }

    }

    insertarGraficoSeleccionMultiple(baseDosCol, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta) {

        var resultado = this.graficos.generarGraficoSeleccionMultiple(baseDosCol, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta, true);

        if (resultado <= 0) {

            this.limpiarBase(baseDosCol);
            this.insertarMensajeNoEncontrado(baseDosCol);

        }

    }

    insertarGraficoSeleccionCerrada(baseDosCol, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta) {

        var resultado = this.graficos.generarGraficoSeleccionCerrada(baseDosCol, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta, true);

        if (resultado <= 0) {

            this.limpiarBase(baseDosCol);
            this.insertarMensajeNoEncontrado(baseDosCol);

        }

    }

    insertarEstadisticas(baseEstadisticas, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta) {

        var resultado = this.estadisticas.generarEstadisticas(baseEstadisticas.getElementoEstadisticas(), codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta);

        if (resultado <= 0) {

            this.limpiarBase(baseEstadisticas);
            this.insertarMensajeNoEncontrado(baseEstadisticas);

        }

    }

    insertarJustificaciones(baseDosCol, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta) {

        var resultado = this.textoLibre.generarJustificaciones(baseDosCol, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta);

        if (resultado <= 0) {

            this.limpiarBase(baseDosCol);
            this.insertarMensajeNoEncontrado(baseDosCol);

        }

    }

    insertarTextoAbierto(baseTexto, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta) {

        var resultado = this.textoLibre.generarTextoAbierto(baseTexto.getTextoAbierto(), codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta);

        if (resultado <= 0) {

            this.limpiarBase(baseTexto);
            this.insertarMensajeNoEncontrado(baseTexto);

        }

    }

}