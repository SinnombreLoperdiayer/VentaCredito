using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Servicio.Entidades.Tarifas;
using Servicio.Entidades.Tarifas.Precios;
using Servicio.Entidades.Tarifas.Trayectos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Tarifas.Comun;
using VentaCredito.Tarifas.Datos.ContratosDatos;

namespace VentaCredito.Tarifas.Datos.Repositorio
{
    public class PrecioMensajeria
    {
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;



        private static PrecioMensajeria instancia = new PrecioMensajeria();

        public static PrecioMensajeria Instancia
        {
            get
            {
                return instancia;
            }
        }

        

        //public CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios.TAPrecioMensajeriaDC ObtenerPrecioMensajeria(int sERVICIO_CARGA_AEREA, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega)
        //{
        //    throw new NotImplementedException();
        //}
        /// <summary>
        /// Retorna el valor de mensajeria
        /// </summary>
        /// <returns></returns>
        public TAPrecioMensajeriaDC ObtenerPrecioMensajeria(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
        {
            TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC();

            var precioTrayecto = ObtenerListaPrecioTrayecto(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino);

            var excepciones = ObtenerExcepcionesTrayecto(idServicio, idLocalidadOrigen, idLocalidadDestino, idListaPrecio).FirstOrDefault();

            if (precioTrayecto == null || precioTrayecto.Count == 0)
            {
                if (excepciones != null)
                    precioTrayecto.Add(new PrecioTrayectoDC { TRS_IdTipoSubTrayecto = TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL });
                else
                    throw new FaultException<ControllerException>(
                        new ControllerException(COConstantesModulos.TARIFAS,
                                                TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_PARA_TRAYECTO.ToString(),
                                                TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_PARA_TRAYECTO)));
            }
            if ((precioTrayecto.Where(pt => pt.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL).Count()) == 0)
                throw new FaultException<ControllerException>(
                        new ControllerException(COConstantesModulos.TARIFAS,
                                                TAEnumTipoErrorTarifas.EX_KILO_INICIAL_NO_CONFIGURADO.ToString(),
                                                TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_KILO_INICIAL_NO_CONFIGURADO)));

            precio.ValorKiloInicial = precioTrayecto.Where(pt => pt.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                 .FirstOrDefault()
                 .PTR_ValorFijo;

            precio.ValorKiloAdicional = 0;

            precioTrayecto.Where(r => r.TRS_IdTipoSubTrayecto != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
              .ToList()
              .ForEach(f =>
              {
                  precio.ValorKiloAdicional += f.PTR_ValorFijo;
              });

            bool aplicaTipoEntrega = false;
            if (idTipoEntrega != "-1")
            {
                var precioTipoEntrega = ObtenerPrecioTipoEntregaListaPrecios(idListaPrecio, idTipoEntrega, idServicio, idLocalidadOrigen, idLocalidadDestino).FirstOrDefault();

                if (precioTipoEntrega != null)
                {
                    aplicaTipoEntrega = true;
                    precio.ValorKiloInicial = precioTipoEntrega.PTE_ValorKiloInicial;
                    precio.ValorKiloAdicional = precioTipoEntrega.PTE_ValorKiloAdicional;
                }
            }

            ///Si hay excepciones obtiene el valor del kilo inicial(Valor configurado en la excepcion)
            ///valor del kilo adicional(valor adicional del trayecto)
            if (!aplicaTipoEntrega && excepciones != null)
            {
                precio.ValorKiloInicial = excepciones.SET_ValorKiloInicial;
                precio.ValorKiloAdicional = excepciones.SET_ValorKiloAdicional;
            }

            decimal totalAdicional = (peso - TAConstantesTarifas.VALOR_KILO_INICIAL_EXCEPCION_NOTIFICACIONES) * precio.ValorKiloAdicional;
            precio.ValorPrimaSeguro = ObtenerPrimaSeguro(idListaPrecio, valorDeclarado, idServicio);

            if (esPrimeraAdmision)
            {
                precio.Valor = (precio.ValorKiloInicial + totalAdicional);
            }
            else
            {
                precio.Valor = precio.ValorKiloAdicional + totalAdicional;
            }

            return precio;

        }

        public IEnumerable<TAImpuestosDC> ObtenerValorImpuestosServicio(int idServicio)
        {

            List<TAImpuestosDC> resultado = null;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("select * from ServicioImpuestos_VTAR where sei_idservicio = @IdServicio", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    resultado = new List<TAImpuestosDC>();

                    if (reader.Read())
                    {
                        TAImpuestosDC r = new TAImpuestosDC()
                        {
                            Identificador = Convert.ToInt16(reader["SEI_IdImpuesto"]),
                            Descripcion = Convert.ToString(reader["IMP_Descripcion"]),
                            Valor = Convert.ToDecimal(reader["IMP_Valor"])
                        };

                        resultado.Add(r);
                    }
                }

                return resultado;

            }
        }

        /// <summary>
        /// Obtiene la prima de seguro de una lista de precio servicio
        /// </summary>
        /// <param name="idListaPrecioServicio">Identificador lista de precio servicio</param>
        /// <returns>Prima de seguro</returns>
        public decimal ObtenerPrimaSeguro(int idListaPrecio, decimal valorDeclarado, int idServicio)
        {
            decimal resultado = 0;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPrimaSeguro_TAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                cmd.Parameters.AddWithValue("@IdListaPrecio", idListaPrecio);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        decimal res = 0;
                        bool validacion = false;
                        validacion = Decimal.TryParse(reader["LPS_PrimaSeguros"].ToString(), out res);
                        if (validacion)
                        {
                            resultado = (res / 100) * valorDeclarado;
                        }
                        else
                        {
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO)));
                        }
                    }
                }

                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO)));
                }

            }
            return resultado;


        }

        public List<PrecioTipoEntregaListaPrecios_TAR> ObtenerPrecioTipoEntregaListaPrecios(int idListaPrecio, string idTipoEntrega, int idServicio, string idLocalidadOrigen, string idLocalidadDestino)
        {
            List<PrecioTipoEntregaListaPrecios_TAR> resultado = null;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPrecioTipoEntregaListaPrecios_TAR ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                cmd.Parameters.AddWithValue("@IdTipoEntrega", idTipoEntrega);                
                cmd.Parameters.AddWithValue("@IdListaPrecio", idListaPrecio);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    resultado = new List<PrecioTipoEntregaListaPrecios_TAR>();
                    while (reader.Read())
                    {
                        PrecioTipoEntregaListaPrecios_TAR r = new PrecioTipoEntregaListaPrecios_TAR
                        {
                            LPS_IdListaPrecioServicio = Convert.ToInt32(reader["LPS_IdListaPrecioServicio"]),
                            LPS_IdServicio = Convert.ToInt32(reader["LPS_IdServicio"]),
                            LPS_IdListaPrecios = Convert.ToInt32(reader["LPS_IdListaPrecios"]),
                            LPS_PrimaSeguros = Convert.ToDecimal(reader["LPS_PrimaSeguros"]),
                            LPS_Estado = Convert.ToString(reader["LPS_Estado"]),
                            PTE_IdPrecioTipoEntrega = Convert.ToInt64(reader["PTE_IdPrecioTipoEntrega"]),
                            PTE_IdTipoEntrega = Convert.ToString(reader["PTE_IdTipoEntrega"]),
                            PTE_IdListaPrecioServicio = Convert.ToInt32(reader["PTE_IdListaPrecioServicio"]),
                            PTE_ValorKiloInicial = Convert.ToDecimal(reader["PTE_ValorKiloInicial"]),
                            PTE_ValorKiloAdicional = Convert.ToDecimal(reader["PTE_ValorKiloAdicional"]),
                            PTR_Inicial = DBNull.Value.Equals( reader["PTR_Inicial"]) ? 0 :  Convert.ToDecimal(reader["PTR_Inicial"]),
                            PTR_Final = DBNull.Value.Equals(reader["PTR_Final"]) ? 0 :Convert.ToDecimal(reader["PTR_Final"]),
                            PTR_IdPrecioTipoEntrega = DBNull.Value.Equals(reader["PTR_IdPrecioTipoEntrega"]) ? 0 : Convert.ToInt64(reader["PTR_IdPrecioTipoEntrega"]),
                            
                        };

                        resultado.Add(r);

                    }
                }

                return resultado;
            }
        }

        public List<ExcepcionesTrayectoDC> ObtenerExcepcionesTrayecto(int idServicio, string idLocalidadOrigen, string idLocalidadDestino, int idListaPrecio)
        {
            List<ExcepcionesTrayectoDC> resultado = null;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerExTrayecto_TAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                cmd.Parameters.AddWithValue("@IdLocalidadOrigen", idLocalidadOrigen);
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", idLocalidadDestino);
                cmd.Parameters.AddWithValue("@IdListaPrecio", idListaPrecio);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    resultado = new List<ExcepcionesTrayectoDC>();
                    while (reader.Read())
                    {
                        ExcepcionesTrayectoDC r = new ExcepcionesTrayectoDC
                        {
                            SET_IdPrecioServicioExcepionTrayecto = Convert.ToInt64(reader["SET_IdPrecioServicioExcepionTrayecto"]),
                            SET_IdListaPrecioServicio = Convert.ToInt32(reader["SET_IdListaPrecioServicio"]),
                            SET_IdLocalidadOrigen = Convert.ToString(reader["SET_IdLocalidadOrigen"]),
                            SET_IdLocalidadDestino = Convert.ToString(reader["SET_IdLocalidadDestino"]),
                            SET_FechaGrabacion = Convert.ToDateTime(reader["SET_FechaGrabacion"]),
                            SET_CreadoPor = Convert.ToString(reader["SET_CreadoPor"]),
                            SET_ValorKiloInicial = Convert.ToDecimal(reader["SET_ValorKiloInicial"]),
                            SET_ValorKiloAdicional = Convert.ToDecimal(reader["SET_ValorKiloAdicional"]),
                            SET_EsOrigenTodoElPais = Convert.ToBoolean(reader["SET_EsOrigenTodoElPais"]),
                            SET_EsDestinoTodoElPais = Convert.ToBoolean(reader["SET_EsDestinoTodoElPais"]),
                            LPS_IdListaPrecios = Convert.ToInt32(reader["LPS_IdListaPrecios"]),
                            LPS_PrimaSeguros = Convert.ToDecimal(reader["LPS_PrimaSeguros"]),
                            LPS_IdServicio = Convert.ToInt32(reader["LPS_IdServicio"]),
                            PTR_Inicial = Convert.ToDecimal(reader["PTR_Inicial"]),
                            PTR_Final = Convert.ToDecimal(reader["PTR_Final"]),
                        };

                        resultado.Add(r);

                    }
                }

                return resultado;
            }
        }

        public List<PrecioTrayectoDC> ObtenerListaPrecioTrayecto(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino)
        {
            List<PrecioTrayectoDC> resultado = null;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paPrecioTrayecto_TAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                cmd.Parameters.AddWithValue("@IdLocalidadOrigen", idLocalidadOrigen);
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", idLocalidadDestino);
                cmd.Parameters.AddWithValue("@IdListaPrecios", idListaPrecio);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    resultado = new List<PrecioTrayectoDC>();
                    while (reader.Read())
                    {
                        PrecioTrayectoDC r = new PrecioTrayectoDC
                        {
                            TRA_IdTrayecto = Convert.ToInt64(reader["TRA_IdTrayecto"]),
                            TRA_IdLocalidadOrigen = Convert.ToString(reader["TRA_IdLocalidadOrigen"]),
                            TRA_IdTrayectoSubTrayecto = Convert.ToInt32(reader["TRA_IdTrayectoSubTrayecto"]),
                            TRA_IdLocalidadDestino = Convert.ToString(reader["TRA_IdLocalidadDestino"]),
                            TRS_IdTipoSubTrayecto = Convert.ToString(reader["TRS_IdTipoSubTrayecto"]),
                            TRS_IdTipoTrayecto = Convert.ToString(reader["TRS_IdTipoTrayecto"]),
                            STR_IdTrayecto = Convert.ToInt32(reader["STR_IdTrayecto"]),
                            STR_IdServicio = Convert.ToInt32(reader["STR_IdServicio"]),
                            PTR_IdTrayectoSubTrayecto = Convert.ToInt32(reader["PTR_IdTrayectoSubTrayecto"]),
                            PTR_ValorFijo = Convert.ToDecimal(reader["PTR_ValorFijo"]),
                            PTR_IdListaPrecioServicio = Convert.ToInt32(reader["PTR_IdListaPrecioServicio"])
                        };

                        resultado.Add(r);

                    }
                }

                return resultado;

            }


            //using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC();

            //    var precioTrayecto = contexto.paPrecioTrayecto_TAR(idServicio, idLocalidadOrigen, idLocalidadDestino, idListaPrecio)
            //             .ToList();

            //    ///Obtiene las excepciones del trayecto
            //    var excepciones = contexto.paObtenerExTrayecto_TAR(idServicio, idLocalidadOrigen, idLocalidadDestino, idListaPrecio).FirstOrDefault();

            //    if (precioTrayecto.Count() == 0)
            //        if (excepciones != null)
            //            precioTrayecto.Add(new paPrecioTrayectoRS_TAR() { TRS_IdTipoSubTrayecto = TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL });
            //        else
            //            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_PARA_TRAYECTO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_PARA_TRAYECTO)));

            //    if ((precioTrayecto.Where(pt => pt.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL).Count()) == 0)
            //        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_KILO_INICIAL_NO_CONFIGURADO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_KILO_INICIAL_NO_CONFIGURADO)));

            //    precio.ValorKiloInicial = precioTrayecto.Where(pt => pt.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
            //      .FirstOrDefault()
            //      .PTR_ValorFijo;

            //    precio.ValorKiloAdicional = 0;

            //    precioTrayecto.Where(r => r.TRS_IdTipoSubTrayecto != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
            //      .ToList()
            //      .ForEach(f =>
            //      {
            //          precio.ValorKiloAdicional += f.PTR_ValorFijo;
            //      });

            //    bool aplicaTipoEntrega = false;
            //    if (idTipoEntrega != "-1")
            //    {
            //        var precioTipoEntrega = contexto.paObtenerPrecioTipoEntregaListaPrecios_TAR(idListaPrecio, idTipoEntrega, idServicio).FirstOrDefault();

            //        if (precioTipoEntrega != null)
            //        {
            //            aplicaTipoEntrega = true;
            //            precio.ValorKiloInicial = precioTipoEntrega.PTE_ValorKiloInicial;
            //            precio.ValorKiloAdicional = precioTipoEntrega.PTE_ValorKiloAdicional;
            //        }
            //    }

            //    ///Si hay excepciones obtiene el valor del kilo inicial(Valor configurado en la excepcion)
            //    ///valor del kilo adicional(valor adicional del trayecto)
            //    if (!aplicaTipoEntrega && excepciones != null)
            //    {
            //        precio.ValorKiloInicial = excepciones.SET_ValorKiloInicial;
            //        precio.ValorKiloAdicional = excepciones.SET_ValorKiloAdicional;
            //    }

            //    decimal totalAdicional = (peso - TAConstantesTarifas.VALOR_KILO_INICIAL_EXCEPCION_NOTIFICACIONES) * precio.ValorKiloAdicional;
            //    precio.ValorPrimaSeguro = ObtenerPrimaSeguro(idListaPrecio, valorDeclarado, idServicio);

            //    if (esPrimeraAdmision)
            //    {
            //        precio.Valor = (precio.ValorKiloInicial + totalAdicional);
            //    }
            //    else
            //    {
            //        precio.Valor = precio.ValorKiloAdicional + totalAdicional;
            //    }

            //    return precio;
            //}
        }
    }
}
