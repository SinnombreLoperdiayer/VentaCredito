using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Servidor.Comun;

namespace CO.Controller.Servidor.Integraciones.AccesoDatos.DALCuatroSieteDos
{
    public  class DatosCuatroSieteDos
    {
         public static readonly DatosCuatroSieteDos Instancia = new DatosCuatroSieteDos();

        private const string NombreModelo = "ModeloCuatroSieteDos";

        private DatosCuatroSieteDos()
        {

        }

        

        /// <summary>
        /// graba en base de datos una transaccion fallida
        /// </summary>
        /// <param name="datos"></param>
        public  void GrabarTransaccionFallida(CuatroSieteDos.CuatroSieteDos datos,string mensaje)
        {
            
            using (Modelo.ModeloCuatroSieteDos contexto = new Modelo.ModeloCuatroSieteDos(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Modelo.TransaccionesAliados472 trans = new Modelo.TransaccionesAliados472()
                {
                    CelularDestino = string.IsNullOrWhiteSpace(datos.CelularDestinatario) ? null : datos.CelularDestinatario,
                    CelularImpositor = string.IsNullOrWhiteSpace(datos.CelularRemitente) ? null : datos.CelularRemitente,
                    CiudadDestino = string.IsNullOrWhiteSpace(datos.IdCiudadDestinatario) ? null : datos.IdCiudadDestinatario,
                    CiudadImpositor = string.IsNullOrWhiteSpace(datos.IdCiudadRemitente) ? null : datos.IdCiudadRemitente,
                    CodigoOficinaAdmisión = string.IsNullOrWhiteSpace(datos.CodigoPuntoAdmision) ? null : datos.CodigoPuntoAdmision,
                    CodigoOficinaPago = string.IsNullOrWhiteSpace(datos.CodigoPuntoDestino) ? null : datos.CodigoPuntoDestino,
                    DireccionDestino = string.IsNullOrWhiteSpace(datos.DireccionDestinatario) ? null : datos.DireccionDestinatario,
                    DireccionImpositor = string.IsNullOrWhiteSpace(datos.DireccionRemitente) ? null : datos.DireccionRemitente,
                    Estado = string.IsNullOrWhiteSpace(datos.EstadoGiro) ? null : datos.EstadoGiro,
                     FechaAdmisión = string.IsNullOrWhiteSpace(datos.FechaAdmision) ? null :datos.FechaAdmision,
                     FechaPago = string.IsNullOrWhiteSpace(datos.FechaPago) ? null :datos.FechaPago,
                     FechaRecuperacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    IdentificacionDestinatario = string.IsNullOrWhiteSpace(datos.IdentificadorDestinatario) ? null : datos.IdentificadorDestinatario,
                    IdentificacionImpositor = string.IsNullOrWhiteSpace(datos.IdentificadorRemitente) ? null : datos.IdentificadorRemitente,
                    MontoOperación = string.IsNullOrWhiteSpace(datos.ValorOperacion) ? null : datos.ValorOperacion,
                    NumeroFactura = string.IsNullOrWhiteSpace(datos.NumeroFactura) ? null : datos.NumeroFactura,
                    PorteComision = string.IsNullOrWhiteSpace(datos.ValorPorte) ? null : datos.ValorPorte,
                    PrimerApellidoDestino = string.IsNullOrWhiteSpace(datos.PrimerApellidoDestinatario) ? null : datos.PrimerApellidoDestinatario,
                    PrimerApellidoImpositor = string.IsNullOrWhiteSpace(datos.PrimerApellidoRemitente) ? null : datos.PrimerApellidoRemitente,
                    PrimerNombreDestino = string.IsNullOrWhiteSpace(datos.PrimerNombreDestinatario) ? null : datos.PrimerNombreDestinatario,
                    PrimerNombreImpositor = string.IsNullOrWhiteSpace(datos.PrimerNombreRemitente) ? null : datos.PrimerNombreRemitente,
                    SegundoApellidoDestino = string.IsNullOrWhiteSpace(datos.SegundoApellidoDestinatario) ? null : datos.SegundoApellidoDestinatario,
                    SegundoApellidoImpositor = string.IsNullOrWhiteSpace(datos.SegundoApellidoRemitente) ? null : datos.SegundoApellidoRemitente,
                    SegundoNombreDestino = string.IsNullOrWhiteSpace(datos.SegundoNombreDestinatario) ? null : datos.SegundoNombreDestinatario,
                    SegundoNombreImpositor = string.IsNullOrWhiteSpace(datos.SegundoNombreRemitente) ? null : datos.SegundoNombreRemitente,
                    TelefonoDestino = string.IsNullOrWhiteSpace(datos.TelefonoDestinatario) ? null : datos.TelefonoDestinatario,
                    TelefonoImpositor = string.IsNullOrWhiteSpace(datos.TelefonoRemitente) ? null : datos.TelefonoRemitente,
                    Observaciones=datos.Observaciones,
                    TipoIdDestinatario= datos.TipoIdentificacionDestinatario,
                    TipoIdRemitente=datos.TipoIdentificacionRemitente,
                     DescripcionError= mensaje  
                };

                contexto.TransaccionesAliados472.Add(trans);
                contexto.SaveChanges();

            }
        }

        /// <summary>
        /// Carga todas las transacciones fallidas
        /// </summary>
        public  List<CuatroSieteDos.CuatroSieteDos> CargarTransaccionesFallidas(string usuario, string password)
        {
            using (Modelo.ModeloCuatroSieteDos contexto = new Modelo.ModeloCuatroSieteDos(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
               return  contexto.TransaccionesAliados472.ToList().ConvertAll<CuatroSieteDos.CuatroSieteDos>(t =>
                    new CuatroSieteDos.CuatroSieteDos()
                    {
                        CelularDestinatario =t.CelularDestino,
                        CelularRemitente=t.CelularImpositor,
                        IdCiudadDestinatario=t.CiudadDestino,
                        IdCiudadRemitente=t.CiudadImpositor,
                        CodigoPuntoAdmision=t.CodigoOficinaAdmisión,
                        CodigoPuntoDestino=t.CodigoOficinaPago,
                        DireccionDestinatario=t.DireccionDestino,
                        DireccionRemitente=t.DireccionImpositor,
                        EstadoGiro=t.Estado,
                        FechaAdmision=t.FechaAdmisión,
                        FechaPago=t.FechaPago,
                         FechaRecuperacion= t.FechaRecuperacion,
                        IdentificadorDestinatario=t.IdentificacionDestinatario,
                        IdentificadorRemitente=t.IdentificacionImpositor,
                        ValorOperacion=t.MontoOperación,
                        NumeroFactura=t.NumeroFactura,
                        ValorPorte=t.PorteComision,
                        PrimerApellidoDestinatario=t.PrimerApellidoDestino,
                        PrimerApellidoRemitente=t.PrimerApellidoImpositor,
                        PrimerNombreDestinatario=t.PrimerNombreDestino,
                        PrimerNombreRemitente=t.PrimerNombreImpositor,
                        SegundoApellidoDestinatario=t.SegundoApellidoDestino,
                        SegundoApellidoRemitente=t.SegundoApellidoImpositor,
                        SegundoNombreDestinatario=t.SegundoNombreDestino,
                        SegundoNombreRemitente=t.SegundoNombreImpositor,
                        TelefonoDestinatario=t.TelefonoDestino,
                        TelefonoRemitente=t.TelefonoImpositor,
                        IdTransaccionFallida=t.IdTransaccionesAliados,
                         Contrasena=password,
                         Usuario=usuario,
                          Observaciones=t.Observaciones,
                           TipoIdentificacionDestinatario=t.TipoIdDestinatario,
                            TipoIdentificacionRemitente=t.TipoIdRemitente
                    });

            }
        }

        /// <summary>
        /// Borra una transaccion que se retransmitio de forma exitosa
        /// </summary>
        /// <param name="idTransaccion"></param>
        public void BorrarTransaccionFallida(decimal idTransaccion)
        {
            using (Modelo.ModeloCuatroSieteDos contexto = new Modelo.ModeloCuatroSieteDos(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {

                var transaccion = contexto.TransaccionesAliados472.Where(t => t.IdTransaccionesAliados == idTransaccion).FirstOrDefault();

                if (transaccion != null)
                {
                    contexto.TransaccionesAliados472.Remove(transaccion);
                    contexto.SaveChanges();
                }
            }

        }
    }
}
