$(window).resize(function () {
    mediaquery();
})

$(window).load(function () {
    mediaquery();
    //$('#preloader').fadeOut('slow');
    //$('body').css({ 'overflow': 'visible' });

    switch ($("#page").val()) {
        case "DivisionesEdit":
            idHidden('sociedad', $("#id_sociedad").val());
            break;
        case "SubdivisionesEdit":
            idHidden('division', $("#id_division").val());
            break;
        case "RolesUsuarioEdit":
            idHidden('rol', $("#id_Rol").val());
            break;
        case "EmpleadosEdit":
            $("#fecha").val(formatFecha2($("#created").val()));
            $("#created").val($("#fecha").val());
            idHidden('subdivision', $("#sucursal").val());
            idHidden('estatusEmpleado', $("#status_empleado").val());
            datepicker();
            break;
        case "BiometricosEdit":
            idHidden('model', $("#modelo").val());
            idHidden('subdivision', $("#id_subdivision").val());
            idHidden('estatus', $("#status").val());
            break;
        case "BiometricosCreate":
            $("#status").val(selectHidden("estatus"));
            break;
        case "Marcaciones":
            datepicker();
            break;
        case "MarcacionesCreate":
            $("#fecha").val(getDate());
            $("#hora").val(getTime());
            datepicker();
            $("#zatza").val(selectHidden("in-out"));
            $("#dallf").val(selectHidden("jornada"));
            $("#status").val(selectHidden("estatus"));
            validateZatza();
            fulldata();
            break;
        case "MarcacionesEdit":
            idHidden('dirIp', $("#id_biometrico").val());
            $("#fecha").val(formatFecha($("#ldate").val()));
            $("#hora").val(formatHora($("#ltime").val()));
            datepicker();
            idHidden('in-out', $("#zatza").val());
            idHidden('jornada', $("#dallf").val());
            idHidden('estatus', $("#status").val());
            validateZatza();
            fulldata();
            break;
        case "EmpleadosCreate":
            $("#fecha").val(getDate());
            $("#created").val(getDate());
            datepicker();
            break;
        case "UserBiometric":
            $("#hora").val(getTime());
            datepicker();
            $(".hora-hidden").css("display", "none");
            break;
    }
});

$(document).ready(function () {
    var options = {
        onKeyPress: function (text, event, currentField, options) {
            if (text) {
                var ipArray = text.split(".");
                var lastValue = ipArray[ipArray.length - 1];
                if (lastValue != "" && parseInt(lastValue) > 255) {
                    ipArray[ipArray.length - 1] = '255';
                    var resultingValue = ipArray.join(".");
                    currentField.text(resultingValue).val(resultingValue);
                }
            }
        },
        translation: {
            'Z': {
                pattern: /[0-9]/, optional: true
            }
        }
    };

    $('#ip').mask('0ZZ.0ZZ.0ZZ.0ZZ', options);

    $("#estatus").change(function (e) {
        $("#status").val(selectHidden('estatus'));
        if ($("#page").val() == "MarcacionesCreate")
            fulldata();
    });

    $("#estatusEmpleado").change(function (e) {
        $("#status_empleado").val(selectHidden('estatusEmpleado'));
    });

    /*$("#status").change(function (e) {
        $("#btn-buscar").removeAttr("disabled");
    });*/

    $("#sociedad").change(function (e) {
        $("#id_sociedad").val(selectHidden('sociedad'));
    });

    $("#division").change(function (e) {
        $("#id_division").val(selectHidden('division'));
    });

    $("#subdivision").change(function (e) {
        $("#id_subdivision").val(selectHidden('subdivision'));

    });

    $("#rol").change(function (e) {
        $("#id_Rol").val(selectHidden('rol'));
    });

    $("#subdiv").change(function (e) {
        $("#btn-buscar").removeAttr("disabled");
    });

    $("#model").change(function (e) {
        $("#modelo").val(selectHidden('model'));

        $("#btn-buscar").removeAttr("disabled");
    });

    $("#dirIp").change(function (e) {
        $("#id_biometrico").val(selectHidden('dirIp'));
        
        $("#btn-buscar").removeAttr("disabled");
    });

    $("#pernr").keyup(function (e) {
        if ($("#page").val() == "EmpleadosCreate") {
            if ($("#pernr").val() != '') {
                buscaSap_empleado($("#pernr").val());
            }
        } else if ($("#page").val() == "MarcacionesCreate") {
            fulldata();
        }
    });

    $("#fecha").on("dp.change", function (e) {
        var fecha = moment($('#fecha').val(), "YYYY/MM/DD");
        var dat = new Date(fecha);
        $("#created").val(FechaActual(dat));

        fecha = fecha.format("YYYYMMDD");
        $("#ldate").val(fecha);
        
        fulldata();
    });

    $("#hora").on("dp.change", function (e) {
        var hora = moment($('#hora').val(), "HH:mm:ss");
        hora = hora.format("HHmmss");

        $("#ltime").val(hora);
        fulldata();
    });

    $("#finicio").on("dp.change", function (e) {
        var finicio = moment($("#finicio").val(), "YYYY/MM/DD");
        var fecha = new Date(finicio);

        $("#ffin").data("DateTimePicker").minDate(fecha);
        $("#ffin").val(getDate());
        $("#btn-buscar").removeAttr("disabled");

        $("#buscar").val(fechasplit($('#finicio').val())).trigger('change');
    });

    $("#ffin").on("dp.change", function (e) {

        if ($("#finicio").val() == $("#ffin").val()) {

            var finicio = moment($("#finicio").val(), "YYYY/MM/DD");
            var fechaini = new Date(finicio);

            var ffin = moment($("#ffin").val(), "YYYY/MM/DD");
            var fechafin = new Date(ffin);

            fechafin.setDate(fechafin.getDate() + 1);

            $("#ffin").val(FechaActual(fechafin));
        }

        $("#buscar2").val(fechasplit($('#ffin').val())).trigger('change');
    });

    $("#in-out").change(function (e) {
        $("#zatza").val(selectHidden('in-out'));
        validateZatza();
        fulldata();
    });
    
    $("#in_out").change(function (e) {
        $("#btn-buscar").removeAttr("disabled");
    });

    $("#jornada").change(function (e) {
        $("#dallf").val(selectHidden('jornada'));
        fulldata();
    });

    $("#moment").change(function (e) {
        $("#btn-buscar").removeAttr("disabled");
    });

    $("#buscar").change(function (e) {
        if ($("#buscar").val() == "")
            $("#btn-buscar").attr("disabled", "disabled");
        else
            $("#btn-buscar").removeAttr("disabled");
    });

    $("#marca").change(function () {
        var combo = document.getElementById('marca');
        var selected = combo.options[combo.selectedIndex].value;

        if (selected != "0") {
            $(".hora-hidden").css("display", "block");
            $("#hora").val(getTime());
            datepicker();
            $(".bio-hidden").css("display", "none");
        } else {
            $(".hora-hidden").css("display", "none");
            $(".bio-hidden").css("display", "block");
            $("#ListBiometricos").prop("selectedIndex", 0);
        }
    });
});

function fechasplit(date) {
    var fecha = moment(date, "YYYY/MM/DD");
    fecha = fecha.format("YYYYMMDD");

    return fecha;
}

function FechaActual(esteMomento) {
    var dianum = esteMomento.getDate();
    var mesnum = esteMomento.getMonth() + 1;
    if (dianum < 10) dianum = '0' + dianum;
    if (mesnum < 10) mesnum = '0' + mesnum;
    var fecha = esteMomento.getFullYear() + "/" + mesnum + "/" + dianum;
    return fecha;
}

function formatFecha(date) {
    var anio = date.substring(0, 4);
    var mes = date.substring(4, 6);
    var dia = date.substring(6);
    var fecha = anio + "/" + mes + "/" + dia;

    return fecha;
}

function formatFecha2(date) {
    var dia = date.substring(0, 2);
    var mes = date.substring(3, 5);
    var anio = date.substring(6, 10);
    var fecha = anio + "/" + mes + "/" + dia;

    return fecha;
}

function formatHora(time) {
    var hora = time.substring(0, 2);
    var minuto = time.substring(2, 4);
    var segundo = time.substring(4);
    var hora = hora + ":" + minuto + ":" + segundo;

    return hora;
}

function fulldata() {
    var pernr = $("#pernr").val();
    var ldate = $("#ldate").val();
    var ltime = $("#ltime").val();
    var zatza = $("#zatza").val();
    var dallf = $("#dallf").val();
    
    $("#fulldata").val(pernr + ldate + ltime + zatza + dallf);
}

function getDate() {
    var esteMomento = new Date();
    var dianum = esteMomento.getDate();
    var mesnum = esteMomento.getMonth() + 1;

    if (dianum < 10) dianum = '0' + dianum;
    if (mesnum < 10) mesnum = '0' + mesnum;

    var fecha = esteMomento.getFullYear() + "/" + mesnum + "/" + dianum;

    return fecha;
}

function getTime() {
    var esteMomento = new Date();
    var hora = esteMomento.getHours();
    var minuto = esteMomento.getMinutes();
    var segundo = esteMomento.getSeconds();

    if (hora < 10) hora = '0' + hora;
    if (minuto < 10) minuto = '0' + minuto;
    if (segundo < 10) segundo = '0' + segundo;

    var laHora = hora + ":" + minuto + ":" + segundo;

    return laHora;
}

function datepicker() {
    $('.date').datetimepicker({
        locale: 'es',
        maxDate: moment()
    });
    
    $('.time').datetimepicker({
        locale: 'es'
    });
}

function mediaquery() {
    var windowview = $(window).height();
    var renderbody = $("#renderbody").height();
    var navbanner = 60;
    var footer = $("#footerdiv").height() + 11;
    var body = renderbody + navbanner + footer;

    if (windowview > body) {
        $("#renderbody").height(windowview - (navbanner + footer));
    }
}

function selectHidden(sel) {
    var combo = document.getElementById(sel);
    var selected = combo.options[combo.selectedIndex].value;

    return selected;
}

function validateZatza() {
    if ($("#zatza").val() == "P10") {
        $("#jornada").attr("disabled", "disabled");
        $("#jornada").prop("selectedIndex", 0);
        $("#dallf").val("");
    }
    else {
        $("#jornada").removeAttr("disabled");
        $("#zatza").val(selectHidden('in-out'));
    }
}

function idHidden(select, id) {
    var combo = document.getElementById(select);

    for (var i = 1; i < combo.length; i++) {
        if (combo.options[i].value == id) {
            combo.selectedIndex = i;
        }
    }
}

function deleteobject(id, page) {
    switch (page) {
        case "roles_Usuario":
            url = "/roles_Usuario/Delete/" + id;
            titulo = "Eliminar Rol";
            texto = "Va a eliminar este Rol del Registro, ¿Desea continuar?";
            textoend = "El Rol ha sido Eliminado Exitosamente";
            ubicacion = "/roles_Usuario";
            break;
        case "User":
            url = "/User/Delete/" + id;
            titulo = "Eliminar Usuario";
            texto = "Va a eliminar este Usuario del Registro, ¿Desea continuar?";
            textoend = "El Usuario ha sido Eliminado Exitosamente";
            ubicacion = "/User";
            break;
        case "sociedad":
            url = "/sociedad/Delete/" + id;
            titulo = "Eliminar Sociedad";
            texto = "Va a eliminar esta sociedad del Registro, ¿Desea continuar?";
            textoend = "La Sociedad ha sido Eliminada Exitosamente";
            ubicacion = "/sociedad";
            break;
        case "division":
            url = "/division/Delete/" + id;
            titulo = "Eliminar División";
            texto = "Va a eliminar esta división del Registro, ¿Desea continuar?";
            textoend = "La División ha sido Eliminada Exitosamente";
            ubicacion = "/division";
            break;
        case "subdivision":
            url = "/subdivision/Delete/" + id;
            titulo = "Eliminar Subdivisión";
            texto = "Va a eliminar esta subdivisión del Registro, ¿Desea continuar?";
            textoend = "La Subdivisión ha sido Eliminada Exitosamente";
            ubicacion = "/subdivision";
            break;
        case "sap_empleado":
            url = "/sap_empleado/Delete/" + id;
            titulo = "Eliminar Empleado";
            texto = "Va a eliminar este Empleado del Registro, ¿Desea continuar?";
            textoend = "El Empleado ha sido Eliminado Exitosamente";
            ubicacion = "/sap_empleado";
            break;
        case "biometrico":
            url = "/biometrico/Delete/" + id;
            titulo = "Eliminar Biométrico";
            texto = "Va a eliminar este biométrico del Registro, ¿Desea continuar?";
            textoend = "El Biométrico ha sido Eliminado Exitosamente";
            ubicacion = "/biometrico";
            break;
        case "sap_marcacion":
            url = "/sap_marcacion/Delete/" + id;
            titulo = "Eliminar Marcación";
            texto = "Va a eliminar esta marcación del Registro, ¿Desea continuar?";
            textoend = "La Marcación ha sido Eliminada Exitosamente";
            ubicacion = "/sap_marcacion";
            break;
    }
    swal({
        title: titulo,
        text: texto,
        icon: "warning",
        buttons: ["Cancelar", "SI"],
        dangerMode: true
    }).then((Eliminar) => {
        if (Eliminar) {
            $.ajax({
                url: url,
                type: 'POST',
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                error: function (status) {
                    if (status.status != 200) {
                        swal({
                            title: "Error Detectado",
                            text: status.status,
                            icon: "warning",
                            dangerMode: true
                        })
                    } else {
                        swal({
                            icon: 'success',
                            text: textoend,
                            buttons: false,
                            timer: 1500,
                        }).then(function () {
                            swal.close();
                            window.location.href = ubicacion;
                        });
                    }
                }
            });
		}
	}); 
}

function buscaSap_empleado(pernr) {
    
    url = "/sap_empleado/searchEmpleado?pernr=" + pernr;
    url = "/User/searchEmpleado?pernr=" + pernr;

    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (json) {
            if (json.pernr != "") {
                $("#name").val(json.name);
                $("#lastname").val(json.lastname);
                $("#dni").val(json.dni);
                var select = document.getElementById("subdivision");
                for (var i = 0; i < select.length; i++) {
                    if (select.options[i].value == json.sucursal) {
                        select.selectedIndex = i;
                    }
                }
                $("#sucursal").val(json.sucursal);
                $("#fecha").val(moment(json.created).format('YYYY/MM/DD'));
                var select = document.getElementById("estatusEmpleado");
                for (var i = 0; i < select.length; i++) {
                    if (select.options[i].value == json.status_empleado) {
                        select.selectedIndex = i;
                    }
                }
                $("#status_empleado").val(json.status_empleado);

                $("#name").attr("readonly", "readonly");
                $("#lastname").attr("readonly", "readonly");
                $("#dni").attr("readonly", "readonly");
                $("#fecha").attr("readonly", "readonly");
                $("#subdivision").prop("disabled", "disabled");
                $("#estatusEmpleado").prop("disabled", "disabled");
                $("#registrar").prop("disabled", "disabled");
            } else {
                $("#name").val("");
                $("#lastname").val("");
                $("#dni").val("");
                $("#subdivision").prop("selectedIndex", 0);
                $("#sucursal").val("");
                $("#fecha").val("");
                $("#estatusEmpleado").prop("selectedIndex", 0);
                $("#status_empleado").val("");

                $("#name").removeAttr("readonly");
                $("#lastname").removeAttr("readonly");
                $("#dni").removeAttr("readonly");
                $("#fecha").removeAttr("readonly");
                $("#subdivision").removeProp("disabled");
                $("#estatusEmpleado").removeProp("disabled");
                $("#registrar").removeProp("disabled");
            }
        }
    });
}