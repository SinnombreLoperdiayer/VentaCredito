using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using CO.Servidor.Dominio.Comun.AdmEstadosConsolidado;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.OperacionNacional.Comun;
using CO.Servidor.OperacionNacional.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.OperacionNacional
{
    internal class ONManejadorIngresoManifiesto : ControllerBase
    {
        private static readonly ONManejadorIngresoManifiesto instancia = (ONManejadorIngresoManifiesto)FabricaInterceptores.GetProxy(new ONManejadorIngresoManifiesto(), COConstantesModulos.MODULO_OPERACION_NACIONAL);

        /// <summary>
        /// Retorna una instancia de ONManejadorIngresoRuta
        /// </summary>
        public static ONManejadorIngresoManifiesto Instancia
        {
            get { return ONManejadorIngresoManifiesto.instancia; }
        }

        /// <summary>
        /// Obtiene los manifietos donde la ciudad origen, destino o estacion de ruta sea la ciudad
        /// donde se esta haciendo el descargue
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<ONManifiestoOperacionNacional> ObtenerManifiestosXLocalidad(IDictionary<string, string> filtro, string idLocalidad, int indicePagina, int registrosPorPagina)
        {
            return ONRepositorio.Instancia.ObtenerManifiestosXLocalidad(filtro, idLocalidad, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Consulta el racol responsable de la agencia seleccionada
        /// </summary>
        /// <param name="idAgencia"></param>
        public ONIngresoOperativoDC ObtenerRacolAgenciaManifestada(ONIngresoOperativoDC ingreso)
        {
            PUAgenciaDeRacolDC racol = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerRacolResponsable(ingreso.IdAgencia);
            ingreso.ManifiestoIngreso.IdRacolDespacho = racol.IdResponsable;
            ingreso.ManifiestoIngreso.RacolDespacho = racol.IdResponsable + " - " + racol.NombreResponsable;
            return ingreso;
        }

        /// <summary>
        /// Consulta el detalle del manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        public ONIngresoOperativoDC ObtenerDetalleManifiesto(ONIngresoOperativoDC ingreso)
        {
            var ingr = ingreso;

            ///Obtiene el racol de la agencia para el ingreso
            ingreso = ObtenerRacolAgenciaManifestada(ingreso);

            ///Obtiene los datos del manifiesto
            ingreso = ONRepositorio.Instancia.ObtenerDetalleManifiesto(ingreso);

            ///Valida si el vehiculo del manifiesto ya realizo ingreso
            ONManejadorIngresoRuta.Instancia.ValidacionVehiculoIngreso(ingreso);

            GuardarIngresoAgenciaOperativo(ref ingreso);

            return ingreso;
        }

        /// <summary>
        /// Obtiene los envios de un manifiesto por numero de manifesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<ONConsolidado> ObtenerEnviosXManifiesto(long idManifiesto, int indicePagina, int registrosPorPagina)
        {
            return ONRepositorio.Instancia.ObtenerEnviosXManifiesto(idManifiesto, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Guarda el ingreso a la agencia del vehiculo del operativo
        /// </summary>
        /// <param name="ingresoOperativo"></param>
        /// <returns></returns>
        public void GuardarIngresoAgenciaOperativo(ref ONIngresoOperativoDC ingresoOperativo)
        {
            ONIngresoOperativoDC Operativo = null;
            if (ingresoOperativo.IdIngresoOperativo == 0)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    ///Consulta el ultimo ingreso del vehiculo a la agencia, si el ingreso esta abierto
                    ///retorna el id del ingreso, sino realiza el ingreso
                    Operativo = ONRepositorio.Instancia.ObtenerUltimoIngresoOperativoAgenciaManifiesto(ingresoOperativo.Vehiculo.IdVehiculo, ingresoOperativo.IdAgencia, ingresoOperativo.ManifiestoIngreso.IdManifiestoOperacionNacional);

                    if (Operativo == null)
                    {
                        ingresoOperativo.IdIngresoOperativo = ONRepositorio.Instancia.GuardarIngresoOperativoAgencia(ingresoOperativo, OPConstantesOperacionNacional.ID_TIPO_OPERATIVO_MANIFIESTO);
                    }
                    else
                    {
                        ingresoOperativo.IdIngresoOperativo = Operativo.IdIngresoOperativo;
                        ingresoOperativo.IngresoCerrado = Operativo.IngresoCerrado;
                    }
                    scope.Complete();
                }
            }
        }

        /// <summary>
        /// Retorna o asigan los consolidados del manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<ONConsolidado> ObtenerDetalleConsolidadosManifiesto(long idManifiesto, int indicePagina, int registrosPorPagina)
        {
            return ONRepositorio.Instancia.ObtenerDetalleConsolidadosManifiesto(idManifiesto, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Obtiene los envios de consolidados de un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public List<ONEnviosDescargueRutaDC> ObtenerEnviosConsolidadoXManifiesto(long idManifiesto)
        {
            return ONRepositorio.Instancia.ObtenerEnviosConsolidadoXManifiesto(idManifiesto);
        }

        /// <summary>
        /// Obtiene los envios sueltos de un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public List<ONEnviosDescargueRutaDC> ObtenerEnviosSueltosXManifiesto(long idManifiesto)
        {
            return ONRepositorio.Instancia.ObtenerEnviosSueltosXManifiesto(idManifiesto);
        }

        /// <summary>
        /// Obtiene los totales de manifiesto, total sobrantes, total faltantes
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <param name="idOperativo"></param>
        public ONCierreDescargueManifiestoDC ObtenerTotalCierreManifiesto(long idManifiesto, long idOperativo)
        {
            return ONRepositorio.Instancia.ObtenerTotalCierreManifiesto(idManifiesto, idOperativo);
        }
    }
}