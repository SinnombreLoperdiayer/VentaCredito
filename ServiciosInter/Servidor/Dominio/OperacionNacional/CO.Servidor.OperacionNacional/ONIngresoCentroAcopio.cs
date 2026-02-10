using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.OperacionNacional.Comun;
using CO.Servidor.OperacionNacional.Datos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using CO.Servidor.Dominio.Comun.AdmEstadosConsolidado;
using System.ServiceModel;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
namespace CO.Servidor.OperacionNacional
{
    internal class ONIngresoCentroAcopio : ControllerBase
    {
        private static readonly ONIngresoCentroAcopio instancia = (ONIngresoCentroAcopio)FabricaInterceptores.GetProxy(new ONIngresoCentroAcopio(), COConstantesModulos.MODULO_OPERACION_NACIONAL);


        /// <summary>
        /// Retorna una instancia de ONManejadorIngresoRuta
        /// </summary>
        public static ONIngresoCentroAcopio Instancia
        {
            get { return ONIngresoCentroAcopio.instancia; }
        }

        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        private IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        /// <summary>
        /// Método para ingresar los consolidados en el ingreso a centro de acopio nacional
        /// </summary>
        /// <param name="controlTrans"></param>
        /// <param name="noPrecinto"></param>
        /// <param name="noConsolidado"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<ONConsolidado> IngresarManifiestoConsolidado(long controlTrans, long noPrecinto, string noConsolidado, long idCentroServicio)
        {
            List<ONConsolidado> consolidados = new List<ONConsolidado>();
            //Consultar una lista con los consolidados de acuerdo al filtro
            consolidados = ONRepositorio.Instancia.ObtenerManifiestosConsolidados(controlTrans, noPrecinto, noConsolidado);
            if (consolidados.Any())
                using (TransactionScope scope = new TransactionScope())
                {
                    {
                        consolidados.ForEach(con =>
                        {
                            ONRepositorio.Instancia.CambiarEstadoConsolidado(con.IdManfiestoConsolidado);
                            ONRepositorio.Instancia.IngresoConsolidado(con, idCentroServicio);
                            ECAdminEstadosConsolidado.GuardarEstadoConsolidado(new ECEstadoConsolidado { NoTula = con.NumeroContenedorTula, Estado = EnumEstadosConsolidados.INN, IdCentroServicios = idCentroServicio, Observaciones = string.Empty });
                        });
                    }
                    scope.Complete();
                }
            else
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL,
                    EnumTipoErrorOperacionNacional.EX_CONSOLIDADO_NO_ENCONTRADO.ToString(),
                    MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_CONSOLIDADO_NO_ENCONTRADO)));

            }
            return consolidados;
        }

        /// <summary>
        /// Método para ingresar una guía suelta a centro de acopio nacional
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public ONEnviosDescargueRutaDC IngresarGuiaSuelta(ONEnviosDescargueRutaDC guia, List<OUNovedadIngresoDC> listaNovedades)
        {
            ADGuia guiaAdmision;
            guiaAdmision = fachadaMensajeria.ObtenerInfoGuiaXNumeroGuia(guia.NumeroGuia.Value);

            PUCentroServiciosDC agencia = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(guia.IdCiudadOrigen);



            using (TransactionScope scope = new TransactionScope())
            {
                if (guiaAdmision.IdAdmision != 0)
                {
                    guia.IdAdmisionMensajeria = guiaAdmision.IdAdmision;

                    ADTrazaGuia estadoActual = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(guia.NumeroGuia.Value);


                    if (guia.IdCiudadOrigen == guiaAdmision.IdCiudadOrigen && estadoActual.IdCentroServicioEstado == ControllerContext.Current.IdCentroServicio)
                    {

                        ControllerException excepcion =
                                                 new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_INGRESO_INVALIDO.ToString(),
                                                 MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_INGRESO_INVALIDO));
                        throw new FaultException<ControllerException>(excepcion);
                    }

                    if (estadoActual.IdEstadoGuia != (Int16)ADEnumEstadoGuia.TransitoNacional 
                        && estadoActual.IdEstadoGuia != (Int16)ADEnumEstadoGuia.TransitoRegional 
                        && estadoActual.IdEstadoGuia != (Int16)ADEnumEstadoGuia.TransitoUrbano 
                        && estadoActual.IdEstadoGuia != (Int16)ADEnumEstadoGuia.CentroAcopio 
                        && estadoActual.IdEstadoGuia != (Int16)ADEnumEstadoGuia.Admitida 
                        && estadoActual.IdEstadoGuia != (Int16)ADEnumEstadoGuia.Reenvio 
                        && estadoActual.IdEstadoGuia != (Int16)ADEnumEstadoGuia.PendienteIngresoaCustodia 
                        && estadoActual.IdEstadoGuia != (Int16)ADEnumEstadoGuia.DevolucionRegional
                        && estadoActual.IdEstadoGuia != (Int16)ADEnumEstadoGuia.Distribucion) 
                    {

                        string descripcionEstado = ", Estado actual " + estadoActual.DescripcionEstadoGuia;
                        ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL
                                        , EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO.ToString()
                                        , MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO));
                        excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                        throw new FaultException<ControllerException>(excepcion);
                    }



                    ADTrazaGuia estadoGuia = new ADTrazaGuia
                    {
                        Ciudad = guia.NombreCiudadOrigen,
                        IdCiudad = guia.IdCiudadOrigen,
                        IdAdmision = guia.IdAdmisionMensajeria,
                        IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(guia.IdAdmisionMensajeria)),
                        IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.CentroAcopio,
                        Modulo = COConstantesModulos.MODULO_OPERACION_NACIONAL,
                        NumeroGuia = guia.NumeroGuia,
                        Observaciones = string.Empty,
                        NumeroPieza = guia.PiezaActualRotulo,
                        TotalPiezas = guia.TotalPiezasRotulo
                    };
                    estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);
             
                    //// Movimiento-Inventario  (Ingreso al COL)
                    //PUMovimientoInventario movInventario = new PUMovimientoInventario();
                    //movInventario.TipoMovimiento = PUEnumTipoMovimientoInventario.Ingreso;
                    //movInventario.IdCentroServicioOrigen =  ControllerContext.Current.IdCentroServicio ;
                    //movInventario.Bodega = new PUCentroServiciosDC() { IdCentroServicio = ControllerContext.Current.IdCentroServicio };
                    //movInventario.NumeroGuia = guia.NumeroGuia.Value;
                    //movInventario.FechaGrabacion = DateTime.Now;
                    //movInventario.FechaEstimadaIngreso = DateTime.Now;
                    //movInventario.CreadoPor = ControllerContext.Current.Usuario;
                    //COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().AdicionarMovimientoInventario(movInventario);
                    ////////////////////////////////////////////

                    guia.IdIngresoGuia = ONRepositorio.Instancia.GuardarIngresoGuiaAgencia(guia, agencia.IdCentroServicio);
                    if (listaNovedades != null && listaNovedades.Any())
                    {
                        listaNovedades.ForEach(nov =>
                        {
                            if (nov.Asignado)
                                ONRepositorio.Instancia.GuardarNovedadGuiaIngresada(nov, guia.IdIngresoGuia);
                        });
                    }

                }
                else
                {
                    guia.IdIngresoGuia = ONRepositorio.Instancia.GuardarIngresoGuiaNoAgencia(guia);
                    if (listaNovedades != null && listaNovedades.Any())
                    {
                        listaNovedades.ForEach(nov =>
                        {
                            if (nov.Asignado)
                                ONRepositorio.Instancia.GuardarNovedadGuiaNoIngresada(nov, guia.IdIngresoGuia);
                        });
                    }
                }

                scope.Complete();
                return guia;
            }
        }


    }
}
