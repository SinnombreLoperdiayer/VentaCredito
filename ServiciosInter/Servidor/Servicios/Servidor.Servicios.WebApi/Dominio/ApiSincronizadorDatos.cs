using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.ModeloResponse.SincronizadorDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.SincronizadorDatos;
using Framework.Servidor.SincronizacionDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiSincronizadorDatos : ApiDominioBase
    {
        private static readonly ApiSincronizadorDatos instancia = (ApiSincronizadorDatos)FabricaInterceptorApi.GetProxy(new ApiSincronizadorDatos(), COConstantesModulos.PARAMETROS_GENERALES);

        public static ApiSincronizadorDatos Instancia
        {
            get { return ApiSincronizadorDatos.instancia; }
        }

        private ApiSincronizadorDatos()
        { 
        }

        /// <summary>
        /// GET ObtenerEsquema
        /// </summary>
        /// <param name="armarQueryCreacion"></param>
        /// <returns></returns>       
        public IEnumerable<EsquemaDB> ObtenerEsquema(bool armarQueryCreacion)
        {
            return COSincronizacionDatos.Instancia.ObtenerEsquema(armarQueryCreacion);
        }

        /// <summary>
        /// Obtiene el esquema de una tabla
        /// </summary>
        /// <param name="nombreTabla"></param>
        /// <returns></returns>      
        public EsquemaDB ObtenerEsquemaTabla(string nombreTabla)
        {
            return COSincronizacionDatos.Instancia.ObtenerEsquemaTabla(nombreTabla);
        }





       
        public IEnumerable<Registros> ObtenerDatosTabla(string nombreTabla, int batchSize, string filtro, string actualAnchor, int batchActual, int totalbatch)
        {
            actualAnchor = actualAnchor.Replace("SLASH", "/");
            actualAnchor = actualAnchor == "_" ? "" : actualAnchor;

            return COSincronizacionDatos.Instancia.ObtenerDatosTabla(nombreTabla, batchSize, filtro == "_" ? "" : filtro, actualAnchor, batchActual, totalbatch);
        }



      /// <summary>
      /// Obtiene los datos de la tabla y retorna un insert con todos los datos para SQLite
      /// </summary>
      /// <param name="nombreTabla"></param>
      /// <param name="batchSize"></param>
      /// <param name="filtro"></param>
      /// <param name="actualAnchor"></param>
      /// <param name="batchActual"></param>
      /// <param name="totalbatch"></param>
      /// <returns></returns>
        public RegistrosResponse ObtenerDatosTablaString(string nombreTabla, int batchSize, string filtro, string actualAnchor, int batchActual, int totalbatch)
        {
            StringBuilder sb = new StringBuilder();
          
            if (nombreTabla == "Trayecto_TAR")
            {

            }

            if (actualAnchor == "_")
                actualAnchor = "";


            List<Registros> lstRegistros = COSincronizacionDatos.Instancia.ObtenerDatosTablaWebApi(nombreTabla, batchSize, filtro == "_" ? "" : filtro, actualAnchor, batchActual, totalbatch);

            RegistrosResponse respuesta = new RegistrosResponse();
            sb.AppendLine("insert or replace into " + nombreTabla + " values");
            lstRegistros.ForEach(r =>
            {
                if (string.IsNullOrEmpty(r.Error))
                {
                    string reg = "(";
                    r.Columnas.ForEach(c =>
                    {
                        if (c.NombreColumna.ToLower() != "anchornumero" && c.NombreColumna.ToLower() != "anchor")
                        {

                            switch (c.TipoDato)
                            {
                                case "System.DateTime":
                                    reg += "'" + ((DateTime)c.Valor).ToString("yyyy-MM-dd HH:mm:ss") + "',";
                                    break;

                                case "System.Int32":
                                    reg += c.Valor.ToString() + ",";
                                    break;
                                case "System.Int64":
                                    reg += c.Valor.ToString() + ",";
                                    break;
                                case "System.Decimal":
                                    reg += c.Valor.ToString().Replace(",", ".") + ",";
                                    break;

                                default:
                                    reg += "'" + c.Valor.ToString() + "',";
                                    break;
                            }

                        }
                        else if (c.NombreColumna.ToLower() == "anchor")
                        {
                            //reg += "'" + Convert.ToBase64String(c.Valor as byte[]) + "',";

                            // reg += Math.Abs(ConvertByteArrayToInteger(c.Valor as byte[]))+",";


                            reg += r.Columnas.Where(n => n.NombreColumna.ToLower() == "anchornumero").First().Valor.ToString() + ",";

                        }
                    });
                    reg = reg.Substring(0, reg.Length - 1);
                    sb.AppendLine(reg + "),");

                    respuesta.ActualAnchor = r.ActualAnchor;
                    respuesta.BatchActual = r.BatchActual;
                    respuesta.Error = r.Error;
                    respuesta.NombreTabla = r.NombreTabla;
                    respuesta.TotalBatch = r.TotalBatch;
                }
                else
                {
                    respuesta.Error = r.Error;

                }
            });

            string InsertUpdate = sb.ToString().Replace("\r", "").Replace("\n", "");
            InsertUpdate = InsertUpdate.Substring(0, InsertUpdate.Length - 1);
            respuesta.InsertUpdate = InsertUpdate;

            if (string.IsNullOrWhiteSpace(respuesta.NombreTabla))
            {
                respuesta.InsertUpdate = "";
            }


            //string retornoEncriptado = Framework.Servidor.Seguridad.AlgoritmoEncripcion.Instance.EncriptarCadena(retorno).Replace(@"/","ACA-HAY-UN-SLASH");

            return respuesta;
        }


        /// <summary>
        /// Realiza la sincronizacion de los puntos offline
        /// </summary>
        /// <param name="contenido">el contenido del archivo encriptado</param>
        /// <returns></returns>
     
        public string SincronizacionCentrosServicioOffLine(string contenido)
        {

            return COSincronizacionDatos.Instancia.SincronizacionCentrosServicioOffLine(contenido);
        }


        /// <summary>
        /// Convierte un array de bytes a un entero
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static long ConvertByteArrayToInteger(byte[] input)
        {
            long num = BitConverter.ToInt64(input, 0);
            return num;
        }


        /// <summary>
        /// Convierte un entero a un array de bytes
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static byte[] ConvertNumberToByteArray(long input)
        {
            byte[] ByteData = BitConverter.GetBytes(input);
            return ByteData;
        }

        


    }
}
