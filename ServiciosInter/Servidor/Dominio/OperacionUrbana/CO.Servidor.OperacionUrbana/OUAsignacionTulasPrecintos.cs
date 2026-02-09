using System.Collections.Generic;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.OperacionUrbana.Datos;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using CO.Servidor.Dominio.Comun.AdmEstadosConsolidado;
using System.ServiceModel;
using CO.Servidor.OperacionUrbana.Comun;

namespace CO.Servidor.OperacionUrbana
{
    internal class OUAsignacionTulasPrecintos: ControllerBase
    {
        private static readonly OUAsignacionTulasPrecintos instancia = (OUAsignacionTulasPrecintos)FabricaInterceptores.GetProxy(new OUAsignacionTulasPrecintos(), COConstantesModulos.MODULO_OPERACION_URBANA);

        /// <summary>
        /// Retorna una instancia de OUAsignacionTulasPrecintos
        /// /// </summary>
        public static OUAsignacionTulasPrecintos Instancia
        {
            get { return OUAsignacionTulasPrecintos.instancia; }
        }

        /// <summary>
        /// Método para obtener los tipos de asignación posibles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OUTipoAsignacionDC> ObtenerTiposAsignacion()
        {
            return OURepositorio.Instancia.ObtenerTiposAsignacion();
        }

        /// <summary>
        /// Método para obtener las tulas y precintos sin utilizar generadas desde una racol
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public IEnumerable<OUAsignacionDC> ObtenerAsignacionCentroServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return OURepositorio.Instancia.ObtenerAsignacionCentroServicio (filtro,  campoOrdenamiento,  indicePagina,  registrosPorPagina,  ordenamientoAscendente, out  totalRegistros);       
        }

        /// <summary>
        /// Método para asignar una tula y un precinto a un centro de servicio
        /// </summary>
        /// <param name="asignacionTula"></param>
        /// <returns></returns>
        public OUAsignacionDC AdicionarAsignacionCentroServicio(OUAsignacionDC asignacion)
        {
         using (TransactionScope transaccion = new TransactionScope())
         {
             //Validar que el contenedor este aprovisionado
             EnumEstadosConsolidados estadoActual = ECAdminEstadosConsolidado.ObtenerUltimoEstadoConsolidado(asignacion.NoTula);

             if ( estadoActual== EnumEstadosConsolidados.DES || estadoActual == EnumEstadosConsolidados.CRE)
             {
                 ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
                 fachadaSuministros.ValidarContenedor(asignacion);
                 OURepositorio.Instancia.ValidarTulaContenedor(asignacion);
                 asignacion.NumContTransDespacho = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ObtenerConsecutivo(Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos.PAEnumConsecutivos.Control_Transporte_Manifiesto_Urbano_Despacho);
                 if (!asignacion.CentroServicioDestino.Sistematizado)
                     asignacion.NumContTransRetorno = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ObtenerConsecutivo(Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos.PAEnumConsecutivos.Control_Transporte_Manifiesto_Urbano_Retorno);
                 OUAsignacionDC asignacionRtn;
                 asignacionRtn = OURepositorio.Instancia.AdicionarAsignacionCentroServicio(asignacion);
                 ECAdminEstadosConsolidado.GuardarEstadoConsolidado(new ECEstadoConsolidado { NoTula = asignacion.NoTula, Estado = EnumEstadosConsolidados.ASU, IdCentroServicios = asignacion.CentroServicioOrigen.IdCentroServicio, Observaciones = string.Empty });
                 transaccion.Complete();
                 return asignacionRtn;
             }
             else
                 throw new FaultException<ControllerException>
                     (new ControllerException
                         (COConstantesModulos.MODULO_OPERACION_URBANA,
                         OUEnumTipoErrorOU.EX_ERROR_ESTADO_CONSOLIDADO.ToString(),
                         OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_ERROR_ESTADO_CONSOLIDADO)));
   

         }
        }



        /// <summary>
        /// Método para eliminar una asignacion de tulas o contenedores
        /// </summary>
        /// <param name="asignacion"></param>
        public void EliminarAsignacionTulaContenedor(OUAsignacionDC asignacion)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                OURepositorio.Instancia.EliminarAsignacionTulaContenedor(asignacion);
                transaccion.Complete();
            }
        }


    }
}
