class Estadisticas {

    constructor() { }

    recuperarMedia(codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta) {

        var media = null;
        $.ajax({
            url: "/ResultadosFormulario/getPromedio",
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

                if (resultados !== null) {

                    media = resultados.toFixed(2);

                }
            }
        });

        return media;

    }

    recuperarMediana(codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta) {

        var mediana = null;
        $.ajax({
            url: "/ResultadosFormulario/getMedianaRespuestaEscalar",
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

                if (resultados !== null) {

                    mediana = resultados.toFixed(2);

                }
            }
        });

        return mediana;

    }

    recuperarDesviacion(codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta) {

        var desviacion = null;
        $.ajax({
            url: "/ResultadosFormulario/obtenerDesviacionEstandar",
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

                if (resultados !== null) {

                    desviacion = resultados.toFixed(2);

                }
            }
        });

        return desviacion;

    }

    generarEstadisticas(row, codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta) {

        var media = this.recuperarMedia(codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta);
        var mediana = this.recuperarMediana(codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta);
        var desviacion = this.recuperarDesviacion(codigoFormulario, siglaCurso, numeroGrupo, semestre, ano, fechaInicio, fechaFin, codigoSeccion, codigoPregunta);

        var resultado = 0;

        if (typeof media !== null && typeof mediana !== null && typeof desviacion !== null) {

            resultado = 1;

            var colMedia = row.firstElementChild;
            var colMediana = colMedia.nextElementSibling;
            var colDesviacion = colMediana.nextElementSibling;

            var mediaTitulo = document.createElement("h5");
            mediaTitulo.className = "tituloEstadisticas";
            mediaTitulo.innerText = "Promedio";
            var medianaTitulo = document.createElement("h5");
            medianaTitulo.className = "tituloEstadisticas";
            medianaTitulo.innerText = "Mediana";
            var desviacionTitulo = document.createElement("h5");
            desviacionTitulo.className = "tituloEstadisticas";
            desviacionTitulo.innerText = "Desviación Estándar";

            var mediaDiv = document.createElement("div");
            mediaDiv.className = "estadisticas";
            mediaDiv.innerText = ("" + media).replace(".", ",");
            var medianaDiv = document.createElement("div");
            medianaDiv.className = "estadisticas";
            medianaDiv.innerText = ("" + mediana).replace(".", ",");
            var desviacionDiv = document.createElement("div");
            desviacionDiv.className = "estadisticas";
            desviacionDiv.innerText = ("" + desviacion).replace(".", ",");


            colMedia.appendChild(mediaTitulo);
            colMedia.appendChild(mediaDiv);
            colMediana.appendChild(medianaTitulo);
            colMediana.appendChild(medianaDiv);
            colDesviacion.appendChild(desviacionTitulo);
            colDesviacion.appendChild(desviacionDiv);

        }

        return resultado;

    }

}