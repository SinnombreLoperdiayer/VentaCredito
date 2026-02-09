using CO.Servidor.Clientes.Comun;
using CO.Servidor.Clientes.Datos;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;
using Framework.Servidor.Archivo;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.ParametrosFW.Datos;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;

namespace CO.Servidor.Clientes
{
    /// <summary>
    /// Clase de dominio para la configuración de clientes
    /// </summary>
    public class CLConfiguradorClientes : ControllerBase
    {
        private static readonly CLConfiguradorClientes instancia = (CLConfiguradorClientes)FabricaInterceptores.GetProxy(new CLConfiguradorClientes(), COConstantesModulos.CLIENTES);

        public static CLConfiguradorClientes Instancia
        {
            get { return CLConfiguradorClientes.instancia; }
        }

        #region Metodos basicos

        /// <summary>
        /// Obtiene una lista con los clientes para filtrar
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Tipos de envío</returns>
        public IEnumerable<CLClientesDC> ObtenerClientesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return CLRepositorio.Instancia.ObtenerClientesFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Metodo para la manipulacion de clientes de acuerdo al estado del registro
        /// </summary>
        /// <param name="cliente"></param>
        public void ModificarCliente(CLClientesDC cliente)
        {
            if (cliente.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    cliente.IdCliente = CLRepositorio.Instancia.AdicionarCliente(cliente);
                    GuardarEstadosCliente(cliente);
                    ModificarLocalidadConvenio(cliente.LocalidadesConvenio);
                    transaccion.Complete();
                }
            }
            else if (cliente.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    CLRepositorio.Instancia.ModificarCliente(cliente);
                    GuardarEstadosCliente(cliente);
                    ModificarLocalidadConvenio(cliente.LocalidadesConvenio);
                    transaccion.Complete();
                }
            }
            OperacionesArchivosClientes(cliente);
        }

        /// <summary>
        /// Obtiene las sucursales del cliente por racol
        /// </summary>
        /// <param name="idRacol">Id del racol</param>
        /// <param name="idCliente">id del cliente</param>
        /// <returns>Lista de las sucursales del cliente y del racol</returns>
        public List<CLSucursalDC> ObtenerSucursalesClienteRacol(long idRacol, long idCliente)
        {
            return CLRepositorio.Instancia.ObtenerSucursalesClienteRacol(idRacol, idCliente);
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito de una localidad
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClientesDC> ObtenerClientesLocalidad(string idLocalidad)
        {
            return CLRepositorio.Instancia.ObtenerClientesLocalidad(idLocalidad);
        }

        #endregion Metodos basicos

        #region EstadosCliente

        /// <summary>
        /// Obtiene lista con los estados de un cliente
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLEstadosClienteDC> ObtenerEstadosCliente(CLClientesDC cliente)
        {
            return CLRepositorio.Instancia.ObtenerEstadosCliente(cliente);
        }

        /// <summary>
        /// Obtiene lista con los estados
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EstadoDC> ObtenerEstados()
        {
            return CLRepositorio.Instancia.ObtenerEstados();
        }

        /// <summary>
        /// Obtiene lista con los estados y motivos
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLMotivoEstadosDC> ObtenerMotivosEstados()
        {
            return CLRepositorio.Instancia.ObtenerMotivosEstados();
        }

        /// <summary>
        /// Guarda el estado de un cliente
        /// </summary>
        /// <param name="EstadosCliente"></param>
        private void GuardarEstadosCliente(CLClientesDC cliente)
        {
            if (cliente.EstadoCliente != null)
                if (!string.IsNullOrEmpty(cliente.EstadoCliente.Estado))
                    CLRepositorio.Instancia.AdicionarEstadosCliente(cliente);
        }


        #endregion EstadosCliente

        #region Archivos del cliente

        /// <summary>
        /// Obtiene lista con los archivos de un cliente
        /// </summary>
        /// <returns>objeto de tipo cliente</returns>
        public IEnumerable<CLArchivosDC> ObtenerArchivosCliente(CLClientesDC cliente)
        {
            return CLRepositorio.Instancia.ObtenerArchivosCliente(cliente);
        }

        /// <summary>
        /// Adiciona o elimina los archivos de un cliente
        /// </summary>
        /// <param name="archivos">objeto de tipo lista con los archivos de un cliente</param>
        public void OperacionesArchivosClientes(CLClientesDC cliente)
        {
            if (cliente.ArchivosCliente != null)
                if (cliente.ArchivosCliente.Any())
                {
                    foreach (CLArchivosDC archivo in cliente.ArchivosCliente)
                    {
                        archivo.IdCliente = cliente.IdCliente;
                        if (archivo.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                            CLRepositorio.Instancia.AdicionarArchivoCliente(archivo);
                        if (archivo.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                            CLRepositorio.Instancia.EliminarArchivoCliente(archivo);
                    }
                }
        }

        /// <summary>
        ///Metodo para obtener un archivo asociado a un cliente
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoCliente(CLArchivosDC archivo)
        {
            return CLRepositorio.Instancia.ObtenerArchivoCliente(archivo);
        }

        #endregion Archivos del cliente

        #region Divulgación de cliente

        /// <summary>
        /// Envia la divulgacion de una agencia
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="destinatarios">Diccionario con la informacion de los destinatarios key=Email Value = NombreDestinatario</param>
        public void DivulgarCliente(CLContratosDC contrato, PADivulgacion divulgacion)
        {
            int IdPlantilla = int.Parse(PAAdministrador.Instancia.ConsultarParametrosFramework("IdPlantDivuClientes"));

            byte[] Plantilla = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerPlantillaFramework(IdPlantilla);

            InformacionAlerta informacionAlerta = PARepositorio.Instancia.ConsultarInformacionAlerta(PAConstantesParametros.ALERTA_DIVULGACION_CLIENTE);

            if (Plantilla == null)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CLIENTES, CLEnumTipoErrorCliente.EX_PLANTILLA_DIVULGACION_NO_CONFIGURADA.ToString(), CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_PLANTILLA_DIVULGACION_NO_CONFIGURADA)));
            }

            string pathArchivos = PAAdministrador.Instancia.ObtenerParametrosAplicacion(PAAdministrador.RutaArchivosDivulgacion);

            pathArchivos = Path.Combine(pathArchivos, COConstantesModulos.CLIENTES);
            string nombrePlantilla = Path.Combine(pathArchivos, "P_" + Guid.NewGuid().ToString() + ".docx");

            if (!Directory.Exists(pathArchivos))
            {
                Directory.CreateDirectory(pathArchivos);
            }

            File.WriteAllBytes(nombrePlantilla, Plantilla);

            CLClientesDC InfoClientes = CLRepositorio.Instancia.ObtenerClienteDivulgacion(contrato.IdCliente);

            //List<CLSucursalDC> Sucursales = CLRepositorio.Instancia.ObtenerSucursalesDivulgacion(contrato.IdCliente).OrderBy(h => h.IdSucursal).ToList();
            List<CLContratosDC> Contratos = CLRepositorio.Instancia.ObtenerContratosDivulgacion(contrato.IdCliente).ToList();

            if (Contratos.Count <= 0)
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CLIENTES, CLEnumTipoErrorCliente.EX_SUCURSAL_SIN_CONTRATOS.ToString(),
                        CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_SUCURSAL_SIN_CONTRATOS)));



            List<CLServiciosSucursal> Servicios = CLRepositorio.Instancia.ObtenerServiciosContrato(contrato.IdContrato);

            CLContratosDC ContratoDivulgacion = Contratos.Where(c => c.IdContrato == contrato.IdContrato).FirstOrDefault();

            List<TAServicioCentroDeCorrespondenciaDC> CentrosCorrespondencia = CLRepositorio.Instancia.ObtenerCentrosCorrespondenciaContrato(contrato.IdContrato);

            decimal valorOtroSi = CLRepositorio.Instancia.ObtenerValorPresupuestoOtrosi(contrato.IdContrato);

            List<CLFacturaDC> facturas = CLRepositorio.Instancia.ObtenerFacturacionContrato(contrato.IdContrato);

            decimal valorContrato = ContratoDivulgacion.Valor + valorOtroSi;

            Dictionary<string, string> valores = new Dictionary<string, string>();
            int totalRegistros = 0;
            CLOtroSiDC otroSi = CLRepositorio.Instancia.ObtenerOtroSiContratoFiltro(new Dictionary<string, string>(), "OSC_FechaGrabacion", 0, 100, false, out totalRegistros, contrato.IdContrato).FirstOrDefault();

            PAPersonaExterna representanteLegal = PAParametros.Instancia.ObtenerPersonaExterna(InfoClientes.IdRepresentanteLegal);
            PAPersonaInternaDC EjecutivoCuenta = PAParametros.Instancia.ObtenerPersonaInterna(ContratoDivulgacion.EjecutivoCuenta);

            List<CLSucursalDC> sucursales = CLRepositorio.Instancia.ObtenerSucursalesPorContrato(contrato.IdContrato);

            valores.Add("Fecha", DateTime.Now.ToShortDateString());
            valores.Add("RazonSocialCliente", InfoClientes.RazonSocial);
            valores.Add("NumeroContrato", ContratoDivulgacion.NumeroContrato);
            valores.Add("CodigoContrato", contrato.IdContrato.ToString());
            valores.Add("Nit", InfoClientes.Nit);
            valores.Add("ActividadEconomica", InfoClientes.ActividadEconomica.Descripcion);
            valores.Add("RegimenTributario", InfoClientes.NombreRegimenContributivo);
            valores.Add("FechaActivacion", InfoClientes.FechaVinculacion.ToString("dd/MM/yyyy"));
            valores.Add("FechaInicioServicio", ContratoDivulgacion.FechaInicial.ToString("dd/MM/yyyy"));
            valores.Add("FechaTerminacion", ContratoDivulgacion.FechaFinalExtension.ToString("dd/MM/yyyy"));
            valores.Add("ObjetoContrato", ContratoDivulgacion.ObjetoContrato);
            valores.Add("ValorTotalContrato", valorContrato.ToString("C"));

            //valores.Add("ReservaPresupuestal",
            valores.Add("OtroSiAdicion", otroSi != null ? otroSi.Valor.ToString("C") : "");
            valores.Add("OtroSiProrroga", otroSi != null ? otroSi.FechaFinal.ToString("dd/MM/yyyy") : "");
            valores.Add("Direccion", InfoClientes.Direccion);
            valores.Add("Telefono", InfoClientes.Telefono);
            valores.Add("Ciudad", InfoClientes.NombreMunicipio);
            valores.Add("RepresentanteLegal", representanteLegal != null ? representanteLegal.NombreCompleto : "");

            //valores.Add("EmailRepreLegal", representanteLegal != null ? representanteLegal. : "");
            valores.Add("TelefonoRepreLegal", representanteLegal != null ? representanteLegal.Telefono : "");

            valores.Add("ResponsablePagos", ContratoDivulgacion.NombreGestor);
            valores.Add("TelefonoRespPagos", ContratoDivulgacion.TelefonoGestor);

            //valores.Add("EmailRespPagos", );

            valores.Add("InterventorContrato", ContratoDivulgacion.NombreInterventor);
            valores.Add("TelefonoInterContrato", ContratoDivulgacion.TelefonoInterventor);

            //valores.Add("EmailInterContrato",ContratoDivulgacion. );
            valores.Add("EjecutivoVenta", EjecutivoCuenta.NombreCompleto);

            DataSet dsServicios = new DataSet();
            dsServicios.Tables.Add("SERVICIOS");
            dsServicios.Tables["SERVICIOS"].Columns.Add("UnidadNegocio");
            dsServicios.Tables["SERVICIOS"].Columns.Add("Servicio");
            dsServicios.Tables["SERVICIOS"].Columns.Add("PrimaSeguro");
            dsServicios.Tables["SERVICIOS"].Columns.Add("TipoTarifa");

            Servicios.ForEach(s =>
            {
                dsServicios.Tables["SERVICIOS"].Rows.Add(s.Servicio.UnidadNegocio, s.Servicio.Nombre, s.ListaPrecio.PrimaSeguro, s.ListaPrecio.TarifaPlena ? "Tarifa Plena" : "Tarifa Especial");
            });

            dsServicios.Tables.Add("CENTROSCORRES");
            dsServicios.Tables["CENTROSCORRES"].Columns.Add("NombreServicio");
            dsServicios.Tables["CENTROSCORRES"].Columns.Add("Cantidad");
            dsServicios.Tables["CENTROSCORRES"].Columns.Add("Valor");

            CentrosCorrespondencia.ForEach(s =>
              {
                  dsServicios.Tables["CENTROSCORRES"].Rows.Add(s.Descripcion, "N/A", s.Valor.ToString("C"));
              });

            valores.Add("ObservacionesGenerales", divulgacion.ObservacionesGenerales);

            dsServicios.Tables.Add("FACTURACION");
            dsServicios.Tables["FACTURACION"].Columns.Add("NombreFactura");
            dsServicios.Tables["FACTURACION"].Columns.Add("FechasFacturacion");
            dsServicios.Tables["FACTURACION"].Columns.Add("CiudadRadicacionFactura");
            dsServicios.Tables["FACTURACION"].Columns.Add("DescuentosAutorizado");
            dsServicios.Tables["FACTURACION"].Columns.Add("DocumentosAnexos");
            dsServicios.Tables["FACTURACION"].Columns.Add("Cartera");

            dsServicios.Tables.Add("CONTACTOPAGOS");
            dsServicios.Tables["CONTACTOPAGOS"].Columns.Add("NombreContacto");
            dsServicios.Tables["CONTACTOPAGOS"].Columns.Add("TelefonoContacto");
            dsServicios.Tables["CONTACTOPAGOS"].Columns.Add("EmailContacto");

            dsServicios.Tables.Add("SUCURSALES");
            dsServicios.Tables["SUCURSALES"].Columns.Add("Ciudad");
            dsServicios.Tables["SUCURSALES"].Columns.Add("CodigoSucursal");
            dsServicios.Tables["SUCURSALES"].Columns.Add("NombreSucursal");
            dsServicios.Tables["SUCURSALES"].Columns.Add("Direccion");
            dsServicios.Tables["SUCURSALES"].Columns.Add("Telefono");
            dsServicios.Tables["SUCURSALES"].Columns.Add("Contacto");

            dsServicios.Tables.Add("SUCURSALES2");
            dsServicios.Tables["SUCURSALES2"].Columns.Add("NombreSucursal");
            dsServicios.Tables["SUCURSALES2"].Columns.Add("Email");
            dsServicios.Tables["SUCURSALES2"].Columns.Add("Horario");
            dsServicios.Tables["SUCURSALES2"].Columns.Add("TipoTransporte");
            dsServicios.Tables["SUCURSALES2"].Columns.Add("FechaCreacion");

            dsServicios.Tables.Add("SUMINISTRO");
            dsServicios.Tables["SUMINISTRO"].Columns.Add("NombreSucursal");
            dsServicios.Tables["SUMINISTRO"].Columns.Add("Suministros");

            sucursales.ForEach(s =>
           {
               List<CLSucursalHorarioDC> horariosSucursal = CLRepositorio.Instancia.ObtenerHorariosRecogidaSucursalContrato(s.IdSucursalContrato);

               StringBuilder horariosSuc = new StringBuilder();

               horariosSucursal.ForEach(h =>
                 {
                     horariosSuc.AppendLine(h.NombreDia + " a las " + h.Hora.ToString("HH:mm").Trim());
                 });

               List<SUSuministroSucursalDC> suministroSuc = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>().ObtenerSuministrosSucursal(s.IdSucursal);

               StringBuilder suministros = new StringBuilder();
               suministroSuc.ForEach(sum =>
                 {
                     suministros.AppendLine(sum.Suministro.Descripcion + "  " + (int)sum.CantidadInicialAutorizada + " Unidades");
                 });

               dsServicios.Tables["SUCURSALES"].Rows.Add(s.Ciudad.Nombre, s.IdSucursal.ToString(), s.Nombre, s.Direccion, s.Telefono, s.Contacto);
               dsServicios.Tables["SUCURSALES2"].Rows.Add(s.Nombre, s.Email, horariosSuc.ToString(), "", s.Fecha.ToString("dd/MM/yyyy HH:mm"));

               dsServicios.Tables["SUMINISTRO"].Rows.Add(s.Nombre, suministros.ToString());
           });

            List<MergeWord> lstFacturacion = new List<MergeWord>();

            facturas.ForEach(f =>
              {
                  List<CLFacturaDescuentoDC> descuentos = CLRepositorio.Instancia.ObtenerDescuentosFacturas(f.IdFactura);
                  List<CLFacturaRequisitosDC> requisitos = CLRepositorio.Instancia.ObtenerRequisitosFacturas(f.IdFactura).ToList();
                  StringBuilder stringDescuentos = new StringBuilder();
                  StringBuilder stringRequisitos = new StringBuilder();

                  if (descuentos.Count() > 0)
                  {
                      descuentos.ForEach(d =>
                        {
                            stringDescuentos.AppendLine(d.Motivo + " " + d.FechaAplicacion.ToString("dd/MM/yyyy"));
                        });
                  }
                  if (requisitos.Count() > 0)
                  {
                      requisitos.ForEach(d =>
                      {
                          stringRequisitos.AppendLine(d.Descripcion);
                      });
                  }

                  StringBuilder fechasFactura = new StringBuilder();

                  fechasFactura.AppendLine("Fecha Generación Factura:");
                  fechasFactura.AppendLine(CLRepositorio.Instancia.TraducirNotacionDiaInter(f.DiaFacturacion.IdNotacion));
                  fechasFactura.AppendLine();
                  fechasFactura.AppendLine();
                  fechasFactura.AppendLine("Fecha Radicación Factura:");
                  fechasFactura.AppendLine(CLRepositorio.Instancia.TraducirNotacionDiaInter(f.DiaRadicacion.IdNotacion));
                  fechasFactura.AppendLine();
                  fechasFactura.AppendLine();
                  fechasFactura.AppendLine("Fecha Corte Factura:");
                  fechasFactura.AppendLine("Día " + f.FinalCorte + " de cada mes");

                  StringBuilder cartera = new StringBuilder();
                  cartera.AppendLine("Convenio de pago: ");
                  cartera.AppendLine(f.PlazoPago + " días");
                  cartera.AppendLine();
                  cartera.AppendLine();
                  cartera.AppendLine("Días de Pago: ");
                  cartera.AppendLine(CLRepositorio.Instancia.TraducirNotacionDiaInter(f.DiaPagoFactura.IdNotacion));
                  cartera.AppendLine();
                  cartera.AppendLine();
                  cartera.AppendLine("Forma de pago: ");
                  cartera.AppendLine(f.DescFormaPago);

                  dsServicios.Tables["FACTURACION"].Rows.Add(f.NombreFactura, fechasFactura, f.LocalidadRadicacion.Nombre, stringDescuentos.ToString(), stringRequisitos.ToString(), cartera.ToString());
                  dsServicios.Tables["CONTACTOPAGOS"].Rows.Add(f.DirigidoA, "", "");
              });

            string archivoSalida = Path.Combine(pathArchivos, "S_" + Guid.NewGuid().ToString() + ".docx");
            DocumentosWordMerge.CombinarPlantilla(nombrePlantilla, archivoSalida, dsServicios, valores);

            string destinatarios = string.Empty;

            if (divulgacion.DestinatariosAdicionales != null)
                divulgacion.DestinatariosAdicionales.ToList().ForEach(d =>
                {
                    destinatarios = destinatarios != string.Empty ? string.Join(",", destinatarios, d) : d;
                });

            divulgacion.Grupos.ToList().ForEach(g =>
            {
                if (g.Seleccionado)
                    destinatarios = destinatarios != string.Empty ? string.Join(",", destinatarios, g.CorrerosDestinatarios) : g.CorrerosDestinatarios;
            });
            if (string.IsNullOrWhiteSpace(destinatarios))
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CLIENTES, ETipoErrorFramework.EX_NO_EXISTEN_DESTINATARIOS_CONFIGURADOS.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_NO_EXISTEN_DESTINATARIOS_CONFIGURADOS)));

            CorreoElectronico.Instancia.EnviarAdjunto(destinatarios, informacionAlerta.Asunto, informacionAlerta.Mensaje, archivoSalida);
        }

        #endregion Divulgación de cliente

        #region Sucursales

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un nit de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLSucursalDC> ObtenerSucursalesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idCliente)
        {
            return CLRepositorio.Instancia.ObtenerSucursalesFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCliente);
        }

        /// <summary>
        /// Metodo para modificar sucursales en el repositorio
        /// </summary>
        /// <param name="sucursal">objeto de tipo sucursal</param>
        public int ModificarSucursal(CLSucursalDC sucursal)
        {
            int idSucursal = new int();
            if (sucursal.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                using (TransactionScope scope = new TransactionScope())
                {
                    idSucursal = CLRepositorio.Instancia.AdicionarSucursal(sucursal);
                    scope.Complete();
                }
            if (sucursal.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                using (TransactionScope scope = new TransactionScope())
                {
                    CLRepositorio.Instancia.ModificaSucursal(sucursal);
                    scope.Complete();
                }
            return idSucursal;
        }

        #endregion Sucursales

        #region Tipo de guia de la sucursal

        /// <summary>
        /// Obtiene el tipo de guia por sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public CLSucursalGuiasDC ObtenerGuiaPorSucursal(int idSucursal)
        {
            return CLRepositorio.Instancia.ObtenerGuiaPorSucursal(idSucursal);
        }

        /// <summary>
        /// guarda los cambios de guia por sucursal
        /// </summary>
        /// <param name="guiaSucursal"></param>
        public void GuardarCambiosGuiaPorSucursal(CLSucursalGuiasDC guiaSucursal)
        {
            if (guiaSucursal.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    CLRepositorio.Instancia.AdicionarGuiaPorSucursal(guiaSucursal);
                    scope.Complete();
                }
            }
            if (guiaSucursal.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    CLRepositorio.Instancia.ModificarGuiaPorSucursal(guiaSucursal);
                    scope.Complete();
                }
            }
            if (guiaSucursal.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    CLRepositorio.Instancia.EliminarGuiaPorSucursal(guiaSucursal);
                    scope.Complete();
                }
            }
        }

        #endregion Tipo de guia de la sucursal

        #region Contratos

        /// <summary>
        /// Obtiene una lista con los contratos para filtrar a partir de una identificacion de un cliente
        /// </summary>
        /// <returns>Colección con los contratos configurados en la base de datos</returns>
        public List<CLContratosDC> ObtenerContratosFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idCliente)
        {
            return CLRepositorio.Instancia.ObtenerContratosFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idCliente);
        }

        /// <summary>
        /// Metodo para modificar contratos en el repositorio
        /// </summary>
        /// <param name="sucursal">objeto de tipo contrato</param>
        public int ModificarContrato(CLContratosDC contrato)
        {
            int idcontrato = new int();

            if (contrato.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                if (ValidarContratoNuevo(contrato))
                {
                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        idcontrato = CLRepositorio.Instancia.AdicionarContrato(contrato);
                        transaccion.Complete();
                    }

                    if (contrato.ArchivosContrato != null && contrato.ArchivosContrato.Count > 0)
                    {
                        contrato.ArchivosContrato.ForEach(a =>
                          a.IdContrato = idcontrato);
                        this.OperacionesArchivosContrato(contrato.ArchivosContrato);
                    }
                }
                else
                {
                    var x = new ControllerException
                        (
                        COConstantesModulos.CLIENTES,
                        CLEnumTipoErrorCliente.EX_FALLO_VALOR_CONTRATO.ToString(),
                        CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_FALLO_VALOR_CONTRATO)
                        );
                    throw new FaultException<ControllerException>(x);
                }
            }

            if (contrato.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                if (ValidarContratoExistente(contrato))
                {
                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        CLRepositorio.Instancia.ModificarContrato(contrato);
                        transaccion.Complete();
                    }
                    if (contrato.ArchivosContrato != null && contrato.ArchivosContrato.Count > 0)
                    {
                        contrato.ArchivosContrato.ForEach(a =>
                         a.IdContrato = contrato.IdContrato);
                        this.OperacionesArchivosContrato(contrato.ArchivosContrato);
                    }
                }
                else
                {
                    ControllerException x = new ControllerException
                        (
                        COConstantesModulos.CLIENTES,
                        CLEnumTipoErrorCliente.EX_FALLO_VALOR_CONTRATO.ToString(),
                        CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_FALLO_VALOR_CONTRATO)
                        );
                    throw new FaultException<ControllerException>(x);
                }
            }

            if (contrato.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                CLRepositorio.Instancia.EliminarContrato(contrato);
            }

            return idcontrato;
        }

        /// <summary>
        /// Metodo para validar el cupo al ingresar un contrato
        /// </summary>
        /// <param name="contrato"></param>
        /// <returns></returns>
        public bool ValidarContratoNuevo(CLContratosDC contrato)
        {
            if (contrato.Valor <= ((CLRepositorio.Instancia.ObtenerCupoMaximoCliente(contrato.IdCliente)) - (CLRepositorio.Instancia.ObtenerValorContratos(contrato.IdCliente))))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Metodo para validar el cupo al modificar un contrato
        /// </summary>
        /// <param name="contrato"></param>
        /// <returns></returns>
        public bool ValidarContratoExistente(CLContratosDC contrato)
        {
            if (contrato.Valor <= (((CLRepositorio.Instancia.ObtenerCupoMaximoCliente(contrato.IdCliente)) - (CLRepositorio.Instancia.ObtenerValorContratos(contrato.IdCliente))) + CLRepositorio.Instancia.ObtenerValorPresupuestoContrato(contrato.IdContrato)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Obtiene los contratos activos por cliente
        /// </summary>
        /// <param name="idCliente">lista con los contratios activos por cliente</param>
        /// <returns></returns>
        public IEnumerable<CLContratosDC> ObtenerContratosActivos(int idCliente)
        {
            return CLRepositorio.Instancia.ObtenerContratosActivos(idCliente);
        }

        #endregion Contratos

        #region Personal del contrato

        /// <summary>
        /// Obtiene las personas asociadas a un contrato
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public IEnumerable<PAPersonaInternaDC> ObtenerPersonalContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            return CLRepositorio.Instancia.ObtenerPersonalContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato);
        }

        /// <summary>
        /// Metodo para modificar personal de un contrato
        /// </summary>
        /// <param name="persona"></param>
        /// <returns></returns>
        public void ModificarPersonalContrato(PAPersonaInternaDC persona)
        {
            if (persona.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                if (CLRepositorio.Instancia.ValidarPersonal(persona) == false)
                {
                    CLRepositorio.Instancia.AdicionarPersonal(persona);
                }
                else
                {
                    var x = new ControllerException
                      (
                      COConstantesModulos.CLIENTES,
                      CLEnumTipoErrorCliente.EX_EXISTE_PERSONA_CONTRATO.ToString(),
                      CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_EXISTE_PERSONA_CONTRATO)
                      );
                    throw new FaultException<ControllerException>(x);
                }
            }

            if (persona.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                CLRepositorio.Instancia.EliminarPersonal(persona);
            }
        }

        #endregion Personal del contrato

        #region Contactos del contrato

        /// <summary>
        /// Obtiene loc contactos asociadas a un contrato
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public IEnumerable<CLContactosDC> ObtenerContactosContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            return CLRepositorio.Instancia.ObtenerContactosContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato);
        }

        /// <summary>
        /// Operaciones sobre los contactos
        /// </summary>
        /// <param name="contacto">objeto de tipo contacto</param>
        public void GuardarCambiosContactos(CLContactosDC contacto)
        {
            if (contacto.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                CLRepositorio.Instancia.AdicionarContacto(contacto);
            }
            if (contacto.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                CLRepositorio.Instancia.ModificarContacto(contacto);
            }
            if (contacto.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                CLRepositorio.Instancia.EliminarContacto(contacto);
            }
        }

        #endregion Contactos del contrato

        #region Deducciones del contrato

        /// <summary>
        /// Obtiene las deducciones asociadas a un contrato
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public IEnumerable<CLDeduccionesContratoDC> ObtenerDeduccionesContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            return CLRepositorio.Instancia.ObtenerDeduccionesContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato);
        }

        /// <summary>
        /// Operaciones sobre las deducciones de un contrato
        /// </summary>
        /// <param name="deduccion">objeto de tipo deduccion</param>
        public void GuardarCambiosDeducciones(CLDeduccionesContratoDC deduccion)
        {
            if (deduccion.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                CLRepositorio.Instancia.AdicionarDeduccion(deduccion);
            }
            if (deduccion.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                CLRepositorio.Instancia.ModificarDeduccion(deduccion);
            }
            if (deduccion.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                CLRepositorio.Instancia.EliminarDeduccion(deduccion);
            }
        }

        #endregion Deducciones del contrato

        #region otrosi del contrato

        /// <summary>
        /// obtiene los otro si de un contrato
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public IEnumerable<CLOtroSiDC> ObtenerOtroSiContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            return CLRepositorio.Instancia.ObtenerOtroSiContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato);
        }

        /// <summary>
        /// Lista con los tipos de otrosi de un contrato
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLTipoOtroSiDC> ObtenerListaTiposOtrosi()
        {
            return CLRepositorio.Instancia.ObtenerListaTiposOtrosi();
        }

        /// <summary>
        /// Metodo encargado de guardar cambio de un otros si de un contrato
        /// </summary>
        /// <param name="otrosi"></param>
        public void GuardarCambiosOtroSi(CLOtroSiDC otrosi)
        {
            int idOtroSi;
            if (otrosi.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    idOtroSi = CLRepositorio.Instancia.AdicionarOtroSi(otrosi);
                    transaccion.Complete();
                }

                if (otrosi.ArchivosOtroSi != null && otrosi.ArchivosOtroSi.Count > 0)
                {
                    otrosi.ArchivosOtroSi.ForEach(a =>
                      a.IdOtroSi = idOtroSi);
                    this.OperacionesArchivosOtroSi(otrosi.ArchivosOtroSi);
                }

            }
            if (otrosi.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    CLRepositorio.Instancia.ModificarOtroSi(otrosi);
                    transaccion.Complete();
                }

                if (otrosi.ArchivosOtroSi != null && otrosi.ArchivosOtroSi.Count > 0)
                {
                    otrosi.ArchivosOtroSi.ForEach(a =>
                     a.IdOtroSi = otrosi.IdOtroSi);
                    this.OperacionesArchivosOtroSi(otrosi.ArchivosOtroSi);
                }
            }
        }

        #endregion otrosi del contrato

        #region Archivos del contrato

        /// <summary>
        /// Obtiene lista con los archivos de un contrato
        /// </summary>
        /// <returns>objeto de tipo contrato</returns>
        public IEnumerable<CLContratosArchivosDC> ObtenerArchivosContrato(CLContratosDC contrato)
        {
            return CLRepositorio.Instancia.ObtenerArchivosContrato(contrato);
        }

        /// <summary>
        /// Adiciona o elimina los archivos de un contrato
        /// </summary>
        /// <param name="archivos">objeto de tipo lista con los archivos de un contrato</param>
        public void OperacionesArchivosContrato(IEnumerable<CLContratosArchivosDC> archivos)
        {
            foreach (CLContratosArchivosDC archivo in archivos)
            {
                if (archivo.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                    CLRepositorio.Instancia.AdicionarArchivoContrato(archivo);
                if (archivo.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                    CLRepositorio.Instancia.EliminarArchivoContrato(archivo);
            }

        }

        /// <summary>
        /// Adiciona o elimina los archivos de un otroSi
        /// </summary>
        /// <param name="archivos">objeto de tipo lista con los archivos de un OtroSi</param>
        public void OperacionesArchivosOtroSi(IEnumerable<CLContratosArchivosDC> archivos)
        {
            foreach (CLContratosArchivosDC archivo in archivos)
            {
                if (archivo.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                    CLRepositorio.Instancia.AdicionarArchivoOtroSi(archivo);
                if (archivo.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                    CLRepositorio.Instancia.EliminarArchivoOtroSi(archivo);
            }
        }

        /// <summary>
        ///Metodo para obtener un archivo asociado a un contrato
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoContrato(CLContratosArchivosDC archivo)
        {
            return CLRepositorio.Instancia.ObtenerArchivoContrato(archivo);
        }

        #endregion Archivos del contrato

        #region Sucursales del contrato

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un contrato de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLSucursalContratoDC> ObtenerSucursalesContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            return CLRepositorio.Instancia.ObtenerSucursalesContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato);
        }

        public List<CLContratosDC> ObtenerContratosActivos_ClienteCredito(long idAgencia, int idCliente)
        {
            return CLRepositorio.Instancia.ObtenerContratosActivos_ClienteCredito(idAgencia, idCliente);
        }

        public List<CLSucursalContratoDC> ObtenerSucursalesContrato_CliCredito(long IdAgencia, int IdContrato)
        {
            return CLRepositorio.Instancia.ObtenerSucursalesContrato_CliCredito(IdAgencia, IdContrato);
        }


        /// <summary>
        /// Metodo para guardar cambios de las sucursales de un contrato
        /// </summary>
        /// <param name="sucursal"></param>
        public void GuardarCambiosSucursalesContrato(CLSucursalContratoDC sucursal)
        {
            if (sucursal.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    sucursal.IdSucursalContrato = CLRepositorio.Instancia.AdicionarSucursalContrato(sucursal);
                    GuardarSucursalEstados(sucursal);
                    OperacionesHorarioSucursal(sucursal);
                    transaccion.Complete();
                }
            }

            if (sucursal.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    CLRepositorio.Instancia.ModificarSucursalContrato(sucursal);
                    GuardarSucursalEstados(sucursal);
                    OperacionesHorarioSucursal(sucursal);
                    transaccion.Complete();
                }
            }

            if (sucursal.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                using (TransactionScope transaccion = new TransactionScope())
                {
                    CLRepositorio.Instancia.EliminarSucursalContrato(sucursal);
                    transaccion.Complete();
                }
        }

        /// <summary>
        /// Obtiene lista con los estados de una sucursal
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLSucursalEstadosDC> ObtenerSucursalEstados(int Idcontrato)
        {
            return CLRepositorio.Instancia.ObtenerSucursalEstados(Idcontrato);
        }

        /// <summary>
        /// Guarda el estado de una sucursal
        /// </summary>
        /// <param name="EstadosCliente"></param>
        public void GuardarSucursalEstados(CLSucursalContratoDC sucursal)
        {
            if (sucursal.EstadoDetalle != null)
                if (!string.IsNullOrEmpty(sucursal.EstadoDetalle.Estado))
                    CLRepositorio.Instancia.AdicionarSucursalEstados(sucursal);
        }

        public void OperacionesHorarioSucursal(CLSucursalContratoDC sucursal)
        {
            if (sucursal.Horario != null)
                CLRepositorio.Instancia.HorariosSucursal(sucursal);
        }

        /// <summary>
        /// Obtiene una lista con las sucursales de un cliente que no esten en un contrato especifico
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLSucursalDC> ObtenerSucursalesFiltroExcepcion(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, int idContrato, int idCliente)
        {
            return CLRepositorio.Instancia.ObtenerSucursalesFiltroExcepcion(filtro, indicePagina, registrosPorPagina, idContrato, idCliente);
        }

        #endregion Sucursales del contrato

        #region Facturas

        /// <summary>
        /// Metodo encargado de traer los tipos de notacion para la factura
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLTipoNotacionDC> ObtenerTipoNotacion()
        {
            return CLRepositorio.Instancia.ObtenerTipoNotacion();
        }

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un contrato de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLFacturaDC> ObtenerFacturasContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            return CLRepositorio.Instancia.ObtenerFacturasContratoFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idContrato);
        }

        /// <summary>
        /// Metodo para guardar cambios de una factura de una contrato
        /// </summary>
        /// <param name="?"></param>
        public int GuardarCambiosFacturas(CLFacturaDC factura)
        {
            int idfactura = new int();
            if (factura.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    idfactura = CLRepositorio.Instancia.AdicionarFactura(factura);
                    transaccion.Complete();
                }
            }
            else if (factura.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    CLRepositorio.Instancia.ModificarNotacion(factura.DiaFacturacion);
                    CLRepositorio.Instancia.ModificarNotacion(factura.DiaPagoFactura);
                    CLRepositorio.Instancia.ModificarNotacion(factura.DiaRadicacion);
                    CLRepositorio.Instancia.ModificarFactura(factura);
                    transaccion.Complete();
                }
            }
            else
            {
                EliminarFactura(factura);
            }
            return idfactura;
        }

        /// <summary>
        /// elimino la informacion de la Factura
        /// </summary>
        /// <param name="factura"></param>
        private void EliminarFactura(CLFacturaDC factura)
        {
            //Elimino el descuento de factura
            CLRepositorio.Instancia.EliminarDescuentoFacturaPorFactura(factura.IdFactura);

            // Metodo para eliminar requisitos de una factura
            //por la factura
            CLRepositorio.Instancia.EliminarRequisitosFacturasXFactura(factura.IdFactura);

            //Elimino Agrupacion factura
            CLRepositorio.Instancia.EliminarAgrupacionFactura(factura.IdFactura);

            //Elimino Factura de Parametros Factura
            CLRepositorio.Instancia.EliminarFatura(factura);
        }

        #endregion Facturas

        #region Requisitos de la factura

        /// <summary>
        /// Obtiene una lista con los requisitos para filtrar a partir de una factura de un contrato
        /// </summary>
        /// <returns>Colección con los requisitos configuradas en la base de datos</returns>
        public IEnumerable<CLFacturaRequisitosDC> ObtenerRequisitosFacturas(int idFactura)
        {
            return CLRepositorio.Instancia.ObtenerRequisitosFacturas(idFactura);
        }

        /// <summary>
        /// Metodo para operaciones sobre requisitos de una factura
        /// </summary>
        /// <param name="requisito"></param>
        public void GuardarRequisitosFacturas(CLFacturaRequisitosDC requisito)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                if (requisito.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                    CLRepositorio.Instancia.AdicionarRequisitosFacturas(requisito);
                if (requisito.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                    CLRepositorio.Instancia.EliminarRequisitosFacturas(requisito);
                transaccion.Complete();
            }
        }

        #endregion Requisitos de la factura

        #region Descuentos de la factura

        /// <summary>
        /// Obtiene una lista con los descuentos para filtrar a partir de una factura de un contrato
        /// </summary>
        /// <returns>Colección con los descuentos configuradas en la base de datos</returns>
        public IEnumerable<CLFacturaDescuentoDC> ObtenerDescuentosFacturas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idFactura)
        {
            return CLRepositorio.Instancia.ObtenerDescuentosFacturas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idFactura);
        }

        /// <summary>
        /// Metodo para guardar cambios descuentos a una factura
        /// </summary>
        /// <param name="descuento"></param>
        public void GuardarCambiosDescuentoFactura(CLFacturaDescuentoDC descuento)
        {
            if (descuento.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                CLRepositorio.Instancia.AdicionarDescuentoFactura(descuento);
            }
            else
                if (descuento.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                CLRepositorio.Instancia.ModificarDescuentoFactura(descuento);
            }
        }

        #endregion Descuentos de la factura

        #region Servicios de la factura

        /// <summary>
        /// Obtiene los servicios de una factura
        /// </summary>
        /// <param name="factura"></param>
        /// <returns></returns>
        public IEnumerable<CLFacturaServiciosDC> ObtenerServiciosFactura(CLFacturaDC factura)
        {
            return CLRepositorio.Instancia.ObtenerServiciosFactura(factura);
        }

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un servicio de un contrato
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLSucursalServicioDC> ObtenerSucursalesServicioFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, CLFacturaServiciosDC servicioFactura)
        {
            return CLRepositorio.Instancia.ObtenerSucursalesServicioFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, servicioFactura);
        }

        /// <summary>
        /// Metodo para guardar cambios de Sucursales por servicio
        /// </summary>
        /// <param name="servicioSucursal"></param>
        public void GuardarCambiosServiciosSucursales(CLFacturaServiciosDC servicioSucursal)
        {
            if (servicioSucursal.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    if (servicioSucursal.Sucursales != null)
                    {
                        foreach (CLSucursalServicioDC sucursal in servicioSucursal.Sucursales)
                        {
                            if (sucursal.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                            {
                                CLRepositorio.Instancia.AdicionarSucursalesServicio(new CLFacturaServiciosDC()
                                {
                                    IdContrato = servicioSucursal.IdContrato,
                                    IdFactura = servicioSucursal.IdFactura,
                                    IdServicio = servicioSucursal.IdServicio,
                                    IdSucursal = sucursal.IdSucursal,
                                });
                            }
                        }
                    }
                    transaccion.Complete();
                }
            }

            if (servicioSucursal.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    CLRepositorio.Instancia.EliminarServicio(servicioSucursal);
                    transaccion.Complete();
                }
            }

            if (servicioSucursal.EstadoRegistro == 0 || servicioSucursal.EstadoRegistro == EnumEstadoRegistro.SIN_CAMBIOS)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    if (servicioSucursal.Sucursales != null)
                    {
                        foreach (CLSucursalServicioDC sucursal in servicioSucursal.Sucursales)
                        {
                            if (sucursal.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                            {
                                CLRepositorio.Instancia.AdicionarSucursalesServicio(new CLFacturaServiciosDC()
                                {
                                    IdContrato = servicioSucursal.IdContrato,
                                    IdFactura = servicioSucursal.IdFactura,
                                    IdServicio = servicioSucursal.IdServicio,
                                    IdSucursal = sucursal.IdSucursal,
                                });
                            }
                            if (sucursal.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                            {
                                CLRepositorio.Instancia.EliminarSucursalesServicio(new CLFacturaServiciosDC()
                                {
                                    IdContrato = servicioSucursal.IdContrato,
                                    IdFactura = servicioSucursal.IdFactura,
                                    IdServicio = servicioSucursal.IdServicio,
                                    IdSucursal = sucursal.IdSucursal,
                                });
                            }
                        }
                    }
                    transaccion.Complete();
                }
            }
        }

        /// <summary>
        /// Método para obtener las sucursales excluidas de una factura
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="servicioFactura"></param>
        /// <returns></returns>
        public IEnumerable<CLSucursalServicioDC> ObtenerSucursalesServicioExcluidosFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, CLFacturaServiciosDC servicioFactura)
        {
            return CLRepositorio.Instancia.ObtenerSucursalesServicioExcluidosFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, servicioFactura);
        }

        #endregion Servicios de la factura

        #region Cliente Contado

        #endregion Cliente Contado

        #region Validaciones

        /// <summary>
        /// Valida si una identificacion se encuentra en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public bool ValidarNitExistente(string identificacion)
        {
            return CLRepositorio.Instancia.ValidarNitExistente(identificacion);
        }

        /// <summary>
        /// Valida que el Nit exista y no se encuentre ne la lista restrictiva
        /// </summary>
        /// <param name="Nit"></param>
        public void ValidarNit(string Nit)
        {
            if (CLRepositorio.Instancia.ValidarNitExistente(Nit))
                throw new
                    FaultException<ControllerException>
                    (new ControllerException(COConstantesModulos.CLIENTES,
                        CLEnumTipoErrorCliente.EX_NIT_YA_EXISTE.ToString(),
                        CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_NIT_YA_EXISTE)));
            else
                if (PAParametros.Instancia.ValidarListaRestrictiva(Nit))
                throw new
                    FaultException<ControllerException>
                    (new ControllerException(COConstantesModulos.CLIENTES,
                        CLEnumTipoErrorCliente.EX_NIT_LISTA_NEGRA.ToString(),
                        CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_NIT_LISTA_NEGRA)));
        }

        #endregion Validaciones

        #region Cliente convenio

        /// <summary>
        /// Método para modificar una localidad de origen a un cliente convenio
        /// </summary>
        /// <param name="localidadConvenio"></param>
        public void ModificarLocalidadConvenio(List<CLConvenioLocalidadDC> listaLocalidadConvenio)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                listaLocalidadConvenio.ToList().ForEach(loCon =>
                {
                    switch (loCon.EstadoRegistro)
                    {
                        case EnumEstadoRegistro.ADICIONADO:
                            CLRepositorio.Instancia.AdicionarLocalidadConvenio(loCon);
                            break;

                        case EnumEstadoRegistro.BORRADO:
                            CLRepositorio.Instancia.EliminarLocalidadConvenio(loCon);
                            break;
                    }
                });
                transaccion.Complete();
            }
        }

        #endregion Cliente convenio

        internal IEnumerable<CLTipoClienteCreditoDC> ObtenerTipoClienteCredito()
        {
            return CLRepositorio.Instancia.ObtenerTipoClienteCredito();
        }

    }// fin clase
}