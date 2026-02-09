using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos.SincronizadorDatos;
using System.Data;
using Framework.Servidor.SincronizacionDatos.Datos;
using System.IO;
using Framework.Servidor.Seguridad;
using System.Xml.Serialization;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun.Util;

namespace Framework.Servidor.SincronizacionDatos
{
    public class COSincronizacionDatos
    {
        private static readonly COSincronizacionDatos instancia = new COSincronizacionDatos();

        public static COSincronizacionDatos Instancia
        {
            get { return instancia; }
        }

        private List<EsquemaDB> esquemaSincronizacionQuery;

        private List<EsquemaDB> esquemaSincronizacionSinQuery;

        private COSincronizacionDatos()
        {

        }

        public string ObtenerArchivoBaseDatosSinEsquema()
        {
            string ruta = @"C:\db\ControllerDB.sdf";

            byte[] db = File.ReadAllBytes(ruta);

            return Convert.ToBase64String(db);
        }

        public List<EsquemaDB> ObtenerEsquema(bool armarQueryCreacion)
        {
            try
            {
                if (armarQueryCreacion)
                {
                    if (esquemaSincronizacionQuery == null)
                    {
                        InicializarObtenerEsquema(armarQueryCreacion);
                    }
                    return esquemaSincronizacionQuery;
                }
                else
                {
                    if (esquemaSincronizacionSinQuery == null)
                    {
                        InicializarObtenerEsquema(armarQueryCreacion);
                    }
                    return esquemaSincronizacionSinQuery;
                }

            }
            catch (Exception ex)
            {
                List<EsquemaDB> lst = new List<EsquemaDB>();
                lst.Add(new EsquemaDB() { Error = ExtraerInformacionExcepcion(ex) });
                return lst;
            }
        }

        public EsquemaDB ObtenerEsquemaTabla(string nombreTabla)
        {
            try
            {
                List<EsquemaDB> esquema = new List<EsquemaDB>();
                List<DataRow> lstTablasSincro = COSincronizacionDatosRepositorio.Instancia.ObtenerTablasSincronizacion();
                lstTablasSincro = lstTablasSincro.Where(l => l.ItemArray[0].ToString() == nombreTabla).ToList();


                GenerarEsquemasTablas(true, esquema, lstTablasSincro);


                return esquema.FirstOrDefault();

            }
            catch (Exception ex)
            {
                EsquemaDB lst = new EsquemaDB() { Error = ExtraerInformacionExcepcion(ex) };

                return lst;
            }
        }


        /// <summary>
        /// Obtiene los datos de una tabla
        /// </summary>
        /// <param name="nombreTabla"></param>
        /// <param name="BatchSize"></param>
        /// <param name="filtro"></param>
        public List<Registros> ObtenerDatosTabla(string nombreTabla, int BatchSize, string filtro, string actualAnchor, int batchActual, int totalbatch)
        {
            try
            {
                return COSincronizacionDatosRepositorio.Instancia.ObtenerDatosTabla(nombreTabla, BatchSize, filtro, actualAnchor, batchActual, totalbatch);
            }
            catch (Exception ex)
            {
                List<Registros> lst = new List<Registros>();
                lst.Add(new Registros() { Error = ExtraerInformacionExcepcion(ex) });
                return lst;
            }
        }


        /// <summary>
        /// Obtiene los datos de una tabla
        /// </summary>
        /// <param name="nombreTabla"></param>
        /// <param name="BatchSize"></param>
        /// <param name="filtro"></param>
        public List<Registros> ObtenerDatosTablaWebApi(string nombreTabla, int BatchSize, string filtro, string actualAnchor, int batchActual, int totalbatch)
        {
            try
            {
                return COSincronizacionDatosRepositorio.Instancia.ObtenerDatosTablaWebApi(nombreTabla, BatchSize, filtro, actualAnchor, batchActual, totalbatch);
            }
            catch (Exception ex)
            {
                List<Registros> lst = new List<Registros>();
                lst.Add(new Registros() { Error = ExtraerInformacionExcepcion(ex) });
                return lst;
            }
        }

        /// <summary>
        /// Obtiene las tablas a sincronizar junto con el script de creacion y el filtro a aplicar
        /// </summary>
        /// <param name="armarQueryCreacion"></param>
        /// <returns></returns>
        public void InicializarObtenerEsquema(bool armarQueryCreacion)
        {
            List<EsquemaDB> esquema = new List<EsquemaDB>();
            List<DataRow> lstTablasSincro = COSincronizacionDatosRepositorio.Instancia.ObtenerTablasSincronizacion();


            GenerarEsquemasTablas(armarQueryCreacion, esquema, lstTablasSincro);


            if (armarQueryCreacion)
            {
                this.esquemaSincronizacionQuery = esquema;
            }
            else
            {
                this.esquemaSincronizacionSinQuery = esquema;
            }

        }




        private void GenerarEsquemasTablas(bool armarQueryCreacion, List<EsquemaDB> esquema, List<DataRow> lstTablasSincro)
        {
            lstTablasSincro.ForEach(tabla =>
            {
                string nombreTabla = tabla.ItemArray[0].ToString();

                EsquemaDB esq = new EsquemaDB()
                {
                    BatchSize = Convert.ToInt32(tabla.ItemArray[2]),
                    NombreTabla = nombreTabla,
                    QueryCreacion = null,
                    Filtro = tabla.ItemArray[1].ToString()
                };

                string PK = "";
                List<DataRow> lstEsquemaTabla = COSincronizacionDatosRepositorio.Instancia.ObtenerEsquemaTablasSincronizacion(nombreTabla);

                esq.NumeroCampos = lstEsquemaTabla.Count();
                int longitud = lstEsquemaTabla.Count() - 1;

                var row = lstEsquemaTabla.Where(r => Convert.ToBoolean(r.ItemArray[6]));
                if (row != null)
                {

                    row.ToList().ForEach(r =>
                    {
                        PK = PK + r.ItemArray[0].ToString() + ",";
                    });


                    esq.Pk = PK.Substring(0, PK.Length - 1);
                }

                if (armarQueryCreacion)
                {

                    StringBuilder sentenciaCreateTable = new StringBuilder();

                    if (longitud > 0)
                    {
                        sentenciaCreateTable.AppendLine("CREATE TABLE " + nombreTabla + "( ");
                        for (int i = 0; i <= longitud; i++)
                        {
                            var c = lstEsquemaTabla[i];
                            string s = "";
                            if (c.ItemArray[1].ToString() == "numeric")
                            {
                                s = c.ItemArray[0] + " " + EquivalenciasTiposDatos(c.ItemArray[1].ToString(), c.ItemArray[3].ToString() + "," + c.ItemArray[4].ToString()) + " " + TiposNulables(Convert.ToBoolean(c.ItemArray[5])) + ",";
                            }
                            else
                            {
                                s = c.ItemArray[0] + " " + EquivalenciasTiposDatos(c.ItemArray[1].ToString(), c.ItemArray[2].ToString()) + " " + TiposNulables(Convert.ToBoolean(c.ItemArray[5])) + ",";
                            }

                            if (i < longitud)
                                sentenciaCreateTable.AppendLine(s);
                            else
                            {

                                //                                         DefinirPrimaryKey(Convert.ToBoolean(c.ItemArray[6]), nombreTabla)
                                s = s + "PRIMARY KEY (" + esq.Pk + "));";
                                sentenciaCreateTable.AppendLine(s.Substring(0, s.Length - 1));
                            }



                        };

                        //sentenciaCreateTable.AppendLine(")");
                        esq.QueryCreacion = sentenciaCreateTable.ToString();
                    }
                }
                esquema.Add(esq);
            });
        }

        private string EquivalenciasTiposDatos(string tipoSqlServer, string longitud)
        {
            string tipoDatoSqlServerCE = tipoSqlServer;

            switch (tipoSqlServer)
            {
                case "varchar":
                    tipoDatoSqlServerCE = "nvarchar (" + longitud + ")";
                    break;

                case "char":
                    tipoDatoSqlServerCE = "nchar (" + longitud + ")";
                    break;

                case "text":
                    tipoDatoSqlServerCE = "ntext";
                    break;

                case "timestamp":
                    tipoDatoSqlServerCE = "varbinary (8)";
                    break;

                case "numeric":
                    tipoDatoSqlServerCE = "numeric (" + longitud + ")";
                    break;
            }

            return tipoDatoSqlServerCE;
        }
        private string TiposNulables(bool nulable)
        {
            if (nulable)
                return "NULL";
            else
                return "NOT NULL";
        }
        private string DefinirPrimaryKey(bool pk, string tableName)
        {
            if (pk)
            {
                return "CONSTRAINT " + tableName + "_PK PRIMARY KEY";
            }
            else
                return "";
        }

        /// <summary>
        /// Extrae la traza completa del error
        /// </summary>
        /// <param name="excepcion">Excepción</param>
        /// <returns>Traza del error</returns>
        private static string ExtraerInformacionExcepcion(Exception excepcion)
        {
            //traza completa del error
            StringBuilder detalleError = new StringBuilder();
            Exception excep = excepcion;
            detalleError.AppendLine(excep.Message);
            detalleError.AppendLine("----------------------------------");
            detalleError.AppendLine("Trace Exception :" + excep.StackTrace);
            detalleError.AppendLine("----------------------------------");
            int i = 0;
            while (excep.InnerException != null)
            {
                i += 1;
                excep = excep.InnerException;
                detalleError.AppendLine("----------------------------------");
                detalleError.AppendLine("Mensaje InnerException " + i + ":");
                detalleError.AppendLine(excep.Message);
                detalleError.AppendLine("----------------------------------");
                detalleError.AppendLine("Trace InnerException " + i + " :");
                detalleError.AppendLine(excep.StackTrace);
            }
            return detalleError.ToString();
        }


        /// <summary>
        /// Realiza la sincronizacion de los puntos offline
        /// </summary>
        /// <param name="contenido">el contenido del archivo encriptado</param>
        /// <returns></returns>
        public string SincronizacionCentrosServicioOffLine(string contenido)
        {
            try
            {
                string objSerializado = AlgoritmoEncripcion.Instance.DesencriptarCadena(contenido);
                SincronizacionOffLine objSincronizacion = Serializacion.DeserializeObject<SincronizacionOffLine>(objSerializado);
                objSincronizacion.LstGuiasSincronizadas = new List<long>();
                objSincronizacion.LstGuiaOffline.ForEach(g =>
                    {
                        ADGuia guia = Serializacion.DeserializarObjetoDataContract<ADGuia>(g.ObjetoGuia);
                        guia.EsAutomatico = false;
                        ADMensajeriaTipoCliente destinoRemite = Serializacion.DeserializarObjetoDataContract<ADMensajeriaTipoCliente>(g.DestinatarioRemitente);
                        ADRetornoAdmision guiaAdmitida = null;

                        IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

                        ADResultadoAdmision ADResultadoAd = null;
                        switch (g.IdServicio)
                        {
                            case TAConstantesServicios.SERVICIO_NOTIFICACIONES:

                                ADNotificacion objetoAdicional = Serializacion.DeserializarObjetoDataContract<ADNotificacion>(g.ObjetoParametroAdicional);
                                var guiaAd = fachadaMensajeria.RegistrarGuiaManualNotificacion(guia, g.IdCaja, destinoRemite, objetoAdicional);
                                g.EstaSincronizado = true;
                                objSincronizacion.LstGuiasSincronizadas.Add(guia.NumeroGuia);
                                break;

                            case TAConstantesServicios.SERVICIO_INTERNACIONAL:
                                TATipoEmpaque tipoEmpaque = Serializacion.DeserializarObjetoDataContract<TATipoEmpaque>(g.ObjetoParametroAdicional);
                                ADResultadoAd = fachadaMensajeria.RegistrarGuiaManualInternacional(guia, g.IdCaja, destinoRemite, tipoEmpaque);
                                g.EstaSincronizado = true;
                                objSincronizacion.LstGuiasSincronizadas.Add(guia.NumeroGuia);
                                break;

                            case TAConstantesServicios.SERVICIO_RAPIRADICADO:
                                ADRapiRadicado rapiRadicado = Serializacion.DeserializarObjetoDataContract<ADRapiRadicado>(g.ObjetoParametroAdicional);
                                ADResultadoAd = fachadaMensajeria.RegistrarGuiaManualRapiRadicado(guia, g.IdCaja, destinoRemite, rapiRadicado);
                                g.EstaSincronizado = true;
                                objSincronizacion.LstGuiasSincronizadas.Add(guia.NumeroGuia);
                                break;
                            default:
                                try
                                {
                                    ADResultadoAd = fachadaMensajeria.RegistrarGuiaManual(guia, g.IdCaja, destinoRemite);
                                    g.EstaSincronizado = true;
                                    objSincronizacion.LstGuiasSincronizadas.Add(guia.NumeroGuia);
                                }
                                catch (Exception ex)
                                {
                                    string error = ex.Message;
                                    g.EstaSincronizado = false;
                                }
                                break;
                        }

                    });
                ISUFachadaSuministros fachadaSuministos = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

                SURemisionSuministroDC remision = fachadaSuministos.GenerarRangoGuiaManualOffline(objSincronizacion.IdPuntoServicio, objSincronizacion.CantidadSuministrosSolicitar);

                List<SuministrosOffLine> sumOffLine = new List<SuministrosOffLine>();
                for (long i = remision.GrupoSuministros.SuministroGrupo.RangoInicial; i <= remision.GrupoSuministros.SuministroGrupo.RangoFinal; i++)
                {
                    sumOffLine.Add(new SuministrosOffLine() { NumeroGuia = i, FechaSincronizacion = DateTime.Now });

                }


                //TODO:CED sincronizacion de tablas a partir del max anchor de cada tabla


                objSincronizacion.LstGuiaOffline = new List<GuiaOffline>();
                string retorno = Serializacion.SerializarObjetoDataContract<SincronizacionOffLine>(objSincronizacion);
                retorno = AlgoritmoEncripcion.Instance.EncriptarCadena(retorno);

                return retorno;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        

    }
}
