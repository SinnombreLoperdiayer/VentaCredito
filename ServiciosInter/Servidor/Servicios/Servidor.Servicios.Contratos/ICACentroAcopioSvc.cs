using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.MensajeriaNN;
using CO.Servidor.Servicios.ContratoDatos.CentroAcopio;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface ICACentroAcopioSvc
    {
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CAAsignacionGuiaDC Asignar_a_ReclameOficina(CAAsignacionGuiaDC AsignacionGuia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAAsignacionGuiaDC> ConsultarGuias_EnCenAco_ParaREO(long IdCentroServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAManifiestoREO> ConsultarManifiestosREO(DateTime? Fecha, long IdManifiesto);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAAsignacionGuiaDC> ConsultarManifiestoREO_Guias(long IdManifiesto);


        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CAAsignacionGuiaDC Asignar_a_ConfirmacionesyDev(CAAsignacionGuiaDC AsignacionGuia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAAsignacionGuiaDC> ConsultarGuias_EnConfirmacionesyDev(long IdCentroServicio, string IdLocalidad);


        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CAAsignacionGuiaDC Asignar_a_Custodia(CAAsignacionGuiaDC AsignacionGuia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAAsignacionGuiaDC> ConsultarGuias_EnCustodia(long IdCentroServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CrearManifiesto_REO(long IdCSManif, long IdCSDesti, long IdVehiculo, long IdMensajero);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CAAsignacionGuiaDC CambiarTipoEntrega_REO(long numeroguia, long IdCSDestino);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool validarAsignacionInventario(long numeroGuia, long idCSAsigna);

        #region Envios NN
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long InsertarEnvioNN(ADEnvioNN envioNN);


        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADEnvioNN> ObtieneEnvioNN(AdEnvioNNFiltro envioNNFiltro);

        /// <summary>
        /// Relaciona Guia a envío NN
        /// </summary>
        /// <param name="idGuia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool AsignacionGuiaAEnvioNN(long numeroEnvioNN, long idGuia, string creadoPor);

        /// <summary>
        /// Obtiene la ruta de las imagenes del envío NN
        /// </summary>
        /// <param name="numeroEnvioNN"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RutasImagenesEnvioNN> ObtieneRutaImagenesEnvioNN(long numeroEnvioNN);


        /// <summary>
        /// obtiene clasificación de los envios nn
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ClasificacionEnvioNN> ObtieneClasificacionEnvioNN();
        
        #endregion


        #region Ingreso a Centro de Acopio Bodegas


        /// <summary>
        /// Método encargado de realizar la insercion del movimiento de un consolidado.
        /// </summary>
        /// <param name="movimientoConsolidado">Movimiento consolidado a insertar</param>                
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void InsertarMovimientoConsolidado(MovimientoConsolidado movimientoConsolidado);

        /// <summary>
        /// Método encargado de realizar la consulta y el retorno del ultimo movimiento de un consolidado.
        /// </summary>
        /// <param name="numeroConsolidado"></param>
        /// <param name="tipoConsolidado"></param>
        /// <returns>Retorna el ultimo movimiento vigente de un consolidado</returns>        
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        MovimientoConsolidado MovimientoConsolidadoVigente(string numeroConsolidado, CACEnumTipoConsolidado tipoConsolidado);

        /// <summary>
        /// Método encargado de realizar la consulta de los tipos de consolidado.
        /// </summary>
        /// <param name="numeroConsolidado"></param>
        /// <param name="tipoConsolidado"></param>
        /// <returns>Retorna una lista con los tipos de consolidado disponibles</returns>        
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<TipoConsolidado> ObtenerTipoConsolidado();

          /// <summary>
          /// Obtiene los reenvíos enviados desde LOI a Centro de Acopio
          /// </summary>
          /// <param name="idCentroServicio"></param>
          /// <param name="idCentroServicioOrigen"></param>
          /// <returns>Lista de los reenvíos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAAsignacionGuiaDC> ObtenerReenviosBodegas_CAC(long idCentroServicioOrigen, ADEnumEstadoGuia idEstado);
  
   
        /// <summary>
        /// Ingresar Guia a Centro de Acopio desde Logistica Inversa
        /// </summary>
        /// <param name="movInventario"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void IngresaraCentrodeAcopioValidandoEstado(PUMovimientoInventario movInventario, OUNovedadIngresoDC novedad, ADEnumEstadoGuia Estado);

        /// <summary>
        /// Obtiene las Guias que se Eliminan de la planilla desde Centro de Acopio por Envio Fuera de Zona
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAAsignacionGuiaDC> ObtenerGuiasEliminadasPlanillaCentroAcopio(string usuario);

        #endregion


         #region TulasyContenedores

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void InsertarConsolidado(CATipoConsolidado Consolidado);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CATipoConsolidado> ObtenerConsolidadosCSPropietario();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ModificarCentroServicioDestinoConsolidado(List<CATipoConsolidado> tipoConsolidado);
        

        #endregion
    }
}