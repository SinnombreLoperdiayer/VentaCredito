using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Seguridad.Datos;
using VentaCredito.Transversal;
using VentaCredito.Transversal.Entidades.Clientes;

namespace VentaCredito.Seguridad
{
    public class SeguridadServicio
    {
        private static SeguridadServicio instancia = new SeguridadServicio();

        public static SeguridadServicio Instancia
        {
            get { return instancia; }
        }

        public Autenticacion ValidarAutorizacionServicio()
        {
            var autenticacion = new Autenticacion();
            try
            {
                autenticacion = SeguridadRepositorio.Instancia.ValidarAutorizacionServicio();
            }
            catch(Exception ex)
            {                
            }
            return autenticacion;
        }

        public Autenticacion ValidarUsuarioServicio()
        {
            var autenticacion = new Autenticacion();
            try
            {
                autenticacion = SeguridadRepositorio.Instancia.ValidarUsuarioServicio();
            }
            catch (Exception ex)
            {
            }
            return autenticacion;
        }

        /// <summary>
        /// Servicio que obtiene el usuario y el token asociado a un cliente credito
        /// Hevelin Dayana Diaz Susa - 22/09/2022
        /// </summary>
        /// <param name="IdClienteCredito"></param>
        public UsuarioIntegracion ObtenerUsuarioIntegracionPorIdCliente(int IdClienteCredito)
        {
            UsuarioIntegracion usuario = new UsuarioIntegracion();
            try
            {
                usuario = SeguridadRepositorio.Instancia.ObtenerUsuarioIntegracionPorIdCliente(IdClienteCredito);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return usuario;
        }

        /// <summary>
        /// Servicio que obtiene el token registrado en tabla usuarios integracion, filtrado por el id del usuario integracion
        /// Hevelin Dayana Diaz Susa - 01/11/2022
        /// </summary>
        /// <param name="UsuarioIntegracion"></param>
        public UsuarioIntegracion ObtenerTokenUsuarioIntegracionPorUsuario(string UsuarioIntegracion)
        {
            UsuarioIntegracion usuario = new UsuarioIntegracion();
            try
            {
                usuario = SeguridadRepositorio.Instancia.ObtenerTokenUsuarioIntegracionPorUsuario(UsuarioIntegracion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return usuario;
        }


        /// <summary>
        /// método que valida si un cliente crédito tiene las mismas credenciales en base de datos que las que ingresa en los headers al momento de 
        /// consumir un servicio con el decorador [AdministradorSeguridad]
        /// Hevelin Dayana Diaz Susa - 22/09/2022
        /// </summary>
        /// <param name="IdClienteCredito"></param>
        /// <param name="Usuario"></param>
        /// <param name="Token"></param>
        public Autenticacion ValidarCredencialesClienteCredito(string Usuario, string Token, int IdClienteCredito)
        {
            Autenticacion autenticacion = new Autenticacion();
            UsuarioIntegracion usuarioCredencial = new UsuarioIntegracion();

            if (Usuario == "userAlcarritoPrd")
            {
                usuarioCredencial = ObtenerTokenUsuarioIntegracionPorUsuario(Usuario);
            }
            else
            {
                usuarioCredencial = ObtenerUsuarioIntegracionPorIdCliente(IdClienteCredito);
            }

            if (!string.IsNullOrEmpty(usuarioCredencial.UsuarioCliente))
            {
                if(usuarioCredencial.UsuarioCliente == Usuario && (usuarioCredencial.Token).ToLower() == Token.ToLower())
                {
                    autenticacion.ErrorAutenticacion = "Usuario autorizado";
                    autenticacion.EstaAutorizado = true;
                }
                else
                {
                    autenticacion.ErrorAutenticacion = "Usuario no autorizado";
                    autenticacion.EstaAutorizado = false;
                }
            }

            return autenticacion;
        }

        /// <summary>
        /// método que consulta columna verificación de contenido de la tabla clientescredito_seg
        /// Hevelin Dayana Diaz Susa - 12/10/2022
        /// </summary>
        /// <param name="IdCliente"></param>
        public bool ConsultarVeriContenidoClienteCredito(int IdCliente)
        {
            return SeguridadRepositorio.Instancia.ConsultarVeriContenidoClienteCredito(IdCliente);
        }

    }
}
