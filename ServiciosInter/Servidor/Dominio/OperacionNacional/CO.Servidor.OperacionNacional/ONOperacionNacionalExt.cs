using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using CO.Servidor.Dominio.Comun.AdmEstadosConsolidado;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.Rutas;
using CO.Servidor.OperacionNacional.Comun;
using CO.Servidor.OperacionNacional.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;

namespace CO.Servidor.OperacionNacional
{
    public partial class ONOperacionNacional
    {
        /// <summary>
        /// Adiciona el IngresoConsolidado a la Tbla
        /// </summary>
        /// <param name="ingresoConsolidado">info a Ingresar</param>
        public void AdicionarIngresoConsolidado(ONDescargueConsolidadosUrbRegNalDC ingresoConsolidado)
        {
            ONRepositorio.Instancia.AdicionarIngresoConsolidado(ingresoConsolidado);
        }

        /// <summary>
        ///Obtiene las novedades de Un descarque Especifico
        /// </summary>
        /// <returns>Lista de Novedades Asociadas</returns>
        public List<ONNovedadesConsolidadoDC> ObtenerNovedadesDescargueUnico(long NumCtrolTransMan)
        {
            return ONRepositorio.Instancia.ObtenerNovedadesDescargueUnico(NumCtrolTransMan);
        }

        /// <summary>
        /// Adiciona el Proceso de Descargue Consolidado
        /// </summary>
        /// <param name="nuevoDescargue"></param>
        public bool AdicionarProcesoDescargueConsolidado(ONDescargueConsolidadosUrbRegNalDC nuevoDescargue)
        {
            bool esIngresoConGuia = false;

            using (TransactionScope transaccion = new TransactionScope())
            {
                //Valido si existe el NumCtrlTranMan
                if (ONRepositorio.Instancia.ValidarIngresoConsolidado(nuevoDescargue.CtrolManifiesto))
                {
                    //Realizo la insercion o Actualizacion de las
                    //Novedades del Ingreso de Consolidado
                    List<ONNovedadesConsolidadoDC> lstNovedadesAnteriores = ObtenerNovedadesDescargueUnico(nuevoDescargue.CtrolManifiesto);
                    if (lstNovedadesAnteriores != null && lstNovedadesAnteriores.Count() > 0)
                    {
                        //Borro la Novedades Anteriores
                        lstNovedadesAnteriores.ForEach(novAnt =>
                        {
                            ONRepositorio.Instancia.BorrarNovedadIngresoConsolidado(nuevoDescargue.CtrolManifiesto, novAnt.IdNovedadConsolidado);
                        });
                    }

                    //Adiciono las Nuevas Novedades
                    nuevoDescargue.ListNovedadConsolidado.ForEach(novNva =>
                    {
                        ONRepositorio.Instancia.AdicionarNovedadIngresoConsolidado(novNva, nuevoDescargue.CtrolManifiesto);
                    });

                    //Adiciono las Nuevas Guias
                    nuevoDescargue.ListGuiasIngresadas.ForEach(nvaGuia =>
                    {
                        esIngresoConGuia = AdicionarGuiaIngresoConsolidado(nvaGuia, nuevoDescargue.CtrolManifiesto);
                    });
                }
                else
                {
                    //Adiciono la Nueva Transaccion en ingreso Consolidado
                    AdicionarIngresoConsolidado(nuevoDescargue);

                    //Adiciono las Nuevas Novedades
                    if (nuevoDescargue.ListNovedadConsolidado.Count > 0)
                    {
                        nuevoDescargue.ListNovedadConsolidado.ForEach(novNva =>
                        {
                            ONRepositorio.Instancia.AdicionarNovedadIngresoConsolidado(novNva, nuevoDescargue.CtrolManifiesto);
                        });
                    }

                    //Adiciono las Nuevas Guias
                    if (nuevoDescargue.ListGuiasIngresadas.Count > 0)
                    {
                        nuevoDescargue.ListGuiasIngresadas.ForEach(nvaGuia =>
                        {
                            esIngresoConGuia = AdicionarGuiaIngresoConsolidado(nvaGuia, nuevoDescargue.CtrolManifiesto);
                        });
                    }
                }

                //Actualizo la TblManifestoOperacion ó Asignacion Punto si se tomo la Info desde Alli
                if (string.IsNullOrEmpty(nuevoDescargue.EstadoAsignacion))
                {
                    //Actualizo ManifiestoOPeraNacionalConsolidado
                    ONRepositorio.Instancia.ActualizarManifiestoOperacionNalConsolidado(nuevoDescargue.CtrolManifiesto);
                }
                else
                {
                    //Actualizo AsignacionTulaServicio
                    ONRepositorio.Instancia.ActualizarAsignacionTulaPuntoServicio(nuevoDescargue.CtrolManifiesto);
                }

                ///Actualizo el suministro
                ECAdminEstadosConsolidado.GuardarEstadoConsolidado(new ECEstadoConsolidado
                {
                    NoTula = nuevoDescargue.NumTula,
                    Estado = EnumEstadosConsolidados.DES,
                    IdCentroServicios = nuevoDescargue.IdCentroServicio,
                    Observaciones = string.Empty
                });

                transaccion.Complete();
            }
            return esIngresoConGuia;
        }

        /// <summary>
        /// Adiciona la Guia al Consolidado
        /// </summary>
        /// <param name="guiasAIngresar"></param>
        public bool AdicionarGuiaIngresoConsolidado(OnDescargueEnvioSueltoDC guiaAIngresar, long numCtrlMan)
        {
            bool esIngresoConGuia = false;

            string NumeroGuia = Regex.Match(guiaAIngresar.NumeroGuia, @"\d+").Value;
            long NumGuia = Convert.ToInt64(NumeroGuia);

            //Consulto si la Guia ya fue Ingresada
            OnDescargueEnvioSueltoDC guiaYaIngresada = ONRepositorio.Instancia.ObtenerGuiaIngresadaDescargada(NumGuia);
            if (guiaYaIngresada != null)
            {
                if (guiaYaIngresada.IdAdminMensajeria != 0)
                {
                    //Borro las Novedades de la Guia
                    List<ONNovedadesEnvioDC> novedadesAnte = ONRepositorio.Instancia.ObtenerNovedadesIngresoGuia(guiaYaIngresada.IdIngresoGuia);

                    novedadesAnte.ForEach(novAnt =>
                    {
                        ONRepositorio.Instancia.BorrarNovedadSinGuiaConsolidado(guiaYaIngresada.IdIngresoGuia, novAnt.IdNovedadEnvioSuelto);
                    });

                    //Ingreso NovedadGuiaconGuia
                    guiaAIngresar.LstNovedadesGuias.ForEach(novGuia =>
                    {
                        ONRepositorio.Instancia.AdicionarNovedadIngresoConGuia(novGuia, guiaYaIngresada.IdIngresoGuia);
                    });
                }
                else
                {
                    //Borro las Novedades de la Guia
                    List<ONNovedadesEnvioDC> novedadesAnte = ONRepositorio.Instancia.ObtenerNovedadesIngresoGuia(guiaYaIngresada.IdIngresoGuia);

                    novedadesAnte.ForEach(novAnt =>
                    {
                        ONRepositorio.Instancia.BorrarNovedadSinGuiaConsolidado(guiaYaIngresada.IdIngresoGuia, novAnt.IdNovedadEnvioSuelto);
                    });

                    //Ingreso NovedadGuiaSinGuia
                    guiaAIngresar.LstNovedadesGuias.ForEach(novGuia =>
                    {
                        ONRepositorio.Instancia.AdicionarNovedadIngresoSinGuia(novGuia, guiaYaIngresada.IdIngresoGuia);
                    });
                }
            }
            else
            {
                //Consulto si al Guia Existe En Admin Mensajeria
                IADFachadaAdmisionesMensajeria fachadaAdminMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
                ADGuia guia = fachadaAdminMensajeria.ObtenerInfoGuiaXNumeroGuia(NumGuia);

                //Si existe inserto en la Tbl IngeresoConGuia
                if (guia != null && guia.IdAdmision != 0)
                {
                    ///Validar insertar Estado Guia

                    #region EstadoGuia

                    ADTrazaGuia estadoGuia = new ADTrazaGuia
                    {
                        Ciudad = guia.NombreCiudadOrigen,
                        IdCiudad = guia.IdCiudadOrigen,
                        IdAdmision = guia.IdAdmision,
                        IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(guia.IdAdmision)),
                        IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.CentroAcopio,
                        Modulo = COConstantesModulos.MODULO_OPERACION_URBANA,
                        NumeroGuia = guia.NumeroGuia,
                        Observaciones = string.Empty,
                        NumeroPieza = guia.NumeroPieza,
                        TotalPiezas = guia.TotalPiezas
                    };

                    estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                    if (estadoGuia.IdTrazaGuia == 0)
                    {
                        string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();

                        //no pudo realizar el cambio de estado
                        ControllerException excepcion =
                        new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO.ToString(),
                        MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO));
                        excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                        throw new FaultException<ControllerException>(excepcion);
                    }

                    #endregion EstadoGuia

                    ///Inserto Guia en IngresoConGuia_OPN
                    long idIngreso = ONRepositorio.Instancia.AdicionarIngresoConGuia(guia, numCtrlMan);
                    esIngresoConGuia = true;

                    //Ingreso NovedadGuia
                    guiaAIngresar.LstNovedadesGuias.ForEach(novGuia =>
                    {
                        ONRepositorio.Instancia.AdicionarNovedadIngresoConGuia(novGuia, idIngreso);
                    });
                }
                else
                {
                    ///Inserto Guia en IngresoConNOGuia_OPN
                    long idIngreso = ONRepositorio.Instancia.AdicionarIngresoSinGuia(guiaAIngresar, numCtrlMan);
                    esIngresoConGuia = false;

                    //Ingreso NovedadGuia
                    guiaAIngresar.LstNovedadesGuias.ForEach(novGuia =>
                    {
                        ONRepositorio.Instancia.AdicionarNovedadIngresoSinGuia(novGuia, idIngreso);
                    });
                }
            }
            return esIngresoConGuia;
        }

        /// <summary>
        /// Ingreso en la tabla de ingreso salida transportador
        /// </summary>
        /// <param name="ingresoSalidaTrans"></param>
        internal void IngresarIngresosSalidasTrasnportador(ONIngresoSalidaTransportadorDC ingresoSalidaTrans)
        {
            string error = string.Empty;
            PUCentroServiciosDC centroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(ingresoSalidaTrans.IdLocalidadIngresoSalida);
            ingresoSalidaTrans.IdAgenciaIngresoSalida = centroServicio.IdCentroServicio;
            bool? EsIngreso = ONRepositorio.Instancia.ObtenerTipoIngresoVehiculoAgencia(ingresoSalidaTrans.IdVehiculo, ingresoSalidaTrans.IdAgenciaIngresoSalida);

            if (EsIngreso.HasValue && EsIngreso == ingresoSalidaTrans.EsIngreso)
            {
                if (EsIngreso.Value)
                {
                    error = MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.IN_EL_INGRESO);
                }
                else
                {
                    error = MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.IN_LA_SALIDA);
                }
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL,
                  EnumTipoErrorOperacionNacional.EX_VEHICULO_YA_REGISTRADO.ToString(),
                  string.Format(MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_VEHICULO_YA_REGISTRADO), error, ingresoSalidaTrans.Placa, ingresoSalidaTrans.IdAgenciaIngresoSalida)));
            }

            if (!ingresoSalidaTrans.EsIngreso)
            {
               List<long> manifiestos = ONRepositorio.Instancia.ObtenerManiAbiCiudadOrigVehiculo(ingresoSalidaTrans.IdLocalidadIngresoSalida, ingresoSalidaTrans.Placa);

               if (manifiestos.Count > 0)
               {

                
                   StringBuilder man = new StringBuilder ();
                   manifiestos.ForEach(m=>
                       {
                           man.AppendLine(m.ToString());
                       });

                   throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL,
                   EnumTipoErrorOperacionNacional.EX_VEHICULO_ASIGNADO_A_MANIFIESTO_ABIERTO.ToString(),
                   string.Format(MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_VEHICULO_ASIGNADO_A_MANIFIESTO_ABIERTO), man.ToString())));

               }

            }
            ONRepositorio.Instancia.IngresarIngresosSalidasTrasnportador(ingresoSalidaTrans);
        }
    }
}