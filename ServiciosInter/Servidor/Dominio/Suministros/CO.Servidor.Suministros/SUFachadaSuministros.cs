using System.Collections.Generic;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Consumo;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;

namespace CO.Servidor.Suministros
{
    public class SUFachadaSuministros : ISUFachadaSuministros
    {
        /// <summary>
        /// Consulta la agencia a la cual se le suministro la factura de venta con el numero de giro IdGiro
        /// </summary>
        /// <param name="IdGiro">Numero del giro</param>
        /// <returns>Centro de servicio</returns>
        public PUCentroServiciosDC ConsultarAgenciaPropietariaDelNumeroGiro(long idGiro)
        {
            return SUSuministros.Instancia.ConsultarAgenciaPropietariaDelNumeroGiro(idGiro);
        }

        /// <summary>
        /// Retorna los suministros asignados a un centro de servicio
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        public IEnumerable<SUSuministro> ObtenerSuministrosCentroServicio(PUCentroServiciosDC centroServicio)
        {
            return SUSuministros.Instancia.ObtenerSuministrosCentroServicio(centroServicio);
        }

        /// <summary>
        /// Retorna el consecutivo dle suministro dado
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <returns></returns>
        public long ObtenerConsecutivoSuministro(SUEnumSuministro idSuministro)
        {
            return SUSuministros.Instancia.ObtenerConsecutivoSuministro(idSuministro);
        }

        /// <summary>
        /// Obtiene el numero prefijo + valorActual
        /// </summary>
        /// <param name="idSuministro">id del suministro</param>
        /// <returns>numero del giro</returns>
        public SUNumeradorPrefijo ObtenerNumeroPrefijoValor(SUEnumSuministro idSuministro)
        {
            return SUSuministros.Instancia.ObtenerNumeroPrefijoValor(idSuministro);
        }

        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioGuia(long numeroSuministro)
        {
            return SUSuministros.Instancia.ObtenerPropietarioGuia(numeroSuministro);
        }

        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioSuministro(long numeroGuia, SUEnumSuministro idSuministro, long idPropietario = 0)
        {
            return SUSuministros.Instancia.ObtenerPropietarioSuministro(numeroGuia, idSuministro, idPropietario);
        }

        /// <summary>
        /// Consulta la agencia a la cual se le suministro es el comprobante de pago manual con el numero de
        /// comprobante de pago
        /// </summary>
        /// <param name="idComprobantePago" >Comprobante de pago</param>
        public PUCentroServiciosDC ConsultarAgenciaPropietariaDelNumeroComprobante(long idComprobantePago)
        {
            return SUSuministros.Instancia.ConsultarAgenciaPropietariaDelNumeroComprobante(idComprobantePago);
        }

        /// <summary>
        /// Almacena el consumo de un suministro en la base de datos
        /// </summary>
        /// <param name="consumoSuministro"></param>
        public void GuardarConsumoSuministro(SUConsumoSuministroDC consumoSuministro)
        {
            SUSuministros.Instancia.GuardarConsumoSuministro(consumoSuministro);
        }

        /// <summary>
        /// Almacena el consumo de un suministro en la base de datos
        /// </summary>
        /// <param name="consumoSuministro"></param>
        /// <param name="datosCentroServicio"></param>
        /// <param name="datosServicio"></param>
        /// /// <param name="conexion"> conexion principal</param>
        /// /// <param name="transaccion">transaccion principal</param>
        public void GuardarConsumoSuministro(SUConsumoSuministroDC consumoSuministro, System.Data.SqlClient.SqlConnection conexion, System.Data.SqlClient.SqlTransaction transaccion)
        {
            SUSuministros.Instancia.GuardarConsumoSuministro(consumoSuministro, conexion, transaccion);
        }

        /// <summary>
        /// Almacena el traslado de un suministro entre un origen y un destino
        /// </summary>
        /// <param name="trasladoSuministro"></param>
        public void GuardarTrasladoSuministro(SUTrasladoSuministroDC trasladoSuministro)
        {
            SUSuministros.Instancia.GuardarTrasladoSuministro(trasladoSuministro);
        }

        /// <summary>
        /// Reasigna los suministros de una agencia a otra
        /// </summary>
        /// <param name="anteriorAgencia"></param>
        /// <param name="nuevaAgencia"></param>
        public void ModificarSuministroAgencia(long anteriorAgencia, long nuevaAgencia)
        {
            SUSuministros.Instancia.ModificarSuministroAgencia(anteriorAgencia, nuevaAgencia);
        }

        /// <summary>
        /// Guardar los suministros que posee un centro de servicio
        /// </summary>
        /// <param name="suministroCentroServicio"></param>
        public void GuardarSuministroCentroServicio(SUSuministroCentroServicioDC suministroCentroServicio)
        {
            SUAdministradorSuministros.Instancia.GuardarSuministroCentroServicio(suministroCentroServicio);
        }

        /// <summary>
        /// Obtiene los tipos de  suministros existentes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SUSuministro> ObtenerTiposSuministros()
        {
            return SUSuministros.Instancia.ObtenerTiposSuministros();
        }

        /// <summary>
        /// Consulta el suministro asociado a un prefijo especifico
        /// </summary>
        /// <param name="prefijo"></param>
        /// <returns></returns>
        public SUSuministro ConsultarSuministroxPrefijo(string prefijo)
        {
            return SUSuministros.Instancia.ConsultarSuministroxPrefijo(prefijo);
        }

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
        public List<SUGrupoSuministrosDC> ObtenerGrupoSuministrosConSuminGrupo(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return SUAdministradorSuministros.Instancia.ObtenerGrupoSuministrosConSuminGrupo(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

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
        public List<SUGrupoSuministrosDC> ObtenerGrupoSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return SUAdministradorSuministros.Instancia.ObtenerGrupoSuministros(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Obtiene los suministros de una sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public List<SUSuministroSucursalDC> ObtenerSuministrosSucursal(int idSucursal)
        {
            return SUAdministradorSuministros.Instancia.ObtenerSuministrosSucursal(idSucursal);
        }

        /// <summary>
        /// agrega o modifica un suministro de una sucursal
        /// </summary>
        /// <param name="sumSuc"></param>
        public void AgregarModificarSuministroSucursal(List<SUSuministroSucursalDC> sumSuc)
        {
            SUAdministradorSuministros.Instancia.AgregarModificarSuministroSucursal(sumSuc);
        }

        /// <summary>
        /// Almacena el consumo de la bolsa de seguridad 
        /// </summary>
        /// <param name="numeroBolsaSeguridad">Numero de la bolsa de seguridad, puede contener con su prefijo</param>
        /// <param name="grupoConsume">Grupo al cual pertener quien consume la bolsa de seguridad</param>
        /// <param name="idServiciosAsociado">Identificador del servicio asocial</param>
        public void GuardarConsumoBolsaSeguridad(string numeroBolsaSeguridad, int idServiciosAsociado, long idPropietario)
        {
            SUConsumo.Instancia.GuardarConsumoBolsaSeguridad(numeroBolsaSeguridad, idServiciosAsociado, idPropietario);
        }


        /// <summary>
        /// Método para validar el dueño de un contenedor
        /// </summary>
        /// <param name="asignacion"></param>
        public void ValidarContenedor(OUAsignacionDC asignacion)
        {
            SUSuministros.Instancia.ValidarContenedor(asignacion);
        }

        /// <summary>
        /// Valida que un consolidado activo y que pertenezca a una ciudad a una ciudad
        /// </summary>
        /// <param name="codigoConsolidado"></param>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        public bool ValidarConsolidadoActivoCiudadAsignacion(string codigoConsolidado, string idCiudad)
        {
           return SUSuministros.Instancia.ValidarConsolidadoActivoCiudadAsignacion(codigoConsolidado, idCiudad);
        }

        /// <summary>
        /// Valida que un consolidado activo y que pertenezca a una ciudad a una ciudad
        /// </summary>
        /// <param name="codigoConsolidado"></param>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        public List<SUConsolidadoDC> ObtenerInventarioConsolidado(string codigoConsolidado)
        {
            return SUSuministros.Instancia.ObtenerInventarioConsolidado(codigoConsolidado);
        }

        /// <summary>
        /// Actualizar el valor del número actual de un suministro específico
        /// </summary>
        /// <param name="tipoSuministro"></param>
        /// <param name="numeroActual"></param>
        public void ActualizarNumeroActualSuministro(SUEnumSuministro tipoSuministro, long numeroActual)
        {
            SUSuministros.Instancia.ActualizarNumeroActualSuministro(tipoSuministro, numeroActual);
        }

        /// <summary>
        /// Genera la remision de los suministros para el canal de ventas
        /// </summary>
        /// <param name="remision"></param>
        public SURemisionSuministroDC GenerarRangoGuiaManualOffline(long idCentroServicio, int cantidad)
        {
            return SUAdministradorRemisiones.Instancia.GenerarRangoGuiaManualOffline(idCentroServicio, cantidad);
        }

        /// <summary>
        /// Obtiene el responsable de un suministro
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public SUDatosResponsableSuministroDC ObtenerResponsableSuministro(long numeroGuia)
        {
            return SUAdministradorSuministros.Instancia.ObtenerResponsableSuministro(numeroGuia);
        }
    }
}