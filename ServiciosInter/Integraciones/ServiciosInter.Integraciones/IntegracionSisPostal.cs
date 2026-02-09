using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.ServiceModel;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Integraciones;

namespace ServiciosInter.Integraciones
{
    public class IntegracionSisPostal
    {
        private static readonly IntegracionSisPostal instancia = new IntegracionSisPostal();

        /// <summary>
        /// Retorna una instancia de administracion de produccion
        /// /// </summary>
        public static IntegracionSisPostal Instancia
        {
            get { return instancia; }
        }

        /// <summary>
        /// Metodo para obtener los estados de guia si es sispostal.
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public List<INEstadosSWSispostal> ObtenerEstadosGuiaSispostal(long NGuia)
        {
            string Usuario = ConfigurationManager.AppSettings["UsuarioSispostal"];
            string Password = ConfigurationManager.AppSettings["PasswordSispostal"];
            var type = typeof(SIEnumEstadosGuiasMasivos);

            WSSispostal.sispostal1SoapClient proxySispostal = new WSSispostal.sispostal1SoapClient();

            var results = proxySispostal.wssisptracking(Usuario, Password, NGuia.ToString());
            List<INEstadosSWSispostal> estados = null;

            if (results != null)
            {
                estados = new List<INEstadosSWSispostal>();
                foreach (DataTable result in results.Tables)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        if (row["estado"].ToString() != "ALISTAMIENTO" && row["estado"].ToString() != "NO NEGOCIACION")
                        {
                            estados.Add(new INEstadosSWSispostal
                            {
                                Estado = row["estado"] == DBNull.Value ? string.Empty : row["estado"].ToString(),
                                Fecha = row["fecha"] == DBNull.Value ? DateTime.Now : DateTime.Parse(row["fecha"].ToString()),
                                Ciudad = row["ciudad"] == DBNull.Value ? string.Empty : row["ciudad"].ToString(),
                                Guia = row["guia"].ToString()
                            });
                        }
                    }
                }

                if (estados.Count == 0)
                {
                    throw new FaultException<Exception>(new Exception("Sin traza en servicios SISPOSTAL"));
                }

                foreach (var item in estados)
                {
                    foreach (var field in type.GetFields())
                    {
                        var attribute = Attribute.GetCustomAttribute(field,
                            typeof(DescriptionAttribute)) as DescriptionAttribute;
                        if (attribute != null)
                        {
                            if (field.Name.ToUpper() == item.Estado.Replace(" ", "").ToUpper())
                            {
                                item.Estado = attribute.Description;
                                break;
                            }
                        }
                    }
                }
            }

            return estados;
        }
    }
}