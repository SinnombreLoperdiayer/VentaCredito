using CO.Controller.Servidor.Integraciones;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Recogidas.Comun
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

        public void EnviarMensajeTexto(RGEnumMensajesTexto tipo, string numeroCelular,  string campo1 = "" , string campo2 = "")
        {
            if (NumeroCelValido(numeroCelular))
            {
                string mensaje = string.Empty;
                switch (tipo)
                {
                    case RGEnumMensajesTexto.Solicitudrec:
                        mensaje = string.Format(MensajesTexto.Instancia.ObtenerMensajeTexto(RGEnumMensajesTexto.Solicitudrec.ToString()), campo1 , campo2);
                        break;

                    default:
                        Console.WriteLine("Default case");
                        break;
                }
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    bool rtaParametroEsPruebas = true;
                    try
                    {
                        bool.TryParse(PAAdministrador.Instancia.ConsultarParametrosFramework("EsAmbientePruebas"), out rtaParametroEsPruebas);                            

                        if (!rtaParametroEsPruebas)
                        {
                            MensajesTexto.Instancia.EnviarMensajeTexto(numeroCelular, mensaje);
                        }
                    }
                    catch (Exception exc)
                    {
                        AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, exc.ToString(), COConstantesModulos.MODULO_RECOGIDAS);
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
