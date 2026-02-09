using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Transactions;
using CO.Servidor.Dominio.Comun.AdmEstadosConsolidado;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Administracion;
using CO.Servidor.Suministros.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using CO.Servidor.Suministros.Comun;
using System.ServiceModel;

namespace CO.Servidor.Suministros
{
    public partial class SUSuministros : ControllerBase
    {
        #region CrearInstancia

        private static readonly SUSuministros instancia = (SUSuministros)FabricaInterceptores.GetProxy(new SUSuministros(), COConstantesModulos.MODULO_SUMINISTROS);

        /// <summary>
        /// Retorna una instancia de centro Servicios
        /// /// </summary>
        public static SUSuministros Instancia
        {
            get { return SUSuministros.instancia; }
        }

        #endregion CrearInstancia

        /// <summary>
        /// Retorna instancia del configurador de suministros
        /// </summary>
        internal CO.Servidor.Suministros.Administracion.SUConfigurador Configurador
        {
            get
            {
                return new SUConfigurador();
            }
        }

        #region Metodos

        /// <summary>
        /// Consulta la agencia a la cual se le suministro la factura de venta, con el numero de giro IdGiro
        /// </summary>
        /// <param name="IdGiro">Numero del giro</param>
        /// <returns>Centro de servicio</returns>
        internal PUCentroServiciosDC ConsultarAgenciaPropietariaDelNumeroGiro(long idGiro)
        {
            long idCentroServicio = SURepositorio.Instancia.ConsultarAgenciaPropietariaDelNumeroGiro(idGiro);
            return COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerInformacionCentroServicioPorId(idCentroServicio);
        }

        /// <summary>
        /// Retorna los suministros asignados a un centro de servicio
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        internal IEnumerable<SUSuministro> ObtenerSuministrosCentroServicio(PUCentroServiciosDC centroServicio)
        {
            return SURepositorio.Instancia.ObtenerSuministrosCentroServicio(centroServicio);
        }

        /// <summary>
        /// Retorna el consecutivo dle suministro dado
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <returns></returns>
        internal long ObtenerConsecutivoSuministro(SUEnumSuministro idSuministro)
        {
            return SURepositorio.Instancia.ObtenerConsecutivoSuministro(idSuministro);
        }

        /// <summary>
        /// Obtiene el numero  prefijo + valorActual
        /// </summary>
        /// <param name="idSuministro">id del suministro</param>
        /// <returns>numero del giro</returns>
        internal SUNumeradorPrefijo ObtenerNumeroPrefijoValor(SUEnumSuministro idSuministro)
        {
            return SURepositorio.Instancia.ObtenerNumeroPrefijoValor(idSuministro);
        }


        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioGuia(long numeroSuministro)
        {
            return SURepositorio.Instancia.ObtenerPropietarioGuia(numeroSuministro);
        }

        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioSuministro(long numeroSuministro, SUEnumSuministro idSuministro, long idPropietario)
        {
            if (SURepositorio.Instancia.ObtenerSuministro(idSuministro).ValidaPropietario)
            {
                SUPropietarioGuia propietarioGuia = SURepositorio.Instancia.ObtenerPropietarioSuministro(numeroSuministro, idSuministro);
                IPUFachadaCentroServicios fachadaCentroServicios;

                /// Se obtiene el nombre de acuerdo al tipo de propietario
                switch (propietarioGuia.Propietario)
                {
                    case SUEnumGrupoSuministroDC.PTO:
                    case SUEnumGrupoSuministroDC.RAC:
                    case SUEnumGrupoSuministroDC.AGE:
                        fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
                        PUCentroServiciosDC centro = fachadaCentroServicios.ObtenerCentroServicio(propietarioGuia.Id);
                        propietarioGuia.Nombre = centro.Nombre;
                        propietarioGuia.CentroServicios = centro;
                        break;

                    case SUEnumGrupoSuministroDC.MEN:
                        IPOFachadaParametrosOperacion fachadaParametrosOperacion = COFabricaDominio.Instancia.CrearInstancia<IPOFachadaParametrosOperacion>();
                        fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
                        POMensajero mensajero = fachadaParametrosOperacion.ObtenerMensajero(propietarioGuia.Id);
                        propietarioGuia.CedulaMensajero = mensajero.PersonaInterna.Identificacion;
                        propietarioGuia.Nombre = mensajero.Nombre;
                        propietarioGuia.CentroServicios = fachadaCentroServicios.ObtenerCentroServicio(mensajero.IdAgencia);
                        break;

                    case SUEnumGrupoSuministroDC.CLI:
                        ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
                        fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
                        propietarioGuia.Cliente = new CLClientesDC();
                        CLSucursalDC sucursal = fachadaClientes.ObtenerSucursal((int)propietarioGuia.Id, propietarioGuia.Cliente);
                        propietarioGuia.Cliente = fachadaClientes.ObtenerClientexId(sucursal.IdCliente);
                        propietarioGuia.Nombre = sucursal.Nombre;
                        propietarioGuia.CentroServicios = fachadaCentroServicios.ObtenerCentroServicio(sucursal.Agencia);
                        propietarioGuia.PaisSucursal = sucursal.Pais;
                        propietarioGuia.CiudadSucursal = sucursal.Ciudad;

                        break;
                }
                return propietarioGuia;
            }
            else
            {
                return new SUPropietarioGuia
                 {
                     Id = idPropietario,
                     Nombre = string.Empty,
                     CentroServicios = new PUCentroServiciosDC { Tipo = SUEnumGrupoSuministroDC.AGE.ToString() },
                     CedulaMensajero = string.Empty,
                     PaisSucursal = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC(),
                     CiudadSucursal = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC(),
                     Propietario = SUEnumGrupoSuministroDC.AGE,
                 };
            }
        }

        // 
        /// <summary>
        /// Retorna el propietario de un suministro sin validar 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioSuministroSinValidar(long numeroSuministro, SUEnumSuministro idSuministro)
        {

            SUPropietarioGuia propietarioGuia = SURepositorio.Instancia.ObtenerPropietarioSuministroSinValidar(numeroSuministro, idSuministro);
            IPUFachadaCentroServicios fachadaCentroServicios;

            /// Se obtiene el nombre de acuerdo al tipo de propietario
            switch (propietarioGuia.Propietario)
            {
                case SUEnumGrupoSuministroDC.PTO:
                case SUEnumGrupoSuministroDC.RAC:
                case SUEnumGrupoSuministroDC.AGE:
                    if (propietarioGuia.Id != 0)
                    {
                        fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
                        PUCentroServiciosDC centro = fachadaCentroServicios.ObtenerCentroServicio(propietarioGuia.Id);
                        propietarioGuia.Nombre = centro.Nombre;
                        propietarioGuia.CentroServicios = centro;
                    }
                    break;

                case SUEnumGrupoSuministroDC.MEN:
                    IPOFachadaParametrosOperacion fachadaParametrosOperacion = COFabricaDominio.Instancia.CrearInstancia<IPOFachadaParametrosOperacion>();
                    fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
                    POMensajero mensajero = fachadaParametrosOperacion.ObtenerMensajero(propietarioGuia.Id);
                    propietarioGuia.CedulaMensajero = mensajero.PersonaInterna.Identificacion;
                    propietarioGuia.Nombre = mensajero.Nombre;
                    propietarioGuia.CentroServicios = fachadaCentroServicios.ObtenerCentroServicio(mensajero.IdAgencia);
                    break;

                case SUEnumGrupoSuministroDC.CLI:
                    ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
                    fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
                    propietarioGuia.Cliente = new CLClientesDC();
                    CLSucursalDC sucursal = fachadaClientes.ObtenerSucursal((int)propietarioGuia.Id, propietarioGuia.Cliente);
                    propietarioGuia.Cliente = fachadaClientes.ObtenerClientexId(sucursal.IdCliente);
                    propietarioGuia.Nombre = sucursal.Nombre;
                    propietarioGuia.CentroServicios = fachadaCentroServicios.ObtenerCentroServicio(sucursal.Agencia);
                    propietarioGuia.PaisSucursal = sucursal.Pais;
                    propietarioGuia.CiudadSucursal = sucursal.Ciudad;

                    break;
            }
            return propietarioGuia;
        }

        /// <summary>
        /// Consulta la agencia a la cual se le suministro es el comprobante de pago manual con el numero de
        /// comprobante de pago
        /// </summary>
        /// <param name="idComprobantePago" >Comprobante de pago</param>
        internal PUCentroServiciosDC ConsultarAgenciaPropietariaDelNumeroComprobante(long idComprobantePago)
        {
            long idCentroServicio = SURepositorio.Instancia.ConsultarAgenciaPropietariaDelNumeroComprobante(idComprobantePago);
            return COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerInformacionCentroServicioPorId(idCentroServicio);
        }

        /// <summary>
        /// Almacena el consumo de un suministro en la base de datos
        /// </summary>
        /// <param name="consumoSuministro"></param>
        public void GuardarConsumoSuministro(SUConsumoSuministroDC consumoSuministro)
        {
            SURepositorio.Instancia.GuardarConsumoSuministro(consumoSuministro);
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
            SURepositorio.Instancia.GuardarConsumoSuministro(consumoSuministro, conexion, transaccion);
        }

        /// <summary>
        /// Almacena el traslado de un suministro entre un origen y un destino
        /// </summary>
        /// <param name="trasladoSuministro"></param>
        public void GuardarTrasladoSuministro(SUTrasladoSuministroDC trasladoSuministro)
        {
            SURepositorio.Instancia.GuardarTrasladoSuministro(trasladoSuministro);
        }

        /// <summary>
        /// Reasigna los suministros de una agencia a otra
        /// </summary>
        /// <param name="anteriorAgencia"></param>
        /// <param name="nuevaAgencia"></param>
        public void ModificarSuministroAgencia(long anteriorAgencia, long nuevaAgencia)
        {
            PUCentroServiciosDC centroServ = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServicio(anteriorAgencia);
            SUEnumGrupoSuministroDC grupoAnterior = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), centroServ.Tipo);

            centroServ = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServicio(nuevaAgencia);
            SUEnumGrupoSuministroDC grupoNuevo = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), centroServ.Tipo);

            using (TransactionScope transaccion = new TransactionScope())
            {
                SURepositorio.Instancia.ModificarSuministroAgencia(anteriorAgencia, nuevaAgencia, grupoAnterior, grupoNuevo);

                transaccion.Complete();
            };
        }

        /// <summary>
        /// Obtiene los suministros de una sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public List<SUSuministroSucursalDC> ObtenerSuministrosSucursal(int idSucursal)
        {
            return SURepositorio.Instancia.ObtenerSuministrosSucursal(idSucursal);
        }

        public List<SUSuministro> ObtenerSuministroSucursal(int idSucursal)
        {
            List<SUSuministroSucursalDC> suministrosSucursal = SURepositorio.Instancia.ObtenerSuministrosSucursal(idSucursal);
            return suministrosSucursal.ConvertAll<SUSuministro>(s =>
              {
                  return s.Suministro;
              });
        }

        /// <summary>
        /// agrega o modifica un suministro de una sucursal
        /// </summary>
        /// <param name="sumSuc"></param>
        public void AgregarModificarSuministroSucursal(List<SUSuministroSucursalDC> sumSuc)
        {
            sumSuc.ForEach(s =>
              SURepositorio.Instancia.AgregarModificarSuministroSucursal(s)
              );
        }

        /// <summary>
        /// Obtiene los suministros que no estan en el grupo seleccionado ni asignados en la sucursal
        /// </summary>
        /// <param name="idGrupo">id del grupo</param>
        /// <returns>Lista de suministro</returns>
        public List<SUSuministro> ObtenerSuministrosSucursalNoIncluidosEnGrupo(string idGrupo, int idSucursal)
        {
            return SURepositorio.Instancia.ObtenerSuministrosSucursalNoIncluidosEnGrupo(idGrupo, idSucursal);
        }

        /// <summary>
        /// Obtiene los suministros de un proceso
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public List<SUSuministrosProcesoDC> ObtenerSuministrosProceso(long codGestion)
        {
            return SURepositorio.Instancia.ObtenerSuministrosProceso(codGestion);
        }

        /// <summary>
        /// agrega o modifica un suministro de un proceso
        /// </summary>
        /// <param name="sumPro"></param>
        public void AgregarModificarSuministroProceso(List<SUSuministrosProcesoDC> sumPro)
        {
            sumPro.ForEach(s =>
          SURepositorio.Instancia.AgregarModificarSuministroProceso(s)
          );
        }

        /// <summary>
        /// Obtiene los grupos de suministros configurados para las gestiones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<SUSuministrosProcesoDC> ObtenerProcesosSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long CodProceso)
        {
            if (!filtro.ContainsKey("GRS_IdGrupoSuministro"))
            {
                filtro.Add("GRS_IdGrupoSuministro", SUEnumGrupoSuministroDC.PRO.ToString());
            }
            List<SUGrupoSuministrosDC> grupoSuministro = Configurador.ObtenerGrupoSuministrosConSuminGrupo(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);

            if (grupoSuministro.FirstOrDefault() == null)
                return new List<SUSuministrosProcesoDC>();

            List<SUSuministrosProcesoDC> suministroSuc = this.ObtenerSuministrosProceso(CodProceso);

            List<SUSuministrosProcesoDC> sumSuc = new List<SUSuministrosProcesoDC>();
            grupoSuministro.ForEach(g =>
            {
                g.SuministrosGrupo.ForEach(sg =>
                {
                    if (suministroSuc.Where(s => s.IdSuministro == sg.Id).Count() <= 0)
                    {
                        sg.SuministroAutorizado = false;
                        sumSuc.Add(new SUSuministrosProcesoDC()
                        {
                            CodProceso = CodProceso,
                            CantidadInicialAutorizada = sg.CantidadInicialAutorizada,
                            IdSuministro = sg.Id,
                            IdSuministroProceso = 0,
                            StockMinimo = sg.StockMinimo,
                            EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                            Suministro = sg
                        });
                    }
                });

                suministroSuc.ForEach(s =>
                {
                    s.Suministro.SuministroAutorizado = true;
                    sumSuc.Add(new SUSuministrosProcesoDC()
                    {
                        CodProceso = CodProceso,
                        CantidadInicialAutorizada = s.CantidadInicialAutorizada,
                        IdSuministro = s.IdSuministro,
                        IdSuministroProceso = s.IdSuministroProceso,
                        StockMinimo = s.StockMinimo,
                        EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                        Suministro = s.Suministro
                    });
                });
            });

            sumSuc = sumSuc.OrderBy(s => s.Suministro.Descripcion).ToList();

            return sumSuc;
        }

        /// <summary>
        /// Obtiene los suministros que no estan en el grupo seleccionado ni asignados a un proceso
        /// </summary>
        /// <param name="idGrupo">id del grupo</param>
        /// <returns>Lista de suministro</returns>
        public List<SUSuministro> ObtenerSuministrosProcesoNoIncluidosEnGrupo(string idGrupo, long codProceso, Dictionary<string, string> filtro)
        {
            return SURepositorio.Instancia.ObtenerSuministrosProcesoNoIncluidosEnGrupo(idGrupo, codProceso, filtro);
        }

        /// <summary>
        /// Consulta los parametros de suministros
        /// </summary>
        /// <param name="IdParametro"></param>
        /// <returns></returns>
        public string ObtenerParametrosSuministro(string idParametro)
        {
            return SURepositorioAdministracion.Instancia.ObtenerParametrosSuministro(idParametro);
        }

        /// <summary>
        /// Obtiene los suministros aprabados para realizar la remision al proceso
        /// </summary>
        /// <param name="idProceso"></param>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosAsignadosProceso(long idProceso)
        {
            return SURepositorio.Instancia.ObtenerSuministrosAsignadosProceso(idProceso);
        }

        /// <summary>
        /// Obtiene los Correos a notificar la alerta del fallo en
        /// la sincronización a Novasoft de la salida ó
        /// traslado de suministros asignados desde Controller
        /// </summary>
        /// <returns>Lista de Correos</returns>
        public IList<SUCorreoNotificacionesSumDC> ObtenerCorreosNotificacionesSuministro()
        {
            return SURepositorio.Instancia.ObtenerCorreosNotificacionesSuministro();
        }

        /// <summary>
        /// Gestiona los mail a la lista de notificaciones
        /// de sincronizacion de Novasoft para Adicionar ó Borrar
        /// </summary>
        /// <param name="email">Correo a Adicionar</param>
        public void GestionarCorreoNotificacionSuministro(ObservableCollection<SUCorreoNotificacionesSumDC> correosGestionar)
        {
            correosGestionar.ToList().ForEach(correo =>
            {
                if (correo.EstadoRegistro == Framework.Servidor.Comun.EnumEstadoRegistro.ADICIONADO)
                {
                    SURepositorio.Instancia.AdicionarCorreoNotificacionSuministro(correo.Email);
                }
                if (correo.EstadoRegistro == Framework.Servidor.Comun.EnumEstadoRegistro.BORRADO)
                {
                    SURepositorio.Instancia.BorrarCorreoNotificacionSuministro(correo.IdCorreoNotificacion);
                }
            });
        }

        /// <summary>
        /// Indica si un consolidado dado está activo o inactivo
        /// </summary>
        /// <returns></returns>
        /// <param name="codigo">Código del consolidado</param>
        public string ObtenerEstadoActivoConsolidado(string codigo)
        {
            return SURepositorioConsolidados.Instancia.ObtenerEstadoActivoConsolidado(codigo);
        }

        /// <summary>
        /// Retorna los tipos de consolidado
        /// </summary>
        /// <returns></returns>
        public List<OUTipoConsolidadoDC> ObtenerTiposConsolidado()
        {
            return SURepositorioConsolidados.Instancia.ObtenerTiposConsolidado();
        }

        /// <summary>
        /// Retorna los tamaños de la tula
        /// </summary>
        /// <returns></returns>
        public List<SUTamanoTulaDC> ObtenerTamanosTula()
        {
            return SURepositorioConsolidados.Instancia.ObtenerTamanosTula();
        }

        /// <summary>
        /// Retorna los motivos de cambios de un contenedor
        /// </summary>
        /// <returns></returns>
        public List<SUMotivoCambioDC> ObtenerMotivosCambioContenedor()
        {
            return SURepositorioConsolidados.Instancia.ObtenerMotivosCambioContenedor();
        }

        /// <summary>
        /// Registra un nuevo contenedor en la base de datos
        /// </summary>
        /// <param name="contenedor"></param>
        public void RegistrarNuevoContenedor(SUConsolidadoDC contenedor)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                SURepositorioConsolidados.Instancia.RegistrarNuevoContenedor(contenedor);
                ECAdminEstadosConsolidado.GuardarEstadoConsolidado(new ECEstadoConsolidado
                {
                    Estado = EnumEstadosConsolidados.CRE,
                    IdCentroServicios = contenedor.IdCentroServicios,
                    NoTula = contenedor.Codigo,
                    Observaciones = "Creado"
                });
                ts.Complete();
            }
        }

        /// <summary>
        /// Registra una modificación de un contendor
        /// </summary>
        /// <param name="consolidado"></param>
        public void RegistrarModificacionContenedor(SUModificacionConsolidadoDC consolidado)
        {
            SURepositorioConsolidados.Instancia.RegistrarModificacionContenedor(consolidado);
        }

        /// <summary>
        /// Método para validar el dueño de un contenedor
        /// </summary>
        /// <param name="asignacion"></param>
        public void ValidarContenedor(OUAsignacionDC asignacion)
        {
            SURepositorioConsolidados.Instancia.ValidarContenedor(asignacion);
        }

        /// <summary>
        /// Valida que un consolidado activo y que pertenezca a una ciudad a una ciudad
        /// </summary>
        /// <param name="codigoConsolidado"></param>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        public bool ValidarConsolidadoActivoCiudadAsignacion(string codigoConsolidado, string idCiudad)
        {
            return SURepositorioConsolidados.Instancia.ValidarConsolidadoActivoCiudadAsignacion(codigoConsolidado, idCiudad);
        }


        /// <summary>
        /// Valida que un consolidado activo y que pertenezca a una ciudad a una ciudad
        /// </summary>
        /// <param name="codigoConsolidado"></param>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        public List<SUConsolidadoDC> ObtenerInventarioConsolidado(string codigoConsolidado)
        {
            return SURepositorioConsolidados.Instancia.ObtenerInventarioConsolidado(codigoConsolidado);
        }

        /// <summary>
        /// Actualizar el valor del número actual de un suministro específico
        /// </summary>
        /// <param name="tipoSuministro"></param>
        /// <param name="numeroActual"></param>
        public void ActualizarNumeroActualSuministro(SUEnumSuministro tipoSuministro, long numeroActual)
        {
            SURepositorio.Instancia.ActualizarNumeroActualSuministro(tipoSuministro, numeroActual);
        }

        /// <summary>
        /// Metodo para obtener los suministros disponibles por mensajero
        /// </summary>
        /// <param name="IdMensajero"></param>
        /// <param name="IdSuministro"></param>
        /// <returns></returns>
        public List<long> GenerarSuministrosDisponiblesMensajero(long IdMensajero, long IdSuministro)
        {
            return SURepositorio.Instancia.GenerarSuministrosDisponiblesMensajero(IdMensajero, IdSuministro);
        }

        #endregion Metodos
    }
}