using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;

namespace CO.Servidor.Dominio.Comun.Tarifas
{
    /// <summary>
    ///  Interfaz para acceso a la fachada tarifas
    /// </summary>
    public interface ITAFachadaTarifas
    {
        /// <summary>
        /// Valida trayecto para la sucursal dada y calcula duración en días del trayecto y el valor de la prima de seguro para clientes crédito
        /// </summary>
        /// <param name="municipioOrigen">Municipio de origen del trayecto</param>
        /// <param name="municipioDestino">Municipio de origen del trayecto</param>
        /// <param name="servicio">Servicio a validar</param>
        /// <param name="idListaPrecios">Identificador de la lista de precios</param>
        /// <returns></returns>
        ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestinoCliente(Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC municipioOrigen, Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC municipioDestino, TAServicioDC servicio, int idListaPrecios, int idServicio, int idSucursal);

        /// <summary>
        /// Retorna el número de dias del trayecto si existe
        /// </summary>
        /// <param name="municipioOrigen">Municipio destino</param>
        /// <param name="municipioDestino">Municipio de destino</param>
        /// <param name="servicio">Servicio a validar</param>
        /// <returns></returns>
        TATiempoDigitalizacionArchivo ValidarServicioTrayectoDestino(Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC municipioOrigen, Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC municipioDestino, TAServicioDC servicio);

        /// <summary>
        /// Retorna los tiempos para la digitalizacion y archivo de una guia despues de entregada
        /// </summary>
        /// <param name="idCiudadOrigen"></param>
        /// <param name="idCiudadDestino"></param>
        /// <returns></returns>
        TATiempoDigitalizacionArchivo ObtenerTiempoDigitalizacionArchivo(string idCiudadOrigen, string idCiudadDestino);

         /// <summary>
        /// Retorna Validacion si el Servicio-Origen-Destino, debe etiquetarse como AEREO en el campo del casillero de la Guia
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        bool ValidarServicioTrayectoCasilleroAereo(string municipioOrigen, string municipioDestino, int idServicio);


        /// <summary>
        /// Retorna la lista del horario de determinado centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        List<TAHorarioRecogidaCsvDC> ObtenerHorarioRecogidaDeCsv(long idCentroServicio);

        /// <summary>
        /// Retorna la lista de horario de determinada sucursal para cliente credito
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        List<TAHorarioRecogidaCsvDC> ObtenerHorarioRecogidaDeSucursal(int idSucursal);


        /// <summary>
        /// Retorna el precio del servicio
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Colección con precios</returns>
        TAPrecioCentroCorrespondenciaDC CalcularPrecioCentroCorrespondencia(int idListaPrecios);

        /// <summary>
        /// Calcula precio del servicio internacional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valor Precio</returns>
        TAPrecioServicioDC CalcularPrecioInternacional(int idListaPrecios, int tipoEmpaque, string idLocalidadDestino, decimal peso, string idZona, decimal valorDeclarado);

        /// <summary>
        /// Obtiene el precio del servicio trámites
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioTramiteDC CalcularPrecioTramites(int idListaPrecios, int idTramite);

        /// <summary>
        /// Obtiene el precio del servicio rapi promocional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioServicioDC CalcularPrecioRapiPromocional(int idListaPrecios, decimal cantidad, string idTipoEntrega = "-1");

        /// <summary>
        /// Obtiene el precio del servicio rapi personalizado
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioMensajeriaDC CalcularPrecioRapiPersonalizado(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");


        TAServicioPesoDC ObtenerServicioPeso(int idServicio);

        /// <summary>
        /// Obtiene el precio del servicio rapi hoy
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioMensajeriaDC CalcularPrecioRapiHoy(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// Obtiene el precio del servicio rapi envíos contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioMensajeriaDC CalcularPrecioRapiEnvioContraPago(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// Obtiene el precio del servicio rapi carga contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioCargaDC CalcularPrecioRapiCargaContraPago(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// Obtiene el precio del servicio rapi carga
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioCargaDC CalcularPrecioRapiCarga(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// Obtiene el precio del servicio rapi AM
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioMensajeriaDC CalcularPrecioRapiAm(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// Obtiene el precio del servicio notificaciones
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioMensajeriaDC CalcularPrecioNotificaciones(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        TAPrecioMensajeriaDC CalcularPrecioRapiTulas(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        TAPrecioMensajeriaDC CalcularPrecioRapiValoresMsj(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        TAPrecioMensajeriaDC CalcularPrecioRapiValoresCarga(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        TAPrecioMensajeriaDC CalcularPrecioRapiCargaConsolidado(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        TAPrecioMensajeriaDC CalcularPrecioRapiValijas(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");


        /// <summary>
        /// Obtiene el precio del servicio mensajeria
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioMensajeriaDC CalcularPrecioMensajeria(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1");


        /// <summary>
        /// Obtiene el precio del servicio mensajeria
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioMensajeriaDC CalcularPrecioMensajeriaCredito(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1");


        /// <summary>
        /// Obtiene el precio del servicio mensajeria
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioCargaDC CalcularPrecioCargaCredito(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1");


        /// <summary>
        /// Obtiene el precio del servicio rapi radicado
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <param name="idListaPrecio">Identificador de la lista de precio</param>
        /// <param name="idLocalidadOrigen">Identificador localidad de origen</param>
        /// <param name="idLocalidadDestino">Identificador localidad de destino</param>
        /// <param name="peso">Peso</param>
        /// <param name="valorDeclarado">Valor declarado</param>
        /// <returns>Precio</returns>
        TAPrecioMensajeriaDC CalcularPrecioRapiRadicado(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// Calcula precio
        /// </summary>
        /// <param name="idServicio">Identificador servicio</param>
        /// <param name="idListaPrecio">Identificador id lista de precio</param>
        /// <param name="idLocalidadOrigen">Identificador ciudad de origen</param>
        /// <param name="idLocalidadDestino">Identificador ciudad de destino</param>
        /// <param name="peso">Peso</param>
        /// <returns>Objeto valor</returns>
        TAPrecioMensajeriaDC CalcularPrecioCargaExpress(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// Obtiene los valores adicionales de un servicio
        /// </summary>
        /// <param name="idServicio">Identificador servicio</param>
        /// <returns>Colección de valores adicionales</returns>
        IEnumerable<TAValorAdicional> ObtenerValorValoresAdicionalesServicio(int idServicio);

        /// <summary>
        /// Obtener concepto de caja a partir del numero del servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        int ObtenerConceptoCaja(int idServicio);

        /// <summary>
        /// Metodo encargado de devolver el id de la lista de precios vigente
        /// </summary>
        /// <returns>int con el id de la lista de precio</returns>
        int ObtenerIdListaPrecioVigente();

        /// <summary>
        /// Obtiene el valor total,servicio,tarifas de un giro dirigido a un cliente contado a partir de un contrato
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        TAPrecioDC ObtenerValorGiroClienteContadoGiro(int idContrato, decimal valor);

        /// <summary>
        /// Indica si un trayecto determinado tiene o no una excepción de precios
        /// </summary>
        /// <param name="idListaPrecios"></param>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        bool TrayectoTieneExcepcion(int idListaPrecios, string idLocalidadOrigen, string idLocalidadDestino, int idServicio);


        /// <summary>
        ///  Obtiene el valor a cobrar a un servicio segun su  valor total.
        /// </summary>
        TAPrecioDC CalcularValorServicoAPartirValorTotal(int idContrato, decimal valorTotal);

        /// <summary>
        /// Obtener los impuestos de el servicio de giros
        /// </summary>
        /// <returns></returns>
        TAServicioImpuestosDC ObtenerImpuestosGiros();

        /// <summary>
        /// Obtiene los datos básicos del servicio
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <returns>Objeto Servicio</returns>
        TAServicioDC ObtenerDatosServicio(int idServicio);

        /// <summary>
        /// Obtiene de precio rango
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        IEnumerable<TAPrecioRangoDC> ObtenerPrecioRango(int idServicio);

        /// <summary>
        /// Obtejer Servicios vigentes
        /// </summary>
        /// <returns></returns>
        List<TAServicioDC> ObtenerServicios();

        /// <summary>
        /// Obtiene formas de pago segun servicios.
        /// </summary>
        /// <param name="lstServicios"></param>
        /// <returns></returns>
        TAFormaPagoServicio ObtenerFormasPagoPorServicios(int servicio);

        TAPrecioMensajeriaDC CalcularPrecioCargaAerea(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// Consulta los servicios con los pesos minimos y maximos 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        List<TAServicioPesoDC> ConsultarServiciosPesosMinimoxMaximos();

        /// <summary>
        /// Obtiene los servicios de rapicarga, Rapi Carga Terrestre y mensajeria por municipio origen y destino 
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <returns></returns>
        List<int> ObtenerListaServiciosParaMensajeriaExpresaMayorPeso(string municipioOrigen, string municipioDestino);
    }
}