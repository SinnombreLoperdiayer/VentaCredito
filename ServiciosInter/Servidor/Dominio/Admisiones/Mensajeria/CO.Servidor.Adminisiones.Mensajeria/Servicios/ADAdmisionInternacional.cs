using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Adminisiones.Mensajeria.Datos;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Excepciones;
using System.Data.SqlClient;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;

namespace CO.Servidor.Adminisiones.Mensajeria.Servicios
{
    internal class ADAdmisionInternacional : ADAdmisionServicio
    {
        #region Singleton

        public static readonly ADAdmisionInternacional Instancia = new ADAdmisionInternacional();

        #endregion Singleton

        #region Métodos

        /// <summary>
        /// Inserta la admisión de una notificación
        /// </summary>
        /// <param name="idAdmisionesMensajeria"></param>
        /// <param name="notificacion"></param>
        public void AdicionarAdmisionTipoEmpaque(long idAdmisionesMensajeria, TATipoEmpaque tipoEmpaque)
        {
            ADRepositorio.Instancia.AdicionarAdmisionTipoEmpaque(idAdmisionesMensajeria, tipoEmpaque, ControllerContext.Current.Usuario);
        }


        // todo:id Campos Adicionales en Guia Internacional (Nueva Tabla uno a uno con AdmisionMensajeria_MEN)
        public void AdicionarAdmisionInternacional(ADGuiaInternacionalDC guiaInternacional, SqlConnection conexion, SqlTransaction transaccion)
        {
            ADRepositorio.Instancia.AdicionarAdmisionInternacional( guiaInternacional, ControllerContext.Current.Usuario,conexion,transaccion);
        }

        #endregion Métodos
    }
}