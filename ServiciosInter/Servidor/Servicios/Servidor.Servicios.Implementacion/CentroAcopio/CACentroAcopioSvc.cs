using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;
using CO.Servidor.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

using CO.Servidor.CentroAcopio;
using CO.Servidor.Servicios.ContratoDatos.CentroAcopio;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.MensajeriaNN;


namespace CO.Servidor.Servicios.Implementacion.CentroAcopio
{
    /// <summary>
    /// Clase para los servicios de administración de Tarifas
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CACentroAcopioSvc : ICACentroAcopioSvc
    {
        public CACentroAcopioSvc()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        public CAAsignacionGuiaDC Asignar_a_ReclameOficina(CAAsignacionGuiaDC AsignacionGuia)
        {
            CAAsignacionGuiaDC rta = CACentroAcopio.Instancia.Asignar_a_ReclameOficina(AsignacionGuia);
            return rta;
        }
        public List<CAAsignacionGuiaDC> ConsultarGuias_EnCenAco_ParaREO(long IdCentroServicio)
        {
            return CACentroAcopio.Instancia.ConsultarGuias_EnCenAco_ParaREO(IdCentroServicio);
        }
        public List<CAManifiestoREO> ConsultarManifiestosREO(DateTime? Fecha, long IdManifiesto)
        {
            return CACentroAcopio.Instancia.ConsultarManifiestosREO(Fecha, IdManifiesto);
        }
        public List<CAAsignacionGuiaDC> ConsultarManifiestoREO_Guias(long IdManifiesto)
        {
            return CACentroAcopio.Instancia.ConsultarManifiestoREO_Guias(IdManifiesto);
        }


        public CAAsignacionGuiaDC Asignar_a_ConfirmacionesyDev(CAAsignacionGuiaDC AsignacionGuia)
        {
            return CACentroAcopio.Instancia.Asignar_a_ConfirmacionesyDev(AsignacionGuia);
        }
        public List<CAAsignacionGuiaDC> ConsultarGuias_EnConfirmacionesyDev(long IdCentroServicio, string IdLocalidad)
        {
            return CACentroAcopio.Instancia.ConsultarGuias_EnConfirmacionesyDev(IdCentroServicio, IdLocalidad);
        }


        public CAAsignacionGuiaDC Asignar_a_Custodia(CAAsignacionGuiaDC AsignacionGuia)
        {
            return CACentroAcopio.Instancia.Asignar_a_Custodia(AsignacionGuia);
        }

        public List<CAAsignacionGuiaDC> ConsultarGuias_EnCustodia(long IdCentroServicio)
        {
            return CACentroAcopio.Instancia.ConsultarGuias_EnCustodia(IdCentroServicio);
        }



        public CAAsignacionGuiaDC CambiarTipoEntrega_REO(long NumeroGuia, long IdCSDestino)
        {
            return CACentroAcopio.Instancia.CambiarTipoEntrega_REO(NumeroGuia, IdCSDestino);
        }

        public void CrearManifiesto_REO(long IdCSManif, long IdCSDesti, long IdVehiculo, long IdMensajero)
        {
            CACentroAcopio.Instancia.CrearManifiesto_REO(IdCSManif, IdCSDesti, IdVehiculo, IdMensajero);
        }

        public bool validarAsignacionInventario(long numeroGuia, long idCSAsigna)
        {
            return CACentroAcopio.Instancia.validarAsignacionMovimientoInventario(numeroGuia, idCSAsigna);
        }
        #region Envios NN
        public long InsertarEnvioNN(ADEnvioNN envioNN)
        {
           return CACentroAcopio.Instancia.InsertarEnvioNN(envioNN);
        }

        public List<ADEnvioNN> ObtieneEnvioNN(AdEnvioNNFiltro envioNNFiltro) 
        {
            return CACentroAcopio.Instancia.ObtieneEnvioNN(envioNNFiltro);
        }

        public bool AsignacionGuiaAEnvioNN(long numeroEnvioNN, long idGuia, string creadoPor)
        {
            return CACentroAcopio.Instancia.AsignacionGuiaAEnvioNN(numeroEnvioNN, idGuia, creadoPor);
        }


        public List<RutasImagenesEnvioNN> ObtieneRutaImagenesEnvioNN(long numeroEnvioNN)
        {
           return CACentroAcopio.Instancia.ObtieneRutaImagenesEnvioNN(numeroEnvioNN);
        }

        public List<ClasificacionEnvioNN> ObtieneClasificacionEnvioNN()
        {
            return CACentroAcopio.Instancia.ObtieneClasificacionEnvioNN();
        }
        #endregion


        
        #region Ingreso a Centro de Acopio Bodegas

        /// <summary>
        /// Obtener Reenvíos enviados desde LOI a Centro de Acopio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="idCentroServicioOrigen"></param>
        /// <returns></returns>
        public List<CAAsignacionGuiaDC> ObtenerReenviosBodegas_CAC(long idCentroServicioOrigen, ADEnumEstadoGuia idEstado)
        {
            return CACentroAcopio.Instancia.ObtenerReenviosBodegas_CAC(idCentroServicioOrigen, idEstado);
        }
        

        /// <summary>
        /// Ingresar Guia a Centro de Acopio desde Logistica Inversa
        /// </summary>
        /// <param name="movInventario"></param>
        public void IngresaraCentrodeAcopioValidandoEstado(PUMovimientoInventario movInventario, OUNovedadIngresoDC novedad, ADEnumEstadoGuia Estado)
        {
            CACentroAcopio.Instancia.IngresaraCentrodeAcopioValidandoEstado(movInventario,novedad,Estado);
        }

        /// <summary>
        /// Obtiene las Guias que se Eliminan de la planilla desde Centro de Acopio por Envio Fuera de Zona
        /// </summary>
        public List<CAAsignacionGuiaDC> ObtenerGuiasEliminadasPlanillaCentroAcopio(string usuario)
        {
            return CACentroAcopio.Instancia.ObtenerGuiasEliminadasPlanillaCentroAcopio(usuario);
        }

        #endregion


        #region TulasYContenedores

        /// <summary>
        /// Método para crear el consolidado Tula o Contenedor
        /// </summary>
        public void InsertarConsolidado(CATipoConsolidado Consolidado)
        {
            CATulasContenedores.Instancia.InsertarConsolidado(Consolidado);
        }

        /// <summary>
        /// Consulta las Tulas y Contenedores de un Centro de Servicio
        /// </summary>
        /// <returns></returns>
        public List<CATipoConsolidado> ObtenerConsolidadosCSPropietario()
        {
            return CATulasContenedores.Instancia.ObtenerConsolidadosCSPropietario();
        }


        /// <summary>
        /// Actualiza el Centro de Servicio Destino del Contenedor o Tula
        /// </summary>
        public void ModificarCentroServicioDestinoConsolidado(List<CATipoConsolidado> listaTipoConsolidado)
        {
            CATulasContenedores.Instancia.ModificarCentroServicioDestinoConsolidado(listaTipoConsolidado);
        }
        #endregion

                
        public MovimientoConsolidado MovimientoConsolidadoVigente(string numeroConsolidado, CACEnumTipoConsolidado tipoConsolidado)
        {
            return CACentroAcopio.Instancia.MovimientoConsolidadoVigente(numeroConsolidado, tipoConsolidado);
        }

        public List<TipoConsolidado> ObtenerTipoConsolidado()
        {
            return CACentroAcopio.Instancia.ObtenerTipoConsolidado();
        }

        public void InsertarMovimientoConsolidado(MovimientoConsolidado movimientoConsolidado)
        {
            CACentroAcopio.Instancia.InsertarMovimientoConsolidado(movimientoConsolidado);
        }

        
    }
}