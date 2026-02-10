using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;

namespace CO.Servidor.Dominio.Comun.Suministros
{
  /// <summary>
  /// Interfaz para el acceso a la lógica de suministros
  /// </summary>
    public interface ISUFachadaSuministros
    {
        /// <summary>
        /// Consulta la agencia a la cual se le suministro la factura de venta con el numero de giro IdGiro
        /// </summary>
        /// <param name="IdGiro">Numero del giro</param>
        /// <returns>Centro de servicio</returns>
        PUCentroServiciosDC ConsultarAgenciaPropietariaDelNumeroGiro(long idGiro);

        /// <summary>
        /// Retorna los suministros asignados a un centro de servicio
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        IEnumerable<SUSuministro> ObtenerSuministrosCentroServicio(PUCentroServiciosDC centroServicio);

        /// <summary>
        /// Retorna el consecutivo dle suministro dado
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <returns></returns>
        long ObtenerConsecutivoSuministro(SUEnumSuministro idSuministro);

        /// <summary>
        /// Obtiene el numero de giro prefijo + valorActual
        /// </summary>
        /// <param name="idSuministro">id del suministro</param>
        /// <returns>numero del giro</returns>
        SUNumeradorPrefijo ObtenerNumeroPrefijoValor(SUEnumSuministro idSuministro);


               /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        SUPropietarioGuia ObtenerPropietarioGuia(long numeroSuministro);

        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        SUPropietarioGuia ObtenerPropietarioSuministro(long numeroGuia, SUEnumSuministro idSuministro, long idPropietario = 0);

        /// <summary>
        /// Consulta la agencia a la cual se le suministro es el comprobante de pago manual con el numero de
        /// comprobante de pago
        /// </summary>
        /// <param name="idComprobantePago" >Comprobante de pago</param>
        PUCentroServiciosDC ConsultarAgenciaPropietariaDelNumeroComprobante(long idComprobantePago);

        /// <summary>
        /// Almacena el consumo de un suministro en la base de datos
        /// </summary>
        /// <param name="consumoSuministro"></param>
        void GuardarConsumoSuministro(SUConsumoSuministroDC consumoSuministro);

        /// <summary>
        /// Almacena el consumo de un suministro en la base de datos
        /// </summary>
        /// <param name="consumoSuministro"></param>
        /// <param name="datosCentroServicio"></param>
        /// <param name="datosServicio"></param>
        /// /// <param name="conexion"> conexion principal</param>
        /// /// <param name="transaccion">transaccion principal</param>
        void GuardarConsumoSuministro(SUConsumoSuministroDC consumoSuministro, System.Data.SqlClient.SqlConnection conexion, System.Data.SqlClient.SqlTransaction transaccion);

        /// <summary>
        /// Almacena el traslado de un suministro entre un origen y un destino
        /// </summary>
        /// <param name="trasladoSuministro"></param>
        void GuardarTrasladoSuministro(SUTrasladoSuministroDC trasladoSuministro);

        /// <summary>
        /// Reasigna los suministros de una agencia a otra
        /// </summary>
        /// <param name="anteriorAgencia"></param>
        /// <param name="nuevaAgencia"></param>
        void ModificarSuministroAgencia(long anteriorAgencia, long nuevaAgencia);

        /// <summary>
        /// Guardar los suministros que posee un centro de servicio
        /// </summary>
        /// <param name="suministroCentroServicio"></param>
        void GuardarSuministroCentroServicio(SUSuministroCentroServicioDC suministroCentroServicio);

        /// <summary>
        /// Obtiene los tipos de  suministros existentes
        /// </summary>
        /// <returns></returns>
        IEnumerable<SUSuministro> ObtenerTiposSuministros();

        /// <summary>
        /// Consulta el suministro asociado a un prefijo especifico
        /// </summary>
        /// <param name="prefijo"></param>
        /// <returns></returns>
        SUSuministro ConsultarSuministroxPrefijo(string prefijo);

        /// <summary>
        /// Obtiene los grupos de suministros configurados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        List<SUGrupoSuministrosDC> ObtenerGrupoSuministrosConSuminGrupo(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros);

        /// <summary>
        /// Obtiene los grupos de suministros configurados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        List<SUGrupoSuministrosDC> ObtenerGrupoSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros);

        /// <summary>
        /// Obtiene los suministros de una sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        List<SUSuministroSucursalDC> ObtenerSuministrosSucursal(int idSucursal);

        /// <summary>
        /// agrega o modifica un suministro de una sucursal
        /// </summary>
        /// <param name="sumSuc"></param>
        void AgregarModificarSuministroSucursal(List<SUSuministroSucursalDC> sumSuc);

        /// <summary>
        /// Almacena el consumo de la bolsa de seguridad 
        /// </summary>
        /// <param name="numeroBolsaSeguridad">Numero de la bolsa de seguridad, puede contener con su prefijo</param>
        /// <param name="idServiciosAsociado">Identificador del servicio asocial</param>
        void GuardarConsumoBolsaSeguridad(string numeroBolsaSeguridad, int idServiciosAsociado, long idPropietario);



        /// <summary>
        /// Método para validar el dueño de un contenedor
        /// </summary>
        /// <param name="asignacion"></param>
        void ValidarContenedor(OUAsignacionDC asignacion);

        /// <summary>
        /// Valida que un consolidado activo y que pertenezca a una ciudad a una ciudad
        /// </summary>
        /// <param name="codigoConsolidado"></param>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        bool ValidarConsolidadoActivoCiudadAsignacion(string codigoConsolidado, string idCiudad);


        /// <summary>
        /// Valida que un consolidado activo y que pertenezca a una ciudad a una ciudad
        /// </summary>
        /// <param name="codigoConsolidado"></param>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        List<SUConsolidadoDC> ObtenerInventarioConsolidado(string codigoConsolidado);

        /// <summary>
        /// Actualizar el valor del número actual de un suministro específico
        /// </summary>
        /// <param name="tipoSuministro"></param>
        /// <param name="numeroActual"></param>
        void ActualizarNumeroActualSuministro(SUEnumSuministro tipoSuministro, long numeroActual);

        /// <summary>
        /// Genera la remision de los suministros para el canal de ventas
        /// </summary>
        /// <param name="remision"></param>
        SURemisionSuministroDC GenerarRangoGuiaManualOffline(long idCentroServicio, int cantidad);

        /// <summary>
        /// Obtiene el responsable de un suministro
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        SUDatosResponsableSuministroDC ObtenerResponsableSuministro(long numeroGuia);
    }
}