using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using Servicio.Entidades.Admisiones.Mensajeria;
using Servicio.Entidades.Tarifas;
using Framework.Servidor.Excepciones;
using VentaCredito.Transversal;
using Framework.Servidor.Comun;
using VentaCredito.Transversal.Entidades.Clientes;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;
using VentaCredito.AdmisionPreenvio.Datos;

namespace VentaCredito.Datos.Repositorio
{
    public class AdmisionMensajeriaRepositorio
    {

        private static AdmisionMensajeriaRepositorio instancia = new AdmisionMensajeriaRepositorio ();
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private static Dictionary<string, string> ParametrosAdmisionCache = null;
        private static DateTime fechaHoraCacheParametros = DateTime.Now;

        public static AdmisionMensajeriaRepositorio Instancia { get { return instancia; } }

        /// <summary>
        /// Consumo servicios externos
        /// </summary>
        public String conexionApi(string conexion, string nombreApi)
        {
            String urlServicios = ConfigurationManager.AppSettings[conexion];

            if (String.IsNullOrEmpty(urlServicios))
            {
                throw new Exception("Url servidor " + nombreApi + " no encontrado en configuración");
            }
            else
            {
                return urlServicios;
            }
        }

        /// <summary>
        /// Método para adicionar una guia en la tabla de admisiones. Si "registraIngresoACentroAcopio" es true, entonces adiciona un estado deingreso al centro de acopio
        /// </summary>
        /// <param name="guia">Guía de admisiones con la información de la guía interna</param>
        /// <param name="destinatarioRemitente">Información del destinatario y remitente</param>
        /// <param name="registraIngresoACentroAcopio">Indica si se debe generar ingreso a centro de acopio</param>
        /// <returns>Identificador de la admisión de la guía</returns>
        public long AdicionarAdmision(ADGuia guia, ADMensajeriaTipoCliente destinatarioRemitente)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                string tipoIdRemitente = string.Empty;
                string idRemitente = string.Empty;
                string nombreRemitente = string.Empty;


                string tipoIdDestinatario = string.Empty;
                string idDestinatario = string.Empty;
                string nombreDestinatario = string.Empty;
                string telefonoRemitente = string.Empty;
                string direccionRemitente = string.Empty;
                string mailRemitente = guia.Remitente.Email == null ? string.Empty : guia.Remitente.Email;
                string mailDestinatario = destinatarioRemitente.PeatonDestinatario.Email == null ? string.Empty : destinatarioRemitente.PeatonDestinatario.Email;
                switch (guia.TipoCliente)
                {
                    case ADEnumTipoCliente.CCO:
                        tipoIdRemitente = ConstantesFramework.TIPO_DOCUMENTO_NIT;
                        if (destinatarioRemitente.ConvenioRemitente != null)
                        {
                            idRemitente = destinatarioRemitente.ConvenioRemitente.Nit;
                            nombreRemitente = destinatarioRemitente.ConvenioRemitente.RazonSocial;
                            telefonoRemitente = destinatarioRemitente.ConvenioRemitente.Telefono;
                            direccionRemitente = destinatarioRemitente.ConvenioRemitente.Direccion;
                            mailRemitente = destinatarioRemitente.ConvenioRemitente.EMail == null ? string.Empty : destinatarioRemitente.ConvenioRemitente.EMail;
                        }
                        tipoIdDestinatario = ConstantesFramework.TIPO_DOCUMENTO_NIT;
                        if (destinatarioRemitente.ConvenioDestinatario != null)
                        {
                            idDestinatario = destinatarioRemitente.ConvenioDestinatario.Nit;
                            nombreDestinatario = destinatarioRemitente.ConvenioDestinatario.RazonSocial;
                            mailDestinatario = destinatarioRemitente.ConvenioDestinatario.EMail == null ? string.Empty : destinatarioRemitente.ConvenioDestinatario.EMail;
                        }
                        break;

                    case ADEnumTipoCliente.CPE:
                        tipoIdRemitente = Framework.Servidor.Comun.ConstantesFramework.TIPO_DOCUMENTO_NIT;
                        if (destinatarioRemitente.ConvenioRemitente != null)
                        {
                            idRemitente = destinatarioRemitente.ConvenioRemitente.Nit;
                            nombreRemitente = destinatarioRemitente.ConvenioRemitente.RazonSocial;
                            telefonoRemitente = destinatarioRemitente.ConvenioRemitente.Telefono;
                            direccionRemitente = destinatarioRemitente.ConvenioRemitente.Direccion;
                            mailRemitente = destinatarioRemitente.ConvenioRemitente.EMail == null ? string.Empty : destinatarioRemitente.ConvenioRemitente.EMail;
                        }
                        if (destinatarioRemitente.PeatonDestinatario != null)
                        {
                            tipoIdDestinatario = destinatarioRemitente.PeatonDestinatario.TipoIdentificacion;
                            idDestinatario = destinatarioRemitente.PeatonDestinatario.Identificacion;
                            nombreDestinatario = string.Join(" ", destinatarioRemitente.PeatonDestinatario.Nombre, destinatarioRemitente.PeatonDestinatario.Apellido1, destinatarioRemitente.PeatonDestinatario.Apellido2);
                            mailDestinatario = destinatarioRemitente.PeatonDestinatario.Email == null ? string.Empty : destinatarioRemitente.ConvenioRemitente.EMail;
                        }
                        break;

                    case ADEnumTipoCliente.PCO:
                        if (destinatarioRemitente.PeatonRemitente != null)
                        {
                            tipoIdRemitente = destinatarioRemitente.PeatonRemitente.TipoIdentificacion;
                            idRemitente = destinatarioRemitente.PeatonRemitente.Identificacion;
                            nombreRemitente = string.Join(" ", destinatarioRemitente.PeatonRemitente.Nombre, destinatarioRemitente.PeatonRemitente.Apellido1, destinatarioRemitente.PeatonRemitente.Apellido2);
                            telefonoRemitente = destinatarioRemitente.PeatonRemitente.Telefono;
                            direccionRemitente = destinatarioRemitente.PeatonRemitente.Direccion;
                            mailRemitente = destinatarioRemitente.PeatonRemitente.Email == null ? string.Empty: destinatarioRemitente.PeatonRemitente.Email;
                        }
                        tipoIdDestinatario = ConstantesFramework.TIPO_DOCUMENTO_NIT;
                        if (destinatarioRemitente.ConvenioDestinatario != null)
                        {
                            idDestinatario = destinatarioRemitente.ConvenioDestinatario.Nit;
                            nombreDestinatario = destinatarioRemitente.ConvenioDestinatario.RazonSocial;
                            mailDestinatario =  destinatarioRemitente.ConvenioDestinatario.EMail == null ? mailDestinatario :destinatarioRemitente.ConvenioDestinatario.EMail ;
                        }
                        break;

                    case ADEnumTipoCliente.PPE:
                        if (destinatarioRemitente.PeatonRemitente != null)
                        {
                            tipoIdRemitente = destinatarioRemitente.PeatonRemitente.TipoIdentificacion;
                            idRemitente = destinatarioRemitente.PeatonRemitente.Identificacion;
                            nombreRemitente = string.Join(" ", destinatarioRemitente.PeatonRemitente.Nombre, destinatarioRemitente.PeatonRemitente.Apellido1, destinatarioRemitente.PeatonRemitente.Apellido2);
                            telefonoRemitente = destinatarioRemitente.PeatonRemitente.Telefono;
                            direccionRemitente = destinatarioRemitente.PeatonRemitente.Direccion;
                            mailRemitente = destinatarioRemitente.PeatonRemitente.Email;
                        }
                        if (destinatarioRemitente.PeatonDestinatario != null)
                        {
                            tipoIdDestinatario = destinatarioRemitente.PeatonDestinatario.TipoIdentificacion;
                            idDestinatario = destinatarioRemitente.PeatonDestinatario.Identificacion;
                            nombreDestinatario = string.Join(" ", destinatarioRemitente.PeatonDestinatario.Nombre, destinatarioRemitente.PeatonDestinatario.Apellido1, destinatarioRemitente.PeatonDestinatario.Apellido2);
                            mailDestinatario = destinatarioRemitente.PeatonDestinatario.Email;
                        }
                        break;

                    case ADEnumTipoCliente.INT:
                        nombreRemitente = guia.Remitente.Nombre;
                        telefonoRemitente = guia.Remitente.Telefono;
                        direccionRemitente = guia.Remitente.Direccion;
                        nombreDestinatario = guia.Destinatario.Nombre;
                        mailRemitente = guia.Remitente.Email == null ? string.Empty : guia.Remitente.Email;
                        mailDestinatario = guia.Destinatario.Email == null ? string.Empty : guia.Destinatario.Email;
                        idRemitente = guia.Remitente.Identificacion;
                        idDestinatario = guia.Destinatario.Identificacion;
                        break;
                }
                var suma = Math.Ceiling(guia.ValorServicio + guia.ValorPrimaSeguro + guia.ValorAdicionales + guia.ValorEmpaque + guia.ValorTotalImpuestos);
                if (suma > 1 && (long)suma != (long)Math.Ceiling(guia.ValorTotal))
                    throw new FaultException<ControllerException>(new ControllerException("MEN", "0", "Verifique el valor total de la guía, ya que se encuentra en cero"));

                SqlCommand cmd = new SqlCommand("paCrearAdmisionMensajeria_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ADM_NumeroGuia", guia.NumeroGuia);
                cmd.Parameters.AddWithValue("@ADM_DigitoVerificacion", guia.DigitoVerificacion);
                cmd.Parameters.AddWithValue("@ADM_GuidDeChequeo", guia.GuidDeChequeo);
                cmd.Parameters.AddWithValue("@ADM_EsAutomatico", guia.EsAutomatico);
                cmd.Parameters.AddWithValue("@ADM_IdUnidadNegocio", guia.IdUnidadNegocio);
                cmd.Parameters.AddWithValue("@ADM_IdServicio", guia.IdServicio);
                cmd.Parameters.AddWithValue("@ADM_NombreServicio", guia.NombreServicio);
                cmd.Parameters.AddWithValue("@ADM_IdTipoEntrega", guia.IdTipoEntrega);
                cmd.Parameters.AddWithValue("@ADM_DescripcionTipoEntrega", guia.DescripcionTipoEntrega);
                cmd.Parameters.AddWithValue("@ADM_IdCentroServicioOrigen", guia.IdCentroServicioOrigen);
                cmd.Parameters.AddWithValue("@ADM_NombreCentroServicioOrigen", guia.NombreCentroServicioOrigen);
                cmd.Parameters.AddWithValue("@ADM_IdCentroServicioDestino", guia.IdCentroServicioDestino);
                cmd.Parameters.AddWithValue("@ADM_NombreCentroServicioDestino", guia.NombreCentroServicioDestino);
                cmd.Parameters.AddWithValue("@ADM_IdPaisOrigen", guia.IdPaisOrigen);
                cmd.Parameters.AddWithValue("@ADM_NombrePaisOrigen", guia.NombrePaisOrigen);
                cmd.Parameters.AddWithValue("@ADM_IdCiudadOrigen", guia.IdCiudadOrigen);
                cmd.Parameters.AddWithValue("@ADM_NombreCiudadOrigen", guia.NombreCiudadOrigen);
                cmd.Parameters.AddWithValue("@ADM_CodigoPostalOrigen", guia.CodigoPostalOrigen);
                cmd.Parameters.AddWithValue("@ADM_IdPaisDestino", guia.IdPaisDestino);
                cmd.Parameters.AddWithValue("@ADM_NombrePaisDestino", guia.NombrePaisDestino);
                cmd.Parameters.AddWithValue("@ADM_IdCiudadDestino", guia.IdCiudadDestino);
                cmd.Parameters.AddWithValue("@ADM_NombreCiudadDestino", guia.NombreCiudadDestino);
                cmd.Parameters.AddWithValue("@ADM_CodigoPostalDestino", guia.CodigoPostalDestino);
                cmd.Parameters.AddWithValue("@ADM_TelefonoDestinatario", guia.TelefonoDestinatario);
                cmd.Parameters.AddWithValue("@ADM_DireccionDestinatario", guia.DireccionDestinatario);
                cmd.Parameters.AddWithValue("@ADM_TipoCliente", guia.TipoCliente.ToString());
                cmd.Parameters.AddWithValue("@ADM_DiasDeEntrega", guia.DiasDeEntrega);
                cmd.Parameters.AddWithValue("@ADM_FechaEstimadaEntrega", guia.FechaEstimadaEntrega);
                cmd.Parameters.AddWithValue("@ADM_FechaEstimadaDigitalizacion", guia.FechaEstimadaDigitalizacion);
                cmd.Parameters.AddWithValue("@ADM_FechaEstimadaArchivo", guia.FechaEstimadaArchivo);
                cmd.Parameters.AddWithValue("@ADM_ValorAdmision", guia.ValorAdmision);
                cmd.Parameters.AddWithValue("@ADM_ValorTotal", guia.ValorTotal);
                cmd.Parameters.AddWithValue("@ADM_ValorTotalImpuestos", guia.ValorTotalImpuestos);
                cmd.Parameters.AddWithValue("@ADM_ValorTotalRetenciones", guia.ValorTotalRetenciones);
                cmd.Parameters.AddWithValue("@ADM_ValorPrimaSeguro", guia.ValorPrimaSeguro);
                cmd.Parameters.AddWithValue("@ADM_ValorEmpaque", guia.ValorEmpaque);
                cmd.Parameters.AddWithValue("@ADM_ValorAdicionales", guia.ValorAdicionales);
                cmd.Parameters.AddWithValue("@ADM_ValorDeclarado", guia.ValorDeclarado);
                cmd.Parameters.AddWithValue("@ADM_DiceContener", guia.DiceContener);
                cmd.Parameters.AddWithValue("@ADM_Observaciones", guia.Observaciones);
                cmd.Parameters.AddWithValue("@ADM_NumeroPieza", guia.NumeroPieza);
                cmd.Parameters.AddWithValue("@ADM_TotalPiezas", guia.TotalPiezas);
                cmd.Parameters.AddWithValue("@ADM_FechaAdmision", guia.FechaAdmision.Year != 1 ? guia.FechaAdmision : DateTime.Now);
                cmd.Parameters.AddWithValue("@ADM_Peso", guia.Peso);
                cmd.Parameters.AddWithValue("@ADM_PesoLiqVolumetrico", guia.PesoLiqVolumetrico);
                cmd.Parameters.AddWithValue("@ADM_PesoLiqMasa", guia.PesoLiqMasa);
                cmd.Parameters.AddWithValue("@ADM_EsPesoVolumetrico", guia.EsPesoVolumetrico);
                cmd.Parameters.AddWithValue("@ADM_NumeroBolsaSeguridad", guia.NumeroBolsaSeguridad);
                cmd.Parameters.AddWithValue("@ADM_IdMotivoNoUsoBolsaSegurida", guia.IdMotivoNoUsoBolsaSegurida == null ? 0 : guia.IdMotivoNoUsoBolsaSegurida.Value);
                cmd.Parameters.AddWithValue("@ADM_MotivoNoUsoBolsaSeguriDesc", guia.MotivoNoUsoBolsaSeguriDesc == null ? "" : guia.MotivoNoUsoBolsaSeguriDesc);
                cmd.Parameters.AddWithValue("@ADM_NoUsoaBolsaSeguridadObserv", guia.NoUsoaBolsaSeguridadObserv == null ? "" : guia.NoUsoaBolsaSeguridadObserv);
                cmd.Parameters.AddWithValue("@ADM_IdUnidadMedida", guia.IdUnidadMedida);
                cmd.Parameters.AddWithValue("@ADM_Largo", guia.Largo);
                cmd.Parameters.AddWithValue("@ADM_Ancho", guia.Ancho);
                cmd.Parameters.AddWithValue("@ADM_Alto", guia.Alto);
                cmd.Parameters.AddWithValue("@ADM_EsRecomendado", guia.EsRecomendado);
                cmd.Parameters.AddWithValue("@ADM_IdTipoEnvio", guia.IdTipoEnvio);
                cmd.Parameters.AddWithValue("@ADM_NombreTipoEnvio", guia.NombreTipoEnvio);
                cmd.Parameters.AddWithValue("@ADM_AdmisionSistemaMensajero", guia.AdmisionSistemaMensajero);
                cmd.Parameters.AddWithValue("@ADM_EsAlCobro", guia.EsAlCobro);
                cmd.Parameters.AddWithValue("@ADM_CreadoPor", ContextoSitio.Current.Usuario); // ContextoSitio.Current.Usuario);
                cmd.Parameters.AddWithValue("@EGT_IdEstadoGuia", guia.EstadoGuia);
                cmd.Parameters.AddWithValue("@EGT_ObservacionEstadoGuia", guia.ObservacionEstadoGuia == null ? "" : guia.ObservacionEstadoGuia);
                cmd.Parameters.AddWithValue("@ADM_IdTipoIdentificacionRemitente", tipoIdRemitente);
                cmd.Parameters.AddWithValue("@ADM_IdRemitente", !string.IsNullOrWhiteSpace(idRemitente) ? idRemitente : "0");
                cmd.Parameters.AddWithValue("@ADM_NombreRemitente", nombreRemitente);
                cmd.Parameters.AddWithValue("@ADM_IdTipoIdentificacionDestinatario", tipoIdDestinatario);
                cmd.Parameters.AddWithValue("@ADM_IdDestinatario", idDestinatario);
                cmd.Parameters.AddWithValue("@ADM_NombreDestinatario", nombreDestinatario);
                cmd.Parameters.AddWithValue("@ADM_TelefonoRemitente", telefonoRemitente);
                cmd.Parameters.AddWithValue("@ADM_DireccionRemitente", direccionRemitente);
                cmd.Parameters.AddWithValue("@EGT_IdLocalidad", guia.IdCiudadOrigen);
                cmd.Parameters.AddWithValue("@EGT_NombreLocalidad", guia.NombreCiudadOrigen);
                cmd.Parameters.AddWithValue("@EGT_IdModulo", COConstantesModulos.MENSAJERIA);
                cmd.Parameters.AddWithValue("@ADM_IdMensajero", guia.IdMensajero);
                cmd.Parameters.AddWithValue("@ADM_NombreMensajero", guia.NombreMensajero);
                cmd.Parameters.AddWithValue("@ADM_EstaPagada", guia.EstaPagada);
                cmd.Parameters.AddWithValue("@ADM_FechaPago", guia.FechaPago);
                cmd.Parameters.AddWithValue("@ADM_PrefijoNumeroGuia", guia.PrefijoNumeroGuia == null ? "" : guia.PrefijoNumeroGuia);
                cmd.Parameters.AddWithValue("@AMD_EsSupervisada", false);
                cmd.Parameters.AddWithValue("@AMD_FechaSupervision", ConstantesFramework.MinDateTimeController);
                cmd.Parameters.AddWithValue("@AMD_EmailRemitente", mailRemitente);
                cmd.Parameters.AddWithValue("@AMD_EmailDestinatario", mailDestinatario);
                cmd.Parameters.AddWithValue("@ADM_NotificarEntregaPorEmail", guia.NotificarEntregaPorEmail);
                cmd.Parameters.AddWithValue("@NoPedido", guia.NoPedido);
                cmd.Parameters.AddWithValue("@ADM_ValorContraPago", (guia.ValorContraPago != null && guia.ValorContraPago.Value > 0) ? guia.ValorDeclarado : 0);
                cmd.Parameters.AddWithValue("@ADM_ValorFleteContraPago", guia.ValorContraPago != null ? guia.ValorContraPago.Value : 0);

                sqlConn.Open();
                long? idAdmisionMensajeria = null;
                var idAdm = cmd.ExecuteScalar();
                sqlConn.Close();
                if (idAdm != null)
                {
                    idAdmisionMensajeria = Convert.ToInt64(idAdm);
                    return idAdmisionMensajeria.Value;
                }
                else
                    return 0;
            }
        }

        public void AdicionarAdminRapiRadicado(long numeroGuia)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("pa_InsertarAdminRapiRadicado_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserta en base de datos las formas de pago de mensajería
        /// </summary>
        /// <param name="formasPagoGuia"></param>
        /// <param name="usuario"></param>
        public void AdicionarGuiaFormasPago(long idAdmisionMensajeria, ADGuiaFormaPago formaPago, string usuario)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paCrearAdmiGuiaFormaPago_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AGF_IdAdminisionMensajeria", idAdmisionMensajeria);
                cmd.Parameters.AddWithValue("@AGF_IdFormaPago", formaPago.IdFormaPago);
                cmd.Parameters.AddWithValue("@AGF_Valor", formaPago.Valor);
                cmd.Parameters.AddWithValue("@AGF_CreadoPor", usuario);
                cmd.Parameters.AddWithValue("@AGF_NumeroAsociado", formaPago.NumeroAsociadoFormaPago == null ? string.Empty : formaPago.NumeroAsociadoFormaPago);
                cmd.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Inserta los valores adicionales pasados
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="valoresAdicionales"></param>
        /// <param name="usuario"></param>
        public void AdicionarValoresAdicionales(long idAdmisionMensajeria, TAValorAdicional valoresAdicionales, string usuario)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paCrearAdmiValorAdicional_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AVA_IdAdminisionMensajeria", idAdmisionMensajeria);
                cmd.Parameters.AddWithValue("@AVA_IdTipoValorAdicional", valoresAdicionales.IdTipoValorAdicional);
                cmd.Parameters.AddWithValue("@AVA_Descripcion", valoresAdicionales.Descripcion);
                cmd.Parameters.AddWithValue("@AVA_Valor", valoresAdicionales.PrecioValorAdicional);
                cmd.Parameters.AddWithValue("@AVA_CreadoPor", usuario);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserta la información de convenio - convenio
        /// </summary>
        /// <param name="convenio"></param>
        /// <param name="usuario"></param>
        public void AdicionarConvenioConvenio(long idAdmisionesMensajeria, ADMensajeriaTipoCliente convenio, string usuario, int idCliente)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paCrearAdmConvenioConvenio_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MCC_IdAdminisionMensajeria", idAdmisionesMensajeria);
                cmd.Parameters.AddWithValue("@MCC_FacturaRemitente", convenio.FacturaRemitente);
                cmd.Parameters.AddWithValue("@MCC_IdConvenioRemitente", idCliente);
                cmd.Parameters.AddWithValue("@MCC_NitConvenioRemitente", convenio.ConvenioRemitente.Nit);
                cmd.Parameters.AddWithValue("@MCC_RazonSocialConvenioRemitente", convenio.ConvenioRemitente.RazonSocial);
                cmd.Parameters.AddWithValue("@MCC_IdContratoConvenioRemite", convenio.IdContratoConvenioRemitente);
                cmd.Parameters.AddWithValue("@MCC_IdConvenioDestinatario", convenio.ConvenioDestinatario.Id);
                cmd.Parameters.AddWithValue("@MCC_NitConvenioDestinatario", convenio.ConvenioDestinatario.Nit);
                cmd.Parameters.AddWithValue("@MCC_RazonSocialConDestinatario", convenio.ConvenioDestinatario.RazonSocial);
                cmd.Parameters.AddWithValue("@MCC_TelefonoDestinatario", convenio.ConvenioDestinatario.Telefono);
                cmd.Parameters.AddWithValue("@MCC_DireccionDestinatario", convenio.ConvenioDestinatario.Direccion);
                cmd.Parameters.AddWithValue("@MCC_EmailDestinatario", convenio.ConvenioDestinatario.EMail);
                cmd.Parameters.AddWithValue("@MCC_IdSucursalRecogida", convenio.ConvenioRemitente.IdSucursalRecogida);
                cmd.Parameters.AddWithValue("@MCC_CreadoPor", usuario);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserta la información de un envío convenio - peatón
        /// </summary>
        /// <param name="idAdmisionesMensajeria"></param>
        /// <param name="convenioPeaton"></param>
        /// <param name="usuario"></param>
        public void AdicionarConvenioPeaton(long idAdmisionMensajeria, ADMensajeriaTipoCliente convenioPeaton, string usuario, int idCliente)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {


                conn.Open();
                SqlCommand cmd = new SqlCommand("paCrearAdmiConvenioPeaton_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MCP_IdAdminisionMensajeria", idAdmisionMensajeria);
                cmd.Parameters.AddWithValue("@MCP_IdConvenioRemitente", idCliente);
                cmd.Parameters.AddWithValue("@MCP_NitConvenioRemitente", convenioPeaton.ConvenioRemitente.Nit);
                cmd.Parameters.AddWithValue("@MCP_RazonSocialConvenioRemitente", convenioPeaton.ConvenioRemitente.RazonSocial);
                cmd.Parameters.AddWithValue("@MCP_IdContratoConvenioRemite", convenioPeaton.ConvenioRemitente.Contrato);
                cmd.Parameters.AddWithValue("@MCP_IdTipoIdDestinatario", convenioPeaton.PeatonDestinatario.TipoIdentificacion);
                cmd.Parameters.AddWithValue("@MCP_IdDestinatario", convenioPeaton.PeatonDestinatario.Identificacion);
                cmd.Parameters.AddWithValue("@MCP_NombreDestinatario", convenioPeaton.PeatonDestinatario.Nombre);
                cmd.Parameters.AddWithValue("@MCP_Apellido1Destinatario", convenioPeaton.PeatonDestinatario.Apellido1);
                cmd.Parameters.AddWithValue("@MCP_Apellido2Destinatario", convenioPeaton.PeatonDestinatario.Apellido2);
                cmd.Parameters.AddWithValue("@MCP_TelefonoDestinatario", convenioPeaton.PeatonDestinatario.Telefono);
                cmd.Parameters.AddWithValue("@MCP_DireccionDestinatario", convenioPeaton.PeatonDestinatario.Direccion);
                cmd.Parameters.AddWithValue("@MCP_EmailDestinatario", convenioPeaton.PeatonDestinatario.Email);
                cmd.Parameters.AddWithValue("@MCP_IdSucursalRecogida", convenioPeaton.ConvenioRemitente.IdSucursalRecogida);
                cmd.Parameters.AddWithValue("@MCP_CreadoPor", usuario);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserta la información de un envío peatón - convenio
        /// </summary>
        /// <param name="idAdmisionesMensajeria"></param>
        /// <param name="peatonConvenio"></param>
        /// <param name="usuario"></param>
        public void AdicionarPeatonConvenio(int idCliente, long idAdmisionMensajeria, ADMensajeriaTipoCliente peatonConvenio, string usuario)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paCrearAdmiPeatonConvenio_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MPC_IdAdminisionMensajeria", idAdmisionMensajeria);
                cmd.Parameters.AddWithValue("@MPC_IdConvenio", idCliente);
                cmd.Parameters.AddWithValue("@MPC_NitConvenio", peatonConvenio.ConvenioDestinatario.Nit);
                cmd.Parameters.AddWithValue("@MPC_RazonSocialConvenio", peatonConvenio.ConvenioDestinatario.RazonSocial);
                cmd.Parameters.AddWithValue("@MPC_IdTipoIdRemitente", peatonConvenio.PeatonRemitente.TipoIdentificacion);
                cmd.Parameters.AddWithValue("@MPC_IdRemitente", peatonConvenio.PeatonRemitente.Identificacion);
                cmd.Parameters.AddWithValue("@MPC_NombreRemitente", peatonConvenio.PeatonRemitente.Nombre);
                cmd.Parameters.AddWithValue("@MPC_Apellido1Remitente", peatonConvenio.PeatonRemitente.Apellido1);
                cmd.Parameters.AddWithValue("@MPC_Apellido2Remitente", peatonConvenio.PeatonRemitente.Apellido2);
                cmd.Parameters.AddWithValue("@MPC_TelefonoRemitente", peatonConvenio.PeatonRemitente.Telefono);
                cmd.Parameters.AddWithValue("@MPC_DireccionRemitente", peatonConvenio.PeatonRemitente.Direccion);
                cmd.Parameters.AddWithValue("@MPC_IdSucursal", peatonConvenio.ConvenioDestinatario.IdSucursalRecogida);
                cmd.Parameters.AddWithValue("@MPC_IdContratoConvenio", peatonConvenio.ConvenioDestinatario.Contrato);
                cmd.Parameters.AddWithValue("@MPC_EmailRemitente", peatonConvenio.PeatonRemitente.Email);
                cmd.Parameters.AddWithValue("@MPC_CreadoPor", usuario);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserta la información sde peatón - peatón
        /// </summary>
        /// <param name="peatonPeaton"></param>
        /// <param name="usuario"></param>
        public void AdicionarPeatonPeaton(long idAdmisionMensajeria, ADMensajeriaTipoCliente peatonPeaton, string usuario, long idCentroServicioOrigen, string nombreCentroServicioOrigen)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paCrearAdmiPeatonPeaton_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MPP_IdAdminisionMensajeria", idAdmisionMensajeria);
                cmd.Parameters.AddWithValue("@MPP_IdCentroServicioOrigen", idCentroServicioOrigen);
                cmd.Parameters.AddWithValue("@MPP_NombreCentroServicioOrigen", nombreCentroServicioOrigen);
                cmd.Parameters.AddWithValue("@MPP_IdTipoIdentificacionRemitente", peatonPeaton.PeatonRemitente.TipoIdentificacion);
                cmd.Parameters.AddWithValue("@MPP_IdentificacionRemitente", peatonPeaton.PeatonRemitente.Identificacion);
                cmd.Parameters.AddWithValue("@MPP_NombreRemitente", peatonPeaton.PeatonRemitente.Nombre);
                cmd.Parameters.AddWithValue("@MPP_Apellido1Remitente", peatonPeaton.PeatonRemitente.Apellido1);
                cmd.Parameters.AddWithValue("@MPP_Apellido2Remitente", peatonPeaton.PeatonRemitente.Apellido2);
                cmd.Parameters.AddWithValue("@MPP_TelefonoRemitente", peatonPeaton.PeatonRemitente.Telefono);
                cmd.Parameters.AddWithValue("@MPP_DireccionRemitente", peatonPeaton.PeatonRemitente.Direccion);
                cmd.Parameters.AddWithValue("@MPP_EmailRemitente", peatonPeaton.PeatonRemitente.Email);
                cmd.Parameters.AddWithValue("@MPP_TipoIdDestinatario", peatonPeaton.PeatonDestinatario.TipoIdentificacion);
                cmd.Parameters.AddWithValue("@MPP_IdDestinatario", peatonPeaton.PeatonDestinatario.Identificacion);
                cmd.Parameters.AddWithValue("@MPP_NombreDestinatario", peatonPeaton.PeatonDestinatario.Nombre);
                cmd.Parameters.AddWithValue("@MPP_Apellido1Destinatario", peatonPeaton.PeatonDestinatario.Apellido1);
                cmd.Parameters.AddWithValue("@MPP_Apellido2Destinatario", peatonPeaton.PeatonDestinatario.Apellido2);
                cmd.Parameters.AddWithValue("@MPP_TelefonoDestinatario", peatonPeaton.PeatonDestinatario.Telefono);
                cmd.Parameters.AddWithValue("@MPP_DireccionDestinatario", peatonPeaton.PeatonDestinatario.Direccion);
                cmd.Parameters.AddWithValue("@MPP_EmailDestinatario", peatonPeaton.PeatonDestinatario.Email);
                cmd.Parameters.AddWithValue("@MPP_CreadoPor", usuario);
                cmd.ExecuteNonQuery();
            }
        }



        /// <summary>
        /// Consulta los parámetros de configuración de admisiones
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ObtenerParametroAdmisionCache()
        {
            if (ParametrosAdmisionCache == null)
            {
                PrepararParametrosAdmisionesCache();
            }

            return ParametrosAdmisionCache;
        }

        /// <summary>
        /// Levanta diccionario a memoria con todos los parametrosFramework
        /// </summary>
        public void PrepararParametrosAdmisionesCache()
        {
            ///Contrala que cada 10 minutos se refresque la cache
            if (Math.Abs((fechaHoraCacheParametros - DateTime.Now).TotalMinutes) > 10)
            {
                lock (this)
                {
                    if (ParametrosAdmisionCache != null)
                    {
                        ParametrosAdmisionCache = null;
                    }
                }
            }

            if (ParametrosAdmisionCache == null)
            {
                lock (this)
                {
                    if (ParametrosAdmisionCache == null)
                    {
                        try
                        {
                            ParametrosAdmisionCache = new Dictionary<string, string>();
                            fechaHoraCacheParametros = DateTime.Now;
                            using (SqlConnection conn = new SqlConnection(conexionStringController))
                            {
                                SqlCommand cmd = new SqlCommand("paObtenerParametrosAdmision_MEN", conn);
                                cmd.CommandType = CommandType.StoredProcedure;
                                conn.Open();
                                SqlDataReader reader = cmd.ExecuteReader();

                                while (reader.Read())
                                {
                                    ParametrosAdmisionCache.Add(reader["PAM_IdParametro"].ToString(), reader["PAM_ValorParametro"].ToString().Trim());
                                }
                                conn.Close();
                            }
                        }
                        catch
                        {
                            ParametrosAdmisionCache = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el valor de un parámetro de admisiones
        /// </summary>
        /// <typeparam name="T">Tipo del dato que debe retornar</typeparam>
        /// <param name="contexto">Contexto de la base de datos</param>
        /// <param name="parametro">Parámetro a buscar en la tabla</param>
        /// <returns></returns>
        private T ObtenerParametro<T>(string parametro)
        {
            Dictionary<string, string> parametros = ObtenerParametroAdmisionCache();

            T porcentaje;
            string valor;
            if (!parametros.TryGetValue(parametro, out valor))
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA,
                    "EX_PARAMETRIZACION_INVALIDA" , //ADEnumTipoErrorMensajeria.EX_PARAMETRIZACION_INVALIDA.ToString(), 
                    "El módulo de Admisiones Mensajería no ha sido parametrizado correctamente"//ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_PARAMETRIZACION_INVALIDA))
                    ));
            }
            try
            {
                porcentaje = (T)Convert.ChangeType(valor, typeof(T));
                return porcentaje;
            }
            catch
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA,
                    "EX_PARAMETRIZACION_INVALIDA", //ADEnumTipoErrorMensajeria.EX_PARAMETRIZACION_INVALIDA.ToString(), 
                    "El módulo de Admisiones Mensajería no ha sido parametrizado correctamente"//ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_PARAMETRIZACION_INVALIDA))
                    ));
            }
        }

        public int PesoMinimoRotulo { get; set; }
        public ADParametrosAdmisiones ObtenerParametrosAdmisiones()
        {
            ADParametrosAdmisiones parametros = new ADParametrosAdmisiones();
            parametros.PorcentajePrimaSeguro = ObtenerParametro<decimal>("PorcPrimaSeguro");
            parametros.PesoPorDefecto = ObtenerParametro<decimal>("PesoPorDefecto");
            parametros.TopeMaxValorDeclarado = ObtenerParametro<decimal>("TopeMaxValorDeclarad");
            parametros.UnidadMedidaPorDefecto = ObtenerParametro<string>("UnidadMedidaPorDefec");
            parametros.TipoMonedaModificable = ObtenerParametro<bool>("TipMonedaModificable");
            parametros.TipoMonedaPorDefecto = ObtenerParametro<string>("TipoMonedaPorDefecto");
            parametros.PesoMinimoRotulo = ObtenerParametro<int>("PesoMinimoRotulo");
            parametros.PorcentajeRecargo = ObtenerParametro<double>("PorcentajeRecargo");
            parametros.NumeroPiezasAplicaRotulo = ObtenerParametro<int>("NumPiezasApliRotulo");

            // TODO ID: Nuevo parametro para Tope minimo de Vlr Declarado unicamente en tipo de Servicio RAPICARGA
            parametros.TopeMinVlrDeclRapiCarga = ObtenerParametro<decimal>("TopeMinVlrDeclRapiCa");

            parametros.ImagenPublicidadGuia = ObtenerParametro<string>("ImagenPublicidadGuia");
            parametros.ValorReimpresionCertificacion = ObtenerParametro<string>("ValorReimpCertifEnt");
            this.PesoMinimoRotulo = parametros.PesoMinimoRotulo;
            return parametros;

        }

        /// <summary>
        /// Servicio que consulta los estados de una cantidad parametrizada de guias asociadas a un cliente
        /// Hevelin Dayana Diaz - 11/10/2021        //07
        /// </summary>
        /// <param name="request">Objeto que contiene id de cliente y número de guias a consultar</param>
        /// <returns></returns>
        public List<EstadoGuiaCLI_MEN> ConsultarEstadosGuiaPorCliente(long idCliente, long numeroGuia)
        {
            List<EstadoGuiaCLI_MEN> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("pa_Obtener_Historico_Estado_Guias", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    resultado = new List<EstadoGuiaCLI_MEN>();

                    while (reader.Read())
                    {
                        var r = new EstadoGuiaCLI_MEN
                        {
                            IdEstadoGuia = Convert.ToInt64(reader["IdEstadoGuia"]),
                            NombreCiudadOrigen = Convert.ToString(reader["NombreCiudadOrigen"]),
                            NombreCiudadDestino = Convert.ToString(reader["NombreCiudadDestino"]),
                            NombreEstado = Convert.ToString(reader["NombreEstado"]),
                            IdClienteCredito = Convert.ToInt16(reader["IdClienteCredito"]),
                            FechaEstado = Convert.ToDateTime(reader["FechaEstado"]),
                            FechaConsulta = Convert.ToDateTime(reader["FechaConsulta"]),
                            IdLocalidadOrigen = Convert.ToString(reader["IdCiudadOrigen"]),
                            IdLocalidadDestino = Convert.ToString(reader["IdCiudadDestino"]),
                            IdLocalidadenCurso = Convert.ToString(reader["IdLocalidadenCurso"]),
                            NombreCiudadenCurso = Convert.ToString(reader["NombreCiudadenCurso"]),
                            DescripcionAsociada = Convert.ToString(reader["EstadoHomologado"])
                        };
                        resultado.Add(r);
                    }
                }
                return resultado;
            }
        }

        /// <summary>
        /// Consulta el listado de estados validados en la HU-1946 por los que ha pasado el Preenvio por Nro.Guia
        /// Mauricio Hernandez Cabrera - 13/09/2023     //03
        /// </summary>
        /// <returns>Lista de estados por los que ha pasado el Preenvio por Nro.Guia</returns>
        public List<EstadoGuiaCLI_MEN> ConsultarEstadosPreenvioPorGuia(long idCliente, long nroGuia)
        {
            List<EstadoGuiaCLI_MEN> estados = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("pa_ConsultarEstadosPreenvioTraza_PRE", sqlConn);
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                cmd.Parameters.AddWithValue("@NroGuia", nroGuia);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    AdmisionPreenvioDatos datosPreenvio = new AdmisionPreenvioDatos();
                    PreenvioAdmisionCL preenvio = datosPreenvio.ObtenerPreenvioClienteCredito(nroGuia);

                    estados = new List<EstadoGuiaCLI_MEN>();

                    while (reader.Read())
                    {
                        EstadoGuiaCLI_MEN estado = new EstadoGuiaCLI_MEN
                        {
                            IdClienteCredito = Convert.ToInt32(reader["IdClienteCredito"]),
                            IdEstadoGuia = Convert.ToInt16(reader["IdEstadoPreenvio"]),
                            NombreEstado = Convert.ToString(reader["NombreEstadoPreenvio"]),
                            FechaEstado = Convert.ToDateTime(reader["FechaGrabacionEstado"]),
                            IdLocalidadOrigen = preenvio.IdCiudadOrigen,
                            IdLocalidadDestino = preenvio.IdCiudadDestino,
                            NombreCiudadOrigen = preenvio.NombreCiudadOrigen,
                            NombreCiudadDestino = preenvio.NombreCiudadDestino
                        };

                        estados.Add(estado);
                    }
                }

                return estados;
            }
        }

        /// <summary>
        /// Consulta el listado de estados validados en la HU-1946 por los que ha pasado la Recogida por Nro.Guia
        /// Mauricio Hernandez Cabrera - 13/09/2023     //04
        /// </summary>
        /// <returns>Lista de estados por los que ha pasado la Recogida por Nro.Guia</returns>
        public List<EstadoGuiaCLI_MEN> ConsultarEstadosRecogidaPorGuia(long idCliente, long nroGuia)
        {
            List<EstadoGuiaCLI_MEN> estados = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("pa_ConsultarEstadosRecogidaTraza_REC", sqlConn);
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                cmd.Parameters.AddWithValue("@NroGuia", nroGuia);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    AdmisionPreenvioDatos datosPreenvio = new AdmisionPreenvioDatos();
                    PreenvioAdmisionCL preenvio = datosPreenvio.ObtenerPreenvioClienteCredito(nroGuia);

                    estados = new List<EstadoGuiaCLI_MEN>();

                    while (reader.Read())
                    {
                        EstadoGuiaCLI_MEN estado = new EstadoGuiaCLI_MEN
                        {
                            IdClienteCredito = Convert.ToInt32(reader["IdClienteCredito"]),
                            IdEstadoGuia = Convert.ToInt16(reader["IdEstadoRecogida"]),
                            NombreEstado = Convert.ToString(reader["DescripcionEstadoRecogida"]),
                            IdLocalidadOrigen = Convert.ToString(reader["IdLocalidadRecogida"]),
                            NombreCiudadOrigen = Convert.ToString(reader["NombreLocalidadRecogida"]),
                            FechaEstado = Convert.ToDateTime(reader["FechaGrabacionEstado"]),
                            IdLocalidadDestino = preenvio.IdCiudadDestino,
                            NombreCiudadDestino = preenvio.NombreCiudadDestino
                        };

                        estados.Add(estado);
                    }
                }

                return estados;
            }
        }

        /// <summary>
        /// Consulta el listado de estados por los que ha pasado la Guía Interna de Devolución 3mil por Nro.Guia
        /// Mauricio Hernandez Cabrera - 13/09/2023     //05
        /// </summary>
        /// <returns>Lista de estados por los que ha pasado la Guía Interna de Devolución 3mil por Nro.Guia</returns>
        public List<EstadoGuiaCLI_MEN> ConsultarEstadosGuiaDevolucionPorGuia(long numeroGuia)
        {
            List<EstadoGuiaCLI_MEN> estados = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("pa_ConsultarEstadosGuiaDevolucionTraza_MEN", sqlConn);
                cmd.Parameters.AddWithValue("@NroGuia", numeroGuia);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    estados = new List<EstadoGuiaCLI_MEN>();

                    while (reader.Read())
                    {
                        EstadoGuiaCLI_MEN estado = new EstadoGuiaCLI_MEN
                        {
                            IdEstadoGuia = Convert.ToInt16(reader["IdEstadoGuia"]),
                            NombreEstado = Convert.ToString(reader["NombreEstadoGuia"]),
                            NombreCiudadOrigen = Convert.ToString(reader["NombreCiudadOrigen"]),
                            NombreCiudadDestino = Convert.ToString(reader["NombreCiudadDestino"]),
                            FechaEstado = Convert.ToDateTime(reader["FechaGrabacionEstado"]),
                            IdLocalidadOrigen = Convert.ToString(reader["IdCiudadOrigen"]),
                            IdLocalidadDestino = Convert.ToString(reader["IdCiudadDestino"])
                        };

                        estados.Add(estado);
                    }
                }

                return estados;
            }
        }

        /// <summary>
        /// Consulta el detalle del motivo de la devolución de una guía interna por Nro.Guia
        /// Mauricio Hernandez Cabrera - 13/09/2023     //06
        /// </summary>
        /// <returns>Detalle del motivo de la devolución de una guía interna por Nro.Guia</returns>
        public MotivoDevolucionGuia_MEN ConsultarMotivoDevolucionGuia(long nroGuia)
        {
            MotivoDevolucionGuia_MEN motivoDevolucion = new MotivoDevolucionGuia_MEN();

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("pa_ConsultarGuiaMotivoDevolucion_MEN", sqlConn);
                cmd.Parameters.AddWithValue("@NroGuia", nroGuia);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    motivoDevolucion = new MotivoDevolucionGuia_MEN
                    {
                        IdEstadoGuiaLog = Convert.ToInt64(reader["IdEstadoGuiaLog"]),
                        IdMotivoGuia = Convert.ToInt32(reader["IdMotivoGuia"]),
                        FechaMotivo = Convert.ToDateTime(reader["FechaMotivo"]),
                        MotivoDevolucion = Convert.ToString(reader["MotivoDevolucion"])
                    };
                }

                return motivoDevolucion;
            }
        }

        /// <summary>
        /// Registra en IController la venta para generar factura manual en el plazo de tiempo configurado
        /// en la tabla ParametrosFramework
        /// </summary>
        /// <returns>Si se ingreso con exito el registro</returns>
        public void RegistrarVentaParaFacturaManual(long idCliente, long numeroGuia, long idContrato, long idSucursal, string idLocalidad, decimal valorTotal, short idFormaPago)
        {
            bool existeVerificacion;
            using (SqlConnection connection = new SqlConnection(conexionStringController))
            {
                SqlCommand command = new SqlCommand("paIngresarPreenvioPorFacturarAlCarrito", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdCliente", idCliente);
                command.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                command.Parameters.AddWithValue("@IdContrato", idContrato);
                command.Parameters.AddWithValue("@IdSucursal", idSucursal);
                command.Parameters.AddWithValue("@IdLocalidad", idLocalidad);
                command.Parameters.AddWithValue("@IdFormaPago", idFormaPago);
                command.Parameters.AddWithValue("@ValorNeto", valorTotal);
                connection.Open();
                var existe = command.ExecuteScalar();
                connection.Close();
            }
        }
    }
}