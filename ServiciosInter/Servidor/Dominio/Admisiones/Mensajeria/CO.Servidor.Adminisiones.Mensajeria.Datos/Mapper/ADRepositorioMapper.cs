using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.MensajeriaNN;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CO.Servidor.Adminisiones.Mensajeria.Datos.Mapper
{
    public class ADRepositorioMapper
    {
        internal static List<ADGuia> ToListAdmisionesSinEntregar(SqlDataReader reader)
        {
            List<ADGuia> guias = null;
            while (reader.Read())
            {
                if (guias == null)
                {
                    guias = new List<ADGuia>();
                }
                guias.Add(new ADGuia
                {
                    IdAdmision = reader["IdAdminisionMensajeria"] == DBNull.Value ? 0 : Convert.ToInt64(reader["IdAdminisionMensajeria"]),
                    NumeroGuia = reader["NumeroGuia"] == DBNull.Value ? 0 : Convert.ToInt64(reader["NumeroGuia"]),
                    IdCentroServicioOrigen = reader["IdCentroServicioOrigen"] == DBNull.Value ? 0 : Convert.ToInt64(reader["IdCentroServicioOrigen"]),
                    IdCentroServicioDestino = reader["IdCentroServicioDestino"] == DBNull.Value ? 0 : Convert.ToInt64(reader["IdCentroServicioDestino"]),
                    NombreCentroServicioOrigen = reader["NombreCentroServicioOrigen"] == DBNull.Value ? string.Empty : reader["NombreCentroServicioOrigen"].ToString(),
                    NombreCentroServicioDestino = reader["NombreCentroServicioDestino"] == DBNull.Value ? string.Empty : reader["NombreCentroServicioDestino"].ToString(),
                    TelefonoDestinatario = reader["TelefonoDestinatario"] == DBNull.Value ? string.Empty : reader["TelefonoDestinatario"].ToString(),
                    DireccionDestinatario = reader["DireccionDestinatario"] == DBNull.Value ? string.Empty : reader["DireccionDestinatario"].ToString(),
                    DiceContener = reader["DiceContener"] == DBNull.Value ? string.Empty : reader["DiceContener"].ToString(),
                    Remitente = new CLClienteContadoDC
                    {
                        Nombre = reader["NombreRemitente"] == DBNull.Value ? string.Empty : reader["NombreRemitente"].ToString(),
                        Telefono = reader["TelefonoRemitente"] == DBNull.Value ? string.Empty : reader["TelefonoRemitente"].ToString(),
                        Identificacion = reader["IdRemitente"] == DBNull.Value ? string.Empty : reader["IdRemitente"].ToString(),
                        Direccion = reader["DireccionRemitente"] == DBNull.Value ? string.Empty : reader["DireccionRemitente"].ToString(),
                    },
                    Destinatario = new CLClienteContadoDC
                    {
                        Nombre = reader["NombreDestinatario"] == DBNull.Value ? string.Empty : reader["NombreDestinatario"].ToString(),
                        Telefono = reader["TelefonoDestinatario"] == DBNull.Value ? string.Empty : reader["TelefonoDestinatario"].ToString(),
                        Identificacion = reader["IdDestinatario"] == DBNull.Value ? string.Empty : reader["IdDestinatario"].ToString(),
                        Direccion = reader["DireccionDestinatario"] == DBNull.Value ? string.Empty : reader["DireccionDestinatario"].ToString(),
                    },

                    NumeroBolsaSeguridad = reader["NumeroBolsaSeguridad"] == DBNull.Value ? string.Empty : reader["NumeroBolsaSeguridad"].ToString(),
                    NumeroPieza = reader["NumeroPieza"] == DBNull.Value ? Convert.ToInt16(0) : Convert.ToInt16(reader["NumeroPieza"]),


                    EstadoGuia = (ADEnumEstadoGuia)Enum.Parse(typeof(ADEnumEstadoGuia), Convert.ToString(reader["IdEstadoGuia"]).Trim(), true),
                    ObservacionEstadoGuia = reader["DescripcionEstado"] == DBNull.Value ? string.Empty : Convert.ToString(reader["DescripcionEstado"]),
                    TotalPaginas = reader["TotalPaginas"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalPaginas"]),
                    Peso= reader["Peso"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Peso"]),
                    FormasPagoDescripcion = reader["DescripcionFormaPago"] == DBNull.Value ? string.Empty : (reader["DescripcionFormaPago"]).ToString(),

                });
            }
            return guias;
        }

        internal static ADGuia ToListGuiaSispostal(SqlDataReader reader)
        {
            ADGuia guia = new ADGuia();
            if (reader.Read())
            {
                guia.IdTrazaGuia = reader["IdEstado"] == DBNull.Value ? 0 : Convert.ToInt64(reader["IdEstado"]);
                guia.NumeroGuia = Convert.ToInt64(reader["guia"]);
                guia.IdCiudadDestino = reader["IdLocalidad"] == DBNull.Value ? string.Empty : Convert.ToString(reader["IdLocalidad"]);
                guia.NombreCiudadDestino = reader["NombreLocalidad"] == DBNull.Value ? string.Empty : Convert.ToString(reader["NombreLocalidad"]);
                guia.ValorDeclarado = reader["valorDeclarado"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["valorDeclarado"]);
                guia.FechaAdmision = Convert.ToDateTime(reader["fecha"]);
                guia.Peso = reader["peso"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["peso"]);
                guia.NombreServicio = reader["servicio"] == DBNull.Value ? string.Empty : Convert.ToString(reader["servicio"]);
                guia.PrefijoNumeroGuia = reader["Prefijo"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Prefijo"]);
                guia.TelefonoDestinatario = reader["telefono"] == DBNull.Value ? string.Empty : Convert.ToString(reader["telefono"]);
                guia.DireccionDestinatario = reader["direccion"] == DBNull.Value ? string.Empty : Convert.ToString(reader["direccion"]);
                guia.Destinatario = new CLClienteContadoDC()
                {
                    Nombre = reader["nombre"] == DBNull.Value ? string.Empty : Convert.ToString(reader["nombre"]),
                    Direccion = reader["direccion"] == DBNull.Value ? string.Empty : Convert.ToString(reader["direccion"]),
                    Telefono = reader["telefono"] == DBNull.Value ? string.Empty : Convert.ToString(reader["telefono"]),
                    TipoId = "",
                    Identificacion = "",
                    Email = "",
                };
                guia.Remitente = new CLClienteContadoDC()
                {
                    //HABEAS DATA - SOLO SE DEBE VISUALIZAR RAZON SOCIAL DEL REMITENTE, DIRECCIÓN, NIT 
                    Nombre = reader["NombreREmite"] == DBNull.Value ? string.Empty : Convert.ToString(reader["NombreREmite"]),
                    Direccion = reader["DireccionRemite"] == DBNull.Value ? string.Empty : Convert.ToString(reader["DireccionRemite"]),
                    //Telefono = reader["TelefonoRemite"] == DBNull.Value ? string.Empty : Convert.ToString(reader["TelefonoRemite"]),
                    Telefono = "-",
                    TipoId = "NIT",
                    Identificacion = reader["Nit"] == DBNull.Value ? string.Empty : reader["Nit"].ToString(),
                    Email = reader["rep_mail"] == DBNull.Value ? string.Empty : reader["rep_mail"].ToString(),
                };
                guia.DiceContener = reader["DiceContener"] == DBNull.Value ? string.Empty : Convert.ToString(reader["DiceContener"]);
                guia.NombreMensajero = reader["operario"] == DBNull.Value ? string.Empty : Convert.ToString(reader["operario"]);
                guia.DiasDeEntrega = reader["dias_entrega"] == DBNull.Value ? Convert.ToInt16(0) : Convert.ToInt16(reader["dias_entrega"]);
                guia.Alto = reader["alto"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["alto"]);
                guia.Ancho = reader["ancho"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ancho"]);
                guia.Largo = reader["largo"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["largo"]);
                guia.FechaEstimadaEntrega = Convert.ToDateTime(reader["FechaEstimadaEntrega"]);
                guia.IdPaisDestino = "057";
                guia.NombrePaisOrigen = "Colombia";
                guia.CodigoPostalDestino = reader["CodigoPostal"] == DBNull.Value ? string.Empty : Convert.ToString(reader["CodigoPostal"]);
                guia.CreadoPor = reader["CreadoPor"] == DBNull.Value ? string.Empty : Convert.ToString(reader["CreadoPor"]);
                guia.ValorPrimaSeguro = reader["valorPrima"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["valorPrima"]);
                guia.FechaEntrega = reader["fechentre"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["fechentre"]);
                guia.Observaciones = reader["producto"] == DBNull.Value ? string.Empty : reader["producto"].ToString();
                guia.NombreCiudadOrigen = reader["CiudadRemite"] == DBNull.Value ? string.Empty : reader["CiudadRemite"].ToString();
                guia.NombreTipoEnvio = "";
                guia.NumeroBolsaSeguridad = "";

                guia.FormasPago = new List<ADGuiaFormaPago>
                                                {
                                                    new ADGuiaFormaPago {
                                                          Descripcion =  reader["formapago"] == DBNull.Value ? string.Empty : Convert.ToString(reader["formapago"])
                                                    }
                                                };

            }
            return guia;
        }

    }
}
