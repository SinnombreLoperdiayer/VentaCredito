using CO.Servidor.Adminisiones.Mensajeria;
using CO.Servidor.ReversionEstadosGuia.Datos;
using CO.Servidor.ReversionEstadosGuia.ReglasCambioEstado;
using CO.Servidor.ServicioalCliente;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.ReversionEstados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Transactions;

namespace CO.Servidor.ReversionEstadosGuia
{
    public class ADAdministradorReversionEstadosGuia
    {
        private static readonly ADAdministradorReversionEstadosGuia instancia = new ADAdministradorReversionEstadosGuia();
        
        /// <summary>
        /// Retorna una instancia del administrador de clientes
        /// </summary>
        public static ADAdministradorReversionEstadosGuia Instancia
        {
            get { return ADAdministradorReversionEstadosGuia.instancia; }
        }       


        public bool GrabarCambioDeEstadoGuia(ReversionEstado reversionEstado)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    if (reversionEstado.GeneraRAP)
                    {
                        //Pendiente Implementar generacion rap                        
                        //GeneracionRaps(reversionEstado);                        
                    }

                    var datosHistoricoGuia = new DatosGuiaHistorico();

                    datosHistoricoGuia.IdAuditoria = ADReversionEstadosRepositorio.Instancia.GrabarAuditoriaEstadoGuia(reversionEstado);
                    datosHistoricoGuia.NumeroGuia = reversionEstado.NumeroGuia;                    

                    InsertarHistoricoGuia(datosHistoricoGuia);

                    EjecucionReglasCambioEstadoGuia(reversionEstado);

                    if (!AfectacionCajasCambioEstado(reversionEstado))
                    {
                        return false;
                    }

                    transaction.Complete();
                    return true;
                }
                catch(Exception ex)
                {
                    throw new FaultException(string.Format("{0}\n{1}\n{2}",ex.Message, ex.InnerException == null ? string.Empty : ex.InnerException.Message,ex.StackTrace) );
                }
            }

            //Validar si se debe crear el rap con la variable generaRAP y ejecutarlo respectivamente            
        }

        public void GrabarHistoricoEstadoGuia(ReversionEstado reversionEstado)
        {
            throw new NotImplementedException();
        }

        private void GeneracionRaps(ReversionEstado reversionEstado)
        {
            var guiaEstadoAnterior = ObtenerCambioEstadoAnterior(reversionEstado.NumeroGuia);

            if(guiaEstadoAnterior == null)
            {

            }
        }

        private void EjecucionReglasCambioEstadoGuia(ReversionEstado reversionEstado)
        {
            var administradorCambiosEstado = new AdministradorReglasCambioEstadoGuia(reversionEstado);
            administradorCambiosEstado.EjecucionReglasCambioEstado();
        }

        public ADGuia ObtenerEstadoGuia(long numeroGuia)
        {
            return ADFachadaAdmisionesMensajeria.Instancia.ObtenerGuiaXNumeroGuia(numeroGuia);
        }

        public List<ADTrazaGuia> ObtenerTrazaGuia(long numeroGuia)
        {
            List<ADTrazaGuia> traza = ADConsultas.ObtenerEstadosGuia(numeroGuia);
            ADGuia guiatmp = ObtenerEstadoGuia(numeroGuia);
            foreach (ADTrazaGuia guia in traza)
            {
                guia.EstadoHabilitado = VerificarCambioEstadoPermitido(numeroGuia, (int)guiatmp.TrazaGuiaEstado.IdEstadoGuia, (int)guia.IdEstadoGuia);
            }
            return traza;
        }

        public bool VerificarCambioEstadoPermitido(long numeroGuia, int idEstadoOrigen, int idEstadoSolicitado)
        {
            return ADReversionEstadosRepositorio.Instancia.VerificarCambioEstadoPermitido(numeroGuia, idEstadoOrigen, idEstadoSolicitado);
        }

        public bool VerificarExistenciaGuia(long numeroGuia)
        {
            try
            {
                var existeGuia = ADFachadaAdmisionesMensajeria.Instancia.ObtenerGuiaXNumeroGuia(numeroGuia);

                if(existeGuia == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool VerificarGuiaPQRS(long numeroGuia)
        {
            try
            {
                var guiaSolicitud = SCAdministrador.Instancia.ObtenerSolicitud(numeroGuia);
                if (guiaSolicitud.IdSolicitud == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<DatosPersonaNovasoftDC> ObtenerEmpleadosNovasoft()
        {
            return ADReversionEstadosRepositorio.Instancia.ObtenerEmpleadosNovasoft();
        }
        public ReversionEstado ObtenerCambioEstadoAnterior(long numeroGuia)
        {
            return ADReversionEstadosRepositorio.Instancia.ObtenerCambioEstadoAnterior(numeroGuia);
        }
        private bool InsertarHistoricoGuia(DatosGuiaHistorico datosHistoricoGuia)
        {

            return ADReversionEstadosRepositorio.Instancia.InsertarHistoricoGuia(datosHistoricoGuia);

        }

        public DatosAfectacionCaja ObtenerComprobanteFormaPago(long numeroGuia)
        {
            return ADReversionEstadosRepositorio.Instancia.ObtenerComprobanteFormaPago(numeroGuia);
        }


        /// <summary>
        /// Valida si el envío es Al Cobro y Tiene Número de Comprobante
        /// </summary>
        /// <param name="reversionEstado"></param>
        /// <returns></returns>
        private bool AfectacionCajasCambioEstado(ReversionEstado reversionEstado)
        {
            var resultado = false;
            var validaGI = Instancia.ObtenerEstadoGuia(reversionEstado.NumeroGuia);
            var validaComprobanteFP = ObtenerComprobanteFormaPago(reversionEstado.NumeroGuia);

            //Valida si el estado de la guia es Anulado y si es Guia interna para reasignar los valores.              
            if (reversionEstado.IdEstadoOrigen == 15 && reversionEstado.IdEstadoSolicitado != 1 && validaGI.TipoCliente != ADEnumTipoCliente.INT)
            {
                ReasignarValoresCaja(reversionEstado);
            }

            /*Valida que el envío sea concepto Pago de al Cobro y que tenga número de comprobante*/
            if (!string.IsNullOrEmpty(validaComprobanteFP.NumeroComprobante) && (validaComprobanteFP.IdConceptoCaja == 18 || validaComprobanteFP.IdConceptoCaja == 22))
            {
                if (reversionEstado.IdEstadoSolicitado == 1)
                {
                    resultado = false;
                }
                else
                {
                    resultado = true;
                }
            }
            else
            {
                resultado = true;
            }
            return resultado;
        }

        private void ReasignarValoresCaja(ReversionEstado reversionEstado)
        {
            ADReversionEstadosRepositorio.Instancia.ReasignarValoresCaja(reversionEstado);
        }

        private int ConsultarFormaPago(ReversionEstado reversionEstado)
        {
            return ADConsultas.Instancia.ObtenerFormasPagoGuia(reversionEstado.NumeroGuia).FirstOrDefault().IdFormaPago;
        }

        private DatosAfectacionCaja ConsultarAfectacionCaja(ReversionEstado reversionEstado)
        {
            return ADConsultas.Instancia.ObtenerAfectacionCaja(reversionEstado.NumeroGuia);
        }

        private void ReversionCajas(ReversionEstado reversionEstado)
        {
            //ADReversionEstadosRepositorio.Instancia.ReversarCajas(reversionEstado);
        }
    }
}