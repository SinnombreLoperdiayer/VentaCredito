using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.WebApi.Comun;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiSuministros : ApiDominioBase
    {
        private static readonly ApiSuministros instancia = (ApiSuministros)FabricaInterceptorApi.GetProxy(new ApiSuministros(), COConstantesModulos.MODULO_SUMINISTROS);

        public static ApiSuministros Instancia
        {
            get { return ApiSuministros.instancia; }
        }

        private ApiSuministros()
        {
        }


        /// <summary>
        /// Obtiene los suministros para las guias manuales offline
        /// </summary>
        /// <param name="IdCentrosServicio"></param>
        /// <param name="cantidadGuiasSolicitar"></param>
        /// <returns></returns>
        public List<long> GenerarRangoGuiaManualOffline(long IdCentrosServicio, int cantidadGuiasSolicitar)
        {
            SURemisionSuministroDC remision = FabricaServicios.ServicioSuministros.GenerarRangoGuiaManualOffline(IdCentrosServicio, cantidadGuiasSolicitar);


            List<long> guias = new List<long>();


            for (long i = remision.GrupoSuministros.SuministroGrupo.RangoInicial; i <= remision.GrupoSuministros.SuministroGrupo.RangoFinal; i++)
            {
                guias.Add(i);

            }

            return guias;
        }

        /// <summary>
        /// Metodo para obtener suministros disponibles por mensajero
        /// </summary>
        /// <param name="IdMensajero"></param>
        /// <param name="IdSuministro"></param>
        /// <returns></returns>
        public List<long> GenerarSuministrosDisponiblesMensajero(long IdMensajero, long IdSuministro)
        {
            return FabricaServicios.ServicioSuministros.GenerarSuministrosDisponiblesMensajero(IdMensajero, IdSuministro);

        }

        #region Giros

        /// <summary>
        /// Retorna el consecutivo del suministro dado
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <returns></returns>
        public long ObtenerConsecutivoSuministro(SUEnumSuministro idSuministro)
        {
            return FabricaServicios.ServicioSuministros.ObtenerConsecutivoSuministro(idSuministro); 

        }        

        /// <summary>
        /// Almacena el consumo de un suministro en la base de datos
        /// </summary>
        /// <param name="consumoSuministro"></param>
        public void GuardarConsumoSuministro(SUConsumoSuministroDC consumoSuministro)
        {
            FabricaServicios.ServicioSuministros.GuardarConsumoSuministro(consumoSuministro);

        }

        /// <summary>
        /// Retorna los suministros asignados a un centro de servicio
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        public IEnumerable<SUSuministro> ObtenerSuministrosCentroServicio(PUCentroServiciosDC centroServicio)
        {
            return FabricaServicios.ServicioSuministros.ObtenerSuministrosCentroServicio(centroServicio);
        }

        /// <summary>
        /// Obtiene el numero del suministro segun el tipo
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <returns></returns>
        public SUNumeradorPrefijo ObtenerNumeroSuministroActual(SUEnumSuministro idSuministro)
        {
            SUNumeradorPrefijo suministro = new SUNumeradorPrefijo();
            using (TransactionScope scope = new TransactionScope())
            {
                suministro = FabricaServicios.ServicioSuministros.ObtenerNumeroSuministroActual(idSuministro);
                scope.Complete();
            }
            return suministro;
            
        }

        /// <summary>
        /// Obtiene el propietario segun el numero del suministro
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioSuministro(long numeroGuia, SUEnumSuministro idSuministro)
        {
            return FabricaServicios.ServicioSuministros.ObtenerPropietarioSuministro(numeroGuia,idSuministro,0);
        }


        #endregion
    }
}
