using CO.Servidor.Dominio.Comun.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Servicio.Entidades.Tarifas.Precios;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using VentaCredito.Tarifas.Comun;
using VentaCredito.Tarifas.Datos.ContratosDatos;

namespace VentaCredito.Tarifas.Datos.Repositorio
{
    public class PrecioMensajeriaCredito
    {

        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;



        private static PrecioMensajeriaCredito instancia = new PrecioMensajeriaCredito();

        public static PrecioMensajeriaCredito Instancia
        {
            get { return instancia; }
        }


        public decimal ValorCarga
        {
            get
            {
                return valorCarga;
            }

            set
            {
                valorCarga = value;
            }
        }

        private decimal valorCarga;


        public Servicio.Entidades.Tarifas.Precios.TAPrecioMensajeriaDC ObtenerPrecioMensajeriaCredito(int servicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega, bool aplicaContraPago)
        {
            Servicio.Entidades.Tarifas.Precios.TAPrecioMensajeriaDC precio = new Servicio.Entidades.Tarifas.Precios.TAPrecioMensajeriaDC();

            var precioTrayecto = PrecioMensajeria.Instancia.ObtenerListaPrecioTrayecto(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino);

            var listaExcepciones = PrecioMensajeria.Instancia.ObtenerExcepcionesTrayecto(servicio, idLocalidadOrigen, idLocalidadDestino, idListaPrecio);

            ExcepcionesTrayectoDC excepciones = null;
            if (listaExcepciones != null)
            {
                excepciones = listaExcepciones.FirstOrDefault();
            }
            //---------------------------------------------



            if (precioTrayecto == null || precioTrayecto.Count == 0)
                if (excepciones != null)
                    precioTrayecto.Add(new PrecioTrayectoDC { TRS_IdTipoSubTrayecto = TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL });
                else
                    throw new FaultException<ControllerException>(
                        new ControllerException(
                                    COConstantesModulos.TARIFAS,
                                    TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_PARA_TRAYECTO.ToString(),
                                    TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_PARA_TRAYECTO)));

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
                var listaPrecioTipoEntrega = PrecioMensajeria.Instancia.ObtenerPrecioTipoEntregaListaPrecios(idListaPrecio, idTipoEntrega, servicio, idLocalidadOrigen, idLocalidadDestino);
                PrecioTipoEntregaListaPrecios_TAR precioTipoEntrega = null;
                if (listaPrecioTipoEntrega != null)
                {
                    precioTipoEntrega = listaPrecioTipoEntrega.FirstOrDefault();
                }
                //= contexto.paObtenerPrecioTipoEntregaListaPrecios_TAR(idListaPrecio, idTipoEntrega, idServicio).FirstOrDefault();

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
            precio.ValorPrimaSeguro = PrecioMensajeria.Instancia.ObtenerPrimaSeguro(idListaPrecio, valorDeclarado, servicio);

            bool sumaPrima = SumarPrecioPesoValorDeclarado(idListaPrecio, peso, valorDeclarado, servicio == TAConstantesServicios.SERVICIO_RAPI_CARGA);

            if (sumaPrima)
            {
                precio.Valor = precio.ValorKiloInicial + totalAdicional;
            }
            else
            {
                precio.Valor = precio.ValorKiloInicial + totalAdicional - precio.ValorPrimaSeguro;
            }

            if (aplicaContraPago && aplicaContraPago)
            {
                var valorACobrar = TrayectoRepositorio.Instancia.ObtenerPrecioRangoContrapago(idListaPrecio, valorDeclarado, servicio);
                var serviciosContraPago = TrayectoRepositorio.Instancia.ObtenerServiciosFormaPagoContrapago();
                var impuestoValorContraPago = serviciosContraPago.Where(x => x.IdServicio == servicio && x.IdFormapago == (int)Servicio.Entidades.Tarifas.Precios.TAEnumFormaPago.CREDITO).FirstOrDefault();

                if (valorACobrar > 0)
                {
                    precio.ValorOtrosServicios = Convert.ToDecimal(String.Format("{0:0,0}", valorACobrar));
                    precio.ValorImpuestoContraPago = Convert.ToDecimal(String.Format("{0:0,0}", valorACobrar)) * impuestoValorContraPago.Iva;
                }
                else
                {
                    throw new Exception("El valor del contrapago no se encuentra parametrizado en los rangos para el cliente.");
                }
            }


            return precio;

        }

        public CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios.TAPrecioMensajeriaDC CalcularPrecio(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega)
        {
            TAPrecioMensajeriaDC valorPeso = ObtenerPrecioMensajeria(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);

            CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios.TAPrecioMensajeriaDC precio = 
                new CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios.TAPrecioMensajeriaDC()
            {
                Impuestos = TrayectoRepositorio.Instancia.ObtenerValorImpuestosServicio(idServicio),
                ValorKiloInicial = valorPeso.ValorKiloInicial,
                ValorKiloAdicional = valorPeso.ValorKiloAdicional,
                Valor = valorPeso.Valor,
                ValorPrimaSeguro = valorPeso.ValorPrimaSeguro
            };

            return precio;
        }

        private TAPrecioMensajeriaDC ObtenerPrecioMensajeria(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega)
        {
            TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC();

            using (SqlConnection sqlcon = new SqlConnection(conexionStringController))
            {

                List<PrecioTrayectoDC> precioTrayecto = ObtenerPrecioTrayecto(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino);
                ///Obtiene las excepciones del trayecto
                //List<ExcepcionesTrayectoDC> ListaExcepciones = ObtenerExcepcionesTrayecto(idServicio, idLocalidadOrigen, idLocalidadDestino, idListaPrecio);

                //ExcepcionesTrayectoDC excepciones = ListaExcepciones?.FirstOrDefault();
                ExcepcionesTrayectoDC excepciones = ObtenerExcepcionesTrayecto(idServicio, idLocalidadOrigen, idLocalidadDestino, idListaPrecio)?.FirstOrDefault();

                if (precioTrayecto.Count() == 0)
                    if (excepciones != null)
                        precioTrayecto.Add(new PrecioTrayectoDC { TRS_IdTipoSubTrayecto = TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL });
                    else
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_PARA_TRAYECTO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_PARA_TRAYECTO)));

                if ((precioTrayecto.Where(pt => pt.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL).Count()) == 0)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_KILO_INICIAL_NO_CONFIGURADO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_KILO_INICIAL_NO_CONFIGURADO)));

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
                    PrecioTipoEntregaListaPrecios_TAR precioTipoEntrega = ObtenerPrecioTipoEntregaListaPrecios(idListaPrecio, idTipoEntrega, idServicio).FirstOrDefault();

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
        }

        /// <summary>
        /// Obtiene la prima de seguro de una lista de precio servicio
        /// </summary>
        /// <param name="idListaPrecioServicio">Identificador lista de precio servicio</param>
        /// <returns>Prima de seguro</returns>
        public decimal ObtenerPrimaSeguro(int idListaPrecio, decimal valorDeclarado, int idServicio)
        {
            decimal resultado = 0;
            using (SqlConnection sqlcon = new SqlConnection(conexionStringController))
            {
                PrecioTipoEntregaListaPrecios_TAR precio = new PrecioTipoEntregaListaPrecios_TAR();
                sqlcon.Open();
                SqlCommand sqlcmd = new SqlCommand("paObtenerPrimaSeguro_TAR", sqlcon);
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Parameters.AddWithValue("@IdServicio", idServicio);
                sqlcmd.Parameters.AddWithValue("@IdListaPrecio", idListaPrecio);

                SqlDataReader reader = sqlcmd.ExecuteReader();

                List<PrimaSeguroDC> primaSeguro = MapperToPrimaSeguroDC(reader);


                if (primaSeguro != null)
                {
                    var res = primaSeguro.First();
                    if (res != null)
                    {
                        resultado = (res.LPS_PrimaSeguros / 100) * valorDeclarado;
                    }
                    else
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO)));
                }
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO)));
            }

            return resultado;
        }

        private List<PrimaSeguroDC> MapperToPrimaSeguroDC(SqlDataReader reader)
        {
            List<PrimaSeguroDC> resultado = null;

            if (reader.HasRows)
            {
                resultado = new List<PrimaSeguroDC>();
                while (reader.Read())
                {
                    PrimaSeguroDC primaSeguro = new PrimaSeguroDC
                    {
                        LPS_PrimaSeguros = Convert.ToInt32(reader["LPS_PrimaSeguros"]),
                        LPS_IdServicio = Convert.ToInt32(reader["LPS_IdServicio"]),
                        LPS_IdListaPrecios = Convert.ToInt32(reader["LPS_IdListaPrecios"]),
                    };

                    resultado.Add(primaSeguro);
                }
            }

            return resultado;
        }

        private List<PrecioTipoEntregaListaPrecios_TAR> ObtenerPrecioTipoEntregaListaPrecios(int idListaPrecio, string idTipoEntrega, int idServicio)
        {
            List<PrecioTipoEntregaListaPrecios_TAR> resultado = null;
            using (SqlConnection sqlcon = new SqlConnection(conexionStringController))
            {
                PrecioTipoEntregaListaPrecios_TAR precio = new PrecioTipoEntregaListaPrecios_TAR();
                sqlcon.Open();
                SqlCommand sqlcmd = new SqlCommand("paObtenerPrecioTipoEntregaListaPrecios_TAR", sqlcon);
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Parameters.AddWithValue("@IdServicio", idServicio);
                sqlcmd.Parameters.AddWithValue("@IdTipoEntrega", idTipoEntrega);
                sqlcmd.Parameters.AddWithValue("@IdListaPrecio", idListaPrecio);

                SqlDataReader reader = sqlcmd.ExecuteReader();

                resultado = MapperToPrecioTipoEntregaListaPrecios(reader);
            }

            return resultado;

        }

        private List<PrecioTipoEntregaListaPrecios_TAR> MapperToPrecioTipoEntregaListaPrecios(SqlDataReader reader)
        {
            List<PrecioTipoEntregaListaPrecios_TAR> resultado = null;

            if (reader.HasRows)
            {
                resultado = new List<PrecioTipoEntregaListaPrecios_TAR>();
                while (reader.Read())
                {
                    PrecioTipoEntregaListaPrecios_TAR ExcepcionTrayecto = new PrecioTipoEntregaListaPrecios_TAR
                    {
                        LPS_IdListaPrecioServicio = Convert.ToInt32(reader["LPS_IdListaPrecioServicio"]),
                        LPS_IdServicio = Convert.ToInt32(reader["LPS_IdServicio"]),
                        LPS_IdListaPrecios = Convert.ToInt32(reader["LPS_IdListaPrecios"]),
                        LPS_PrimaSeguros = Convert.ToInt32(reader["LPS_PrimaSeguros"]),
                        LPS_Estado = Convert.ToString(reader["LPS_Estado"]),
                        PTE_IdPrecioTipoEntrega = Convert.ToInt64(reader["PTE_IdPrecioTipoEntrega"]),
                        PTE_IdTipoEntrega = Convert.ToString(reader["PTE_IdTipoEntrega"]),
                        PTE_IdListaPrecioServicio = Convert.ToInt32(reader["PTE_IdListaPrecioServicio"]),
                        PTE_ValorKiloInicial = Convert.ToDecimal(reader["PTE_ValorKiloInicial"]),
                        PTE_ValorKiloAdicional = Convert.ToDecimal(reader["PTE_ValorKiloAdicional"]),
                        PTR_Inicial = Convert.ToDecimal(reader["PTR_Inicial"]),
                        PTR_Final = Convert.ToDecimal(reader["PTR_Final"]),
                        PTR_IdPrecioTipoEntrega = Convert.ToInt64(reader["PTR_IdPrecioTipoEntrega"]),
                    };

                    resultado.Add(ExcepcionTrayecto);
                }
            }

            return resultado;
        }

        private List<ExcepcionesTrayectoDC> ObtenerExcepcionesTrayecto(int idServicio, string idLocalidadOrigen, string idLocalidadDestino, int idListaPrecio)
        {
            List<ExcepcionesTrayectoDC> resultado = null;
            using (SqlConnection sqlcon = new SqlConnection(conexionStringController))
            {
                TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC();
                sqlcon.Open();
                SqlCommand sqlcmd = new SqlCommand("paObtenerExTrayecto_TAR", sqlcon);
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Parameters.AddWithValue("@IdServicio", idServicio);
                sqlcmd.Parameters.AddWithValue("@IdLocalidadOrigen", idLocalidadOrigen);
                sqlcmd.Parameters.AddWithValue("@IdLocalidadDestino", idLocalidadDestino);
                sqlcmd.Parameters.AddWithValue("@IdListaPrecio", idListaPrecio);

                SqlDataReader reader = sqlcmd.ExecuteReader();

                resultado = MapperToExcepcionesTrayectoDC(reader);
            }

            return resultado;

        }

        private List<ExcepcionesTrayectoDC> MapperToExcepcionesTrayectoDC(SqlDataReader reader)
        {
            List<ExcepcionesTrayectoDC> resultado = null;

            if (reader.HasRows)
            {
                resultado = new List<ExcepcionesTrayectoDC>();
                while (reader.Read())
                {
                    ExcepcionesTrayectoDC ExcepcionTrayecto = new ExcepcionesTrayectoDC
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
                        LPS_PrimaSeguros = Convert.ToInt32(reader["LPS_PrimaSeguros"]),
                        LPS_IdServicio = Convert.ToInt32(reader["LPS_IdServicio"]),
                        PTR_Inicial = Convert.ToDecimal(reader["PTR_Inicial"]),
                        PTR_Final = Convert.ToDecimal(reader["PTR_Final"]),
                    };

                    resultado.Add(ExcepcionTrayecto);
                }
            }

            return resultado;
        }

        private List<PrecioTrayectoDC> ObtenerPrecioTrayecto(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino)
        {
            List<PrecioTrayectoDC> resultado = null;
            using (SqlConnection sqlcon = new SqlConnection(conexionStringController))
            {
                TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC();
                sqlcon.Open();
                SqlCommand sqlcmd = new SqlCommand("paPrecioTrayecto_TAR", sqlcon);
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Parameters.AddWithValue("@IdServicio", idServicio);
                sqlcmd.Parameters.AddWithValue("@IdLocalidadOrigen", idLocalidadOrigen);
                sqlcmd.Parameters.AddWithValue("@IdLocalidadDestino", idLocalidadDestino);
                sqlcmd.Parameters.AddWithValue("@IdListaPrecios", idListaPrecio);

                SqlDataReader reader = sqlcmd.ExecuteReader();

                resultado = MapperToPrecioTrayecto(reader);
            }

            return resultado;
        }

        private List<PrecioTrayectoDC> MapperToPrecioTrayecto(SqlDataReader reader)
        {
            List<PrecioTrayectoDC> resultado = new List<PrecioTrayectoDC>();

            if (reader.HasRows)
            {                
                while (reader.Read())
                {
                    PrecioTrayectoDC precioTrayecto = new PrecioTrayectoDC
                    {
                        TRA_IdTrayecto = Convert.ToInt64(reader["TRA_IdTrayecto"]),
                        TRA_IdLocalidadOrigen = Convert.ToString(reader["TRA_IdLocalidadOrigen"]),
                        TRA_IdTrayectoSubTrayecto = Convert.ToInt32(reader["TRA_IdTrayectoSubTrayecto"]),
                        TRA_IdLocalidadDestino = Convert.ToString(reader["TRA_IdLocalidadDestino"]),
                        TRS_IdTipoSubTrayecto = Convert.ToString(reader["TRS_IdTipoSubTrayecto"]),
                        TRS_IdTipoTrayecto = Convert.ToString(reader["TRS_IdTipoTrayecto"]),
                        STR_IdTrayecto = Convert.ToInt64(reader["STR_IdTrayecto"]),
                        STR_IdServicio = Convert.ToInt32(reader["STR_IdServicio"]),
                        PTR_IdTrayectoSubTrayecto = Convert.ToInt32(reader["PTR_IdTrayectoSubTrayecto"]),
                        PTR_ValorFijo = Convert.ToDecimal(reader["PTR_ValorFijo"]),
                        PTR_IdListaPrecioServicio = Convert.ToInt32(reader["PTR_IdListaPrecioServicio"]),
                    };

                    resultado.Add(precioTrayecto);
                }
            }

            return resultado;
        }

        public decimal ObtenerValorMinimoDeclarado(int idListaPrecio, decimal peso)
        {
            return ObtenerPesoDeclarado(idListaPrecio).Where(r => r.PesoInicial <= peso && r.PesoFinal >= peso).FirstOrDefault().ValorMinimoDeclarado;
        }

        private bool SumarPrecioPesoValorDeclarado(int idListaPrecio, decimal peso, decimal valorDeclarado, bool esRapiCarga)
        {
            ValorPesoDeclaradoDC consulta = ObtenerPesoDeclarado(idListaPrecio).Where(r => r.PesoInicial <= peso && r.PesoFinal >= peso).FirstOrDefault();

            if (consulta == null)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_PESO_FUERA_RANGO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_PESO_FUERA_RANGO)));
            }
            else if (valorDeclarado < consulta.ValorMinimoDeclarado)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_VALOR_DECLARADO_MENOR_QUE_MINIMO_DECLARADO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_VALOR_DECLARADO_MENOR_QUE_MINIMO_DECLARADO)));
            }
            else if (valorDeclarado == consulta.ValorMinimoDeclarado && !esRapiCarga)
            {
                return false;
            }
            else if (esRapiCarga)
            {
                return !(valorDeclarado == ValorCarga);
            }
            else
            {
                return true;
            }

        }

        private List<ValorPesoDeclaradoDC> ObtenerPesoDeclarado(int idListaPrecio)
        {
            List<ValorPesoDeclaradoDC> resultado = null;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPesoDeclarado", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdListaPrecios", idListaPrecio);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    resultado = new List<ValorPesoDeclaradoDC>();
                    while (reader.Read())
                    {
                        ValorPesoDeclaradoDC r = new ValorPesoDeclaradoDC
                        {
                            IdListaPrecios = Convert.ToInt32(reader["VMD_IdListaPrecios"]),
                            IdValorMinimoDeclarado = Convert.ToInt32(reader["VMD_IdValorMinimoDeclarado"]),
                            FechaGrabacion = Convert.ToDateTime(reader["VMD_FechaGrabacion"]),
                            PesoFinal = Convert.ToDecimal(reader["VMD_PesoFinal"]),
                            PesoInicial = Convert.ToDecimal(reader["VMD_PesoInicial"]),
                            CreadoPor = Convert.ToString(reader["VMD_CreadoPor"]),
                            ValorMaximoDeclarado = Convert.ToDecimal(reader["VMD_ValorMaximoDeclarado"]),
                            ValorMinimoDeclarado = Convert.ToDecimal(reader["VMD_ValorMinimoDeclarado"]),
                        };

                        resultado.Add(r);
                    }
                }
            }

            return resultado;
        }
    }
}