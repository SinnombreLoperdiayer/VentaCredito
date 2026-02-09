using CO.Controller.Servidor.Integraciones;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Comun
{
   public class AdministradorMensajesTexto
    {

        private static readonly AdministradorMensajesTexto instancia = new AdministradorMensajesTexto();

        public static AdministradorMensajesTexto Instancia
        {
            get
            {
                return instancia;
            }
        }

        public void EnviarMensajeTexto(LOIEnumMensajesTexto tipo, string numeroCelular, long numeroGuia , string campo1 = "")
        {
            if (NumeroCelValido(numeroCelular))
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    try
                    {
                        string mensaje = string.Empty;
                switch (tipo)
                {
                    case LOIEnumMensajesTexto.EntregaMovil:
                        mensaje = string.Format(MensajesTexto.Instancia.ObtenerMensajeTexto(LOIEnumMensajesTexto.EntregaMovil.ToString()), numeroGuia.ToString());
                        break;
                    case LOIEnumMensajesTexto.DevolucionMovil:
                        mensaje = string.Format(MensajesTexto.Instancia.ObtenerMensajeTexto(LOIEnumMensajesTexto.DevolucionMovil.ToString()), numeroGuia.ToString(), campo1);
                        break;
                    case LOIEnumMensajesTexto.Incautado:
                        mensaje = string.Format(MensajesTexto.Instancia.ObtenerMensajeTexto(LOIEnumMensajesTexto.Incautado.ToString()), numeroGuia.ToString());
                        break;
                    case LOIEnumMensajesTexto.Hurto:
                        mensaje = string.Format(MensajesTexto.Instancia.ObtenerMensajeTexto(LOIEnumMensajesTexto.Hurto.ToString()), numeroGuia.ToString());
                        break;
                    case LOIEnumMensajesTexto.Custodia:
                        mensaje = string.Format(MensajesTexto.Instancia.ObtenerMensajeTexto(LOIEnumMensajesTexto.Custodia.ToString()), numeroGuia.ToString());
                        break;
                    case LOIEnumMensajesTexto.Vencimientoreo:
                        mensaje = string.Format(MensajesTexto.Instancia.ObtenerMensajeTexto(LOIEnumMensajesTexto.Vencimientoreo.ToString()), numeroGuia.ToString(), DateTime.Now.AddDays(5).Date.ToString() );
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
               
                    bool rtaParametroEsPruebas = true;

                        bool.TryParse(PAAdministrador.Instancia.ConsultarParametrosFramework("EsAmbientePruebas"), out rtaParametroEsPruebas);
                            
                        if (!rtaParametroEsPruebas)
                        {
                            MensajesTexto.Instancia.EnviarMensajeTexto(numeroCelular, mensaje);
                        }
                    }
                    catch (Exception exc)
                    {
                        AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, exc.ToString(), COConstantesModulos.LOGISTICA_INVERSA);
                    }

                }, System.Threading.Tasks.TaskCreationOptions.PreferFairness);


            }
         
        }



        public bool NumeroCelValido(string pNumCel)
        {
            if (pNumCel.Length != 10) return false;

            long Valor;
            if (!long.TryParse(pNumCel, out Valor)) return false;

            if (pNumCel.Substring(0, 1) != "3") return false;

            return true;
        }


    }
}
