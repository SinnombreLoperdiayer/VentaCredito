using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Transactions;
using CO.Servidor.ParametrosOperacion.Comun;
using CO.Servidor.ParametrosOperacion.Datos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using Integraciones.Novasoft.Proxies;
using Integraciones.Novasoft.Proxies.Contratos;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;

namespace CO.Servidor.ParametrosOperacion
{
    internal class POParametrosOperacion : ControllerBase
    {
        private static readonly POParametrosOperacion instancia = (POParametrosOperacion)FabricaInterceptores.GetProxy(new POParametrosOperacion(), COConstantesModulos.PARAMETROS_OPERATIVOS);

        public static POParametrosOperacion Instancia
        {
            get { return POParametrosOperacion.instancia; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        private POParametrosOperacion()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        #region Conductores

        /// <summary>
        /// Consulta todos los tipos de vehiculos
        /// </summary>
        /// <returns></returns>
        public IList<POTipoVehiculo> ObtenerTodosTipoVehiculo()
        {
            return PORepositorio.Instancia.ObtenerTodosTipoVehiculo();
        }

        /// <summary>
        /// Obtiene  los conductores
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los conductores</returns>
        public IList<POConductores> ObtenerConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PORepositorio.Instancia.ObtenerConductores(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Obtiene todos los conductores
        /// </summary>
        public IList<POConductores> ObtenerTodosConductores()
        {
            return PORepositorio.Instancia.ObtenerTodosConductores();
        }

        /// <summary>
        /// Adiciona un nuevo conductor
        /// </summary>
        /// <param name="conductor"></param>
        public void AdicionarConductor(POConductores conductor)
        {
            PORepositorio.Instancia.AdicionarConductor(conductor);
        }

        /// <summary>
        /// Edita la informacion de un conductor
        /// </summary>
        /// <param name="conductor"></param>
        public void EditarConductor(POConductores conductor)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                PORepositorio.Instancia.EditarConductor(conductor);
                transaccion.Complete();
            }

        }

        /// <summary>
        /// Obtiene los estados para los conductores, contiene el estado suspendido
        /// </summary>
        /// <returns></returns>
        public IList<POEstado> ObtenerEstados()
        {
            IList<POEstado> lstEstados = PORepositorio.Instancia.ObtenerEstados();
            lstEstados.Add(new POEstado() { Descripcion = MensajesParametrosOperacion.CargarMensaje(EnumTipoErrorParametrosOperacion.IN_ESTADO_SUSPENDIDO), IdEstado = POConstantesParametrosOperacion.EstadoSuspendido });
            lstEstados.OrderBy(obj => obj.Descripcion);
            return lstEstados;
        }

        /// <summary>
        /// Consulta la informacion de un conductor desde novasoft
        /// </summary>
        /// <param name="cedula">Numero de cedula del conductor</param>
        /// <param name="conductor">Bandera que indica si el conductor es un empleado o un contratista</param>
        /// <returns>Objeto con la informacion del conductor</returns>
        public POConductores ObtenerConductorNovasoft(string cedula, bool esContratista)
        {
            /* Datos que la interfaz con Novasoft debe garantizar:
             * verificar si el novasoft manejara los mismos id de cargo que controller
             * verificar tipo odentificacion
             * verificar si los id corresponden y el tipo de dato
             * Id municipio
             * verificar que los codigos de la ciudades sean iguales
             */
            Dictionary<string, string> filtro = new Dictionary<string, string>();
            filtro.Add("PEI_Identificacion", cedula);

            POConductores conductorDB = PORepositorio.Instancia.ObtenerPersonaInternaConductor(cedula);

            if (conductorDB != null)
            {
                return conductorDB;
            }
            else
            {
                IIntegracionNovasoft integracion = new ImpIntegracionNovasoft();
                INConductor conductor = null;
                try
                {
                    conductor = integracion.ObtenerConductores(cedula, esContratista);
                }
                catch (Exception ex)
                {
                    if (ex is Integraciones.Novasoft.Proxies.ExcepcionIntegrador)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(POConstantesParametrosOperacion.MODULO_PARAMETROS_OPERACION, EnumTipoErrorParametrosOperacion.EX_ERROR_EJECUTANDO_INTEGRACION.ToString(), (ex as Integraciones.Novasoft.Proxies.ExcepcionIntegrador).Message));
                    }
                    else
                        throw;
                }

                if (conductor == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(POConstantesParametrosOperacion.MODULO_PARAMETROS_OPERACION, EnumTipoErrorParametrosOperacion.EX_REGISTRO_NO_ENCONTRADO_NOVASOFT.ToString(), string.Format(MensajesParametrosOperacion.CargarMensaje(EnumTipoErrorParametrosOperacion.EX_REGISTRO_NO_ENCONTRADO_NOVASOFT), cedula)));
                }
                POConductores conduc = new POConductores()
                {
                    EsContratista = esContratista,
                    Estado = conductor.Estado,
                    EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                    FechaIngreso = conductor.FechaIngreso,
                    FechaTerminacionContrato = conductor.FechaTerminacionContrato,
                    FechaVencimientoPase = conductor.FechaVencimientoPase,
                    NumeroPase = conductor.NumeroPase,
                    Telefono2 = conductor.Telefono2,
                    TipoContrato = new POTipoContrato()
                    {
                        Descripcion = conductor.TipoContrato,
                    },

                    PersonaInterna = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PAPersonaInternaDC()
                    {
                        Direccion = conductor.Direccion,
                        EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                        IdCargo = conductor.IdCargo,
                        Identificacion = conductor.Identificacion,
                        IdTipoIdentificacion = conductor.TipoIdentificacion,
                        NombreMunicipio = conductor.Ciudad,
                        IdRegionalAdministrativa = conductor.IdRegionalAdministrativa,
                        NombreRegional = conductor.RegionalAdministrativa,
                        Nombre = conductor.Nombres,
                        PrimerApellido = conductor.Apellidos,
                        NombreCargo = conductor.Cargo,
                        Telefono = conductor.Telefono,
                        Municipio = conductor.IdCiudad
                    },
                    Ciudad = new PALocalidadDC()
                    {
                        Nombre = conductor.Ciudad,
                        IdLocalidad = conductor.IdCiudad
                    }
                };
                return conduc;
            }
        }

        /// <summary>
        /// Obtiene la lista de racol
        /// </summary>
        /// <returns></returns>
        public IList<PURegionalAdministrativa> ObtenerRacol()
        {
            return PORepositorio.Instancia.ObtenerRacol();
        }

        public List<POTerritorial> ObtenerTodasTerritoriales()
        {
            return PORepositorio.Instancia.ObtenerTodasTerritoriales();
        }

        #endregion Conductores

        #region Vehiculos

        /// <summary>
        /// Obtiene  los vehiculos
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los vehiculos</returns>
        public IList<POVehiculo> ObtenerVehiculos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PORepositorio.Instancia.ObtenerVehiculos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Valida si ya existe un vehiculo creado a partir de la placa
        /// </summary>
        /// <param name="placa"></param>
        /// <returns></returns>
        public bool ValidarExisteVehiculoPlaca(string placa)
        {
            placa = placa.Trim().Replace(" ", "").ToUpper();
            return PORepositorio.Instancia.ValidarExisteVehiculoPlaca(placa);
        }

        /// <summary>
        /// Obtiene los estados para los vehiculos, solo activo e inactivo
        /// </summary>
        /// <returns></returns>
        public IList<POEstado> ObtenerEstadosSoloActIna()
        {
            return PORepositorio.Instancia.ObtenerEstados();
        }

        /// <summary>
        /// Obtiene las lineas de los vehiculos filtradas por la marca
        /// </summary>
        /// <returns></returns>
        public IList<POLinea> ObtenerLineaVehiculo(int idMarcaVehiculo)
        {
            return PORepositorio.Instancia.ObtenerLineaVehiculo(idMarcaVehiculo);
        }

        /// <summary>
        /// Obtiene todas las listas requeridas para la configuracion de un vehiculo
        /// </summary>
        /// <returns></returns>
        public POListasDatosVehiculos ObtenerListasConfiguracionVehiculo()
        {
            POListasDatosVehiculos listas = new POListasDatosVehiculos()
            {
                LstColor = new System.Collections.ObjectModel.ObservableCollection<POColor>(),
                LstConfiguracionVehiculo = new System.Collections.ObjectModel.ObservableCollection<POConfiguracionVehiculo>(),

                LstMarca = new System.Collections.ObjectModel.ObservableCollection<POMarca>(),
                LstTipoCarroceria = new System.Collections.ObjectModel.ObservableCollection<POTipoCarroceria>(),
                LstTipoContrato = new System.Collections.ObjectModel.ObservableCollection<POTipoContrato>(),
                LstTipoCombustible = new System.Collections.ObjectModel.ObservableCollection<POTipoCombustibleDC>()
            };

            PORepositorio.Instancia.ObtenerTodosColor().ToList().ForEach(c =>
              {
                  listas.LstColor.Add(c);
              });
            PORepositorio.Instancia.ObtenerTodosConfiguracionVehiculo().ToList().ForEach(c =>
              {
                  listas.LstConfiguracionVehiculo.Add(c);
              });

            PORepositorio.Instancia.ObtenerTodosMarcaVehiculo().ToList().ForEach(m =>
              {
                  listas.LstMarca.Add(m);
              });

            PORepositorio.Instancia.ObtenerTodosTipoCarroceria().ToList().ForEach(c =>
              {
                  listas.LstTipoCarroceria.Add(c);
              });

            PORepositorio.Instancia.ObtenerTodosTipoContrato().ToList().ForEach(t =>
              {
                  listas.LstTipoContrato.Add(t);
              });

            PORepositorio.Instancia.ObtenerTodosTiposCombustible().ToList().ForEach(tc =>
                {
                    listas.LstTipoCombustible.Add(tc);
                });

            return listas;
        }

        /// <summary>
        /// Obtiene todos los tipos de contrato
        /// </summary>
        /// <returns></returns>
        public IList<POTipoContrato> ObtenerTodosTipoContrato()
        {
            return PORepositorio.Instancia.ObtenerTodosTipoContrato();
        }

        /// <summary>
        /// Obtiene todas las categorias de licencia de conduccion
        /// </summary>
        /// <returns></returns>
        public IList<POCategoriaLicencia> ObtenerTodosCategoriaLicencia()
        {
            return PORepositorio.Instancia.ObtenerTodosCategoriaLicencia();
        }

        /// <summary>
        /// Obtiene todos los tipos de poliza de seguro
        /// </summary>
        /// <returns></returns>
        public IList<POTipoPolizaSeguro> ObtenerTodosTipoPolizaSeguro()
        {
            return PORepositorio.Instancia.ObtenerTodosTipoPolizaSeguro();
        }

        /// <summary>
        /// Obtiene todas las aseguradoras
        /// </summary>
        /// <returns></returns>
        public IList<POAseguradora> ObtenerTodosAseguradora()
        {
            return PORepositorio.Instancia.ObtenerTodosAseguradora();
        }

        #endregion Vehiculos

        /// <summary>
        /// Verifica que los vehiculos asociados al mensajero tengan vencido el soat y la revision tecnomecanica
        /// </summary>
        /// <param name="idMensajero">Id del mensajero</param>
        /// <returns>
        ///   true si el mensajero tiene un vehiculo con el soat y la tecnoMecanica vigente
        ///   false si el mensajero tiene un vehiculo con el soat y la tecnoMecanica vencido
        /// </returns>
        public bool VerificaMensajeroSoatTecnoMecanica(long idMensajero)
        {
            return PORepositorio.Instancia.VerificaMensajeroSoatTecnoMecanica(idMensajero);
        }

        /// <summary>
        /// Inserta y edita la informacion de un vehiculo
        /// </summary>
        /// <param name="vehiculo"></param>
        public void ActualizarVehiculo(POVehiculo vehiculo)
        {
            vehiculo.PolizaSeguroSoat.FechaVencimiento = new DateTime(vehiculo.PolizaSeguroSoat.FechaVencimiento.Year, vehiculo.PolizaSeguroSoat.FechaVencimiento.Month, vehiculo.PolizaSeguroSoat.FechaVencimiento.Day, 23, 59, 59);
            vehiculo.RevisionMecanica.FechaVencimiento = new DateTime(vehiculo.RevisionMecanica.FechaVencimiento.Year, vehiculo.RevisionMecanica.FechaVencimiento.Month, vehiculo.RevisionMecanica.FechaVencimiento.Day, 23, 59, 59);
            switch (vehiculo.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        if (!ValidarExisteVehiculoPlaca(vehiculo.Placa))
                        {
                            PORepositorio.Instancia.AdicionarVehiculo(vehiculo);
                            transaccion.Complete();
                        }
                        else
                        {
                            throw new FaultException<ControllerException>(new ControllerException(POConstantesParametrosOperacion.MODULO_PARAMETROS_OPERACION, EnumTipoErrorParametrosOperacion.EX_YA_EXISTE_VEHICULO.ToString(), MensajesParametrosOperacion.CargarMensaje(EnumTipoErrorParametrosOperacion.EX_YA_EXISTE_VEHICULO)));
                        }
                    }
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        PORepositorio.Instancia.EditarVehiculo(vehiculo);
                        if (vehiculo.PropietarioVehiculo.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                            PORepositorio.Instancia.ModificarPropietario(vehiculo.PropietarioVehiculo, vehiculo.IdVehiculo);
                        else if (vehiculo.PropietarioVehiculo.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                            PORepositorio.Instancia.EditarPropietario(vehiculo.PropietarioVehiculo);
                        if (vehiculo.TenedorVehiculo.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                            PORepositorio.Instancia.EditarTenedor(vehiculo.TenedorVehiculo);
                        else
                            PORepositorio.Instancia.ModificarTenedor(vehiculo.TenedorVehiculo, vehiculo.IdVehiculo);

                        transaccion.Complete();
                    }
                    break;
            }
        }

        /// <summary>
        /// Obtiene los tipos de mensajero vehicular
        /// </summary>
        /// <returns></returns>
        public IList<POTipoMensajero> ObtenerTiposMensajeroVehicular(int idTipoVehiculo)
        {
            return PORepositorio.Instancia.ObtenerTiposMensajeroVehicular(idTipoVehiculo);
        }

        /// <summary>
        /// Obtiene  los mensajeros tipo vehicular
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idTipoMensajero">Id de la ruta por la cual se filtraran las estaciones</param>
        /// <returns>Lista  con los mensajeros</returns>
        public IList<POMensajero> ObtenerMensajerosVehicular(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PORepositorio.Instancia.ObtenerMensajerosVehicular(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Obtiene  los conductores y auxiliares de camion (mensajeros tipo auxiliar) activos
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idTipoMensajero">Id de la ruta por la cual se filtraran las estaciones</param>
        /// <returns>Lista  con los conductores y auxiliares en un objeto tipo mensajero</returns>
        public IList<POMensajero> ObtenerConductoresAuxiliares(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PORepositorio.Instancia.ObtenerConductoresAuxiliares(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Obtiene el propietario del vehiculo apartir de la cedula y el tipo de contrato
        /// </summary>
        /// <param name="cedula">Numero de cedula del propietario</param>
        /// <returns>Objeto con la informacion del propietario</returns>
        public POPropietarioVehiculo ObtenerPropietarioVehiculo(string cedula, int tipoContrato)
        {
            if (tipoContrato == POConstantesParametrosOperacion.TipoContrato_Contratista)
            {
                PAPersonaExterna persona = PORepositorio.Instancia.ObtenerPersonaExternaCedula(cedula);

                if (persona == null)
                {
                    IIntegracionNovasoft integracion = new ImpIntegracionNovasoft();
                    INPropietarioVehiculo PropVehiculo = integracion.ObtenerPropietarioVehiculo(cedula);

                    if (PropVehiculo == null)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(POConstantesParametrosOperacion.MODULO_PARAMETROS_OPERACION, EnumTipoErrorParametrosOperacion.EX_REGISTRO_NO_ENCONTRADO_NOVASOFT.ToString(), string.Format(MensajesParametrosOperacion.CargarMensaje(EnumTipoErrorParametrosOperacion.EX_REGISTRO_NO_ENCONTRADO_NOVASOFT), cedula)));
                    }
                    string nombreCompletoLoc = string.Empty;
                    PALocalidadDC loc = Framework.Servidor.ParametrosFW.PALocalidad.Instancia.ObtenerLocalidadPorId(PropVehiculo.IdCiudad);
                    if (loc != null)
                        nombreCompletoLoc = loc.NombreCompleto;

                    return new POPropietarioVehiculo()
                    {
                        CiudadPropietario = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                        {
                            IdLocalidad = PropVehiculo.IdCiudad,
                            Nombre = nombreCompletoLoc
                        },
                        IdTipoContrato = tipoContrato,
                        PersonaExterna = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PAPersonaExterna()
                        {
                            DigitoVerificacion = PropVehiculo.DigitoVerificacion,
                            Direccion = PropVehiculo.Direccion,
                            EstadoRegistro = EnumEstadoRegistro.ADICIONADO,
                            FechaExpedicionDocumento = PropVehiculo.FechaExpedicionDocumento,
                            Identificacion = PropVehiculo.Identificacion,
                            IdTipoIdentificacion = PropVehiculo.TipoIdentificacion,
                            Municipio = PropVehiculo.IdCiudad,
                            NombreCompleto = PropVehiculo.PrimerNombre + " " + PropVehiculo.PrimerApellido,
                            NumeroCelular = PropVehiculo.NumeroCelular,
                            PrimerApellido = PropVehiculo.PrimerApellido,
                            PrimerNombre = PropVehiculo.PrimerNombre,
                            SegundoApellido = PropVehiculo.SegundoApellido,
                            SegundoNombre = PropVehiculo.SegundoNombre,
                            Telefono = PropVehiculo.Telefono
                        }
                    };
                }
                else
                {
                    string nombreCompletoLoc = string.Empty;
                    PALocalidadDC loc = Framework.Servidor.ParametrosFW.PALocalidad.Instancia.ObtenerLocalidadPorId(persona.Municipio);
                    if (loc != null)
                        nombreCompletoLoc = loc.NombreCompleto;

                    return new POPropietarioVehiculo()
                 {
                     CiudadPropietario = new PALocalidadDC() { IdLocalidad = persona.Municipio, Nombre = nombreCompletoLoc },
                     IdTipoContrato = tipoContrato,
                     PersonaExterna = persona
                 };
                }
            }
            if (tipoContrato == POConstantesParametrosOperacion.TipoContrato_Propio)
            {
                PAPersonaExterna persona = PORepositorio.Instancia.ObtenerPropietarioVehiculo_Propio();
                if (persona == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(POConstantesParametrosOperacion.MODULO_PARAMETROS_OPERACION, EnumTipoErrorParametrosOperacion.EX_REGISTRO_NO_ENCONTRADO_NOVASOFT.ToString(), string.Format(MensajesParametrosOperacion.CargarMensaje(EnumTipoErrorParametrosOperacion.EX_REGISTRO_NO_ENCONTRADO_NOVASOFT), cedula)));
                }
                string nombreCompletoLoc = string.Empty;
                PALocalidadDC loc = Framework.Servidor.ParametrosFW.PALocalidad.Instancia.ObtenerLocalidadPorId(persona.Municipio);
                if (loc != null)
                    nombreCompletoLoc = loc.NombreCompleto;
                return new POPropietarioVehiculo()
                {
                    CiudadPropietario = new PALocalidadDC() { IdLocalidad = persona.Municipio, Nombre = nombreCompletoLoc },
                    IdTipoContrato = tipoContrato,
                    PersonaExterna = persona
                };
            }

            return new POPropietarioVehiculo(); ;
        }

        /// <summary>
        /// Obtiene información básica del mensajero dado su identificador
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public POMensajero ObtenerMensajero(long idMensajero)
        {
            return PORepositorio.Instancia.ObtenerMensajero(idMensajero);
        }

        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        public IEnumerable<POMensajero> ObtenerMensajerosAgencia(long idAgencia)
        {
            return PORepositorio.Instancia.ObtenerMensajerosAgencia(idAgencia);
        }

        /// <summary>
        /// Obtiene una lista de los conductores activos y con pase vigente asignados a un vehiculo
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        public IList<POConductores> ObtenerConductoresActivosVehiculos(int idVehiculo)
        {
            POVehiculo vehiculo = PORepositorio.Instancia.ObtenerVehiculoIdVehiculo(idVehiculo);

            if (vehiculo.IdTipoVehiculo == POConstantesParametrosOperacion.TipoVehiculo_Carro)
                return PORepositorio.Instancia.ObtenerConductoresActivosVehiculos(vehiculo.Placa);
            else
                return PORepositorio.Instancia.ObtenerConductoresActivosMoto(vehiculo.Placa);
        }

        /// <summary>
        /// Obtiene los mensajeros y conductores configurados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public IEnumerable<POMensajero> ObtenerMensajerosConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                              int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PORepositorio.Instancia.ObtenerMensajerosConductores(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        #region Creacion Mensajero

        /// <summary>
        /// Obtiene el listado de los mensajeros
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PORepositorio.Instancia.ObtenerMensajero(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Obtiene la informacion de un mensajero a partir del nombre de usuario, esto aplica
        /// para los usuario que tambien son mensajeros en el caso de los puntos moviles
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public long ObtenerIdMensajeroNomUsuario(string usuario)
        {
            return PORepositorio.Instancia.ObtenerIdMensajeroNomUsuario(usuario);
        }

        /// <summary>
        /// Obtiene la informacion de un mensajero a partir del id del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerMensajeroIdMensajero(long idMensajero)
        {
            return PORepositorio.Instancia.ObtenerMensajeroIdMensajero(idMensajero);
        }


        /// <summary>
        /// Obtiene la informacion de un mensajero a partir del id del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerMensajeroIdMensajeroPAM(long idMensajero)
        {
            return PORepositorio.Instancia.ObtenerMensajeroIdMensajeroPAM(idMensajero);
        }

        /// <summary>
        /// Consulta si existe la persona en la bd de nova soft y retorna la informacion de la persona
        /// </summary>
        /// <param name="identificacion">Documento de identificacion</param>
        /// <param name="contratista">Tipo de vinculacion (Contratista o tercero)</param>
        /// <returns></returns>
        public OUMensajeroDC ConsultaExisteMensajero(string identificacion, bool contratista)
        {
            OUMensajeroDC mensajero = new OUMensajeroDC();
            mensajero.PersonaInterna = new OUPersonaInternaDC();

            PORepositorio.Instancia.ConsultaExisteMensajero(identificacion);

            mensajero.PersonaInterna = PORepositorio.Instancia.ObtenerMensajeroPersonaInterna(identificacion);

            return mensajero;
        }

        /// <summary>
        /// Obtiene los tipos de mensajeros
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OUTipoMensajeroDC> ObtenerTiposMensajero()
        {
            return PORepositorio.Instancia.ObtenerTiposMensajero();
        }

        /// <summary>
        /// Adiciona, edita o elimina un mensajero
        /// </summary>
        /// <param name="mensajero"></param>
        public void ActualizarMensajero(OUMensajeroDC mensajero)
        {
            if (mensajero.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                PORepositorio.Instancia.AdicionarMensajero(mensajero);

            else if (mensajero.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                PORepositorio.Instancia.EditarMensajero(mensajero);

        }

        public IList<OUEstadosMensajeroDC> ObtenerEstadosMensajero()
        {
            IList<OUEstadosMensajeroDC> lestados = new List<OUEstadosMensajeroDC>();
            OUEstadosMensajeroDC estadoAdicionar;

            foreach (string estados in Enum.GetNames(typeof(POEnumEstadosMensajero)))
            {
                estadoAdicionar = new OUEstadosMensajeroDC() { IdEstado = estados };

                if (POEnumEstadosMensajero.ACT.ToString().CompareTo(estados) == 0)
                    estadoAdicionar.Descripcion = MensajesParametrosOperacion.CargarMensaje(EnumTipoErrorParametrosOperacion.IN_ACTIVO);

                else if (POEnumEstadosMensajero.INA.ToString().CompareTo(estados) == 0)
                    estadoAdicionar.Descripcion = MensajesParametrosOperacion.CargarMensaje(EnumTipoErrorParametrosOperacion.IN_INACTIVO);

                else if (POEnumEstadosMensajero.SUS.ToString().CompareTo(estados) == 0)
                    estadoAdicionar.Descripcion = MensajesParametrosOperacion.CargarMensaje(EnumTipoErrorParametrosOperacion.IN_ESTADO_SUSPENDIDO);

                lestados.Add(estadoAdicionar);
            }

            return lestados;
        }

        #endregion Creacion Mensajero

        /// <summary>
        /// Obtiene los vehiculos del racol del usuario
        /// </summary>
        /// <param name="idRacol">id del racol</param>
        /// <returns>Lista de los vehiculo del racol</returns>
        public List<POVehiculo> ObtenerVehiculosRacol(long idRacol)
        {
            return PORepositorio.Instancia.ObtenerVehiculosRacol(idRacol);
        }

        /// <summary>
        /// Obtiene los vehiculos del racol del usuario
        /// </summary>
        /// <param name="idRacol">id del racol</param>
        /// <returns>Lista de los vehiculo del racol</returns>
        public List<POVehiculo> ObtenerVehiculosPuntoServicio(long idPuntoServicio)
        {
            PUAgenciaDeRacolDC agenciaResponsable = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaResponsable(idPuntoServicio);
            PUAgenciaDeRacolDC racolResponsable = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerRacolResponsable(agenciaResponsable.IdResponsable);

            return POParametrosOperacion.Instancia.ObtenerVehiculosRacol(racolResponsable.IdResponsable);
        }

        /// <summary>
        /// Obteners el primer conductor de un vehiculo por placa
        /// </summary>
        /// <param name="placa">placa del vehiculo a consultar</param>
        public ONRutaConductorDC ObtenerConductoresPorVehiculo(string placa)
        {
            POVehiculo vehiculo = PORepositorio.Instancia.ObtenerVehiculoPlaca(placa);
            if (vehiculo.IdTipoVehiculo == POConstantesParametrosOperacion.TipoVehiculo_Carro)
                return PORepositorio.Instancia.ObtenerConductoresPorVehiculo(placa);
            else
                return PORepositorio.Instancia.ObtenerConductoresPorMoto(placa);
        }


        /// <summary>
        /// Obtiene el vehiculo asociado a un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public POVehiculo ObtenerVehiculoMensajero(long idMensajero)
        {
            return PORepositorio.Instancia.ObtenerVehiculoMensajero(idMensajero);
        }

        /// <summary>
        /// Guarda un mensajero
        /// </summary>
        /// <param name="mensajero"></param>
        public void GuardarMensajero(POMensajero mensajero)
        {
            PORepositorio.Instancia.GuardarMensajero(mensajero);
        }

        /// <summary>
        /// Consulta los mensajeros activso del sistema activos y pertenecientes a agencias activas
        /// </summary>
        /// <returns></returns>
        public List<OUMensajeroDC> ObtenerMensajerosActivos()
        {
            return PORepositorio.Instancia.ObtenerMensajerosActivos();
        }

        /// <summary>
        /// Retorna un usuario interno dado su número de cédula
        /// </summary>
        /// <param name="idcedula"></param>
        /// <returns></returns>
        public SEAdminUsuario ObtenerUsuarioInternoPorCedula(string idcedula)
        {
            return PORepositorio.Instancia.ObtenerUsuarioInternoPorCedula(idcedula);
        }



        /// <summary>
        /// Agregar una nueva posicion (longitud laitud) de un mensajero
        /// </summary>
        /// <param name="posicionMensajero"></param>        
        public void AgregarPosicionMensajero(POUbicacionMensajero posicionMensajero)
        {
            PORepositorio.Instancia.AgregarPosicionMensajero(posicionMensajero);
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
            return PORepositorio.Instancia.ObtenerUbicacionesMensajero(idMensajero, fechaInicial, fechaFinal);
        }



        /// <summary>
        /// Obtiene la ultima posicion registrada de un mensajero en el dia actual
        /// </summary>
        /// <param name="idMensajero"></param>        
        /// <returns></returns>
        public List<POUbicacionMensajero> ObtenerUltimaUbicacionMensajeroDiaActual(long idMensajero)
        {

            return PORepositorio.Instancia.ObtenerUltimaUbicacionMensajeroDiaActual(idMensajero);
        }

        /// <summary>
        /// Obtiene la ultima posicion (del dia actual) de todos los mensajeros
        /// </summary>        
        /// <returns></returns>
        public List<POUbicacionMensajero> ObtenerUltimaPosicionTodosMensajeros()
        {
            return PORepositorio.Instancia.ObtenerUltimaPosicionTodosMensajeros();
        }

        public List<POPersonaExterna> ObtenerPersonasExternas()
        {
            return PORepositorio.Instancia.ObtenerPersonasExternas();
        }

        public void ActualizarPersonaExterna(POPersonaExterna persona)
        {
            PORepositorio.Instancia.ActualizarPersonaExterna(persona);
        }

        public void AdicionarPersonaExterna(POPersonaExterna persona)
        {
            PORepositorio.Instancia.AdicionarPersonaExterna(persona);
        }

        public void EliminarPersonaExterna(long idPersona)
        {
            PORepositorio.Instancia.EliminarPersonaExterna(idPersona);
        }

        /// <summary>
        /// Obtener la informacion general del mensajero segun el usuario
        /// </summary>
        /// <param name="nomUsuario"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerInformacionMensajeroNomUsuarioPAM(string nomUsuario)
        {
            return PORepositorio.Instancia.ObtenerInformacionMensajeroNomUsuarioPAM(nomUsuario);
        }

        /// <summary>
        /// Metodo para obtener ultima posicion mensajero por numero de guía.
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public POUbicacionMensajero ObtenerUltimaPosicionMensajeroPorNumeroGuia(long NumeroGuia) {

            return PORepositorio.Instancia.ObtenerUltimaPosicionMensajeroPorNumeroGuia(NumeroGuia);
        }
    }
}