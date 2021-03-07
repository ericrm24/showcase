class Filtros {

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

    //Método que permite recuperar los formularios que se colocan en el filtro
    //listaUAs -> lista de unidades académicas
    //listaCEs -> lista de carreras y énfasis
    //listaGs -> lista de grupos
    //listaPs -> lista de profesores
    recuperarFs(listaUA, listaCE, listaG, listaP) {

        var listaF = null;

        $.ajax({
            url: '/Dashboard/ObtenerFormularios',
            data: {
                unidadesAcademicas: listaUA,
                carrerasEnfasis: listaCE,
                grupos: listaG,
                profesores: listaP
            },
            type: 'post',
            dataType: 'json',
            async: false,
            success: function (resultados) {
                listaF = resultados;
            }
        });

        return listaF;

    }

    vaciarFiltro(filtro)
    {

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
        var seleccion = document.createElement("select");
        seleccion.id = "filtroUA";
        seleccion.className = "select";
        seleccion.multiple = "multiple";
        
        //Se agrega el elemento select a la vista
        seleccion.onchange = function () { actualizarDebajoUA() };
        container.appendChild(seleccion);

    }

    //Lena el filtro de unidades académicas con los datos que recupera del controlador según los parámetros provistos
    llenarFiltroUA(listaCE, listaG, listaP) {

        var filtro = document.getElementById("filtroUA");

        this.vaciarFiltro(filtro);

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
        seleccion.className = "select";
        seleccion.multiple = "multiple";

        //Se agrega el elemento select a la vista
        seleccion.onchange = function () { actualizarDebajoCE() };
        container.appendChild(seleccion);

    }

    //Lena el filtro de carreras y énfasis con los datos que recupera del controlador según los parámetros provistos
    llenarFiltroCE(listaUA, listaG, listaP) {

        var filtro = document.getElementById("filtroCarreraEnfasis");

        this.vaciarFiltro(filtro);

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
        seleccion.className = "select";
        seleccion.multiple = "multiple";

        //Se agrega el elemento select a la vista
        seleccion.onchange = function () { actualizarDebajoG() };

        container.appendChild(seleccion);
    }

    //Lena el filtro de grupos con los datos que recupera del controlador según los parámetros provistos
    llenarFiltroG(listaUA, listaCE, listaP) {

        var filtro = document.getElementById("filtroCursoGrupo");

        this.vaciarFiltro(filtro);

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
        seleccion.className = "select";
        seleccion.multiple = "multiple";

        //Se agrega el elemento select a la vista
        seleccion.onchange = function () { actualizarDebajoP() };
        container.appendChild(seleccion);

    }

    //Lena el filtro de profesores con los datos que recupera del controlador según los parámetros provistos
    llenarFiltroP(listaUA, listaCE, listaG) {

        var filtro = document.getElementById("filtroProfesores");

        this.vaciarFiltro(filtro);

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

    agregarFiltroFormularios(container)
    {

        //Título de la categoría
        container.appendChild(document.createElement("hr"));
        var categoria = document.createElement("h5");
        categoria.textContent = "Formulario";
        container.appendChild(categoria);

        //Se agrega el select que contendrá los elementos (opciones)
        var seleccion = document.createElement("select");
        seleccion.id = "filtroFormularios";
        seleccion.className = "select";
        //seleccion.multiple = "multiple";

        //Se agrega el elemento select a la vista
        seleccion.onchange = function () { actualizarVistaParcial() };
        container.appendChild(seleccion);

    }

    //Lena el filtro de formularios con los datos que recupera del controlador según los parámetros provistos
    llenarFiltroF(listaUA, listaCE, listaG, listaP) {

        var filtro = document.getElementById("filtroFormularios");

        this.vaciarFiltro(filtro);

        var resultados = this.recuperarFs(listaUA, listaCE, listaG, listaP);
        var option = document.createElement("option");
        option.className = "option";
        option.text = "Ninguno seleccionado";
        option.value = "null";
        filtro.appendChild(option);

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
    crearFiltros()
    {

        //Se crea un contenedor para agregarlo luego a la vista
        var container = document.createElement("container");
        container.className = "container";
        container.id = "containerFiltros";

        //Llamado al método que crear el desplegable para las Unidades Académicas
        this.agregarFiltroUA(container);
        this.agregarFiltroCarreraEnfasis(container);
        this.agregarFiltroCursoGrupo(container);
        this.agregarFiltroProfesores(container);
        this.agregarFiltroFormularios(container);

        var panelfiltros = document.getElementById("panelFiltros");
        panelfiltros.appendChild(container);

        this.llenarFiltroUA(null, null, null);
        this.llenarFiltroCE(null, null, null);
        this.llenarFiltroG(null, null, null);
        this.llenarFiltroP(null, null, null);
        this.llenarFiltroF(null, null, null, null);

    }

    //Función que retorna el array de modelos de unidades académicas
    recuperarUnidadesAcademicas() {
        var ua = document.getElementById("filtroUA");
        var uaSelec = [];
        for (var i = 0; i < ua.length; i++) //Obtención de las opciones seleccionadas
        {
            if (ua.options[i].selected)
                uaSelec.push(ua.options[i].value);
        }
        var uaObjs= []
        if (uaSelec != "null")  //Se crea un arreglo de objetos con el 
        {                       //código de las Unidades Académicas seleccionadas
            for (var i = 0; i < uaSelec.length; i++)
            {
                uaObjs.push({ CodigoUA: uaSelec[i] });
            }
        }
        //alert(uaSelec);
        return uaSelec == "null" ? null : uaObjs;
    }

    //Función que retorna el array de modelos de carreras y énfasis
    recuperarCarrerasEnfasis() {
        var ce = document.getElementById("filtroCarreraEnfasis");
        var ceSeleccionada = [];
        for (var i = 0; i < ce.length; i++) //Obtención de las opciones seleccionadas
        {
            if (ce.options[i].selected)
                ceSeleccionada.push(ce.options[i].value);
        }
        var ceObjs = []
        if (ceSeleccionada != "null")   //Se crea un arreglo de objetos con las 
        {                               //Carreras y Énfasis seleccionados
            for (var i = 0; i < ceSeleccionada.length; i++) {
                var split = ceSeleccionada[i].split("/");
                ceObjs.push({ CodCarrera: split[0], CodEnfasis: split[1] });
            }
        }
        return ceSeleccionada == "null" ? null : ceObjs;
    }

    //Función que retorna el array de modelos de grupo
    recuperarGrupos() {
        var cg = document.getElementById("filtroCursoGrupo");
        var cgSeleccionado = [];
        for (var i = 0; i < cg.length; i++) //Obtención de las opciones seleccionadas
        {
            if (cg.options[i].selected)
                cgSeleccionado.push(cg.options[i].value);
        }

        var cgObjs = []
        if (cgSeleccionado != "null")   //Se crea un arreglo de objetos con las 
        {                               //Carreras y Énfasis seleccionados
            for (var i = 0; i < cgSeleccionado.length; i++)
            {
                var split = cgSeleccionado[i].split("/");
                cgObjs.push({ SiglaCurso: split[0], NumGrupo: split[1], Semestre: split[2], Anno: split[3] });
            }
        }
        return cgSeleccionado == "null" ? null : cgObjs;
    }

    //Función que retorna el array de modelos de profesor
    recuperarProfesores() {
        var p = document.getElementById("filtroProfesores");
        var pSeleccionada = [];
        for (var i = 0; i < p.length; i++) //Obtención de las opciones seleccionadas
        {
            if (p.options[i].selected)
                pSeleccionada.push(p.options[i].value);
        }

        var pObjs = []
        if (pSeleccionada != "null")  //Se crea un arreglo de objetos con el 
        {                       //código de las Unidades Académicas seleccionadas
            for (var i = 0; i < pSeleccionada.length; i++) {
                pObjs.push({ Correo: pSeleccionada[i] });
            }
        }

        return pSeleccionada == "null" ? null : pObjs ;
    }

    //Función que actualiza el componente de formularios según los filtros.
    recuperarFormulario() {
        var f = document.getElementById("filtroFormularios");
        var fSeleccionado = [];
        for (var i = 0; i < f.length; i++)  //Obtención de las opciones seleccionadas
        {
            if (f.options[i].selected)
                fSeleccionado.push(f.options[i].value);
        }
        return fSeleccionado.length == "null" ? null : fSeleccionado[0].split("*");
    }

}