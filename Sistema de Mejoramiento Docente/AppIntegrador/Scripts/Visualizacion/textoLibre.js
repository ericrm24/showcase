class TextoLibre {

    constructor() { }

    crearLista(caja, respuestas) {

        var list = document.createElement("ul");

        for (var i = 0; i < respuestas.length; ++i) {

            var item = document.createElement("li");


            item.appendChild(document.createTextNode(respuestas[i]));


            list.appendChild(item);
        }

        caja.appendChild(list);

    }

    recuperarTituloJustificacion(codigoPregunta) {

        var titulo = "";

        $.ajax({
            url: '/ResultadosFormulario/getTituloJustificacion',
            data: {
                codigoPregunta: codigoPregunta
            },
            type: 'post',
            dataType: 'html',
            async: false,
            success: function (resultados) {
                titulo = resultados;
            }
        });

        return titulo;

    }

    recuperarJustificaciones(codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta) {

        var justificaciones = [];

        $.ajax({
            url: '/ResultadosFormulario/getJustificacionPregunta',
            data: {
                codigoFormulario: codigoFormulario,
                siglaCurso: siglaCurso,
                numeroGrupo: numeroGrupo,
                semestre: semestre,
                ano: ano,
                fechaInicio: fechaInicio,
                fechaFin: fechaFin,
                codigoSeccion: codigoSeccion,
                codigoPregunta: codigoPregunta
            },
            type: 'post',
            dataType: 'json',
            async: false,
            success: function (resultados) {
                for (var i = 0; i < resultados.length; ++i) {
                    justificaciones.push(resultados[i].Value);
                }
            }
        });

        return justificaciones;

    }

    recuperarTextoAbierto(codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta) {

        var respuestas = [];

        $.ajax({
            url: "/ResultadosFormulario/ObtenerRespuestasTextoAbierto",
            data: {
                codigoFormulario: codigoFormulario,
                siglaCurso: siglaCurso,
                numeroGrupo: numeroGrupo,
                semestre: semestre,
                ano: ano,
                fechaInicio: fechaInicio,
                fechaFin: fechaFin,
                codigoSeccion: codigoSeccion,
                codigoPregunta: codigoPregunta
            },
            type: "post",
            dataType: "json",
            async: false,
            success: function (resultados) {
                for (var i = 0; i < resultados.length; ++i) {
                    respuestas.push(resultados[i].Value);
                }
            }
        });

        return respuestas;

    }

    generarJustificaciones(caja, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta) {

        var titulo = this.recuperarTituloJustificacion(codigoPregunta);
        var respuestas = this.recuperarJustificaciones(codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta);

        if (respuestas.length > 0) {

            caja.getElementoTituloJustificacion().innerText = titulo;
            this.crearLista(caja.getElementoJustificacion(), respuestas);

        }

        return respuestas.length;

    }

    generarTextoAbierto(caja, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta) {

        var respuestas = this.recuperarTextoAbierto(codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta);

        if (respuestas.length > 0) {

            this.crearLista(caja, respuestas);

        }

        return respuestas.length;

    }


}