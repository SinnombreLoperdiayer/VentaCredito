using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.OperacionUrbana.Comun
{
    public class OUConstantesOperacionUrbana
    {
        /// <summary>
        /// constante para el estado activo
        /// </summary>
        public const string ESTADO_ACTIVO = "ACT";

        /// <summary>
        /// Constante para el estado inactivo
        /// </summary>
        public const string ESTADO_INACTIVO = "INA";

        /// <summary>
        /// Constante para el estado descargado
        /// </summary>
        public const string ESTADO_DESCARGADO = "DES";

        /// <summary>
        /// Constante para el estado ingresado
        /// </summary>
        public const string ESTADO_INGRESADO = "ING";

        /// summary>
        /// Constante para el id del cargo Control de cuentas
        /// </summary>
        public const int CARGO_CONTROL_CUENTAS = 2;

        /// <summary>
        /// Constante para el id del cargo Coordinador de Suministros
        /// </summary>
        public const int CARGO_COORDINADOR_SUMINISTROS = 3;

        /// <summary>
        /// Constante para el id del cargo Ejecutivo de cuenta
        /// </summary>
        public const int CARGO_EJECUTIVO_CUENTA = 4;

        /// <summary>
        /// Constante para el id del cargo Supervisor cuenta cliente
        /// </summary>
        public const int CARGO_SUPERV_CTA_CLIE = 5;

        /// <summary>
        /// Constante para el id del cargo Coordinador de COL
        /// </summary>
        public const int CARGO_COORDINADOR_COL = 6;

        /// <summary>
        /// Constante para las guias que no fueron registradas en el sistema
        /// </summary>
        public const string SIN_GUIA_CREADA = "NCR";

        /// <summary>
        /// Constante para las guias registradas en el sistema
        /// </summary>
        public const string CON_GUIA_CREADA = "CRE";

        /// <summary>
        /// Constante para el suministro de bolsa de seguridad
        /// </summary>
        public const int SUMINISTRO_GUIA_MANUAL = 1;

        /// <summary>
        /// Constante para el suministro de bolsa de seguridad
        /// </summary>
        public const int SUMINISTRO_BOLSA_SEGURIDAD = 2;

        /// <summary>
        /// Constante para el peso minimo del estado del empaque menor a 1 Kg
        /// </summary>
        public const int PESO_MINIMO_MENOR = 0;

        /// <summary>
        /// Constante para el peso maximo del estado del empaque menor a 1 Kg
        /// </summary>
        public const decimal PESO_MAXIMO_MENOR = 1;

        /// <summary>
        /// Constante para el peso maximo del estado del empaque menor a 1 Kg
        /// </summary>
        public const int PESO_MAXIMO_MAYOR = 100;

        /// <summary>
        /// Constante para la falla diferencia de peso de las guías
        /// </summary>
        public const int FALLA_DIFERENCIA_PESO = 1;

        /// <summary>
        /// Constante para la falla cuando la guia no fue provisionada
        /// </summary>
        public const int FALLA_GUIA_PROVISIONADA = 2;

        /// <summary>
        /// constante para la falla del cliente con presupuesto vencido
        /// </summary>
        public const int FALLA_PRESUPUESTO_VENCIDO = 3;

        /// <summary>
        /// Constante para la falla cuando el cliente tiene suministro de bolsa de seguridad
        /// y el envio viene sin bolsa
        /// </summary>
        public const int FALLA_SIN_BOLSA_DE_SEGURIDAD = 11;

        /// <summary>
        /// Constante para la falla de las guias q no fueron planilladas
        /// </summary>
        public const int FALLA_GUIAS_SIN_PLANILLAR = 6;

        /// <summary>
        /// Constante para la falla de reabrir recogida esporadica
        /// </summary>
        public const int FALLA_REABRIR_RECOGIDA = 9;

        /// <summary>
        /// Constante para el estado del empaque cuando el cliente
        /// no tiene suministro de bolsa de seguridad
        /// </summary>
        public const int ID_ESTADO_EMPAQUE_SIN_SUMINISTRO = 11;

        /// <summary>
        /// constante para la descripcion del estado de empaque cuando el cliente
        /// no tiene suministro de bolsa de seguridad
        /// </summary>
        public const string ESTADO_EMPAQUE_SIN_SUMINISTRO = "Sin suministro";

        /// <summary>
        /// constante para la Descripcion Si
        /// </summary>
        public const string DESCRIPCION_SI = "Si";

        /// <summary>
        /// Constante para la descripcion no
        /// </summary>
        public const string DESCRIPCION_NO = "No";

        /// <summary>
        /// Constante para el estado del empaque sin bolsa de seguridad
        /// </summary>
        public const int ID_ESTADO_EMPAQUE_SIN_BOLSA_SEGURIDAD = 2;

        /// <summary>
        /// Constante para el estado del empaque mal embalado
        /// </summary>
        public const int ID_ESTADO_EMPAQUE_MAL_EMBALADO = 4;

        /// <summary>
        /// Constante para tipo de envio carga
        /// </summary>
        public const string CARGA = "CAR";

        /// <summary>
        /// Constante para el tipo de envio mensajeria
        /// </summary>
        public const string MENSAJERIA = "MEN";

        /// <summary>
        /// constante para el parametro de la planilla venta
        /// </summary>
        public const string PARAMETRO_REPORTE_ID_PLANILLA = "idPlanilla";

        /// <summary>
        /// Constante para el tipo de servicio rapiradicado
        /// </summary>
        public const int RAPIRADICADO = 16;

        /// <summary>
        /// Constante para el estado de la guía en devolución
        /// </summary>
        public const short ESTADO_GUIA_DEVOLUCION = 11;

        /// <summary>
        /// Constante para el estado de la guía en entrega
        /// </summary>
        public const short ESTADO_GUIA_ENTREGA = 12;

        /// <summary>
        /// Constante para el estado de la planilla cerrada
        /// </summary>
        public const string ESTADO_PLANILLA_CERRADA = "2  ";

        /// <summary>
        /// Constante para el estado de la planilla abierta
        /// </summary>
        public const string ESTADO_PLANILLA_ABIERTA = "1  ";

        /// <summary>
        /// Constante para el nombre del reporte de planilla ventas
        /// </summary>
        public const string REPORTE_PLANILLA_VENTAS = "/OperacionUrbana/ReportePlanillaVenta.aspx";

        /// <summary>
        /// constante para el parametro de los envios pendientes del mensajero
        /// </summary>
        public const string PARAMETRO_REPORTE_ID_MENSAJERO = "idMensajero";

        /// <summary>
        /// Constante para el nombre del reporte de planilla ventas
        /// </summary>
        public const string REPORTE_ENVIOS_PENDIENTES_MENSAJERO = "/OperacionUrbana/ReporteEnviosPendietesMensajero.aspx";

        /// <summary>
        /// Constante para el nombre del reporte de planilla de asignacion de envios
        /// </summary>
        public const string REPORTE_PLANILLA_ASIGNACION_ENVIOS = "/OperacionUrbana/ReportePlanillaAsignacion.aspx";

        /// <summary>
        /// Constante para el nombre del reporte de planilla ventas
        /// </summary>
        public const string REPORTE_PLANILLA_RECOGIDAS = "/OperacionUrbana/ReportePlanillaRecogida.aspx";

        /// <summary>
        /// Constante para el id de la ruta no encontrada
        /// </summary>
        public const int ID_RUTA_NO_ENCONTRADA = 0;

        /// <summary>
        /// constante para la descripcion de la ruta no encontrada
        /// </summary>
        public const string DESCRIPCION_RUTA_NO_ENCONTRADA = "Ruta no encontrada";

        /// <summary>
        /// Constante para el parametro de desfase del peso
        /// </summary>
        public const string PARAMETRO_DESFASE_PESO = "DesfasePeso";

        /// <summary>
        /// Constante para el parametro de desfase del peso
        /// </summary>
        public const string PARAMETRO_PESO_MAXIMO_MENSAJERIA = "PesoMaxMen";

        /// <summary>
        /// Constante para el parametro del estado de la planilla planillada
        /// </summary>
        public const string ESTADO_PLANILLADA = "PLA";

        public const string ESTADO_ASIGNADA = "ASI";

        /// <summary>
        /// Constante para el parametro del estado de una asignación de tulas o contenedores
        /// </summary>
        public const string ESTADO_CREADA = "CRE";

        /// <summary>
        /// Constante para el tipo de recogida esporadica
        /// </summary>
        public const string TIPO_RECOGIDA_ESPORADICA = "ESP";

        /// <summary>
        /// Constante para el tipo de recogida esporadica
        /// </summary>
        public const string TIPO_RECOGIDA_FIJA = "FIJ";

        /// <summary>
        /// Constante para el tipo de recogida esporadica
        /// </summary>
        public const int ID_ESTADO_PENDIENTE_POR_PROGRAMAR = 2;

        /// <summary>
        /// Constante para el tipo de recogida esporadica
        /// </summary>
        public const int ID_ESTADO_RECOGIDA_DISPONIBLE = 8;

        /// <summary>
        /// Constante para el tipo de recogida esporadica
        /// </summary>
        public const int ID_ESTADO_PROGRAMADA = 1;

        /// <summary>
        /// Constante para el parametro del estado de la planilla no planillada
        /// </summary>
        public const string ESTADO_NO_PLANILLADA = "NPL";

        /// <summary>
        /// Constante para el tipo de modificacion modified para la tabla de auditoria
        /// </summary>
        public const int ID_CONSECUTIVO_PARAMETROS_PLANILLA_RECOGIDA = 3;

        /// <summary>
        /// Constante para la descripcion del tipo de recogida esporpádica "ESP"
        /// </summary>
        public const string DESCRIPCION_RECOGIDA_ESPORADICA = "Esporádica";

        /// <summary>
        /// Constante para la descripcion del tipo de recogida fija "FIJ"
        /// </summary>
        public const string DESCRIPCION_RECOGIDA_FIJA = "Fija";

        /// <summary>
        /// Constante para el estado entregado de la planilla
        /// </summary>
        public const string ESTADO_ENTREGADO_PLANILLA = "ENT";

        /// <summary>
        /// Constante para el estado devolución de la planilla
        /// </summary>
        public const string ESTADO_DEVOLUCION = "DEV";

        /// <summary>
        /// Entrega exitosa
        /// </summary>
        public const string ENTREGA_EXITOSA_DESC = "Entrega exitosa";

        /// <summary>
        /// En devolución
        /// </summary>
        public const string DEVOLUCION_DESC = "En devolución";

        /// <summary>
        /// Id del estado de la recogida descargada
        /// </summary>
        public const int ESTADO_RECOGIDA_DESCARGADA = 4;

        /// <summary>
        /// Ruta por defecto de las Imagenes de
        /// Escaneo y cargue de Archivos
        /// </summary>
        public static string RUTA_IMAGENES = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Imagenesescaner";

        /// <summary>
        /// Nombre del archivo del volante de devolución
        /// </summary>
        public const string NOMBRE_ARCHIVO_VOLANTE = "Volantedevolucion";

        /// <summary>
        /// Usuario que imprime el reporte
        /// </summary>
        public const string PARAMETRO_USUARIO_IMPRIME = "UsuarioImprime";

        /// <summary>
        /// Nombre del archivo del volante de devolución
        /// </summary>
        public const string CONVENIO = "Convenio";

        /// <summary>
        /// Nombre del archivo del volante de devolución
        /// </summary>
        public const string PEATON = "Peaton";

        /// <summary>
        /// Nombre del archivo del volante de devolución
        /// </summary>
        public const string PUNTOSERVICIO = "Punto de Servicio";

        /// <summary>
        /// Hora limite para programar las recogidas de un dia
        /// </summary>
        public const string HORA_LIMITE_PROGRAMACION_RECOGIDAS = "HoraLimiteProgRecogi";

        /// <summary>
        /// Tipo de despacho utilizado en la planillacion de envios
        /// </summary>
        public const string TIPO_DESPACHO_CONSOLIDADO = "Consolidado";

        /// <summary>
        /// Tipo de despacho utilizado en la planillacion de envios
        /// </summary>
        public const string TIPO_DESPACHO_SUELTO = "Suelto";

        public const string ESTADO_ABIERTA_DESCRIPCION = "Abierta";
        public const string ESTADO_CERRADO_DESCRIPCION = "Cerrado";

        /// <summary>
        /// Fecha utilizada para conocer las guias planilladas a cierto mensajero
        /// </summary>
        public const string FECHA = "fecha";

        /// <summary>
        /// se envia el numero de auditoria generado
        /// </summary>
        public const string NUMERO_AUDITORIA = "numeroAuditoria";

        /// <summary>
        /// se envia el numero de sobrante
        /// </summary>
        public const string SOBRANTE = "sobrante";

        /// <summary>
        /// Constante para el nombre del reporte de planilla auditoria asignacion a mensajero
        /// </summary>
        public const string REPORTE_PLANILLA_AUDITA_ASIGNACION = "/OperacionUrbana/ReporteAuditoriaAsignacionMensajero.aspx";

    }
}