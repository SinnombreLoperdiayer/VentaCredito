using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Clientes.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System.Data.SqlClient;
using System.Configuration;

namespace CO.Servidor.Clientes.Datos
{
    /// <summary>
    ///  Clase para guardar la Auditoria del Módulo de clientes
    /// </summary>
    internal class CLRepositorioAudit
    {
        private static string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        /// <summary>
        /// Metodo de auditoria de cliente
        /// </summary>
        /// <param name="cliente"></param>
        internal static void MapearAuditModificarCliente(EntidadesClientes contexto)
        {
            contexto.Audit<ClienteCredito_CLI, ClienteCreditoHist_CLI>((record, action) => new ClienteCreditoHist_CLI()
            {
                CLI_IdCliente = record.Field<ClienteCredito_CLI, int>(f => f.CLI_IdCliente),
                CLI_ActividadEconomica = record.Field<ClienteCredito_CLI, short>(f => f.CLI_ActividadEconomica),
                CLI_DigitoVerificacion = record.Field<ClienteCredito_CLI, string>(f => f.CLI_DigitoVerificacion),
                CLI_Direccion = record.Field<ClienteCredito_CLI, string>(f => f.CLI_Direccion),
                CLI_Estado = record.Field<ClienteCredito_CLI, string>(f => f.CLI_Estado),
                CLI_Fax = record.Field<ClienteCredito_CLI, string>(f => f.CLI_Fax),
                CLI_FechaConstitucion = record.Field<ClienteCredito_CLI, DateTime>(f => f.CLI_FechaConstitucion),
                CLI_FechaVinculacion = record.Field<ClienteCredito_CLI, DateTime>(f => f.CLI_FechaVinculacion),
                CLI_IdRepresentanteLegal = record.Field<ClienteCredito_CLI, long>(f => f.CLI_IdRepresentanteLegal),
                CLI_Municipio = record.Field<ClienteCredito_CLI, string>(f => f.CLI_Municipio),
                CLI_NombreGerenteGeneral = record.Field<ClienteCredito_CLI, string>(f => f.CLI_NombreGerenteGeneral),
                CLI_RazonSocial = record.Field<ClienteCredito_CLI, string>(f => f.CLI_RazonSocial),
                CLI_RegimenContributivo = record.Field<ClienteCredito_CLI, short>(f => f.CLI_RegimenContributivo),
                CLI_SegmentoMercado = record.Field<ClienteCredito_CLI, short>(f => f.CLI_SegmentoMercado),
                CLI_Telefono = record.Field<ClienteCredito_CLI, string>(f => f.CLI_Telefono),
                CLI_TipoSociedad = record.Field<ClienteCredito_CLI, short>(f => f.CLI_TipoSociedad),
                CLI_TopeMaximoCredito = record.Field<ClienteCredito_CLI, decimal>(f => f.CLI_TopeMaximoCredito),
                CLI_CreadoPor = record.Field<ClienteCredito_CLI, string>(f => f.CLI_CreadoPor),
                CLI_Nit = record.Field<ClienteCredito_CLI, string>(f => f.CLI_Nit),
                CLI_AplicaConvenio = record.Field<ClienteCredito_CLI, bool?>(f => f.CLI_AplicaConvenio),
                CLI_FechaGrabacion = DateTime.Now,
                CLI_CambiadoPor = ControllerContext.Current.Usuario,
                CLI_TipoCambio = action.ToString(),
            }, (ph) => contexto.ClienteCreditoHist_CLI.Add(ph));
        }

        /// <summary>
        /// Metodo de auditoria de contratos
        /// </summary>
        /// <param name="contexto"></param>
        internal static void MapearAuditModificarContrato(EntidadesClientes contexto)
        {
            contexto.Audit<Contrato_CLI, ContratoHist_CLI>((record, action) => new ContratoHist_CLI()
            {
                CON_IdContrato = record.Field<ContratoHist_CLI, int>(f => f.CON_IdContrato),
                CON_CiudadGestorPago = record.Field<ContratoHist_CLI, string>(f => f.CON_CiudadGestorPago),
                CON_CiudadInterventor = record.Field<ContratoHist_CLI, string>(f => f.CON_CiudadInterventor),
                CON_ClienteCredito = record.Field<ContratoHist_CLI, int>(f => f.CON_ClienteCredito),
                CON_EjecutivoCuenta = record.Field<ContratoHist_CLI, long>(f => f.CON_EjecutivoCuenta),
                CON_FechaFin = record.Field<ContratoHist_CLI, DateTime>(f => f.CON_FechaFin),
                CON_FechaInicio = record.Field<ContratoHist_CLI, DateTime>(f => f.CON_FechaInicio),
                CON_ListaPrecios = record.Field<ContratoHist_CLI, int>(f => f.CON_ListaPrecios),
                CON_NombreContrato = record.Field<ContratoHist_CLI, string>(f => f.CON_NombreGestorPago),
                CON_NombreGestorPago = record.Field<ContratoHist_CLI, string>(f => f.CON_NombreGestorPago),
                CON_NombreInterventor = record.Field<ContratoHist_CLI, string>(f => f.CON_NombreInterventor),
                CON_NumeroContrato = record.Field<ContratoHist_CLI, string>(f => f.CON_NumeroContrato),
                CON_ObjetoContrato = record.Field<ContratoHist_CLI, string>(f => f.CON_ObjetoContrato),
                CON_PorcentajeAviso = record.Field<ContratoHist_CLI, decimal>(f => f.CON_PorcentajeAviso),
                CON_PresupuestoMensual = record.Field<ContratoHist_CLI, decimal>(f => f.CON_PresupuestoMensual),
                CON_TelefonoGestorPago = record.Field<ContratoHist_CLI, string>(f => f.CON_TelefonoGestorPago),
                CON_TelefonoInterventor = record.Field<ContratoHist_CLI, string>(f => f.CON_TelefonoInterventor),
                CON_FechaFinConExtensiones = record.Field<ContratoHist_CLI, DateTime>(f => f.CON_FechaFinConExtensiones),
                CON_ValorContrato = record.Field<ContratoHist_CLI, decimal>(f => f.CON_ValorContrato),
                CON_SupervisorCuenta = record.Field<ContratoHist_CLI, long>(f => f.CON_SupervisorCuenta),
                CON_NumeroAsignacionPresupuest = record.Field<ContratoHist_CLI, string>(f => f.CON_NumeroAsignacionPresupuest),
                CON_CertificadoDisponibilidadPropuestal = record.Field<ContratoHist_CLI, string>(f => f.CON_CertificadoDisponibilidadPropuestal),
                CON_NumeroRegisroDisponibilidad = record.Field<ContratoHist_CLI, string>(f => f.CON_NumeroRegisroDisponibilidad),
                CON_ValorDisponibilidad = record.Field<ContratoHist_CLI, decimal?>(f => f.CON_ValorDisponibilidad),
                CON_CreadoPor = record.Field<ContratoHist_CLI, string>(f => f.CON_CreadoPor),
                CON_AplicaValidacionPesoAdmision = record.Field<ContratoHist_CLI, bool>(f => f.CON_AplicaValidacionPesoAdmision),
                CON_FechaGrabacion = DateTime.Now,
                CON_CambiadoPor = ControllerContext.Current.Usuario,
                CON_TipoCambio = action.ToString()
            }, (ph) => contexto.ContratoHist_CLI.Add(ph));
        }

        /// <summary>
        /// Metodo de auditoria de otrossi de un contrato
        /// </summary>
        /// <param name="contexto"></param>
        internal static void MapearAuditModificarOtrosi(EntidadesClientes contexto)
        {
            contexto.Audit<OtroSiContrato_CLI, OtroSiContratoHist_CLI>((record, action) => new OtroSiContratoHist_CLI()
            {
                OSC_Contrato = record.Field<OtroSiContratoHist_CLI, int>(f => f.OSC_Contrato),
                OSC_FechaFin = record.Field<OtroSiContratoHist_CLI, DateTime>(f => f.OSC_FechaFin),
                OSC_IdModalidadOtroSi = record.Field<OtroSiContratoHist_CLI, short>(f => f.OSC_IdModalidadOtroSi),
                OSC_Descripcion = record.Field<OtroSiContratoHist_CLI, string>(f => f.OSC_Descripcion),
                OSC_IdOtroSi = record.Field<OtroSiContratoHist_CLI, int>(f => f.OSC_IdOtroSi),
                OSC_NumeroOtroSi = record.Field<OtroSiContratoHist_CLI, string>(f => f.OSC_NumeroOtroSi),
                OSC_ValorOtroSi = record.Field<OtroSiContratoHist_CLI, decimal>(f => f.OSC_ValorOtroSi),
                OSC_CreadoPor = record.Field<OtroSiContratoHist_CLI, string>(f => f.OSC_CreadoPor),
                OSC_FechaGrabacion = DateTime.Now,
                OSC_CambiadoPor = ControllerContext.Current.Usuario,
                OSC_TipoCambio = action.ToString()
            }, (ph) => contexto.OtroSiContratoHist_CLI.Add(ph));
        }

        /// <summary>
        /// Metodo de auditoria cambios en Cliente Contado
        /// </summary>
        /// <param name="contexto"></param>
        internal static void MapearAuditModificarClienteContado(CLClienteContadoDC clienteContado, string usuario)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("paInsertarClienteContadoHist_CLI", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@Apellido1", clienteContado.Apellido1.ToUpper().Trim());
                comm.Parameters.AddWithValue("@Apellido2", clienteContado.Apellido2.ToUpper().Trim());
                comm.Parameters.AddWithValue("@Direccion", clienteContado.Direccion.ToUpper().Trim());
                comm.Parameters.AddWithValue("@Email", clienteContado.Email == null ? string.Empty : clienteContado.Email.ToUpper().Trim());
                comm.Parameters.AddWithValue("@Identificacion", clienteContado.Identificacion.ToUpper().Trim());
                comm.Parameters.AddWithValue("@Nombre", clienteContado.Nombre.ToUpper().Trim());
                comm.Parameters.AddWithValue("@Ocupacion", clienteContado.Ocupacion != null ? clienteContado.Ocupacion.IdOcupacion : (short)0);
                comm.Parameters.AddWithValue("@Telefono", clienteContado.Telefono.ToUpper().Trim());
                comm.Parameters.AddWithValue("@TipoId", clienteContado.TipoId);
                SqlParameter paramUltimaCedulaEscaneada = new SqlParameter();
                paramUltimaCedulaEscaneada.ParameterName = "@UltimaCedulaEscaneada";
                if (clienteContado.UltimaCedulaEscaneada.HasValue)
                {
                   paramUltimaCedulaEscaneada.Value = clienteContado.UltimaCedulaEscaneada.Value;
                }
                else
                {
                   paramUltimaCedulaEscaneada.Value = DBNull.Value;
                }
                comm.Parameters.Add(paramUltimaCedulaEscaneada);
                //comm.Parameters.AddWithValue("@UltimaCedulaEscaneada", clienteContado.UltimaCedulaEscaneada == 0 ? null : clienteContado.UltimaCedulaEscaneada);
                comm.Parameters.AddWithValue("@FechaGrabacion", DateTime.Now);
                comm.Parameters.AddWithValue("@CreadoPor", usuario);
                comm.Parameters.AddWithValue("@CambiadoPor", usuario);
                comm.Parameters.AddWithValue("@TipoCambio", "Modified");
                comm.ExecuteNonQuery();
                conn.Close();
            }
            //contexto.Audit<ClienteContado_CLI, ClienteContadoHist_CLI>((record, action) => new ClienteContadoHist_CLI()
            //{
            //    //EntidadesClientes contexto
            //    CLC_Apellido1 = record.Field<ClienteContadoHist_CLI, string>(f => f.CLC_Apellido1),
            //    CLC_Apellido2 = record.Field<ClienteContadoHist_CLI, string>(f => f.CLC_Apellido2),
            //    CLC_Direccion = record.Field<ClienteContadoHist_CLI, string>(f => f.CLC_Direccion),
            //    CLC_Email = record.Field<ClienteContadoHist_CLI, string>(f => f.CLC_Email),
            //    CLC_Identificacion = record.Field<ClienteContadoHist_CLI, string>(f => f.CLC_Identificacion),
            //    CLC_Nombre = record.Field<ClienteContadoHist_CLI, string>(f => f.CLC_Nombre),
            //    CLC_Ocupacion = record.Field<ClienteContadoHist_CLI, short>(f => f.CLC_Ocupacion),
            //    CLC_Telefono = record.Field<ClienteContadoHist_CLI, string>(f => f.CLC_Telefono),
            //    CLC_TipoId = record.Field<ClienteContadoHist_CLI, string>(f => f.CLC_TipoId),
            //    CLC_CreadoPor = record.Field<ClienteContadoHist_CLI, string>(f => f.CLC_CreadoPor),
            //    CLC_FechaGrabacion = record.Field<ClienteContadoHist_CLI, DateTime>(f => f.CLC_FechaGrabacion),
            //    CLC_UltimaCedulaEscaneada = record.Field<ClienteContadoHist_CLI, long?>(f => f.CLC_UltimaCedulaEscaneada),
            //    CLC_FechaCambio = DateTime.Now,
            //    CLC_CambiadoPor = usuario,
            //    CLC_TipoCambio = action.ToString()
            //}, (ph) => contexto.ClienteContadoHist_CLI.Add(ph));
        }

        /// <summary>
        /// Metodo de auditoria cambios en sucursales de un contrato
        /// </summary>
        /// <param name="contexto"></param>
        internal static void MapearAuditModificarSucursalContrato(EntidadesClientes contexto)
        {
            contexto.Audit<SucursalesContrato_CLI, SucursalesContratoHist_CLI>((record, action) => new SucursalesContratoHist_CLI()
            {
                SUC_Contrato = record.Field<SucursalesContrato_CLI, int>(f => f.SUC_Contrato),
                SUC_Estado = record.Field<SucursalesContrato_CLI, string>(f => f.SUC_Estado),
                SUC_IdSucursalContrato = record.Field<SucursalesContrato_CLI, int>(f => f.SUC_IdSucursalContrato),
                SUC_QueDebeRecoger = record.Field<SucursalesContrato_CLI, string>(f => f.SUC_QueDebeRecoger),
                SUC_Sucursal = record.Field<SucursalesContrato_CLI, int>(f => f.SUC_Sucursal),
                SUC_CreadoPor = record.Field<SucursalesContrato_CLI, string>(f => f.SUC_CreadoPor),
                SUC_FechaGrabacion = DateTime.Now,
                SUC_CambiadoPor = ControllerContext.Current.Usuario,
                SUC_TipoCambio = action.ToString(),
            }, (ph) => contexto.SucursalesContratoHist_CLI.Add(ph));
        }

        /// <summary>
        /// Metodo de auditoria cambios en las sucursales de un cliente
        /// </summary>
        /// <param name="contexto"></param>
        internal static void MapearAuditModificarSucursal(EntidadesClientes contexto)
        {
            contexto.Audit<Sucursal_CLI, SucursalHist_CLI>((record, action) => new SucursalHist_CLI()
              {
                  SUC_AgenciaEncargada = record.Field<Sucursal_CLI, long>(f => f.SUC_AgenciaEncargada),
                  SUC_ClienteCredito = record.Field<Sucursal_CLI, int>(f => f.SUC_ClienteCredito),
                  SUC_Direccion = record.Field<Sucursal_CLI, string>(f => f.SUC_Direccion),
                  SUC_Email = record.Field<Sucursal_CLI, string>(f => f.SUC_Email),
                  SUC_Fax = record.Field<Sucursal_CLI, string>(f => f.SUC_Fax),
                  SUC_IdSucursal = record.Field<Sucursal_CLI, int>(f => f.SUC_IdSucursal),
                  SUC_Municipio = record.Field<Sucursal_CLI, string>(f => f.SUC_Municipio),
                  SUC_Nombre = record.Field<Sucursal_CLI, string>(f => f.SUC_Nombre),
                  SUC_NombreContacto = record.Field<Sucursal_CLI, string>(f => f.SUC_NombreContacto),
                  SUC_Telefono = record.Field<Sucursal_CLI, string>(f => f.SUC_Telefono),
                  SUC_Zona = record.Field<Sucursal_CLI, string>(f => f.SUC_Zona),
                  SUC_CreadoPor = record.Field<Sucursal_CLI, string>(f => f.SUC_CreadoPor),
                  SUC_FechaGrabacion = DateTime.Now,
                  PAF_CambiadoPor = ControllerContext.Current.Usuario,
                  PAF_TipoCambio = action.ToString(),
              }, (ph) => contexto.SucursalHist_CLI.Add(ph));
        }

        /// <summary>
        /// Metodo de auditoria cambios en las notaciones de una factura
        /// </summary>
        /// <param name="contexto"></param>
        internal static void MapearAuditModificarNotacionFactura(EntidadesClientes contexto)
        {
            contexto.Audit<NotacionesFacturacion_CLI, NotacionesFacturacionHist_CLI>((record, action) => new NotacionesFacturacionHist_CLI()
            {
                NOF_DiaOrdinal = record.Field<NotacionesFacturacion_CLI, short>(f => f.NOF_DiaOrdinal),
                NOF_DiaSemana = record.Field<NotacionesFacturacion_CLI, string>(f => f.NOF_DiaSemana),
                NOF_IdNotacion = record.Field<NotacionesFacturacion_CLI, short>(f => f.NOF_IdNotacion),
                NOF_Semana = record.Field<NotacionesFacturacion_CLI, short>(f => f.NOF_Semana),
                NOF_TipoNotacion = record.Field<NotacionesFacturacion_CLI, short>(f => f.NOF_TipoNotacion),
                NOF_CreadoPor = record.Field<NotacionesFacturacion_CLI, string>(f => f.NOF_CreadoPor),
                NOF_FechaGrabacion = record.Field<NotacionesFacturacion_CLI, DateTime>(f => f.NOF_FechaGrabacion),
                NOF_CambiadoPor = ControllerContext.Current.Usuario,
                NOF_TipoCambio = action.ToString(),
                NOF_FechaCambio = DateTime.Now,
            }, (ph) => contexto.NotacionesFacturacionHist_CLI.Add(ph));
        }

        /// <summary>
        /// Metodo de auditoria cambios en las facturas de un contrato
        /// </summary>
        /// <param name="contexto"></param>
        internal static void MapearAuditModificarParametrosFactura(EntidadesClientes contexto)
        {
            contexto.Audit<ParametrosFactura_CLI, ParametrosFacturaHist_CLI>((record, action) => new ParametrosFacturaHist_CLI()
            {
                PAF_CiudadRadicacion = record.Field<ParametrosFactura_CLI, string>(f => f.PAF_CiudadRadicacion),
                PAF_Contrato = record.Field<ParametrosFactura_CLI, int>(f => f.PAF_Contrato),
                PAF_DiaFacturacion = record.Field<ParametrosFactura_CLI, short>(f => f.PAF_DiaFacturacion),
                PAF_DiaFinalCorte = record.Field<ParametrosFactura_CLI, short>(f => f.PAF_DiaFinalCorte),
                PAF_DiaInicialCorte = record.Field<ParametrosFactura_CLI, short>(f => f.PAF_DiaInicialCorte),
                PAF_DiaPagoFactura = record.Field<ParametrosFactura_CLI, short>(f => f.PAF_DiaPagoFactura),
                PAF_DiaRadicacion = record.Field<ParametrosFactura_CLI, short>(f => f.PAF_DiaRadicacion),
                PAF_DireccionRadicacion = record.Field<ParametrosFactura_CLI, string>(f => f.PAF_DireccionRadicacion),
                PAF_DirigidoA = record.Field<ParametrosFactura_CLI, string>(f => f.PAF_DirigidoA),
                PAF_FormaPago = record.Field<ParametrosFactura_CLI, string>(f => f.PAF_FormaPago),
                PAF_IdParFacturacion = record.Field<ParametrosFactura_CLI, int>(f => f.PAF_IdParFacturacion),
                PAF_NombreFactura = record.Field<ParametrosFactura_CLI, string>(f => f.PAF_NombreFactura),
                PAF_PlazoPago = record.Field<ParametrosFactura_CLI, short>(f => f.PAF_PlazoPago),
                PAF_PorcDescProntoPago = record.Field<ParametrosFactura_CLI, decimal>(f => f.PAF_PorcDescProntoPago),
                PAF_TelefonoRadicacion = record.Field<ParametrosFactura_CLI, string>(f => f.PAF_TelefonoRadicacion),
                PAF_FechaCambio = record.Field<ParametrosFactura_CLI, DateTime>(f => f.PAF_FechaGrabacion),
                PAF_CreadoPor = record.Field<ParametrosFactura_CLI, string>(f => f.PAF_CreadoPor),
                PAF_FechaGrabacion = DateTime.Now,
                PAF_CambiadoPor = ControllerContext.Current.Usuario,
                PAF_TipoCambio = action.ToString(),
            }, (ph) => contexto.ParametrosFacturaHist_CLI.Add(ph));
        }
    }
}