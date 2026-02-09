using Framework.Servidor.Servicios.ContratoDatos.ConsultasExternas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Framework.Servidor.ConsultasExternas.Datos
{
    public class COConsultasComcelRepositorio
    {
        private static COConsultasComcelRepositorio instancia = new COConsultasComcelRepositorio();

        public static  COConsultasComcelRepositorio Instancia
        {
            get { return instancia; }
        }

        private string conexionString = ConfigurationManager.ConnectionStrings["Sispostal"].ConnectionString;
        private string conexionStringOld = ConfigurationManager.ConnectionStrings["SispostalOld"].ConnectionString;


        private COConsultasComcelRepositorio()
        {

        }

        public List<COConsultaComcelResponse> ConsultarGuiaCliente(COConsultaComcelRequest numeroCuenta)
        {

            DataSet ds = new DataSet();
            string rutaImagenes = WebConfigurationManager.AppSettings["Controller.RutaImagenesServidorSispostal"];
            if (string.IsNullOrWhiteSpace(rutaImagenes))
            {
                throw new Exception("Falta la ruta de las imagenes de sispostal, en el archivo web.config");
            }

            string Anio = numeroCuenta.Fecha.Substring(0, 4);
            string Mes = numeroCuenta.Fecha.Substring(4, 2);
                string query;
                List<COConsultaComcelResponse> lstConsulta = new List<COConsultaComcelResponse>();
                query = @"SELECT top(1) tblorden.Idremite,TBLGUIA.Guia, tblorden.Orden, tblorden.PRODUCTO, isnull(tblorden.fechabase,tblorden.Fecha) as Fecha, TBLGUIA.Nombre, TBLGUIA.Direccion, 
                case 
                when tblguia.estado=3 then 'Entrega' 
                when tblguia.estado=2 then 'Devolución -' + RTRIM(TBLGUIA.novedad ) 
                ELSE 'En proceso' END 
                AS ESTADO, TBLGUIA.novedad AS devolucion, 
                rtrim(tblCiudad.Ciudad) + '-' + rtrim(tblCiudad.Dpto) as Ciudad, TBLGUIA.Radicar, tblguia.Radicar as OBSERVACION, TBLGUIA.identificacion, TBLGUIA.digitaliza, tblorden.Idremite, TBLGUIA.Reenvio, 
                TBLGUIA.fechentre, tblorden.CCOSTO, TBLGUIA.Telefono, TBLGUIA.largo, 
                TBLGUIA.ancho, TBLGUIA.alto, TBLGUIA.DiceContener, TBLGUIA.kilos, '' AS trm, 
                tblguia.imagen,tblorden.ext
                , @rutaImagenes + rtrim(tblguia.imagen) as ruta_imagen 
                FROM TBLGUIA INNER JOIN 
                tblestados ON TBLGUIA.Estado = tblestados.idste INNER JOIN 
                tblCiudad ON TBLGUIA.Ciudad = tblCiudad.Codciud INNER JOIN 
                tblorden ON TBLGUIA.Orden = tblorden.Orden INNER JOIN 
                tblremite ON tblorden.Idremite = tblremite.Idremite 
                WHERE 
                TBLGUIA.identificacion = @cuenta AND 
                tblorden.Idremite = @IdRemite 
                AND YEAR(Fecha) = @Anio AND MONTH(Fecha) = @Mes
                order by Fecha desc";                

                using (SqlConnection sqlConn = new SqlConnection(conexionString))
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand(query, sqlConn);
                    cmd.Parameters.AddWithValue("@rutaImagenes", rutaImagenes);
                    cmd.Parameters.AddWithValue("@Anio", Anio);
                    cmd.Parameters.AddWithValue("@Mes", Mes);
                    cmd.Parameters.AddWithValue("@Cuenta", numeroCuenta.Cuenta);
                    cmd.Parameters.AddWithValue("@IdRemite", 125);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }

                if (ds.Tables[0].Rows.Count <= 0)
                {
                using (SqlConnection sqlConn = new SqlConnection(conexionStringOld))
                {                   
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand(query, sqlConn);
                    cmd.Parameters.AddWithValue("@rutaImagenes", rutaImagenes);
                    cmd.Parameters.AddWithValue("@Anio", Anio);
                    cmd.Parameters.AddWithValue("@Mes", Mes);
                    cmd.Parameters.AddWithValue("@Cuenta", numeroCuenta.Cuenta);
                    cmd.Parameters.AddWithValue("@IdRemite", 83005380);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        COConsultaComcelResponse response = new COConsultaComcelResponse()
                            {
                                Direccion = r["Direccion"].ToString(),
                                MotivoDevolucion = r["devolucion"] != null ? r["devolucion"].ToString() : string.Empty,
                                NombreApellido = r["Nombre"].ToString(),
                                NumeroGuia = r["Guia"].ToString(),
                                Mensaje = "Ok",
                                Cuenta = numeroCuenta.Cuenta.PadLeft(8, '0'),
                                Estado = r["ESTADO"].ToString()
                            };
                        if (r["ruta_imagen"] != null && !string.IsNullOrEmpty(r["ruta_imagen"].ToString()))
                        {
                            string ruta = r["ruta_imagen"].ToString();

                            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(ruta);
                            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            Stream stream = httpWebReponse.GetResponseStream();

                            if (stream != null)
                            {
                                byte[] array = ReadFully(stream);
                                response.Imagen = Convert.ToBase64String(array);
                            }
                            else
                            {
                                response.Imagen = string.Empty;
                            }
                        }
                        else
                        {
                            response.Imagen = string.Empty;
                        }
                        lstConsulta.Add(response);
                    };
                }
                else
                {
                    COConsultaComcelResponse response = new COConsultaComcelResponse()
                    {
                        Direccion = "Informacion no disponible",
                        MotivoDevolucion = "Informacion no disponible",
                        NombreApellido = "Informacion no disponible",
                        NumeroGuia = "Informacion no disponible",
                        Mensaje = "Informacion no disponible",
                        Cuenta = "Informacion no disponible",
                        Estado = "Informacion no disponible",
                    };
                    lstConsulta.Add(response);
                }
                return lstConsulta;  
        }


        /// <summary>
        /// Graba auditoria de integración
        /// </summary>
        /// <param name="tipoIntegracion"></param>
        /// <param name="usuario"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void AuditarIntegracion(string tipoIntegracion, string request, string response)
        {
            using (SqlConnection cnx = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ControllerExcepciones"].ConnectionString))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paAuditarIntegracion_AUD", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("AUI_TipoIntegraciOn", tipoIntegracion));
                cmd.Parameters.Add(new SqlParameter("AUI_Request", request));
                cmd.Parameters.Add(new SqlParameter("AUI_Response", response));
                cmd.ExecuteNonQuery();
                cnx.Close();
                cnx.Dispose();
            }
        }

        public byte[] FileToByteArray(string fileName)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(fileName,
                                           FileMode.Open,
                                           FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


    }
}
