using System;
using System.ServiceModel;
using System.Text.RegularExpressions;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Recogidas;
using ServiciosInter.Infraestructura.AccesoDatos.Repository.Recogidas;
using System.Collections.Generic;

namespace ServiciosInter.Negocio.Recogidas
{
    public class RecogidasNegocio
    {
        private static readonly RecogidasNegocio instancia = new RecogidasNegocio();

        public static RecogidasNegocio Instancia
        {
            get
            {
                return instancia;
            }
        }

        private RecogidasNegocio()
        {
        }

        /// <summary>
        /// Obtiene la ultima solicitud registrada
        /// </summary>
        /// <returns></returns>
        public RGRecogidasDC ObtenerUltimaSolicitud(string numeroDocumento)
        {
            return RecogidasRepository.Instancia.ObtenerUltimaSolicitud(numeroDocumento == null ? "0" : numeroDocumento);
        }

        /// <summary>
        /// Inserta las recogidas esporadicas
        /// </summary>
        /// <param name="recogida"></param>
        /// <returns></returns>
        public long InsertarRecogidaEsporadica(RGRecogidasDC recogida)
        {
            long id = 0;

            if (TiposValidos(recogida))
            {
                id = RecogidasRepository.Instancia.InsertarSolicitudRecogida(recogida);
            }
            else
            {
                throw new FaultException<Exception>(new Exception("Error en contenido de datos"));
            }

            /*if (recogida.FechaRecogida.Year > 1 && recogida.FechaRecogida.Date.ToShortDateString() == DateTime.Now.Date.ToShortDateString())
            {
                MotorRecogidas.Instancia.NotificarNuevaRecogida(recogida.Ciudad, true, recogida.Direccion);
            }

            try
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<HubPrincipal>();

                long documento = 0;
                long.TryParse(recogida.NumeroDocumento, out documento);
                var mensaje = new ParametrosSignalR
                {
                    IdSolicitud = recogida.IdAsignacion,
                    Documento = documento,
                    IdLocalidad = id.ToString(),
                    IdAplicacion = (COEnumIdentificadorAplicacion)Enum.Parse(typeof(COEnumIdentificadorAplicacion), "1" ),
                    Mensaje = "se asigno la recogida :" + id.ToString() + "\n Mensajero :" + recogida.NumeroDocumento
                };

                NotificarAdministradores(mensaje);
                NotificaNuevaAppsRecogidas(mensaje);
            }
            catch
            {
            }*/
            return id;
        }

        private bool TiposValidos(RGRecogidasDC recogida)
        {
            long lngNumero = 0;
            if (!long.TryParse(recogida.NumeroDocumento, out lngNumero))
            {
                return false;
            }

            Regex patron = new Regex(@"^\d{10}|^\d{7}");
            if (!patron.IsMatch(recogida.NumeroTelefono))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Retorna el valor del parametro del id indicado
        /// </summary>
        /// <returns></returns>
        public string ObtenerHoraLimiteRecogida()
        {
            return RecogidasRepository.Instancia.ObtenerHoraLimiteRecogida();
        }

        public List<RGRecogidaEsporadicaDC> ObtenerMisRecogidasClientePeaton(string idUsuario)
        {
            return RecogidasRepository.Instancia.ObtenerMisRecogidasClientePeaton(idUsuario);
        }
    }
}