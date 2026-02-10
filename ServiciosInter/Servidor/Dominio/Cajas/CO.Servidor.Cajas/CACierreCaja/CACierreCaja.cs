using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using CO.Servidor.Cajas.Apertura;
using CO.Servidor.Cajas.Comun;
using CO.Servidor.Cajas.Datos;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System.ServiceModel;
using CO.Servidor.Dominio.Seguridad;

namespace CO.Servidor.Cajas.CACierreCaja
{
    /// <summary>
    /// clase para cerrar las cajas
    /// </summary>
    internal class CACierreCaja : ControllerBase
    {
        #region Atributos

        private static readonly CACierreCaja instancia = (CACierreCaja)FabricaInterceptores.GetProxy(new CACierreCaja(), COConstantesModulos.CAJA);

        #endregion Atributos

        #region Instancia

        public static CACierreCaja Instancia
        {
            get { return CACierreCaja.instancia; }
        }

        #endregion Instancia

        /// <summary>
        /// Cierra la caja del Aux y Totaliza Procesos.
        /// </summary>
        /// <param name="idApertura">Es el id de la apertura.</param>
        /// <param name="idPunto">Es el id del punto.</param>
        public void CerrarCaja(long idCodigoUsuario, long idApertura, long idPunto, int idCaja)
        {
            CARepositorioCaja.Instancia.CerrarCajaAux(idCodigoUsuario, idApertura, idPunto, idCaja);
        }

        /// <summary>
        /// Cierra la Caja del Punto ó Centro de Servicio y actualiza el Id de Cierre de Punto en las Cajas respectivas como reportado.
        /// </summary>
        /// <param name="cajasPuntoCentroServicio">Informacion de Cierre de punto + lista de cajas del punto</param>
        public long CerrarCajaPuntoCentroServcio(CACierreCentroServicioDC cierrePuntoCentroServicio, bool cierreAutomatico = false)
        {
            long idCierreCentroSvc = 0;

            //Verifico la Caja Cero para realizar el Cierre de esta
            if(cierrePuntoCentroServicio != null && cierrePuntoCentroServicio.IdCentroServicio != 0)
            {

                if(!cierreAutomatico)
                    if(!CARepositorioCaja.Instancia.ValidarCierreSistema(cierrePuntoCentroServicio.IdCentroServicio))
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CAJA,
                    "0",
                    "No fué posible ejecutar el cierre manual, ya que no se detectó un cierre automático del sistema para el día de hoy, por favor comuníquese con su Regional Administrativa."));

                using(TransactionScope scope = new TransactionScope())
                {

                    idCierreCentroSvc = CARepositorioCaja.Instancia.CerrarCajaPuntoCentroServcio(cierrePuntoCentroServicio.IdCentroServicio);
                    scope.Complete();
                }
            }

            return idCierreCentroSvc;
        }

        /// <summary>
        /// Cierre automatico de Cajas Auxiliares ejecuta por el
        /// Cajero ppal para cerrar la Caja del Punto o centro de Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <param name="idUsuario">The id usuario.</param>
        public void CerrarCajasAutomaticamentePorCentroSvc(CARegistroTransacCajaDC movimientoCaja)
        {
            List<CACierreCajaDC> cajasParaCerrar = ObtenerCajasAReportarCajeroPrincipal(movimientoCaja.IdCentroServiciosVenta, TAConstantesServicios.ID_FORMA_PAGO_DIF_DEMAS, CAConstantesCaja.OPERADOR_LOGICO_DIFERENCIA);

            if(cajasParaCerrar != null)
            {

                List<SEUsuarioPorCodigoDC> cajasExternas = ConsultarCajerosExternos(cajasParaCerrar);

                cajasParaCerrar.ForEach(caja =>
                {
                    if(caja.FechaCierre == null)
                    {
                        if(caja.IdCaja != 0)
                        {
                            long idUsuario = ConstantesFramework.ID_USUARIO_SISTEMA;

                            using(TransactionScope scope = new TransactionScope())
                            {
                                //se cierra la Caja Auxiliar
                                CerrarCaja(idUsuario, caja.IdCierreCaja, caja.IdPuntoAtencion, caja.IdCaja);

                                //se realiza el proceso de reporte al cajero ppal para que
                                //aparezca la transaccion en el cierre del punto
                                LlenarTransaccReporteCajaPpal(movimientoCaja, caja, cajasExternas.Where(c => c.idCaja == caja.IdCaja).FirstOrDefault());
                                ReportarCajaACajeroPrincipal(movimientoCaja, caja.IdCierreCaja);
                                scope.Complete();
                            }
                        }
                    }
                    else
                    {
                        LlenarTransaccReporteCajaPpal(movimientoCaja, caja, cajasExternas.Where(c => c.idCaja == caja.IdCaja).FirstOrDefault());
                        ReportarCajaACajeroPrincipal(movimientoCaja, caja.IdCierreCaja);
                    }
                });
            }
        }

        private List<SEUsuarioPorCodigoDC> ConsultarCajerosExternos(List<CACierreCajaDC> cajasParaCerrar)
        {
            List<SEUsuarioPorCodigoDC> resultado = new List<SEUsuarioPorCodigoDC>();
                cajasParaCerrar.ForEach(
                caj =>
                        {
                            if(caj.IdCaja >= 1000)
                            {
                                resultado.Add( ConsultarCajeroExterno(caj));
                            }
                        }
                );

            //return (List<SEUsuarioPorCodigoDC>)(from caja in cajasParaCerrar
            //                                    where caja.IdCaja >= 1000
            //                                    select
            //        select SEGSeguridad.Instancia.ObtenerInfoUsuarioPorDocumento(caja.CreadoPor).ConvertAll(r =>
            //             new SEUsuarioPorCodigoDC
            //             {
            //                 Documento = r.IdDocumento,
            //                 EstadoUsuario = r.Estado.ToString(),
            //                 idCaja = caja.IdCaja,
            //                 IdCodigoUsuario = Convert.ToInt64(r.IdUsuario),
            //                 NombreCompleto = r.Nombre,
            //                 Usuario = r.LoginUsuario,
            //             }));


            //return (List<SEUsuarioPorCodigoDC>)cajasParaCerrar
            //        .Where(caj => caj.IdCaja >= 1000)
            //        .Select(caja => SEGSeguridad.Instancia.ObtenerInfoUsuarioPorDocumento(caja.CreadoPor).ConvertAll(r =>
            //              new SEUsuarioPorCodigoDC
            //              {
            //                  Documento = r.IdDocumento,
            //                  EstadoUsuario = r.Estado.ToString(),
            //                  idCaja = caja.IdCaja,
            //                  IdCodigoUsuario = Convert.ToInt64(r.IdUsuario),
            //                  NombreCompleto = r.Nombre,
            //                  Usuario = r.LoginUsuario,
            //              }));

            return resultado;
        }

        private SEUsuarioPorCodigoDC ConsultarCajeroExterno(CACierreCajaDC caja)
        {
            return SEGSeguridad.Instancia.ObtenerInfoUsuarioPorDocumento(caja.CreadoPor).ConvertAll(r =>
                         new SEUsuarioPorCodigoDC
                         {
                             Documento = r.IdDocumento,
                             EstadoUsuario = r.Estado.ToString(),
                             idCaja = caja.IdCaja,
                             IdCodigoUsuario = Convert.ToInt64(r.IdUsuario),
                             NombreCompleto = r.Nombre,
                             Usuario = r.LoginUsuario,
                         }).FirstOrDefault();
        }

        /// <summary>
        /// Este metodo llena la data para registrar la transaccion en la caja ppal para
        /// reportarlas
        /// </summary>
        /// <param name="movimientoCaja"></param>
        /// <param name="caja"></param>
        private static void LlenarTransaccReporteCajaPpal(CARegistroTransacCajaDC movimientoCaja, CACierreCajaDC caja, SEUsuarioPorCodigoDC usuarioExterno)
        {
            SEUsuarioPorCodigoDC infoCajero = null;
            //consulto la info del cierre
            CACierreCajaDC cierreCaja = CARepositorioCaja.Instancia.ObtenerInfoCierreCaja(caja.IdCierreCaja);

            //obtener info cajero
            if(cierreCaja.IdCaja >= 1000)
            {
                infoCajero = usuarioExterno;
            }
            else
            {
                //cajero controller
                infoCajero = CACaja.Instancia.ObtenerUsuarioPorCodigo(cierreCaja.idUsuario);
            }

            //Actualizo los campos requeridos
            movimientoCaja.ValorTotal = cierreCaja.TotalIngresoEfectivo < 0 ? 0 : cierreCaja.TotalIngresoEfectivo;
            movimientoCaja.RegistrosTransacDetallesCaja.First().ValorServicio = cierreCaja.TotalIngresoEfectivo < 0 ? 0 : cierreCaja.TotalIngresoEfectivo;
            movimientoCaja.RegistrosTransacDetallesCaja.First().Observacion = IdentificaNombreCajero(infoCajero);
            movimientoCaja.RegistrosTransacDetallesCaja.First().Descripcion = CAConstantesCaja.TRANS_DINERO_CAJAAUX_CAJAPPAL;
            movimientoCaja.RegistrosTransacDetallesCaja.First().Numero = caja.IdCierreCaja;
            movimientoCaja.RegistrosTransacDetallesCaja.First().NumeroComprobante = CAConstantesCaja.CAJA_NUMERO + cierreCaja.IdCaja.ToString();
            movimientoCaja.RegistroVentaFormaPago.First().Valor = cierreCaja.TotalIngresoEfectivo < 0 ? 0 : cierreCaja.TotalIngresoEfectivo;
            movimientoCaja.RegistrosTransacDetallesCaja.First().ConceptoEsIngreso = true;
            movimientoCaja.RegistrosTransacDetallesCaja.First().ConceptoCaja.IdConceptoCaja = (int)CAEnumConceptosCaja.TRANS_DINERO_ENTRE_CAJAS;
            movimientoCaja.RegistrosTransacDetallesCaja.First().ConceptoCaja.EsIngreso = true;
        }

        private static string IdentificaNombreCajero(SEUsuarioPorCodigoDC cajero)
        {
            return cajero.NombreUsuario == string.Empty
                    ? cajero.NombreCompleto
                    : cajero.NombreUsuario + " " + cajero.PrimerApellido + "|" + cajero.Documento;
        }

        /// <summary>
        /// Obtener Cierres del Cajero.
        /// </summary>
        /// <param name="idCodigoUsuario">The id codigo usuario.</param>
        /// <returns></returns>
        public List<CACierreCajaDC> ObtenerCierresCajero(long idCodigoUsuario, long idCentroServicio, DateTime fechaCierre, int indicePagina, int tamanoPagina)
        {
            return CARepositorioCaja.Instancia.ObtenerCierresCajero(idCodigoUsuario, idCentroServicio, fechaCierre, indicePagina, tamanoPagina);
        }

        /// <summary>
        /// Obtiene las Cajas Cerradas y Pendientes
        /// para Cerrar de un punto o centro de Servicio
        /// </summary>
        /// <param name="idCentroSrv"></param>
        /// <param name="idFormaPago"></param>
        /// <param name="operador"></param>
        /// <returns></returns>
        public List<CACierreCajaDC> ObtenerCajasAReportarCajeroPrincipal(long idCentroSrv, short idFormaPago, string operador)
        {
            return CARepositorioCaja.Instancia.ObtenerCajasAReportarCajeroPrincipal(idCentroSrv, idFormaPago, operador);
        }

        /// <summary>
        /// Actualiza el cierre de caja como Caja Reportada al cajero Ppal
        /// </summary>
        /// <param name="idCierreCajaAux">The id cierre caja aux.</param>
        public void ReportarCajaACajeroPrincipal(CARegistroTransacCajaDC movimientoCaja, long idCierreCaja)
        {
            //if (movimientoCaja != null)
            //{
            //  //Registro la transaccion en Caja Ppal
            //  CAApertura.Instancia.RegistrarVentaRequiereTipoComprobante(movimientoCaja, null);
            //}

            //Actualizo el cierre de la caja Aux como entregado
            CARepositorioCaja.Instancia.ReportarCajaACajeroPrincipal(idCierreCaja);
        }

        /// <summary>
        /// Metodo que consulta todas las Cajas abiertas
        ///  con su respectivo centro de servicio
        /// </summary>
        /// <returns>lista de cajas abiertas</returns>
        public List<CACierreAutomaticoDC> ObtenerCentrosSvcConCajasAbiertas()
        {
            return CARepositorioCaja.Instancia.ObtenerCentrosSvcConCajasAbiertas();
        }

        /// <summary>
        /// Crear aperturas en cero para todos los centros de servicio que no tengan movimientos en una fecha especifica
        /// </summary>
        public void CrearAperturasEnCero(DateTime fechaApertura)
        {
            CARepositorioCaja.Instancia.CrearAperturasEnCero(fechaApertura);
        }

        /// <summary>
        /// Obtener información complementaria para el cierre de una caja
        /// </summary>
        /// <param name="idCierre">Identificacdor del cierre de caja</param>
        /// <returns>Información complementaria del cierre de caja</returns>
        public CAInfoComplementariaCierreCajaDC ObtenerInfoComplementariaCierreCaja(long idCierreCaja)
        {
            return CARepositorioCaja.Instancia.ObtenerInformacionComplementariaCierreCaja(idCierreCaja);
        }
    }
}