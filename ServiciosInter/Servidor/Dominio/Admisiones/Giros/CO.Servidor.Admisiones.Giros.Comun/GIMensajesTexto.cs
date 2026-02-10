using CO.Controller.Servidor.Integraciones;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Admisiones.Giros.Comun
{
    public class GIMensajesTexto
    {


        private static readonly GIMensajesTexto instancia = new GIMensajesTexto();

        public static GIMensajesTexto Instancia
        {
            get
            {
                return instancia;
            }
        }



        public void EnviarMensajeTexto(GIEnumMensajeTexto tipo, string numeroCelular, long? numeroGuia, string municipio = "")
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
                            case GIEnumMensajeTexto.VentaGiro:
                                mensaje = string.Format(MensajesTexto.Instancia.ObtenerMensajeTexto(GIEnumMensajeTexto.VentaGiro.ToString()), municipio.ToString());
                                break;
                            case GIEnumMensajeTexto.EntregaGiro:
                                mensaje = string.Format(MensajesTexto.Instancia.ObtenerMensajeTexto(GIEnumMensajeTexto.EntregaGiro.ToString()), numeroGuia.ToString());
                                break;
                            default:
                                Console.WriteLine("Default case");
                                break;
                        }

                        bool rtaParametroEsPruebas;

                        if (!bool.TryParse(PAAdministrador.Instancia.ConsultarParametrosFramework("EsAmbientePruebas"), out rtaParametroEsPruebas))
                            rtaParametroEsPruebas = true;

                        if (!rtaParametroEsPruebas)
                        {
                            MensajesTexto.Instancia.EnviarMensajeTexto(numeroCelular, mensaje);
                        }
                    }
                    catch (Exception exc)
                    {
                        AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, exc.ToString(), COConstantesModulos.GIROS);
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
