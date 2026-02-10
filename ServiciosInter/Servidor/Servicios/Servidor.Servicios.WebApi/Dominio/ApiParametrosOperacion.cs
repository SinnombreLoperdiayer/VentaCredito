using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Mensajero;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Controllers;
using CO.Servidor.Servicios.WebApi.ModelosRequest.ParametrosOperacion;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CO.Servidor.Servicios.WebApi.ModelosRequest.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.WebApi.Hub;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiParametrosOperacion : ApiDominioBase
    {

        private static readonly ApiParametrosOperacion instancia = (ApiParametrosOperacion)FabricaInterceptorApi.GetProxy(new ApiParametrosOperacion(), COConstantesModulos.PARAMETROS_OPERATIVOS);

        public static ApiParametrosOperacion Instancia
        {
            get { return ApiParametrosOperacion.instancia; }
        }

        private ApiParametrosOperacion()
        {
        }

        /// <summary>
        /// Agregar una nueva posicion (longitud laitud) de un mensajero
        /// </summary>
        /// <param name="posicionMensajero"></param>        
        public void AgregarPosicionMensajero(POUbicacionMensajero posicionMensajero)
        {
            //TODO:CED try catch mientras se encuentra el error que está ocurriendo
            try
            {

                if (posicionMensajero.IdDispositivo <= 0 || string.IsNullOrEmpty(posicionMensajero.IdLocalidad) || posicionMensajero.IdMensajero <= 0)
                {

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("No se puede agregar posicion mensajero");
                    sb.AppendLine("Metodo => AgregarPosicionMensajero(POUbicacionMensajero posicionMensajero)");
                    sb.AppendLine("IdDispositivo=> " + posicionMensajero.IdDispositivo);
                    sb.AppendLine("IdLocalidad=> " + posicionMensajero.IdLocalidad);
                    sb.AppendLine("IdMensajero=> " + posicionMensajero.IdMensajero);


                    Exception excepcion = new Exception(sb.ToString());

                    Util.AuditarExcepcion(excepcion);

                    return;
                }


                FabricaServicios.ServicioParametrosOperacion.AgregarPosicionMensajero(posicionMensajero);
                //Test.ReportarMensajero(posicionMensajero.IdMensajero.ToString(), posicionMensajero.Latitud, posicionMensajero.Longitud);
                MensajeaSignalr.Instancia.ActualizaPosicionMensajero(
                        new ContratoDatos.Recogidas.RGDetalleMensajeroBalance.RGDetalleRutaMensajero()
                        {
                            FechaGrabacion = DateTime.Now,
                            IdMensajero = posicionMensajero.IdMensajero.ToString(),
                            Latitud = posicionMensajero.Latitud.ToString(),
                            Longitud = posicionMensajero.Longitud.ToString()
                        }
                    );
            }
            catch (Exception ex)
            {
                Util.AuditarExcepcion(ex);
            }
        }

        /// <summary>
        /// Metodo para guardar las posiciones en bloque del mensajero
        /// </summary>
        /// <param name="posicionesMensajero"></param>
        internal void AgregarPosicionesMensajero(List<POUbicacionMensajero> posicionesMensajero)
        {
            bool error = false;
            if (posicionesMensajero != null)
            {
                posicionesMensajero = posicionesMensajero.OrderBy(e => e.IdOrden).ToList();
                foreach (var pMensanero in posicionesMensajero)
                {
                    try
                    {
                        if (pMensanero.IdDispositivo <= 0 || string.IsNullOrEmpty(pMensanero.IdLocalidad) || pMensanero.IdMensajero <= 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("No se puede agregar posicion mensajero");
                            sb.AppendLine("Metodo => AgregarPosicionesMensajero(POUbicacionMensajero posicionMensajero)");
                            sb.AppendLine("IdDispositivo=> " + pMensanero.IdDispositivo);
                            sb.AppendLine("IdLocalidad=> " + pMensanero.IdLocalidad);
                            sb.AppendLine("IdMensajero=> " + pMensanero.IdMensajero);
                            Exception excepcion = new Exception(sb.ToString());
                            Util.AuditarExcepcion(excepcion);
                            break;
                        }

                        /********************Insercion de coordenadas********************/
                        FabricaServicios.ServicioParametrosOperacion.AgregarPosicionMensajero(pMensanero);


                    }
                    catch (Exception ex)
                    {
                        error = true;
                        Util.AuditarExcepcion(ex);
                    }
                }

                if (!error)
                {
                    POUbicacionMensajero posicionParaActualizar = posicionesMensajero.LastOrDefault();
                    if (posicionParaActualizar != null)
                    {
                        MensajeaSignalr.Instancia.ActualizaPosicionMensajero(new ContratoDatos.Recogidas.RGDetalleMensajeroBalance.RGDetalleRutaMensajero()
                        {
                            FechaGrabacion = DateTime.Now,
                            IdMensajero = posicionParaActualizar.IdMensajero.ToString(),
                            Latitud = posicionParaActualizar.Latitud.ToString(),
                            Longitud = posicionParaActualizar.Longitud.ToString()
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene las ubicaciones de un mensajero en un rango de fechas determinado
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        public List<POUbicacionMensajero> ObtenerUbicacionesMensajero(long idMensajero, DateTime fechaInicial, DateTime fechaFinal)
        {
            return FabricaServicios.ServicioParametrosOperacion.ObtenerUbicacionesMensajero(idMensajero, fechaInicial, fechaFinal);
        }


        /// <summary>
        /// Obtiene la ultima posicion registrada de un mensajero en el dia actual
        /// </summary>
        /// <param name="idMensajero"></param>        
        /// <returns></returns>
        public List<POUbicacionMensajero> ObtenerUltimaUbicacionMensajeroDiaActual(long idMensajero)
        {

            return FabricaServicios.ServicioParametrosOperacion.ObtenerUltimaUbicacionMensajeroDiaActual(idMensajero);
        }

        /// <summary>
        /// Obtiene la ultima posicion (del dia actual) de todos los mensajeros
        /// </summary>        
        /// <returns></returns>
        public List<POUbicacionMensajero> ObtenerUltimaPosicionTodosMensajeros()
        {
            return FabricaServicios.ServicioParametrosOperacion.ObtenerUltimaPosicionTodosMensajeros();
        }

        public List<POPersonaExterna> ObtenerPersonasExternas()
        {
            return FabricaServicios.ServicioParametrosOperacion.paObtenerPersonasExternas();
        }

        public void ActualizarPersonaExterna(POPersonaExterna persona)
        {
            FabricaServicios.ServicioParametrosOperacion.ActualizarPersonaExterna(persona);
        }

        public void AdicionarPersonaExterna(POPersonaExterna persona)
        {
            FabricaServicios.ServicioParametrosOperacion.AdicionarPersonaExterna(persona);
        }

        public void EliminarPersonaExterna(long idPersona)
        {
            FabricaServicios.ServicioParametrosOperacion.EliminarPersonaExterna(idPersona);
        }

        /// <summary>
        /// metodo para obtener los vehiculos por racol para ser asignados a un auxiliar
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        ///      
        public List<POVehiculo> ObtenerVehiculosRacol(long idRacol)
        {
            return FabricaServicios.ServicioParametrosOperacion.ObtenerVehiculosRacol(idRacol);
        }

        /// <summary>
        /// metodo para obtener los vehiculos por racol para ser asignados a un auxiliar
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        ///      
        public List<PURegionalAdministrativa> ObtenerRacol()
        {
            return FabricaServicios.ServicioParametrosOperacion.ObtenerRacol().ToList();
        }

        public List<POTerritorial> ObtenerTodasTerritoriales()
        {
            return FabricaServicios.ServicioParametrosOperacion.ObtenerTodasTerritoriales();
        }

        /// <summary>
        /// Obtiene lista de configuracion de vehículos 
        /// </summary>
        /// <returns></returns>
        public POListasDatosVehiculos ObtenerListasConfiguracionVehiculo()
        {
            return FabricaServicios.ServicioParametrosOperacion.ObtenerListasConfiguracionVehiculo();
        }

        /// <summary>
        /// Obtiene los tipos de mensajeros
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OUTipoMensajeroDC> ObtenerTiposMensajero()
        {
            return FabricaServicios.ServicioParametrosOperacion.ObtenerTiposMensajero();
        }

        /// <summary>
        /// Retorna los estados de los mensajeros
        /// </summary>
        /// <returns></returns>
        public IList<OUEstadosMensajeroDC> ObtenerEstadosMensajero()
        {
            return FabricaServicios.ServicioParametrosOperacion.ObtenerEstadosMensajero();
        }

        /// <summary>
        /// Obtiene todos los tipos de contrato
        /// </summary>
        /// <returns></returns>
        public IList<POTipoContrato> ObtenerTodosTipoContrato()
        {
            return FabricaServicios.ServicioParametrosOperacion.ObtenerTodosTipoContrato();
        }

        /// <summary>
        /// Adiciona, edita o elimina un mensajero
        /// </summary>
        /// <param name="mensajero"></param>
        public void ActualizarMensajero(PAMensajero mensajero)
        {
            OUMensajeroDC ouMensajero = new OUMensajeroDC();
            ouMensajero.EsContratista = mensajero.EsContratista;
            ouMensajero.EsMensajeroUrbano = mensajero.EsMensajeroUrbano;
            ouMensajero.IdTipoMensajero = mensajero.IdTipoMensajero;
            ouMensajero.NumeroPase = mensajero.NumeroPase;
            ouMensajero.Agencia = mensajero.Agencia;
            ouMensajero.EstadoRegistro = (EnumEstadoRegistro)Enum.Parse(typeof(EnumEstadoRegistro), mensajero.EstadoRegistro);

            ouMensajero.PersonaInterna = new OUPersonaInternaDC();

            ouMensajero.PersonaInterna.Direccion = mensajero.PersonaInterna.Direccion;
            ouMensajero.PersonaInterna.IdCargo = mensajero.PersonaInterna.IdCargo;
            ouMensajero.PersonaInterna.Regional = mensajero.PersonaInterna.Regional;
            ouMensajero.PersonaInterna.Identificacion = mensajero.PersonaInterna.Identificacion;
            ouMensajero.PersonaInterna.Nombre = mensajero.PersonaInterna.Nombre;
            ouMensajero.PersonaInterna.PrimerApellido = mensajero.PersonaInterna.PrimerApellido;
            ouMensajero.PersonaInterna.SegundoApellido = mensajero.PersonaInterna.SegundoApellido;
            ouMensajero.PersonaInterna.Telefono = mensajero.PersonaInterna.Telefono;
            ouMensajero.PersonaInterna.Regional = mensajero.PersonaInterna.Regional;
            ouMensajero.PersonaInterna.FechaInicioContrato = mensajero.PersonaInterna.FechaInicioContrato;
            ouMensajero.PersonaInterna.FechaTerminacionContrato = mensajero.PersonaInterna.FechaTerminacionContrato;

            ouMensajero.LocalidadMensajero = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC();
            ouMensajero.LocalidadMensajero.IdLocalidad = mensajero.IdLocalidad;

            ouMensajero.CargoMensajero = new Framework.Servidor.Servicios.ContratoDatos.Seguridad.SECargo();
            ouMensajero.CargoMensajero.IdCargo = mensajero.IdCargo;


            ouMensajero.Estado = new OUEstadosMensajeroDC();
            ouMensajero.Estado.IdEstado = mensajero.IdEstado;


            ouMensajero.TipoContrato = new POTipoContrato();
            ouMensajero.TipoContrato.IdTipoContrato = mensajero.IdTipoContrato;

            ouMensajero.TipMensajeros = new OUTipoMensajeroDC();
            ouMensajero.TipMensajeros.IdTipoMensajero = mensajero.IdTipoMensajero;


            FabricaServicios.ServicioParametrosOperacion.ActualizarMensajero(ouMensajero);
        }


        #region Registro fallas Interlogis 

        /// <summary>
        /// Metodo para obtener los mensajeros activos 
        /// </summary>
        /// <returns></returns>
        internal List<OUMensajeroRequest> ObtenerMensajerosActivos()
        {
            List<OUMensajeroRequest> misMensajeros = null;
            List<OUMensajeroDC> mensajerosActivos = FabricaServicios.ServicioParametrosOperacion.ObtenerMensajerosActivos();
            if (mensajerosActivos != null)
            {
                misMensajeros = new List<OUMensajeroRequest>();
                foreach (var elemento in mensajerosActivos)
                {
                    misMensajeros.Add(new OUMensajeroRequest()
                    {
                        IdMensajero = elemento.IdMensajero,
                        Identificacion = elemento.PersonaInterna.Identificacion,
                        NombreCompleto = elemento.NombreCompleto,
                        IdCentroServicio = elemento.IdCentroServicio

                    });
                }
            }
            return misMensajeros;
        }

        #endregion 
    }
}
