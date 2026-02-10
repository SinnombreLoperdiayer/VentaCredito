using ServiciosInter.DatosCompartidos.EntidadesNegocio.Clientes;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Comun;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ServiciosInter.Infraestructura.AccesoDatos.Repository.Mapper
{
    public class MensajeriaMapper
    {
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

                /* guia.FormasPago = new List<ADGuiaFormaPago>
                                                 {
                                                     new ADGuiaFormaPago {
                                                           Descripcion =  reader["formapago"] == DBNull.Value ? string.Empty : Convert.ToString(reader["formapago"])
                                                     }
                                                 };*/
            }
            return guia;
        }

        /// <summary>
        /// Metodo para obtener una lista de las guias de sispostal consultadas desde el portal.
        /// </summary>
        /// <param sqlDataReader="reader"></param>
        /// <returns></returns>
        internal static ADGuiaClienteRespuesta ToListGuiaSispostalPorPortal(SqlDataReader reader, bool EncriptaAes = false)
        {
            ADGuiaClienteRespuesta guia = new ADGuiaClienteRespuesta();
            if (reader.Read())
            {
        
                if (!EncriptaAes)
                {
                    guia.Destinatario = new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = null,
                        Telefono = reader["telefono"] == DBNull.Value ? string.Empty : Cifrado.Encrypt(Convert.ToString(reader["telefono"])),
                        Identificacion = Cifrado.Encrypt(""),
                        Direccion = null,
                        TipoId = null,

                    };
                    guia.Remitente = new CLClienteContadoClienteRespuesta()
                    {
                        //HABEAS DATA - SOLO SE DEBE VISUALIZAR RAZON SOCIAL DEL REMITENTE, DIRECCIÓN, NIT
                        Nombre = null,
                        Telefono = Cifrado.Encrypt("-"),
                        Identificacion = reader["Nit"] == DBNull.Value ? string.Empty : Cifrado.Encrypt(reader["Nit"].ToString()),
                        Direccion = null,
                        TipoId = null,
                    };
                }
                else
                {
                    guia.Destinatario = new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = null,
                        Telefono = reader["telefono"] == DBNull.Value ? string.Empty : Cifrado.EncriptarTexto(Convert.ToString(reader["telefono"])),
                        Identificacion = Cifrado.Encrypt(""),
                        Direccion = null,
                        TipoId = null,

                    };
                    guia.Remitente = new CLClienteContadoClienteRespuesta()
                    {
                        //HABEAS DATA - SOLO SE DEBE VISUALIZAR RAZON SOCIAL DEL REMITENTE, DIRECCIÓN, NIT
                        Nombre = null,
                        Telefono = Cifrado.EncriptarTexto("-"),
                        Identificacion = reader["Nit"] == DBNull.Value ? string.Empty : Cifrado.EncriptarTexto(reader["Nit"].ToString()),
                        Direccion = null,
                        TipoId = null,
                    };
                }
                
                guia.NumeroGuia = Convert.ToInt64(reader["guia"]);
                guia.NombreCiudadOrigen = reader["CiudadRemite"] == DBNull.Value ? string.Empty : reader["CiudadRemite"].ToString();
                guia.NombreCiudadDestino = reader["NombreLocalidad"] == DBNull.Value ? string.Empty : Convert.ToString(reader["NombreLocalidad"]);
                guia.FechaAdmision = Convert.ToDateTime(reader["fecha"]);
                guia.NombreTipoEnvio = "";
                guia.Peso = reader["peso"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["peso"]);
                guia.NumeroBolsaSeguridad = "";
                guia.DiceContener = reader["DiceContener"] == DBNull.Value ? string.Empty : Convert.ToString(reader["DiceContener"]);
                guia.Observaciones = reader["producto"] == DBNull.Value ? string.Empty : reader["producto"].ToString();
                guia.NombreServicio = reader["servicio"] == DBNull.Value ? string.Empty : Convert.ToString(reader["servicio"]);
            }
            return guia;
        }

        /// <summary>
        /// Metodo para obtener una lista de las guias de sispostal consultadas desde el portal cuando pertenece al remitente o destinatario.
        /// </summary>
        /// <param sqlDataReader="reader"></param>
        /// <returns></returns>
        internal static ADGuiaClienteRespuesta ToListGuiaSispostalPertenencia(SqlDataReader reader, bool EncriptaAes=false)
        {
            ADGuiaClienteRespuesta guia = new ADGuiaClienteRespuesta();
            if (reader.Read())
            {

                if (!EncriptaAes)
                {
                    guia.Destinatario = new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = reader["nombre"] == DBNull.Value ? string.Empty : Cifrado.Encrypt(Convert.ToString(reader["nombre"])),
                        Telefono = reader["telefono"] == DBNull.Value ? string.Empty : Cifrado.Encrypt(Convert.ToString(reader["telefono"])),
                        Identificacion = Cifrado.Encrypt(""),
                        Direccion = reader["direccion"] == DBNull.Value ? string.Empty : Cifrado.Encrypt(Convert.ToString(reader["direccion"])),
                        TipoId = Cifrado.Encrypt(""),

                    };
                    guia.Remitente = new CLClienteContadoClienteRespuesta()
                    {
                        //HABEAS DATA - SOLO SE DEBE VISUALIZAR RAZON SOCIAL DEL REMITENTE, DIRECCIÓN, NIT
                        Nombre = reader["NombreREmite"] == DBNull.Value ? string.Empty : Cifrado.Encrypt(Convert.ToString(reader["NombreREmite"])),
                        Telefono = Cifrado.Encrypt("-"),
                        Identificacion = reader["Nit"] == DBNull.Value ? string.Empty : Cifrado.Encrypt(reader["Nit"].ToString()),
                        Direccion = reader["DireccionRemite"] == DBNull.Value ? string.Empty : Cifrado.Encrypt(Convert.ToString(reader["DireccionRemite"])),
                        TipoId = Cifrado.Encrypt("NIT"),
                    };
                    guia.NumeroGuia = Convert.ToInt64(reader["guia"]);
                    guia.NombreCiudadOrigen = reader["CiudadRemite"] == DBNull.Value ? string.Empty : reader["CiudadRemite"].ToString();
                    guia.NombreCiudadDestino = reader["NombreLocalidad"] == DBNull.Value ? string.Empty : Convert.ToString(reader["NombreLocalidad"]);
                    guia.FechaAdmision = Convert.ToDateTime(reader["fecha"]);
                    guia.NombreTipoEnvio = "";
                    guia.Peso = reader["peso"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["peso"]);
                    guia.NumeroBolsaSeguridad = "";
                    guia.DiceContener = reader["DiceContener"] == DBNull.Value ? string.Empty : Convert.ToString(reader["DiceContener"]);
                    guia.Observaciones = reader["producto"] == DBNull.Value ? string.Empty : reader["producto"].ToString();
                    guia.NombreServicio = reader["servicio"] == DBNull.Value ? string.Empty : Convert.ToString(reader["servicio"]);
                }
                else
                {
                    guia.Destinatario = new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = reader["nombre"] == DBNull.Value ? string.Empty : Cifrado.EncriptarTexto(Convert.ToString(reader["nombre"])),
                        Telefono = reader["telefono"] == DBNull.Value ? string.Empty : Cifrado.EncriptarTexto(Convert.ToString(reader["telefono"])),
                        Identificacion = Cifrado.EncriptarTexto(""),
                        Direccion = reader["direccion"] == DBNull.Value ? string.Empty : Cifrado.EncriptarTexto(Convert.ToString(reader["direccion"])),
                        TipoId = Cifrado.EncriptarTexto(""),

                    };
                    guia.Remitente = new CLClienteContadoClienteRespuesta()
                    {
                        //HABEAS DATA - SOLO SE DEBE VISUALIZAR RAZON SOCIAL DEL REMITENTE, DIRECCIÓN, NIT
                        Nombre = reader["NombreREmite"] == DBNull.Value ? string.Empty : Cifrado.EncriptarTexto(Convert.ToString(reader["NombreREmite"])),
                        Telefono = Cifrado.EncriptarTexto("-"),
                        Identificacion = reader["Nit"] == DBNull.Value ? string.Empty : Cifrado.EncriptarTexto(reader["Nit"].ToString()),
                        Direccion = reader["DireccionRemite"] == DBNull.Value ? string.Empty : Cifrado.EncriptarTexto(Convert.ToString(reader["DireccionRemite"])),
                        TipoId = Cifrado.EncriptarTexto("NIT"),
                    };
                    guia.NumeroGuia = Convert.ToInt64(reader["guia"]);
                    guia.NombreCiudadOrigen = reader["CiudadRemite"] == DBNull.Value ? string.Empty : reader["CiudadRemite"].ToString();
                    guia.NombreCiudadDestino = reader["NombreLocalidad"] == DBNull.Value ? string.Empty : Convert.ToString(reader["NombreLocalidad"]);
                    guia.FechaAdmision = Convert.ToDateTime(reader["fecha"]);
                    guia.NombreTipoEnvio = "";
                    guia.Peso = reader["peso"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["peso"]);
                    guia.NumeroBolsaSeguridad = "";
                    guia.DiceContener = reader["DiceContener"] == DBNull.Value ? string.Empty : Convert.ToString(reader["DiceContener"]);
                    guia.Observaciones = reader["producto"] == DBNull.Value ? string.Empty : reader["producto"].ToString();
                    guia.NombreServicio = reader["servicio"] == DBNull.Value ? string.Empty : Convert.ToString(reader["servicio"]);

                }
               
            }
            return guia;
        }
    }
}