using CO.Servidor.LogisticaInversa.Comun;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Reglas.Telemercadeo
{
    /// <summary>
    /// Regla para manejo de lógica de negocio para titular confirmar dirección correcta
    /// </summary>
    public class LOIReglaDireccionNoExiste : IReglaNegocio
    {
        private string direccionAdmision;
        private string destinatario;
        private string localidad;
  

        /// <summary>
        /// Ejecutar regla especifica.
        /// </summary>
        /// <param name="parametrosRegla">Parámetros de entrada y salida de la regla.</param>

        public void Ejecutar(IDictionary<string, object> parametrosRegla)
        {
            if (parametrosRegla.ContainsKey(LOIConstantesLogisticaInversa.DIRECCION_ADMISION))
                direccionAdmision = (parametrosRegla[LOIConstantesLogisticaInversa.DIRECCION_ADMISION]).ToString().Trim();
     
         if (parametrosRegla.ContainsKey(LOIConstantesLogisticaInversa.DESTINATARIO))
             destinatario = (parametrosRegla[LOIConstantesLogisticaInversa.DESTINATARIO]).ToString().Trim();

                     if (parametrosRegla.ContainsKey(LOIConstantesLogisticaInversa.LOCALIDAD))
             localidad = (parametrosRegla[LOIConstantesLogisticaInversa.LOCALIDAD]).ToString().Trim();

         
            
            bool esdestinatario;
            if (Boolean.TryParse(destinatario, out esdestinatario))
            {
             long idDireccion = LOIReglasRepositorio.Instancia.ObtenerDireccion(direccionAdmision, esdestinatario, localidad);
                if  (idDireccion != 0)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO,
                    LOIEnumTipoErrorLogisticaInversa.EX_ERROR_DIRECCION_NO_EXISTE.ToString(),
                    string.Format(
                    LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_DIRECCION_NO_EXISTE)
                    ,direccionAdmision));
                    parametrosRegla.Add(ClavesReglasFramework.EXCEPCION, excepcion);
                    parametrosRegla.Add(ClavesReglasFramework.HUBO_ERROR, true);
                }
            }
        }
    }
}