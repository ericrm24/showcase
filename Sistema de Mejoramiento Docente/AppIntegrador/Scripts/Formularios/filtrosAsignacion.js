class FiltrosAsignacion {

    //Constructor de la clase
    constructor() { }

    //Método que permite recuperar las unidades académicas que se colocan en el filtro
    //listaCEs -> lista de carreras y énfasis
    //listaGs -> lista de grupos
    //listaPs -> lista de profesores
    recuperarUAs(listaCE, listaG, listaP) {

        var listaUA = null;

        $.ajax({
            url: '/Dashboard/ObtenerUnidadesAcademicas',
            data: {
                carrerasEnfasis: listaCE,
                grupos: listaG,
                profesores: listaP
            },
            type: 'post',
            dataType: 'json',
            async: false,
            success: function (resultados) {
                listaUA = resultados;
            }
        });

        return listaUA;

    }

    // Metodo que recupera todos los filtros seleccionados por
    // el usuario, con sus respectivas fechas, y le envia los
    // datos al controlador para que se asignen
    asignarConFiltros(listaUA, listaCE, listaP, codigo, extender) {
        // Extrae el codigo del formulario
        var codigoFormularioGet = codigo;

        // Extra los datos de los filtros
        var CESeleccionadas = document.getElementById("filtroCarreraEnfasis");
        var CEValor = CESeleccionadas.options[CESeleccionadas.selectedIndex].value;

        var UASeleccionadas = document.getElementById("filtroUA");
        var UAValor = UASeleccionadas.options[UASeleccionadas.selectedIndex].value;

        var GSeleccionada = document.getElementById("filtroCursoGrupo");
        var GValor = GSeleccionada.options[GSeleccionada.selectedIndex].value;

        var PSeleccionado = document.getElementById("filtroProfesores");
        var PValor = PSeleccionado.options[PSeleccionado.selectedIndex].value;
        // Extrae la fecha de inicio y fin
        var fechaInicio = document.getElementById("fecha-inicio").value;
        var fechaFin = document.getElementById("fecha-fin").value;

        var correos = document.getElementById("enviarCorreos").checked;
        console.log(correos);
        // Hace el llamado al metodoo del controlador con los paramaetros respectivos
        $.ajax({
            url: '/AsignacionFormularios/Asignar',
            data: {
                codigoFormulario: codigoFormularioGet,
                codigoUASeleccionada: UAValor, 
                codigoCarreraEnfasisSeleccionada: CEValor, 
                grupoSeleccionado: GValor, 
                correoProfesorSeleccionado: PValor, 
                fechaInicioSeleccionado: fechaInicio,
                fechaFinSeleccionado: fechaFin,
                extenderPeriodo: extender,
                enviarCorreos: correos
            },
            type: 'post',
            dataType: 'json',
            async: false,
            success: function (data) {
                // Si existen errores los maneja
                if (!data.error) {

                    if (data.tipoError == 1) {
                        sweetAlert("Error", "Debe de seleccionar datos", "error");
                    }
                    if (data.tipoError == 2) {
                        sweetAlert("Error", "Debe de seleccionar las fechas de inicio y finalización", "error");
                    }
                    if (data.tipoError == 3) {
                        sweetAlert("Error", "La fecha de inicio debe de ser menor que la fecha de finalización", "error");
                    }
                    if (data.tipoError == 4) {
                        sweetAlert("Error", "La asignación no se puede efectuar.", "error");
                    }
                    if (data.tipoError == 5) {
                        $(location).attr('href', '/Formularios/Index');
                    }
                    if (data.tipoError == 6) {
                        console.log(data);
                        $("#modal-solapado").draggable({
                            handle: ".modal-header"
                        }); 

                        $("#periodo-og-inicio").html(data.inicio);
                        $("#periodo-og-fin").html(data.fin);
                        $("#modal-solapado").modal('show');
                    }
                } else {
                    if (data.inicio != null) {
                        fechaInicio = data.inicio;
                    }

                    if (data.fin != null) {
                        fechaFin = data.fin;
                    }

                    // Sino, mensaje de exito. Y redirecciona al index de formularios
                    swal({
                        title: "¡El formulario " + codigo + " fue asignado con éxito!",
                        text: "Estará disponible para llenar a partir del: " + fechaInicio + " hasta " + fechaFin + ".",
                        type: "success",
                        showConfirmButton: true
                    },
                    function () {
                        $(location).attr('href', '/Formularios/Index');
                    });
                }
            },
            error: function (data)
            {
            }
        });
    }

    //Método que permite recuperar las carreras y énfasis que se colocan en el filtro
    //listaUAs -> lista de unidades académicas
    //listaGs -> lista de grupos
    //listaPs -> lista de profesores
    recuperarCEs(listaUA, listaG, listaP) {

        var listaCE = null;

        $.ajax({
            url: '/Dashboard/ObtenerCarrerasEnfasis',
            data: {
                unidadesAcademicas: listaUA,
                grupos: listaG,
                profesores: listaP
            },
            type: 'post',
            dataType: 'json',
            async: false,
            success: function (resultados) {
                listaCE = resultados;
            }
        });

        return listaCE;

    }

    //Método que permite recuperar los cursos y grupos que se colocan en el filtro
    //listaUAs -> lista de unidades académicas
    //listaCEs -> lista de carreras y énfasis
    //listaPs -> lista de profesores
    recuperarGs(listaUA, listaCE, listaP) {

        var listaG = null;

        $.ajax({
            url: '/Dashboard/ObtenerGrupos',
            data: {
                unidadesAcademicas: listaUA,
                carrerasEnfasis: listaCE,
                profesores: listaP
            },
            type: 'post',
            dataType: 'json',
            async: false,
            success: function (resultados) {
                listaG = resultados;
            }
        });

        return listaG;

    }

    //Método que permite recuperar los profesores que se colocan en el filtro
    //listaUAs -> lista de unidades académicas
    //listaCEs -> lista de carreras y énfasis
    //listaGs -> lista de grupos
    recuperarPs(listaUA, listaCE, listaG) {

        var listaP = null;

        $.ajax({
            url: '/Dashboard/ObtenerProfesores',
            data: {
                unidadesAcademicas: listaUA,
                carrerasEnfasis: listaCE,
                grupos: listaG
            },
            type: 'post',
            dataType: 'json',
            async: false,
            success: function (resultados) {
                listaP = resultados;
            }
        });

        return listaP;

    }

    vaciarFiltro(filtro) {

        //Vacía el filtro
        while (filtro.firstChild) {

            filtro.removeChild(filtro.firstChild);

        }

    }

    //Método que permite crear el desplegable para filtrar las unidades académicas 
    //Permite construir parte del cshtml de la vista
    agregarFiltroUA(container) {

        //Título de la categoría
        var categoria = document.createElement("h5");
        categoria.textContent = "Unidad Académica";
        container.appendChild(categoria);

        //Se agrega el select que contendrá los elementos (opciones)
        var seleccion = document.createElement('select');
        seleccion.id = "filtroUA";
        seleccion.className = "select form-control";
        seleccion.name = "filtroUA"

        //Se agrega el elemento select a la vista
        seleccion.onchange = function () { actualizarDebajoUA() };
        container.appendChild(seleccion);

    }

    //Lena el filtro de unidades académicas con los datos que recupera del controlador según los parámetros provistos
    llenarFiltroUA(listaCE, listaG, listaP) {

        var filtro = document.getElementById("filtroUA");

        this.vaciarFiltro(filtro);

        var defaultOption = document.createElement("option");
        defaultOption.className = "option";
        defaultOption.value = "null";
        defaultOption.text = "Seleccione una opción...";

        filtro.appendChild(defaultOption);

        var resultados = this.recuperarUAs(listaCE, listaP, listaG);

        //Ciclo que crea cada option para luego agregarlo al select
        for (var i = 0; i < resultados.length; ++i) {
            var option = document.createElement("option");
            option.className = "option";
            option.value = resultados[i].CodigoUA;
            option.text = resultados[i].NombreUA;
            filtro.appendChild(option);
        }

    }

    //Método que permite crear el desplegable para filtrar por los énfasis de una carrera
    //Permite construir parte del cshtml de la vista
    agregarFiltroCarreraEnfasis(container) {

        //Título de la categoría
        container.appendChild(document.createElement("hr"));
        var categoria = document.createElement("h5");
        categoria.textContent = "Énfasis de Carrera";
        container.appendChild(categoria);

        //Se agrega el select que contendrá los elementos (opciones)
        var seleccion = document.createElement("select");
        seleccion.id = "filtroCarreraEnfasis";
        seleccion.className = "select form-control";
        seleccion.name = "filtroCarreraEnfasis"


        //Se agrega el elemento select a la vista
        seleccion.onchange = function () { actualizarDebajoCE() };
        container.appendChild(seleccion);

    }

    //Lena el filtro de carreras y énfasis con los datos que recupera del controlador según los parámetros provistos
    llenarFiltroCE(listaUA, listaG, listaP) {

        var filtro = document.getElementById("filtroCarreraEnfasis");

        this.vaciarFiltro(filtro);

        var defaultOption = document.createElement("option");
        defaultOption.className = "option";
        defaultOption.value = "null";
        defaultOption.text = "Seleccione una opción...";

        filtro.appendChild(defaultOption);

        var resultados = this.recuperarCEs(listaUA, listaG, listaP);

        //Ciclo que crea cada option para luego agregarlo al select
        for (var i = 0; i < resultados.length; ++i) {

            var option = document.createElement("option");
            option.className = "option";
            option.value = resultados[i].CodCarrera + "/" + resultados[i].CodEnfasis;
            option.text = resultados[i].NomCarrera + " - " + resultados[i].NomEnfasis;
            filtro.appendChild(option);

        }

    }

    //Método que permite deslpegar el filtro para seleccionar el grupo de un curso en un período específico
    agregarFiltroCursoGrupo(container) {

        //Título de la categoría
        container.appendChild(document.createElement("hr"));
        var categoria = document.createElement("h5");
        categoria.textContent = "Grupos de Curso";
        container.appendChild(categoria);

        //Se agrega el select que contendrá los elementos (opciones)
        var seleccion = document.createElement("select");
        seleccion.id = "filtroCursoGrupo";
        seleccion.className = "select form-control";
        seleccion.name = "filtroCursoGrupo"

        //Se agrega el elemento select a la vista
        seleccion.onchange = function () { actualizarDebajoG() };

        container.appendChild(seleccion);

    }

    //Lena el filtro de grupos con los datos que recupera del controlador según los parámetros provistos
    llenarFiltroG(listaUA, listaCE, listaP) {

        var filtro = document.getElementById("filtroCursoGrupo");

        this.vaciarFiltro(filtro);

        var defaultOption = document.createElement("option");
        defaultOption.className = "option";
        defaultOption.value = "null";
        defaultOption.text = "Seleccione una opción...";

        filtro.appendChild(defaultOption);

        var resultados = this.recuperarGs(listaUA, listaCE, listaP);

        //Ciclo que crea cada option para luego agregarlo al select
        for (var i = 0; i < resultados.length; ++i) {

            var option = document.createElement("option");
            option.className = "option";
            option.value = resultados[i].SiglaCurso + "/" + resultados[i].NumGrupo + "/" + resultados[i].Semestre + "/" + resultados[i].Anno;
            option.text = resultados[i].SiglaCurso + " - " + resultados[i].NombreCurso + " - " + resultados[i].NumGrupo + " - " + resultados[i].Semestre + " - " + resultados[i].Anno;
            filtro.appendChild(option);

        }

    }

    //Método que permite deslpegar el filtro para seleccionar el grupo de un curso en un período específico
    agregarFiltroProfesores(container) {

        //Título de la categoría
        container.appendChild(document.createElement("hr"));
        var categoria = document.createElement("h5");
        categoria.textContent = "Profesor/a";
        container.appendChild(categoria);

        //Se agrega el select que contendrá los elementos (opciones)
        var seleccion = document.createElement("select");
        seleccion.id = "filtroProfesores";
        seleccion.className = "select form-control";
        seleccion.name = "filtroProfesores"


        //Se agrega el elemento select a la vista
        seleccion.onchange = function () { actualizarDebajoP() };
        container.appendChild(seleccion);

    }

    //Lena el filtro de profesores con los datos que recupera del controlador según los parámetros provistos
    llenarFiltroP(listaUA, listaCE, listaG) {

        var filtro = document.getElementById("filtroProfesores");
        
        this.vaciarFiltro(filtro);

        var defaultOption = document.createElement("option");
        defaultOption.className = "option";
        defaultOption.value = "null";
        defaultOption.text = "Seleccione una opción...";

        filtro.appendChild(defaultOption);

        var resultados = this.recuperarPs(listaUA, listaCE, listaG);

        //Ciclo que crea cada option para luego agregarlo al select
        for (var i = 0; i < resultados.length; ++i) {

            var option = document.createElement("option");
            option.className = "option";
            option.value = resultados[i].Correo;
            var nombreCompleto = "";
            if (resultados[i].Apellido1 != null)
                nombreCompleto += resultados[i].Apellido1 + " ";
            if (resultados[i].Apellido2 != null)
                nombreCompleto += resultados[i].Apellido2 + " ";
            if (resultados[i].Nombre1 != null)
                nombreCompleto += resultados[i].Nombre1 + " ";
            if (resultados[i].Nombre2 != null)
                nombreCompleto += resultados[i].Nombre2;
            option.text = nombreCompleto;
            filtro.appendChild(option);

        }

    }

    agregarFiltroFormularios(container) {

        //Título de la categoría
        container.appendChild(document.createElement("hr"));
        var categoria = document.createElement("h5");
        categoria.textContent = "Formulario";
        container.appendChild(categoria);

        //Se agrega el select que contendrá los elementos (opciones)
        var seleccion = document.createElement("select");
        seleccion.id = "filtroFormularios";
        seleccion.className = "select";

        //Se agrega el elemento select a la vista
        seleccion.onchange = function () { actualizarVistaParcial() };
        container.appendChild(seleccion);

    }

    //Lena el filtro de formularios con los datos que recupera del controlador según los parámetros provistos
    llenarFiltroF(listaUA, listaCE, listaG, listaP) {

        var filtro = document.getElementById("filtroFormularios");

        this.vaciarFiltro(filtro);

        var defaultOption = document.createElement("option");
        defaultOption.className = "option";
        defaultOption.value = "null";
        defaultOption.text = "Seleccione una opción...";

        filtro.appendChild(defaultOption);

        var resultados = this.recuperarFs(listaUA, listaCE, listaG, listaP);

        //Ciclo que crea cada option para luego agregarlo al select
        for (var i = 0; i < resultados.length; ++i) {

            var fechaInicio = resultados[i].FechaInicio;
            var fechaFin = resultados[i].FechaFin;

            var stringFechaInicio = new Date(fechaInicio).toLocaleDateString('en-GB', { day: 'numeric', month: 'numeric', year: 'numeric' });
            var stringFechaFin = new Date(fechaFin).toLocaleDateString('en-GB', { day: 'numeric', month: 'numeric', year: 'numeric' });

            var option = document.createElement("option");
            option.className = "option";
            option.value = resultados[i].FCodigo + "*" + resultados[i].FNombre + "*" + resultados[i].CSigla + "*" + resultados[i].GNumero + "*" + resultados[i].GSemestre + "*" + resultados[i].GAnno + "*" + stringFechaInicio + "*" + stringFechaFin;
            option.text = resultados[i].FCodigo + " - " + resultados[i].FNombre + " - " + resultados[i].CSigla + " - " + resultados[i].GNumero + " - " + resultados[i].GSemestre + " - " + resultados[i].GAnno + " - " + stringFechaInicio + " - " + stringFechaFin;
            filtro.appendChild(option);

        }

    }

    //Método que construye el panel de filtros
    crearFiltros() {

        //Se crea un contenedor para agregarlo luego a la vista
        var container = document.createElement("container");
        container.className = "container";
        container.id = "containerFiltros";
        

        //Llamado al método que crear el desplegable para las Unidades Académicas
        this.agregarFiltroUA(container);
        this.agregarFiltroCarreraEnfasis(container);
        this.agregarFiltroCursoGrupo(container);
        this.agregarFiltroProfesores(container);

        var panelfiltros = document.getElementById("panelFiltros");
        panelfiltros.appendChild(container);


        this.llenarFiltroUA(null, null, null);
        this.llenarFiltroCE(null, null, null);
        this.llenarFiltroG(null, null, null);
        this.llenarFiltroP(null, null, null);

    }

    //Función que retorna el array de modelos de unidades académicas
    recuperarUnidadesAcademicas() {
        var ua = document.getElementById("filtroUA");
        var uaSeleccionada = ua.options[ua.selectedIndex].value;
        return uaSeleccionada == "null" ? null : [{ CodigoUA: uaSeleccionada }];
    }

    //Función que retorna el array de modelos de carreras y énfasis
    recuperarCarrerasEnfasis() {
        var ce = document.getElementById("filtroCarreraEnfasis");
        var ceSeleccionada = ce.options[ce.selectedIndex].value;
        var split = ceSeleccionada.split("/");
        return ceSeleccionada == "null" ? null : [{ CodCarrera: split[0], CodEnfasis: split[1] }];
    }

    //Función que retorna el array de modelos de grupo
    recuperarGrupos() {
        var cg = document.getElementById("filtroCursoGrupo");
        var cgSeleccionada = cg.options[cg.selectedIndex].value;
        var split = cgSeleccionada.split("/");
        return cgSeleccionada == "null" ? null : [{ SiglaCurso: split[0], NumGrupo: split[1], Semestre: split[2], Anno: split[3] }];
    }

    //Función que retorna el array de modelos de profesor
    recuperarProfesores() {
        var p = document.getElementById("filtroProfesores");
        var pSeleccionada = p.options[p.selectedIndex].value;
        return pSeleccionada == "null" ? null : [{ Correo: pSeleccionada }];
    }
}