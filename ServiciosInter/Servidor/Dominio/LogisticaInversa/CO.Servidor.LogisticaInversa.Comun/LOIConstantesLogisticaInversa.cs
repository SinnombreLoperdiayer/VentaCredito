using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Comun
{
    public class LOIConstantesLogisticaInversa
    {
        /// <summary>
        /// Constante para el informe de manifiesto
        /// </summary>
        public const string PAR_REPORTE_MANIFIESTO_DETAL = "idManifiesto";

        /// <summary>
        /// Constante para el informe de mensajero
        /// </summary>
        public const string PAR_REPORTE_MENSAJERO_DETAL = "idMensajero";

        /// <summary>
        /// Constante para el informe de Telemercadeo
        /// </summary>
        public const string PAR_REPORTE_TELEMERCADEO_DETAL = "NumeroGuia";

        /// <summary>
        /// Constante para el informe de Telemercadeo
        /// </summary>
        public const string PAR_REPORTE_GESTION = "Gestion";

        /// <summary>
        /// Constante para el informe de Telemercadeo
        /// </summary>
        public const string PAR_REPORTE_DEVOLUCIONES_DETAL = "numeroPlanilla";

        /// <summary>
        /// Constante para el informe de Telemercadeo usuario
        /// </summary>
        public const string PAR_REPORTE_TELEMERCADEO_USUARIO = "Usuario";

        /// <summary>
        /// Constante para el informe de Telemercadeo
        /// </summary>
        public const string PAR_REPORTE_PLANILLA_DEVOLUCIONES = "NumeroPlanilla";

        public const string PAR_REPORTE_CLIENTE_CREDITO = "ClienCre";

        public const string PAR_REPORTE_FORMA_PAGO = "FormaPago";

        public const string PAR_REPORTE_RACOL = "Racol";

        /// <summary>
        /// Constante para el informe de Notificaciones
        /// </summary>
        public const string PAR_REPORTE_AGENCIA_ORIGEN = "AgenciaOrigen";

        /// <summary>
        /// Constante para el informe de Notificaciones
        /// </summary>
        public const string PAR_REPORTE_AGENCIA_DESTINO = "AgenciaDestino";

        /// <summary>
        /// Constante para el informe de Notificaciones
        /// </summary>
        public const string PAR_REPORTE_FECHA_INICIAL = "FechaInicial";

        /// <summary>
        /// Constante para el informe de Notificaciones
        /// </summary>
        public const string PAR_REPORTE_FECHA_FINAL = "FechaFinal";

        /// <summary>
        /// Constante para el informe de Notificaciones
        /// </summary>
        public const string PAR_REPORTE_NIT = "Nit";

        /// <summary>
        /// Constante para el informe de Notificaciones
        /// </summary>
        public const string PAR_REPORTE_ESTADO = "Estado";

        /// <summary>
        /// Constante para el informe de acta de disposicion
        /// </summary>
        public const string PAR_NUMERO_IMPRESO = "nroImpreso";

        /// <summary>
        /// Creada en el manifiesto
        /// </summary>
        public const string CREADA = "CRE";

        /// <summary>
        /// Creada en el manifiesto
        /// </summary>
        public const string CREADA_DESC = "Creada en el manifiesto";

        /// <summary>
        /// Entrega exitosa
        /// </summary>
        public const string ENTREGA_EXITOSA = "ENT";

        /// <summary>
        /// Entrega exitosa
        /// </summary>
        public const string ENTREGA_EXITOSA_DESC = "Entrega exitosa";

        /// <summary>
        /// Entrega mal diligenciada
        /// </summary>
        public const string ENTREGA_MAL_DILIGENCIADA = "EMD";

        /// <summary>
        /// Entrega mal diligenciada
        /// </summary>
        public const string ENTREGA_MAL_DILIGENCIADA_DESC = "Entrega mal diligenciada";

        /// <summary>
        /// En devolución
        /// </summary>
        public const string DEVOLUCION = "DEV";

        /// <summary>
        /// En devolución
        /// </summary>
        public const string DEVOLUCION_DESC = "En devolución";

        /// <summary>
        /// Pago exitoso
        /// </summary>
        public const string PAGO_EXITOSO = "PEX";

        /// <summary>
        /// Pago exitoso
        /// </summary>
        public const string PAGO_EXITOSO_DESC = "Pago exitoso";

        /// <summary>
        /// Pago mal diligenciado
        /// </summary>
        public const string PAGO_MAL_DILIGENCIADO = "PMD";

        /// <summary>
        /// Pago mal diligenciado
        /// </summary>
        public const string PAGO_MAL_DILIGENCIADO_DESC = "Pago mal diligenciado";

        /// <summary>
        /// Nombre del archivo del volante de devolución
        /// </summary>
        public const string NOMBRE_ARCHIVO_VOLANTE = "Volantedevolucion";

        /// <summary>
        /// Ruta por defecto de las Imagenes de
        /// Escaneo y cargue de Archivos
        /// </summary>
        public static string RUTA_IMAGENES = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Imagenesescaner";

        /// <summary>
        /// Falla guía no descargada
        /// </summary>
        public const int FALLA_GUIA_NO_DESCARGADA = 7;

        /// <summary>
        /// Falla guía mal diligenciada
        /// </summary>
        public const int FALLA_GUIA_MAL_DILIGENCIADA = 8;

        /// <summary>
        /// Nomenclatura nombre archivos
        /// </summary>
        public const string NOMBRE_DIGITALIZACION_FACTURAS_GIROS = "GIR";

        /// <summary>
        /// Nomenclatura nombre archivos
        /// </summary>
        public const string NOMBRE_DIGITALIZACION_PAGOS_GIROS = "PAG";

        /// <summary>
        /// Nomenclatura nombre archivos
        /// </summary>
        public const string NOMBRE_DIGITALIZACION_GUIAS_MENSAJERIA = "GUIA";

        /// <summary>
        /// Número máximo de lotes por caja
        /// </summary>
        public const int NUMERO_MAXIMO_LOTES_POR_CAJA = 20;

        /// <summary>
        /// Número máximo de posiciones por lote
        /// </summary>
        public const int NUMERO_MAXIMO_POSICIONES_POR_LOTE = 100;

        /// <summary>
        /// Identificador estado legigle guías
        /// </summary>
        public const string ID_ESTADO_LEGIBLE = "LEG";

        /// <summary>
        /// Identificador estado ilegible guías
        /// </summary>
        public const string ID_ESTADO_ILEGIBLE = "ILE";

        /// <summary>
        /// Identificador estado Incompleto guías
        /// </summary>
        public const string ID_ESTADO_INCOMPLETO = "INC";

        /// <summary>
        /// Identificador estado no clasificado
        /// </summary>
        public const string ID_ESTADO_NO_CLASIFICADO = "NOC";

        /// <summary>
        /// Identificador estado físico bueno de la guía
        /// </summary>
        public const string ID_ESTADO_BUENO = "BUE";

        /// <summary>
        /// Identificador estado físico regular de la guía
        /// </summary>
        public const string ID_ESTADO_REGULAR = "REG";

        /// <summary>
        /// Identificador estado físico malo de la guía
        /// </summary>
        public const string ID_ESTADO_MALO = "MAL";

        /// <summary>
        /// Acción de auditoria para eliminar
        /// </summary>
        public const string ACCION_ELIMINAR = "ELIMINAR";

        /// <summary>
        /// Constante para el parametro del estado de la planilla planillada
        /// </summary>
        public const string ESTADO_PLANILLADA = "PLA";

        /// <summary>
        /// Constante para la descripcion del estado de la planilla planillada
        /// </summary>
        public const string DES_ESTADO_PLANILLADA = "Planillada";

        /// <summary>
        /// Constante que establece el número mínimo de caracteres del valor decodificado
        /// </summary>
        public const int DIGITALIZACION_NUMERO_MINIMO_CARACTERES_VALOR_DECODIFICADO = 3;

        /// <summary>
        /// Direccion del reporte de detalle manifiesto
        /// </summary>
        public const string REPORTE_MANIFIESTO_DETALLE = "/LogisticaInversa/ReporteManifiesto.aspx";

        /// <summary>
        /// Direccion del reporte de detalle manifiesto
        /// </summary>
        public const string REPORTE_MENSAJERO_DETALLE = "/LogisticaInversa/ReporteMensajero.aspx";

        /// <summary>
        /// Direccion del reporte de detalle manifiesto
        /// </summary>
        ///
        public const string REPORTE_TELEMERCADEO_DETALLE = "/LogisticaInversa/RptResumenTelemercadeo.aspx";

        /// <summary>
        /// Direccion del reporte de telemercadeo nueva direccion
        /// </summary>
        public const string REPORTE_TELEMERCADEO_NUEVADIR = "/LogisticaInversa/RptTelemercadeoNuevaDireccion.aspx";

        /// <summary>
        /// Direccion del reporte de detalle manifiesto
        /// </summary>
        public const string REPORTE_PLANILLA_DEVOLUCIONES = "/LogisticaInversa/RptPlanillaDevolucion.aspx";

        /// <summary>
        /// Direccion del reporte de detalle manifiesto
        /// </summary>
        public const string REPORTE_TAPAS_DEVOLUCIONES = "/LogisticaInversa/RptTapaDevolucionGuia.aspx";

        /// <summary>
        /// Direccion del reporte de detalle manifiesto
        /// </summary>
        public const string REPORTE_PLANILLA_TAPAS_DEVOLUCIONES = "/LogisticaInversa/RptTapaDevolucionPlanilla.aspx";

        /// <summary>
        /// Direccion del reporte de consulta automática de recibidos
        /// </summary>
        public const string REPORTE_CONSULTA_AUT_RECIBIDOS = "/LogisticaInversa/RptConsultaAutomaticaRecibidos.aspx";

        /// <summary>
        /// Direccion del reporte de detalle manifiesto
        /// </summary>
        public const string REPORTE_NOTIFICACIONES_ENTREGA = "/LogisticaInversa/RptCertNotificacionEntrega.aspx";

        /// <summary>
        /// Dirección del reporte de Prueba de Entrega para el cliente DIAN
        /// </summary>
        public const string REPORTE_PRUEBA_ENTREGA_DIAN = "/LogisticaInversa/RptCertNotificacionPruebaEntrega.aspx";

        /// <summary>
        /// Direccion del reporte de detalle manifiesto
        /// </summary>
        public const string REPORTE_NOTIFICACIONES_DEV = "/LogisticaInversa/RptCertNotificacionDevolucion.aspx";

        /// <summary>
        /// Direccion del reporte de detalle manifiesto
        /// </summary>
        public const string REPORTE_PLANILLA_CERTIFICACION = "/LogisticaInversa/RptPlanillaCertificaciones.aspx";

        /// <summary>
        /// Direccion del reporte de planilla guia interna
        /// </summary>
        public const string REPORTE_PLANILLA_GUIA = "/LogisticaInversa/RptGuiaInternaCertificacion.aspx";

        /// <summary>
        /// Direccion del reporte de planilla guia interna devolucion
        /// </summary>
        public const string REPORTE_PLANILLA_GUIA_DEVOLUCION = "/LogisticaInversa/RptGuiaInternaCertificacionDevolucion.aspx";

        /// <summary>
        /// Direccion del reporte de acta de dispocision final
        /// </summary>
        public const string REPORTE_ACTA_DISPOSICION = "/LogisticaInversa/RptActaDisposicionFinal.aspx";

        /// <summary>
        /// Direccion del reporte de Prueba de Devolucion para el cliente DIAN
        /// </summary>
        public const string REPORTE_PRUEBA_DEVOLUCION = "/LogisticaInversa/RptCertNotificacionPruebaDevolucion.aspx";

        /// <summary>
        /// Llave del diccionario, encargada de ejecutar la regla de negocio con le estado
        /// </summary>
        public const string ESTADO_GUIA = "EstadoGuiaKey";

        /// <summary>
        /// Llave del diccionario, encargada de ejecutar la regla de negocio resultado
        /// </summary>
        public const string RESULTADO_TELEMERCADEO = "ResultadoKey";

        /// <summary>
        /// Mensaje para las imagenes no reconocidas durante el escaneo
        /// </summary>
        public const string IMAGEN_NO_RECONOCIDA = "Imagen no reconocida No ";

        /// <summary>
        /// parametro con el numero de archivos a enviar por lote de escaneo
        /// </summary>
        public const string PARAMETRO_REGISTROS_DIGITALIZACION = "NoRegistrosEscaner";

        /// <summary>
        /// parametro con el numero de archivos a enviar por lote de escaneo
        /// </summary>
        public const string DEVOLUCION_GUIA_NO = "Devolución de admisión número ";

        /// <summary>
        ///
        /// </summary>
        public const string PAR_REPORTE_PLANILLA_CERTIFICACION = "idPlanilla";

        /// <summary>
        ///
        /// </summary>
        public const string PAR_REPORTE_TIPO_PLANILLA_CERTIFICACION = "idTipoPlanilla";

        /// <summary>
        /// Numero de Guia
        /// </summary>
        public const string PAR_REPORTE_PLANILLA_NRO_GUIA = "NroGuia";

        /// <summary>
        /// Constantes con las planillas de certificacion
        /// </summary>
        public const string DOCUMENTO_PLANILLA_CERTIFICACION = "Planilla de certificación";

        /// <summary>
        /// Certificacion devolucion
        /// </summary>
        public const string DOCUMENTO_CERTIFICACION_DEVOLUCION = "Certificación de devolución";

        /// <summary>
        /// Certificacion Entrega
        /// </summary>
        public const string DOCUMENTO_CERTIFICACION_ENTREGA = "Certificación de entrega";

        /// <summary>
        /// Guias certificacion
        /// </summary>
        public const string DOCUMENTO_GUIAS_CERTIFICACION = "Guías de correspondencia para las certificaciones";

        /// <summary>
        /// Guias certificacion
        /// </summary>
        public const string DOCUMENTO_GUIAS_PLANILLA = "Guías de correspondencia para las planillas";

        /// <summary>
        /// Guias certificacion
        /// </summary>
        public const string DOCUMENTO_GUIA_INTERNA_CONSOLIDADA = "Guías Interna Consolidada";

        /// <summary>
        /// Guias certificacion
        /// </summary>
        public const string FOLDER_CLIENTE_DIGITALIZACION = "FolderClienteDigita";

        /// <summary>
        /// Direccion admision
        /// </summary>
        public const string DIRECCION_ADMISION = "DireccionAdmision";

        /// <summary>
        /// Destinatario
        /// </summary>
        public const string DESTINATARIO= "Destinatario";

        /// <summary>
        /// Destinatario
        /// </summary>
        public const string DOCUMENTO_PRUEBA_DEVOLUCION = "Prueba Devolucion DIAN";


        public const string LOCALIDAD = "Localidad";
            


        public const int TIPO_MOTIVO_GUIA_HURTO_DESCARGUE_AGENCIA = 47;

        public const int TIPO_MOTIVO_GUIA_INCAUTACION_DESCARGUE_AGENCIA = 48;

        public const int TIPO_EVIDENCIA_DEVOLUCION_ACTA_INCAUTACION = 4;

        public const int TIPO_EVIDENCIA_DEVOLUCION_DENUNCIO = 5;

    }
}