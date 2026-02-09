using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Util;

namespace CO.Controller.Servidor.Integraciones.CuatroSieteDos
{
    public  class Integracion472
    {
        public static readonly Integracion472 Instancia = new Integracion472();

        private Integracion472()
        {

        }

        private  string usuarioIntegracion;

        public  string UsuarioIntegracion
        {
            get { return usuarioIntegracion; }        
        }
        private  string passwordIntegracion;

        public  string PasswordIntegracion
        {
            get { return passwordIntegracion; }
        }

        private string uriServicio;

        public string UriServicio
        {
            get { return uriServicio; }            
        }
        /// <summary>
        /// Integra con el servicio de 472
        /// </summary>
        /// <param name="estadoGiro">estado del giro
        /// 1	Giro Pagado
        /// 2	Giro Anulado
        /// 3	Giro Cancelado
        /// 4	Giro Pendiente Pago
        ///</param>
        public void IntegrarCuatroSieteDosAdmision(GIAdmisionGirosDC giro,string estadoGiro)
        {
            Task h = Task.Factory.StartNew(() =>
                {
                    CuatroSieteDos datos = MapearInfoGiro(giro, estadoGiro, true); 
                    ConsumirServicioCuatroSieteDos(datos);
                });            
        }


        /// <summary>
        /// Integra con el servicio de 472 cuando es un pago, anulación o devolución
        /// </summary>
        /// <param name="estadoGiro">estado del giro
        /// 1	Giro Pagado
        /// 2	Giro Anulado
        /// 3	Giro Cancelado
        /// 4	Giro Pendiente Pago
        ///</param>
        public void IntegrarCuatroSieteDosPagoDevAnul(long idAdmisionGiro, string estadoGiro, DateTime fechaPago)
        {
            Task h = Task.Factory.StartNew(() =>
            {
                GIAdmisionGirosDC giro= COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>().ConsultarInformacionPeatonPeaton(idAdmisionGiro);
                CuatroSieteDos datos = MapearInfoGiro(giro, estadoGiro, false,fechaPago);               
                ConsumirServicioCuatroSieteDos(datos);
            });
        }

        private CuatroSieteDos MapearInfoGiro(GIAdmisionGirosDC giro, string estadogiro, bool admision, DateTime? fechaPago = null)
        {
            return new CuatroSieteDos()
                {
                    CodigoPuntoAdmision = giro.AgenciaOrigen.IdCentroServicio.ToString(),
                    CodigoPuntoDestino = giro.AgenciaDestino.IdCentroServicio.ToString(),
                    DireccionDestinatario = Utilidades.EliminarCaracteresEspeciales(giro.GirosPeatonPeaton.ClienteDestinatario.Direccion),
                    DireccionRemitente = Utilidades.EliminarCaracteresEspeciales(giro.GirosPeatonPeaton.ClienteRemitente.Direccion),
                    EstadoGiro = estadogiro,
                    FechaAdmision = admision ? DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") : giro.FechaGrabacion.ToString("dd/MM/yyyy HH:mm:ss"),
                    IdCiudadDestinatario = giro.AgenciaDestino.IdMunicipio,
                    IdCiudadRemitente = giro.AgenciaOrigen.IdMunicipio,
                    IdentificadorDestinatario = giro.GirosPeatonPeaton.ClienteDestinatario.Identificacion,
                    IdentificadorRemitente = giro.GirosPeatonPeaton.ClienteRemitente.Identificacion,
                    NumeroFactura = giro.IdGiro.ToString(),
                    PrimerApellidoDestinatario = giro.GirosPeatonPeaton.ClienteDestinatario.Apellido1,
                    PrimerApellidoRemitente = giro.GirosPeatonPeaton.ClienteRemitente.Apellido1,
                    PrimerNombreDestinatario = giro.GirosPeatonPeaton.ClienteDestinatario.Nombre,
                    PrimerNombreRemitente = giro.GirosPeatonPeaton.ClienteRemitente.Nombre, 
                    SegundoApellidoDestinatario = !string.IsNullOrEmpty(giro.GirosPeatonPeaton.ClienteDestinatario.Apellido2)? giro.GirosPeatonPeaton.ClienteDestinatario.Apellido2:null,
                    SegundoApellidoRemitente = !string.IsNullOrEmpty(giro.GirosPeatonPeaton.ClienteRemitente.Apellido2)?giro.GirosPeatonPeaton.ClienteRemitente.Apellido2:null,
                    TelefonoDestinatario = giro.GirosPeatonPeaton.ClienteDestinatario.Telefono,
                    TelefonoRemitente = giro.GirosPeatonPeaton.ClienteRemitente.Telefono,
                    ValorOperacion = Convert.ToInt32(giro.Precio.ValorGiro).ToString(),
                    ValorPorte = Convert.ToInt32(giro.Precio.ValorServicio) .ToString(), 
                    TipoIdentificacionRemitente = TipoIdentificacion472.ObtenerTipoIdentificacion472(giro.GirosPeatonPeaton.ClienteRemitente.TipoId),
                    TipoIdentificacionDestinatario = TipoIdentificacion472.ObtenerTipoIdentificacion472(giro.GirosPeatonPeaton.ClienteDestinatario.TipoId),
                    Observaciones = giro.Observaciones,
                    FechaPago = fechaPago.HasValue ? fechaPago.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                    SegundoNombreDestinatario=null,
                    SegundoNombreRemitente=null,
                    CelularDestinatario=null,
                    CelularRemitente=null,
                    FechaRecuperacion=null
                };

        }


        /// <summary>
        /// Transmite la transacciona 472
        /// </summary>
        /// <param name="dato"></param>
        private  void ConsumirServicioCuatroSieteDos(CuatroSieteDos dato)
        {
            try
            {
                //consumir servicio de 472 y validar el mensaje de respuesta si no es correcto, se debe enviar el registro a la base de datos 
                
                if (bool.Parse(PAParametros.Instancia.ConsultarParametrosFramework("PermiteIntegra472")))
                {


                    if (string.IsNullOrWhiteSpace(this.UsuarioIntegracion))
                        this.usuarioIntegracion = PAParametros.Instancia.ConsultarParametrosFramework("UsuarioIntegra472");

                    if (string.IsNullOrWhiteSpace(this.PasswordIntegracion))
                        this.passwordIntegracion = PAParametros.Instancia.ConsultarParametrosFramework("PasswordIntegra472");

                    if(string.IsNullOrWhiteSpace(this.UriServicio))
                        this.uriServicio = PAParametros.Instancia.ConsultarParametrosFramework("UriServicio472");


                    //Solo se tiene encuenta el código del municipio
                    dato.IdCiudadRemitente = dato.IdCiudadRemitente.Substring(0, 5);
                    dato.IdCiudadDestinatario = dato.IdCiudadDestinatario.Substring(0, 5);

                    dato.Usuario = this.UsuarioIntegracion;
                    dato.Contrasena = this.PasswordIntegracion;
                    using (ServicioWeb472.WebService_WS_472 proxy = new ServicioWeb472.WebService_WS_472())
                    {
                        proxy.Url = this.UriServicio;
                        System.Net.ServicePointManager.Expect100Continue = false;
                        //string rst = proxy.ReadTransaction_472(dato.Usuario, dato.Contrasena, dato.NitCanalAliado, dato.NumeroFactura, dato.CodigoPuntoAdmision,
                        //   dato.FechaAdmision, dato.TipoIdentificacionRemitente, dato.IdentificadorRemitente, dato.PrimerNombreRemitente, dato.SegundoNombreRemitente,
                        //   dato.PrimerApellidoRemitente, dato.SegundoApellidoRemitente, dato.IdCiudadRemitente, dato.DireccionRemitente,
                        //    dato.TelefonoRemitente, dato.CelularRemitente, dato.ValorOperacion, dato.ValorPorte,
                        //    dato.CodigoPuntoDestino, dato.EstadoGiro, dato.TipoIdentificacionDestinatario, dato.IdentificadorDestinatario, dato.PrimerNombreDestinatario,
                        //    dato.SegundoNombreDestinatario, dato.PrimerApellidoDestinatario, dato.SegundoApellidoDestinatario, dato.IdCiudadDestinatario,
                        //    dato.DireccionDestinatario, dato.TelefonoDestinatario, dato.CelularDestinatario, dato.FechaPago, dato.FechaRecuperacion, dato.Observaciones);

                        string rst = proxy.ReadTransaction_472(dato.Usuario, dato.Contrasena, dato.NitCanalAliado, dato.NumeroFactura, dato.CodigoPuntoAdmision,
                       dato.FechaAdmision, dato.TipoIdentificacionRemitente, dato.IdentificadorRemitente, dato.PrimerNombreRemitente, !string.IsNullOrWhiteSpace(dato.SegundoNombreRemitente) ? dato.SegundoNombreRemitente : "",
                       dato.PrimerApellidoRemitente, !string.IsNullOrWhiteSpace(dato.SegundoApellidoRemitente) ? dato.SegundoApellidoRemitente : "", dato.IdCiudadRemitente, dato.DireccionRemitente,
                        dato.TelefonoRemitente, !string.IsNullOrWhiteSpace(dato.CelularRemitente) ? dato.CelularRemitente : "", dato.ValorOperacion, dato.ValorPorte,
                        dato.CodigoPuntoDestino, dato.EstadoGiro, dato.TipoIdentificacionDestinatario, dato.IdentificadorDestinatario, dato.PrimerNombreDestinatario,
                        !string.IsNullOrWhiteSpace(dato.SegundoNombreDestinatario) ? dato.SegundoNombreDestinatario : "", dato.PrimerApellidoDestinatario, !string.IsNullOrWhiteSpace(dato.SegundoApellidoDestinatario) ? dato.SegundoApellidoDestinatario : "", dato.IdCiudadDestinatario,
                        dato.DireccionDestinatario, dato.TelefonoDestinatario, !string.IsNullOrWhiteSpace(dato.CelularDestinatario) ? dato.CelularDestinatario : "", !string.IsNullOrWhiteSpace(dato.FechaPago) ? dato.FechaPago : "", !string.IsNullOrWhiteSpace(dato.FechaRecuperacion) ? dato.FechaRecuperacion : "", dato.Observaciones);


                        if (rst != "Transacción Exitosa")
                        {

                            AccesoDatos.DALCuatroSieteDos.DatosCuatroSieteDos.Instancia.GrabarTransaccionFallida(dato, rst);
                        }
                        else
                        {
                            Debug.WriteLine("Transacción Exitosa: " + dato.NumeroFactura);
                        }
                        
                    }

                }
                else
                {                
                    AccesoDatos.DALCuatroSieteDos.DatosCuatroSieteDos.Instancia.GrabarTransaccionFallida(dato,"No esta habilitada la integración");
                }
                

            }
            catch (Exception ex)
            {
                
                // se debe enviar el registro a la base de datos                 
                AccesoDatos.DALCuatroSieteDos.DatosCuatroSieteDos.Instancia.GrabarTransaccionFallida(dato, "Error integración 472: " + ex.Message);
            }
        }

     
        /// <summary>
        /// Retransmite a 472 las transacciones fallidas
        /// </summary>
        public void ReTransmitirTransaccionesFallidas()
        {
            Debug.WriteLine("iniciando retransmision");
            if (string.IsNullOrWhiteSpace(this.usuarioIntegracion))
                this.usuarioIntegracion = PAParametros.Instancia.ConsultarParametrosFramework("UsuarioIntegra472");

            if (string.IsNullOrWhiteSpace(this.passwordIntegracion))
                this.passwordIntegracion = PAParametros.Instancia.ConsultarParametrosFramework("PasswordIntegra472");

                List<CuatroSieteDos> lstTransacciones = AccesoDatos.DALCuatroSieteDos.DatosCuatroSieteDos.Instancia.CargarTransaccionesFallidas(this.UsuarioIntegracion, this.PasswordIntegracion);
                Debug.WriteLine("giros a retransmitir:"+lstTransacciones.Count().ToString());
                
                //lstTransacciones.ForEach(dato=>
                Parallel.For(0, lstTransacciones.Count, (i) =>
                    {
                        var dato = lstTransacciones[i];                   
                   
                        Debug.WriteLine("retransmitiendo giro :"+dato.NumeroFactura);
                        using (ServicioWeb472.WebService_WS_472 proxy = new ServicioWeb472.WebService_WS_472())
                        {
                            System.Net.ServicePointManager.Expect100Continue = false;

                            dato.FechaRecuperacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                           string rst = proxy.ReadTransaction_472(dato.Usuario, dato.Contrasena, dato.NitCanalAliado, dato.NumeroFactura, dato.CodigoPuntoAdmision,
                           dato.FechaAdmision, dato.TipoIdentificacionRemitente, dato.IdentificadorRemitente, dato.PrimerNombreRemitente, dato.SegundoNombreRemitente,
                           dato.PrimerApellidoRemitente, dato.SegundoApellidoRemitente, dato.IdCiudadRemitente, dato.DireccionRemitente,
                           dato.TelefonoRemitente, dato.CelularRemitente, dato.ValorOperacion, dato.ValorPorte,
                           dato.CodigoPuntoDestino, dato.EstadoGiro, dato.TipoIdentificacionDestinatario, dato.IdentificadorDestinatario, dato.PrimerNombreDestinatario,
                           dato.SegundoNombreDestinatario, dato.PrimerApellidoDestinatario, dato.SegundoApellidoDestinatario, dato.IdCiudadDestinatario,
                           dato.DireccionDestinatario, dato.TelefonoDestinatario, dato.CelularDestinatario, dato.FechaPago, dato.FechaRecuperacion, dato.Observaciones);

                    
                            if (rst == "Transacción Exitosa")
                            {
                                //si es la transaccion es exitosa se borra de la tabla de transacciones fallidas si no, no hacer nada
                                AccesoDatos.DALCuatroSieteDos.DatosCuatroSieteDos.Instancia.BorrarTransaccionFallida(dato.IdTransaccionFallida);
                                Debug.WriteLine("retransmision exitosa giro :" + dato.NumeroFactura);
                            }
                            else
                            {
                                Debug.WriteLine("retransmision fallida giro :" + dato.NumeroFactura +" --- "+rst);
                            }
                        }

                    });

                Debug.WriteLine("retransmision finalizada");
           
        }

    }
}
