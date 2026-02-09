using CO.Servidor.Servicios.Implementacion.Admisiones.Mensajeria;
using CO.Servidor.Servicios.Implementacion.Cajas;
using CO.Servidor.Servicios.Implementacion.CentroAcopio;
using CO.Servidor.Servicios.Implementacion.CentroServicios;
using CO.Servidor.Servicios.Implementacion.Clientes;
using CO.Servidor.Servicios.Implementacion.ControlCuentas;
using CO.Servidor.Servicios.Implementacion.GestionGiros; // creo q esto va aki, preguntar (cometario temporal)
using CO.Servidor.Servicios.Implementacion.LogisticaInversa;
using CO.Servidor.Servicios.Implementacion.OperacionNacional;
using CO.Servidor.Servicios.Implementacion.OperacionUrbana;
using CO.Servidor.Servicios.Implementacion.ParametrosOperacion;
using CO.Servidor.Servicios.Implementacion.Raps;
using CO.Servidor.Servicios.Implementacion.Recogidas;
using CO.Servidor.Servicios.Implementacion.Rutas;
using CO.Servidor.Servicios.Implementacion.Suministros;
using CO.Servidor.Servicios.Implementacion.Tarifas;
using CO.Servidor.Servicios.Implementacion.Mensajero;
using Framework.Servidor.Servicios.Implementacion;
using Framework.Servidor.Servicios.Implementacion.Reportes;
using CO.Servidor.Servicios.Implementacion.Integraciones;

namespace CO.Servidor.Servicios.WebApi.Comun
{
    public class FabricaServicios
    {
        private static readonly PAParametrosFrameworkSvc servicioParametros = new PAParametrosFrameworkSvc();

        public static PAParametrosFrameworkSvc ServicioParametros
        {
            get { return FabricaServicios.servicioParametros; }
        }

        private static readonly OUOperacionUrbanaSvc servicioOperacionUrbana = new OUOperacionUrbanaSvc();

        public static OUOperacionUrbanaSvc ServicioOperacionUrbana
        {
            get { return FabricaServicios.servicioOperacionUrbana; }
        }


        private static readonly Framework.Servidor.Servicios.Implementacion.SEAutenticacionSvc serviciosAutenticacion = new Framework.Servidor.Servicios.Implementacion.SEAutenticacionSvc();

        public static Framework.Servidor.Servicios.Implementacion.SEAutenticacionSvc ServiciosAutenticacion
        {
            get { return FabricaServicios.serviciosAutenticacion; }
        }

        private static readonly RAConfiguracionRapsSvc servicioConfiguracionRaps = new RAConfiguracionRapsSvc();

        public static RAConfiguracionRapsSvc ServicioConfiguracionRaps
        {
            get { return FabricaServicios.servicioConfiguracionRaps; }
        }


        private static readonly RASolicitudesRapsSvc servicioSolicitudesRaps = new RASolicitudesRapsSvc();

        public static RASolicitudesRapsSvc ServicioSolicitudesRaps
        {
            get { return FabricaServicios.servicioSolicitudesRaps; }
        }

        private static readonly RASolicitudRapsSvc servicioSolicitudRaps = new RASolicitudRapsSvc();

        public static RASolicitudRapsSvc ServicioSolicitudRaps
        {
            get { return FabricaServicios.servicioSolicitudRaps; }
        }

        private static readonly RAIntegracionesRapsSvc servicioIntegracionesRaps = new RAIntegracionesRapsSvc();

        public static RAIntegracionesRapsSvc ServicioIntegracionesRaps
        {
            get { return FabricaServicios.servicioIntegracionesRaps; }
        }

        private static readonly ADAdmisionesMensajeriaSvc servicioMensajeria = new ADAdmisionesMensajeriaSvc();

        public static ADAdmisionesMensajeriaSvc ServicioMensajeria
        {
            get { return FabricaServicios.servicioMensajeria; }
        }

        private static readonly POParametrosOperacionSvc servicioParametrosOperacion = new POParametrosOperacionSvc();

        public static POParametrosOperacionSvc ServicioParametrosOperacion
        {
            get { return FabricaServicios.servicioParametrosOperacion; }
        }

        private static readonly PUCentroServiciosSvc servicioCentroServicios = new PUCentroServiciosSvc();

        public static PUCentroServiciosSvc ServicioCentroServicios
        {
            get { return servicioCentroServicios; }
        }

        private static readonly TATarifasSvc servicioTarifas = new TATarifasSvc();

        public static TATarifasSvc ServicioTarifas
        {
            get { return FabricaServicios.servicioTarifas; }
        }

        private static readonly CLClientesSvc servicioAdministracionClientes = new CLClientesSvc();

        public static CLClientesSvc ServicioAdministracionClientes
        {
            get { return FabricaServicios.servicioAdministracionClientes; }
        }


        private static readonly LILogisticaInversaSvc servicioLogisticaInversa = new LILogisticaInversaSvc();

        public static LILogisticaInversaSvc ServicioLogisticaInversa
        {
            get { return FabricaServicios.servicioLogisticaInversa; }
        }

        private static readonly IntegracionesControllerSvc servicioIntegraciones = new IntegracionesControllerSvc();

        public static IntegracionesControllerSvc ServicioIntegraciones
        {
            get { return FabricaServicios.servicioIntegraciones; }
        }

        private static readonly SUSuministrosSvc servicioSuministros = new SUSuministrosSvc();

        public static SUSuministrosSvc ServicioSuministros
        {
            get { return FabricaServicios.servicioSuministros; }
        }

        private static readonly ONOperacionNacionalSvc servicioOperacionNacional = new ONOperacionNacionalSvc();
        public static ONOperacionNacionalSvc ServicioOperacionNacional
        {
            get { return FabricaServicios.servicioOperacionNacional; }
        }

        private static readonly GIGestionGirosSvc servicioGiros = new GIGestionGirosSvc();
        public static GIGestionGirosSvc ServicioGiros
        {
            get { return FabricaServicios.servicioGiros; }
        }
        private static readonly RURutasSvc rutasCWeb = new RURutasSvc();

        public static RURutasSvc RutasCWeb
        {
            get { return FabricaServicios.rutasCWeb; }
        }


        private static readonly LILogisticaInversaTelemercadeoSvc servicioLogisticaInversaTelemercadeo = new LILogisticaInversaTelemercadeoSvc();
        public static LILogisticaInversaTelemercadeoSvc ServicioLogisticaInversaTelemercadeo
        {
            get { return FabricaServicios.servicioLogisticaInversaTelemercadeo; }
        }

        private static readonly REPReportesSvc servicioReportes = new REPReportesSvc();
        public static REPReportesSvc ServicioReportes
        {
            get { return FabricaServicios.servicioReportes; }
        }

        private static readonly LILogisticaInversaSvc servicioLogisticaInversaCertificaiconesWeb = new LILogisticaInversaSvc();
        public static LILogisticaInversaSvc ServicioLogisticaInversaCertificaiconesWeb
        {
            get { return FabricaServicios.servicioLogisticaInversaCertificaiconesWeb; }
        }

        private static readonly RAMotorRapsSvc servicioMotorRaps = new RAMotorRapsSvc();

        public static RAMotorRapsSvc ServicioMotorRaps
        {
            get { return FabricaServicios.servicioMotorRaps; }
        }

        private static readonly IntegracionesControllerSvc servicioMotorSispostal = new IntegracionesControllerSvc();

        public static IntegracionesControllerSvc ServicioMotorSispostal
        {
            get { return FabricaServicios.servicioMotorSispostal; }
        }

        private static readonly CACajasSvc servicioCajas = new CACajasSvc();

        public static CACajasSvc ServicioCajas
        {
            get { return FabricaServicios.servicioCajas; }
        }


        private static readonly RGRecogidasSvc servicioRecogidas = new RGRecogidasSvc();

        public static RGRecogidasSvc ServicioRecogidas
        {
            get { return FabricaServicios.servicioRecogidas; }
        }

        private static readonly RGRecogidasMotorSvc servicioRecogidasMotor = new RGRecogidasMotorSvc();

        public static RGRecogidasMotorSvc ServicioRecogidasMotor
        {
            get { return FabricaServicios.servicioRecogidasMotor; }
        }

        private static readonly MeMensajeroSvc servicioMensajero = new MeMensajeroSvc();

        public static MeMensajeroSvc ServicioMensajero
        {
            get { return FabricaServicios.servicioMensajero; }
        }

        private static readonly CCControlCuentasSvc ccControlCuentasSvc = new CCControlCuentasSvc();
        public static CCControlCuentasSvc ServicioControlCuentas
        {
            get { return FabricaServicios.ccControlCuentasSvc; }
        }

        private static readonly CACentroAcopioSvc servicioCentroAcopio = new CACentroAcopioSvc();

        public static CACentroAcopioSvc ServicioCentroAcopio
        {
            get { return FabricaServicios.servicioCentroAcopio; }
        }
        private static readonly RACitasRapsSvc servicioCitasRaps = new RACitasRapsSvc();
        public static RACitasRapsSvc ServicioCitasRaps
        {
            get { return FabricaServicios.servicioCitasRaps; }
        }
        public static readonly MeConfiguracionMensajeroSvc servicioConfiguracionMensajero = new MeConfiguracionMensajeroSvc();
        
        public static MeConfiguracionMensajeroSvc ServicioConfiguracionMensajero
        {
            get { return FabricaServicios.servicioConfiguracionMensajero; }
        }
    }
}
