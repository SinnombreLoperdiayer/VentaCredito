using CO.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Consultas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace CO.Servidor.Raps.Datos
{
    public class RARepositorioSolicitudesMapper
    {

        internal static List<RAGestionDC> MapperListaGestion(SqlDataReader reader)
        {
            List<RAGestionDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAGestionDC>();
                }
                var r = new RAGestionDC
                {
                    IdGestion = Convert.ToInt64(reader["IdGestion"]),
                    IdSolicitud = Convert.ToInt64(reader["IdSolicitud"]),
                    Comentario = Convert.ToString(reader["Comentario"]),
                    IdCargoGestiona = reader["IdCargoGestiona"].ToString(),
                    CorreoEnvia = Convert.ToString(reader["CorreoEnvia"]),
                    //IdAccion = MapperAccion.ToEnumAccion(Convert.ToByte(reader["IdAccion"])),
                    IdCargoDestino = reader["IdCargoDestino"].ToString(),
                    CorreoDestino = Convert.ToString(reader["CorreoDestino"]),
                    IdResponsable = reader["IdResponsable"].ToString(),
                    IdEstado = MapperEstado.ToEnumEstados(Convert.ToInt32(reader["IdEstado"])),
                    IdUsuario = Convert.ToString(reader["CreadoPor"]),
                    DocumentoSolicita = Convert.ToString(reader["DocumentoSolicita"]),
                    DocumentoResponsable = Convert.ToString(reader["DocumentoResponsable"]),
                    ListaAdjuntos = RARepositorioSolicitudes.Instancia.ObtenerAdjuntosPorGestion(Convert.ToInt64(reader["IdGestion"]))
                };

                if (!DBNull.Value.Equals(reader["FechaGrabacion"]))
                {
                    r.Fecha = Convert.ToDateTime(reader["FechaGrabacion"]);
                }

                if (!DBNull.Value.Equals(reader["FechaVencimiento"]))
                {
                    r.FechaVencimiento = Convert.ToDateTime(reader["FechaVencimiento"]);
                }

                if (!DBNull.Value.Equals(reader["DescripcionCargoGestiona"]))
                {
                    r.DescripcionCargoGestiona = Convert.ToString(reader["DescripcionCargoGestiona"]);
                }

                if (!DBNull.Value.Equals(reader["DescripcionCargoDestino"]))
                {
                    r.DescripcionCargoDestino = Convert.ToString(reader["DescripcionCargoDestino"]);
                }

                if (!DBNull.Value.Equals(reader["NombreResponsable"]))
                {
                    r.NombreResponsable = Convert.ToString(reader["NombreResponsable"]);
                }

                if (!DBNull.Value.Equals(reader["NombreSolicita"]))
                {
                    r.NombreSolicita = Convert.ToString(reader["NombreSolicita"]);
                }

                if (!DBNull.Value.Equals(reader["SucursalSolicita"]))
                {
                    r.SucursalSolicita = Convert.ToString(reader["SucursalSolicita"]);
                }

                if (!DBNull.Value.Equals(reader["ProcesoSolicita"]))
                {
                    r.ProcesoSolicita = Convert.ToString(reader["ProcesoSolicita"]);
                }

                if (!DBNull.Value.Equals(reader["ProcedimientoSolicita"]))
                {
                    r.ProcedimientoSolicita = Convert.ToString(reader["ProcedimientoSolicita"]);
                }

                if (!DBNull.Value.Equals(reader["SucursalResponsable"]))
                {
                    r.SucursalResponsable = Convert.ToString(reader["SucursalResponsable"]);
                }

                if (!DBNull.Value.Equals(reader["ProcesoResponsable"]))
                {
                    r.ProcesoResponsable = Convert.ToString(reader["ProcesoResponsable"]);
                }

                if (!DBNull.Value.Equals(reader["ProcedimientoResponsable"]))
                {
                    r.ProcedimientoResponsable = Convert.ToString(reader["ProcedimientoResponsable"]);
                }

                if (!DBNull.Value.Equals(reader["CodigoPlantaSolicita"]))
                {
                    r.CodigoPlantaSolicita = Convert.ToString(reader["CodigoPlantaSolicita"]);
                }

                if (!DBNull.Value.Equals(reader["CodigoPlantaResponsable"]))
                {
                    r.CodigoPlantaResponsable = Convert.ToString(reader["CodigoPlantaResponsable"]);
                }

                resultado.Add(r);
            }

            return resultado;
        }


        internal static RAGestionDC MapperAGestion(SqlDataReader reader)
        {
            RAGestionDC resultado = null;
            if (reader.Read())
            {
                var r = new RAGestionDC
                {

                    IdGestion = Convert.ToInt64(reader["IdGestion"]),
                    IdSolicitud = Convert.ToInt64(reader["IdSolicitud"]),
                    Comentario = Convert.ToString(reader["Comentario"]),
                    IdCargoGestiona = reader["IdCargoGestiona"].ToString(),
                    CorreoEnvia = Convert.ToString(reader["CorreoEnvia"]),
                    //IdAccion = MapperAccion.ToEnumAccion(Convert.ToByte(reader["IdAccion"])),
                    IdCargoDestino = reader["IdCargoDestino"].ToString(),
                    CorreoDestino = Convert.ToString(reader["CorreoDestino"]),
                    IdResponsable = reader["IdResponsable"].ToString(),
                    IdEstado = MapperEstado.ToEnumEstados(Convert.ToInt32(reader["IdEstado"])),
                    IdUsuario = Convert.ToString(reader["CreadoPor"]),
                };

                if (!DBNull.Value.Equals(reader["Fecha"]))
                {
                    r.Fecha = Convert.ToDateTime(reader["FechaGrabacion"]);
                }

                if (!DBNull.Value.Equals(reader["FechaVencimiento"]))
                {
                    r.FechaVencimiento = Convert.ToDateTime(reader["FechaGrabacion"]);
                }

                resultado = r;
            }

            return resultado;
        }


        internal static RAPantillaAccionCorreoDC MapperAPantillaAccionCorreo(SqlDataReader reader)
        {
            RAPantillaAccionCorreoDC resultado = null;
            if (reader.Read())
            {
                var r = new RAPantillaAccionCorreoDC
                {
                    IdPlantilla = Convert.ToInt64(reader["IdPlantilla"]),
                    IdAccion = Convert.ToByte(reader["IdAccion"]),
                    Asunto = Convert.ToString(reader["Asunto"]),
                    Cuerpo = Convert.ToString(reader["Cuerpo"]),
                    EsPredeterminada = Convert.ToBoolean(reader["EsPredeterminada"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };
                resultado = r;
            }
            return resultado;
        }

        internal static List<RASolicitudDC> MapperListaSolicitud(SqlDataReader reader)
        {
            List<RASolicitudDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RASolicitudDC>();
                }
                var r = new RASolicitudDC
                {
                    IdSolicitud = Convert.ToInt64(reader["IdSolicitud"]),
                    IdParametrizacionRap = Convert.ToInt64(reader["IdParametrizacionRap"]),
                    IdCargoSolicita = reader["IdCargoSolicita"].ToString(),
                    IdCargoResponsable = reader["IdCargoResponsable"].ToString(),
                    IdEstado = MapperEstado.ToEnumEstados(Convert.ToInt32(reader["IdEstado"])),
                    Descripcion = Convert.ToString(reader["Descripcion"]),


                    IdSolicitudPadre = Convert.ToInt64(reader["IdSolicitudPadre"]),
                };

                if (!DBNull.Value.Equals(reader["FechaCreacion"]))
                {
                    r.FechaCreacion = Convert.ToDateTime(reader["FechaGrabacion"]);
                }

                if (!DBNull.Value.Equals(reader["FechaVencimiento"]))
                {
                    r.FechaVencimiento = Convert.ToDateTime(reader["FechaGrabacion"]);
                }

                resultado.Add(r);
            }

            return resultado;
        }


        internal static RASolicitudDC MapperASolicitud(SqlDataReader reader)
        {
            RASolicitudDC resultado = null;
            if (reader.Read())
            {
                var r = new RASolicitudDC
                {
                    IdSolicitud = Convert.ToInt64(reader["IdSolicitud"]),
                    IdParametrizacionRap = Convert.ToInt64(reader["IdParametrizacionRap"]),
                    IdCargoSolicita = reader["IdCargoSolicita"].ToString(),
                    IdCargoResponsable = reader["IdCargoResponsable"].ToString(),
                    IdEstado = MapperEstado.ToEnumEstados(Convert.ToInt32(reader["IdEstado"])),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    IdSolicitudPadre = Convert.ToInt64(reader["IdSolicitudPadre"]),
                    Anchor = Convert.ToBase64String((Byte[])reader["Anchor"])
                };

                if (!DBNull.Value.Equals(reader["FechaCreacion"]))
                {
                    r.FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]);
                }

                if (!DBNull.Value.Equals(reader["FechaVencimiento"]))
                {
                    r.FechaVencimiento = Convert.ToDateTime(reader["FechaVencimiento"]);
                }

                if (!DBNull.Value.Equals(reader["DocumentoResponsable"]))
                {
                    r.DocumentoResponsable = reader["DocumentoResponsable"].ToString();
                }

                resultado = r;
            }

            return resultado;
        }


        internal static List<RAResultadoConsultaSolicitudesDC> MapperAResultadoConsultaSolicitudes(SqlDataReader reader)
        {
            List<RAResultadoConsultaSolicitudesDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAResultadoConsultaSolicitudesDC>();
                }
                var r = new RAResultadoConsultaSolicitudesDC
                {
                    IdSolicitud = Convert.ToInt64(reader["IdSolicitud"]),
                    IdParametrizacionRap = Convert.ToInt64(reader["IdParametrizacionRap"]),
                    IdCargoSolicita = Convert.ToInt32(reader["IdCargoSolicita"]),
                    IdCargoResponsable = Convert.ToInt32(reader["IdCargoResponsable"]),
                    IdEstado = Convert.ToInt32(reader["IdEstado"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    IdResponsableInicial = Convert.ToInt32(reader["IdResponsableInicial"]),
                    IdSolicitudPadre = Convert.ToInt64(reader["IdSolicitudPadre"]),
                    Solicita = Convert.ToString(reader["Solicita"]),
                    Proceso = Convert.ToString(reader["Proceso"]),
                    Tipo = Convert.ToString(reader["Tipo"]),
                    Registro = Convert.ToInt64(reader["Registro"]),
                    TotalPaginas = Convert.ToInt32(reader["TotalPaginas"]),
                };

                if (!DBNull.Value.Equals(reader["FechaVencimientoInicial"]))
                {
                    r.FechaVencimientoInicial = Convert.ToDateTime(reader["FechaVencimientoInicial"]);
                }

                if (!DBNull.Value.Equals(reader["FechaCreacion"]))
                {
                    r.FechaCreacion = Convert.ToDateTime(reader["FechaGrabacion"]);
                }

                if (!DBNull.Value.Equals(reader["FechaVencimiento"]))
                {
                    r.FechaVencimiento = Convert.ToDateTime(reader["FechaVencimiento"]);
                }

                resultado.Add(r);
            }

            return resultado;
        }


        internal static RASolicitudConsultaDC MapperAResultadoConsultaSolicitud(SqlDataReader reader)
        {
            RASolicitudConsultaDC resultado = null;

            if (reader.Read())
            {

                resultado = new RASolicitudConsultaDC
                {
                    IdSolicitud = Convert.ToInt64(reader["IdSolicitud"]),
                    IdParametrizacionRap = Convert.ToInt64(reader["IdParametrizacionRap"]),
                    IdCargoSolicita = reader["IdCargoSolicita"].ToString(),
                    IdCargoResponsable = reader["IdCargoResponsable"].ToString(),
                    IdEstado = Convert.ToInt32(reader["IdEstado"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    IdSolicitudPadre = Convert.ToInt64(reader["IdSolicitudPadre"]),
                    DocumentoSolicita = Convert.ToString(reader["DocumentoSolicita"]),
                    NombreSolicita = Convert.ToString(reader["NombreSolicita"]),
                    DocumentoResponsable = Convert.ToString(reader["DocumentoResponsable"]),
                    NombreResponsable = Convert.ToString(reader["NombreResponsable"]),
                    Anchor = Convert.ToBase64String((Byte[])reader["Anchor"])
                };


                if (!DBNull.Value.Equals(reader["NombreParametrizacion"]))
                {
                    resultado.NombreParametrizacion = Convert.ToString(reader["NombreParametrizacion"]);
                }

                if (!DBNull.Value.Equals(reader["CodigoSucursal"]))
                {
                    resultado.CodigoSucursal = Convert.ToString(reader["CodigoSucursal"]);
                }

                if (!DBNull.Value.Equals(reader["FechaVencimiento"]))
                {
                    resultado.FechaVencimiento = Convert.ToDateTime(reader["FechaVencimiento"]);
                }

                if (!DBNull.Value.Equals(reader["FechaCreacion"]))
                {
                    resultado.FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]);
                }

                if (!DBNull.Value.Equals(reader["SucursalSolicita"]))
                {
                    resultado.SucursalSolicita = Convert.ToString(reader["SucursalSolicita"]);
                }

                if (!DBNull.Value.Equals(reader["ProcesoSolicita"]))
                {
                    resultado.ProcesoSolicita = Convert.ToString(reader["ProcesoSolicita"]);
                }

                if (!DBNull.Value.Equals(reader["ProcedimientoSolicita"]))
                {
                    resultado.ProcedimientoSolicita = Convert.ToString(reader["ProcedimientoSolicita"]);
                }

            }
            return resultado;
        }


        internal static List<RAAdjuntoDC> MapperListaAdjunto(SqlDataReader reader)
        {
            List<RAAdjuntoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAAdjuntoDC>();
                }
                var r = new RAAdjuntoDC
                {
                    IdAdjunto = Convert.ToInt32(reader["IdAdjunto"]),
                    IdGestion = Convert.ToInt64(reader["IdGestion"]),
                    Tamaño = Convert.ToDecimal(reader["Tamaño"]),
                    Extension = Convert.ToString(reader["Extension"]),
                    NombreArchivo = reader["UbicacionNombre"].ToString().Split('\\')[reader["UbicacionNombre"].ToString().Split('\\').Length - 1],
                    UbicacionNombre = Convert.ToString(reader["UbicacionNombre"])
                };
                using (FileStream fs = File.OpenRead(reader["UbicacionNombre"].ToString()))
                {
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                    fs.Close();
                    r.AdjuntoBase64 = Convert.ToBase64String(bytes);
                }
                resultado.Add(r);
            }
            return resultado;
        }


        internal static RAAdjuntoDC MapperAAdjunto(SqlDataReader reader)
        {
            RAAdjuntoDC resultado = null;
            if (reader.Read())
            {
                var r = new RAAdjuntoDC
                {
                    IdAdjunto = Convert.ToInt32(reader["IdAdjunto"]),
                    IdGestion = Convert.ToInt64(reader["IdGestion"]),
                    Tamaño = Convert.ToDecimal(reader["Tamaño"]),
                    Extension = Convert.ToString(reader["Extension"]),
                    UbicacionNombre = Convert.ToString(reader["UbicacionNombre"]),
                };

                resultado = r;
            }

            return resultado;
        }


        internal static RAIdentificaEmpleadoDC MapperAIdentificaEmpleado(SqlDataReader reader)
        {
            RAIdentificaEmpleadoDC resultado = null;
            if (reader.Read())
            {
                var r = new RAIdentificaEmpleadoDC
                {
                    CodigoSucursal = Convert.ToString(reader["CodigoSucursal"]),
                    CodigoEmpleado = Convert.ToString(reader["CodigoEmpleado"]),
                    NumeroIdentificacion = Convert.ToString(reader["NumeroIdentificacion"]),
                    Nombre = Convert.ToString(reader["Nombre"]),
                    email = Convert.ToString(reader["email"]),
                };

                resultado = r;
            }

            return resultado;
        }

        internal static List<RAConteoEstadosSolicitante> MapperConteoEstadosSolicitudes(SqlDataReader reader)
        {
            List<RAConteoEstadosSolicitante> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAConteoEstadosSolicitante>();
                }
                var r = new RAConteoEstadosSolicitante
                {
                    Tipo = Convert.ToString(reader["Tipo"]),
                    Estado = MapperEstadoAplicacion.ToEnumEstadosAplicacion(Convert.ToInt32(reader["IdEstado"])),
                    Cantidad = Convert.ToDecimal(reader["Cantidad"]),
                };

                r.Descripcion = Enum.GetName(typeof(EnumEstadosAplicacion), r.Estado);

                resultado.Add(r);
            }
            return resultado;
        }

        internal static List<RAObtenerListaSolicitudesRaps> MapperObtenerListaSolicitudesRaps(SqlDataReader reader)
        {
            List<RAObtenerListaSolicitudesRaps> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAObtenerListaSolicitudesRaps>();
                }

                var r = new RAObtenerListaSolicitudesRaps
                {
                    IdEstado = Convert.ToInt32(reader["IdEstado"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                };

                if (!DBNull.Value.Equals(reader["FechaCreacion"]))
                {
                    r.FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]);
                }

                if (!DBNull.Value.Equals(reader["FechaVencimiento"]))
                {
                    r.FechaVencimiento = Convert.ToDateTime(reader["FechaVencimiento"]);
                }
                resultado.Add(r);
            }
            return resultado;
        }

        internal static List<RASolicitudItemDC> MapperListaSolicitudItem(SqlDataReader reader)
        {
            List<RASolicitudItemDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RASolicitudItemDC>();
                }
                var r = new RASolicitudItemDC
                {
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    IdSolicitud = Convert.ToInt64(reader["IdSolicitud"]),
                };

                if (!DBNull.Value.Equals(reader["FechaCreacion"]))
                {
                    r.FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]);
                }

                if (!DBNull.Value.Equals(reader["FechaVencimiento"]))
                {
                    r.FechaVencimiento = Convert.ToDateTime(reader["FechaVencimiento"]);
                }

                resultado.Add(r);
            }
            return resultado;
        }     
    }
}
