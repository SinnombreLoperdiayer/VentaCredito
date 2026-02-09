using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Tarifas.Comun;
using CO.Servidor.Tarifas.Datos.Modelo;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CO.Servidor.Tarifas.Datos
{
    public class TARepositorioCredito
    {
        #region Campos

        private static readonly TARepositorioCredito instancia = new TARepositorioCredito();
        private const string NombreModelo = "ModeloTarifas";
        private string cadenaTransaccional = System.Configuration.ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        #endregion Campos


        #region Propiedades

        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static TARepositorioCredito Instancia
        {
            get { return instancia; }
        }

        #endregion Propiedades

        #region Metodos

        /// <summary>
        /// Retorna el valor de mensajeria
        /// </summary>
        /// <returns></returns>
        public TAPrecioMensajeriaDC ObtenerPrecioMensajeriaCredito(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC();

                var precioTrayecto = contexto.paPrecioTrayecto_TAR(idServicio, idLocalidadOrigen, idLocalidadDestino, idListaPrecio)
                         .ToList();

                ///Obtiene las excepciones del trayecto
                var excepciones = contexto.paObtenerExTrayecto_TAR(idServicio, idLocalidadOrigen, idLocalidadDestino, idListaPrecio).FirstOrDefault();

                if (precioTrayecto.Count() == 0)
                    if (excepciones != null)
                        precioTrayecto.Add(new paPrecioTrayectoRS_TAR() { TRS_IdTipoSubTrayecto = TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL });
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
                    var precioTipoEntrega = contexto.paObtenerPrecioTipoEntregaListaPrecios_TAR(idListaPrecio, idTipoEntrega, idServicio).FirstOrDefault();

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
                bool sumaPrima = SumarPrecioPesoValorDeclarado(idListaPrecio, peso, valorDeclarado, idServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA);

                if (sumaPrima)
                {
                    precio.Valor = precio.ValorKiloInicial + totalAdicional;

                }
                else
                {
                    precio.Valor = precio.ValorKiloInicial + totalAdicional - precio.ValorPrimaSeguro;

                }

                return precio;
            }
        }


        /// <summary>
        /// Calcula precio rapicarga, el calculo del precio se realiza de acuerdo al peso ingresado y los rangos configurados
        /// Si el peso ingresado esta en un valor intermedio se aplica la siguiente formula
        /// valor=(valorRango * pesoRangoFinal) +(kilosAdicionales * valorRango)
        /// </summary>
        /// <param name="idServicio">Identificador servicio</param>
        /// <param name="idListaPrecio">Identificador lista de precio</param>
        /// <param name="idLocalidadOrigen">Identificador localidad de origen</param>
        /// <param name="idLocalidadDestino">Identificador localidad de destino</param>
        /// <param name="peso">Peso</param>
        /// <returns>Objeto precio</returns>
        public TAPrecioCargaDC ObtenerPrecioCargaCredito(int idServicio, int idListaPrecio, int idLp, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                bool esMensajeria = false;
                TAPrecioCargaDC precio = new TAPrecioCargaDC();

                var precioTrayectoRango = contexto.paPrecioTrayectoRango_TAR(idServicio, idLocalidadOrigen, idLocalidadDestino, idLp).ToList();

                precioTrayectoRango.ForEach(f =>
                {
                    if (f.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_TRAYECTO_ESPECIAL)
                        esMensajeria = true;
                });

                if (esMensajeria == true)
                {
                    TAPrecioMensajeriaDC precioMensajeria = ObtenerPrecioMensajeriaCredito(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
                    precio.Valor = precioMensajeria.Valor;
                    precio.ValorKiloAdicional = precioMensajeria.ValorKiloAdicional;
                }
                else if (esMensajeria == false)
                {
                    ///Obtiene las excepciones del trayecto
                    var excepciones = contexto.paObtenerExTrayecto_TAR(idServicio, idLocalidadOrigen, idLocalidadDestino, idListaPrecio).FirstOrDefault();
                    if (excepciones != null && precioTrayectoRango.Count() == 0)
                    {
                        precioTrayectoRango.Add(new paPrecioTrayectoRango_TAR_Result());
                    }

                    bool aplicoTipoEntrega = false;
                    if (precioTrayectoRango.Count() > 0)
                    {
                        //Obtiene los precios por tipo de entrega
                        if (idTipoEntrega != "-1")
                        {
                            var precioTipoEntrega = contexto.paObtenerPrecioTipoEntregaListaPrecios_TAR(idListaPrecio, idTipoEntrega, idServicio).FirstOrDefault();

                            if (precioTipoEntrega != null)
                            {
                                aplicoTipoEntrega = true;
                                precioTrayectoRango.ForEach(f =>
                                {
                                    f.PPR_Final = precioTipoEntrega.PTR_Final.Value;
                                    f.PPR_Inicial = precioTipoEntrega.PTR_Inicial.Value;
                                    f.PPR_Valor = precioTipoEntrega.PTE_ValorKiloAdicional;
                                });
                            }
                        }

                        ///Obtiene las excepciones del trayecto
                        //var excepciones = contexto.paObtenerExTrayecto_TAR(idServicio, idLocalidadOrigen, idLocalidadDestino, idListaPrecio).FirstOrDefault();

                        ///Si hay excepciones obtiene el valor del kilo inicial(Valor configurado en la excepcion)
                        ///valor del kilo adicional(valor adicional del trayecto)
                        if (!aplicoTipoEntrega && excepciones != null)
                        {
                            precioTrayectoRango.ForEach(f =>
                            {
                                f.PPR_Final = excepciones.PTR_Final.Value;
                                f.PPR_Inicial = excepciones.PTR_Inicial.Value;
                                f.PPR_Valor = excepciones.SET_ValorKiloAdicional;
                            });
                        }

                        precio.ValorPrimaSeguro = ObtenerPrimaSeguro(idListaPrecio, valorDeclarado, idServicio);
                        bool sumaPrima = SumarPrecioPesoValorDeclarado(idListaPrecio, peso, valorDeclarado, idServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA);

                        // Para rapicarga se debe validar una opción que indica si se debe ignorar la prima de seguro sin importar el valor declarado, esto es para ciertos clientes especiales
                        var listaPrecioServicio = contexto.ListaPrecioServicio_TAR.FirstOrDefault(l => l.LPS_IdListaPrecioServicio == idLp);
                        if (listaPrecioServicio != null)
                        {
                            if (listaPrecioServicio.LPS_IgnorarPrima.HasValue && listaPrecioServicio.LPS_IgnorarPrima.Value)
                            {
                                sumaPrima = false;
                            }
                        }

                        if (precioTrayectoRango.Where(p => p.PPR_Inicial <= peso && p.PPR_Final >= peso).Count() > 0)
                        {
                            var consulta = precioTrayectoRango.Where(p => p.PPR_Inicial <= peso && p.PPR_Final >= peso).FirstOrDefault();
                            long idPrecioTrayectoSubTrayecto = consulta.PTR_IdPrecioTrayectoSubTrayect;
                            var valorBase = contexto.PrecioTrayecto_TAR.Where(r => r.PTR_IdPrecioTrayectoSubTrayect == idPrecioTrayectoSubTrayecto).FirstOrDefault();
                            if (sumaPrima == true)
                                precio.Valor = consulta.PPR_Valor * consulta.PPR_Final;
                            else if (sumaPrima == false)
                                precio.Valor = (consulta.PPR_Valor * consulta.PPR_Final) - precio.ValorPrimaSeguro;

                            precio.ValorKiloAdicional = consulta.PTR_ValorFijo;

                            //precio.ValorServicioRetorno = contexto.paObtenerPreTraSubValAdi_TAR(idPrecioTrayectoSubTrayecto).FirstOrDefault().PTV_Valor;
                        }
                        else if (peso < precioTrayectoRango.OrderBy(o => o.PPR_Inicial).First().PPR_Inicial)
                        {
                            TAPrecioMensajeriaDC precioMensajeria = ObtenerPrecioMensajeriaCredito(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
                            precio.Valor = precioMensajeria.Valor;
                            precio.ValorKiloAdicional = precioMensajeria.ValorKiloAdicional;
                        }
                        else if (peso > precioTrayectoRango.OrderBy(o => o.PPR_Final).Last().PPR_Final)
                        {
                            var consulta = precioTrayectoRango.OrderBy(o => o.PPR_Final).Last();
                            decimal pesoAdicional = peso - (consulta.PPR_Final);
                            long idPrecioTrayectoSubTrayecto = consulta.PTR_IdPrecioTrayectoSubTrayect;
                            var valorBase = contexto.PrecioTrayecto_TAR.Where(r => r.PTR_IdPrecioTrayectoSubTrayect == idPrecioTrayectoSubTrayecto).FirstOrDefault();

                            precio.ValorKiloAdicional = consulta.PTR_ValorFijo;

                            if (sumaPrima)
                                precio.Valor = (consulta.PPR_Valor * consulta.PPR_Final) + (pesoAdicional * consulta.PPR_Valor);
                            else if (!sumaPrima)
                                precio.Valor = ((consulta.PPR_Valor * consulta.PPR_Final) + (pesoAdicional * consulta.PPR_Valor)) - precio.ValorPrimaSeguro;

                            //precio.ValorServicioRetorno = contexto.paObtenerPreTraSubValAdi_TAR(idPrecioTrayectoSubTrayecto).FirstOrDefault().PTV_Valor;
                        }
                        else
                        {
                            var rangos = precioTrayectoRango.OrderBy(o => o.PPR_Inicial).ToList();
                            bool calculoTarifa = false;
                            for (int i = 0; i < rangos.Count() - 1; i++)
                            {
                                if (peso > rangos[i].PPR_Final && peso < rangos[i + 1].PPR_Inicial)
                                {
                                    if (!calculoTarifa)
                                    {
                                        decimal pesoAdicional = peso - (rangos[i].PPR_Final);
                                        long idPrecioTrayectoSubTrayecto = rangos[i].PTR_IdPrecioTrayectoSubTrayect;
                                        var valorBase = contexto.PrecioTrayecto_TAR.Where(r => r.PTR_IdPrecioTrayectoSubTrayect == idPrecioTrayectoSubTrayecto).FirstOrDefault();

                                        precio.ValorKiloAdicional = rangos[i].PTR_ValorFijo;

                                        if (sumaPrima)
                                            precio.Valor = (rangos[i].PTR_ValorFijo * rangos[i].PPR_Final) + (pesoAdicional * rangos[i].PTR_ValorFijo);
                                        else if (!sumaPrima)
                                            precio.Valor = ((rangos[i].PTR_ValorFijo * rangos[i].PPR_Final) + (pesoAdicional * rangos[i].PTR_ValorFijo)) - precio.ValorPrimaSeguro;

                                        calculoTarifa = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_PRECIO_SERVICIO_NO_CONFIGURADO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_PRECIO_SERVICIO_NO_CONFIGURADO)));
                    }
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
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var consulta = contexto.paObtenerPrimaSeguro_TAR(idListaPrecio, idServicio);

                if (consulta != null)
                {
                    var res = consulta.First();
                    if (res != null)
                    {
                        return (res.LPS_PrimaSeguros / 100) * valorDeclarado;
                    }
                    else
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO)));
                }
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO)));
            }
        }


        /// <summary>
        /// Obtener parámetros para saber si sumar o restar la prima de seguro
        /// </summary>
        /// <param name="idListaPrecio">Identificador lista de precio</param>
        /// <param name="peso">Peso</param>
        /// <param name="valorDeclarado">Valor peso declarado</param>
        /// <returns>True si se suma o false si se resta</returns>
        public bool SumarPrecioPesoValorDeclarado(int idListaPrecio, decimal peso, decimal valorDeclarado, bool esRapiCarga = false)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ValorPesoDeclarado_TAR consulta = contexto.ValorPesoDeclarado_TAR.Where(r => r.VMD_IdListaPrecios == idListaPrecio && r.VMD_PesoInicial <= peso && r.VMD_PesoFinal >= peso).FirstOrDefault();

                if (consulta == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_PESO_FUERA_RANGO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_PESO_FUERA_RANGO)));
                }
                else if (valorDeclarado < consulta.VMD_ValorMinimoDeclarado)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_VALOR_DECLARADO_MENOR_QUE_MINIMO_DECLARADO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_VALOR_DECLARADO_MENOR_QUE_MINIMO_DECLARADO)));
                }
                else if (valorDeclarado == consulta.VMD_ValorMinimoDeclarado && !esRapiCarga)
                {
                    return false;
                }
                else if (esRapiCarga)
                {
                    return !(valorDeclarado == 300000); // TODO: RON Obtener este valor de un parámetro TopeMinVlrDeclRapiCa
                }
                else
                {
                    return true;
                }
            }
        }



        /// <summary>
        /// Retorna los impuestos asignados a un servicio
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <returns>Colección de servicios</returns>
        public IEnumerable<TAImpuestosDC> ObtenerValorImpuestosServicioCredito(int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ServicioImpuestos_VTAR
                  .Where(r => r.SEI_IdServicio == idServicio)
                  .ToList()
                  .ConvertAll(i => new TAImpuestosDC()
                  {
                      Identificador = i.SEI_IdImpuesto,
                      Descripcion = i.IMP_Descripcion,
                      Valor = i.IMP_Valor
                  });


                #endregion


            }
        }
    }
}

