using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using Framework.Servidor.Servicios.ContratoDatos.SincronizadorDatos;

namespace Framework.Servidor.SincronizacionDatos.Datos
{
    public class COSincronizacionDatosRepositorio
    {

         private static readonly COSincronizacionDatosRepositorio instancia = new COSincronizacionDatosRepositorio();

        public static COSincronizacionDatosRepositorio Instancia
        {
            get { return instancia; }            
        }

        private COSincronizacionDatosRepositorio()
        {

        }

        public List<DataRow> ObtenerTablasSincronizacion()
        {

           // string strCnn = "Integrated Security = SSPI; Server = .; Database = ICONTROLLER";
            string strCnn = ConfigurationManager.ConnectionStrings["ConSincronizacion"].ConnectionString;
        
            DataTable dtTablasSync = new DataTable("Columns");

            using (SqlConnection cnn = new SqlConnection(strCnn))
            {

                SqlCommand cmdTablasSync = new SqlCommand();
                SqlDataAdapter daTablasSync = new SqlDataAdapter();

                cmdTablasSync.CommandText = @"SELECT [NombreTabla]
                                        ,[SentenciaFiltro]
                                        ,[BatchSize]
                                        FROM [TablasSincronizacion]";

                cmdTablasSync.Connection = cnn;
                cmdTablasSync.CommandType = CommandType.Text;
                daTablasSync.SelectCommand = cmdTablasSync;
                daTablasSync.Fill(dtTablasSync);
            }

            return dtTablasSync.AsEnumerable().ToList();
        }


        public List<DataRow> ObtenerEsquemaTablasSincronizacion(string nombreTabla)
        {
            
            string strCnn = ConfigurationManager.ConnectionStrings["ConSincronizacion"].ConnectionString;
             DataTable dt = new DataTable("Columns");
             using (SqlConnection cnn = new SqlConnection(strCnn))
             {
                 SqlCommand cmd = new SqlCommand();
                 SqlDataAdapter da = new SqlDataAdapter();

                 cmd.Connection = cnn;
                 cmd.CommandType = CommandType.Text;
                 cmd.CommandText = @"
                                SELECT 
                                 c.name 'Column Name',
                                 t.Name 'Data type',
                                 c.max_length 'Max Length',
                                 c.precision,
                                 c.scale,
                                 c.is_nullable,
                                 ISNULL(i.is_primary_key, 0) 'Primary Key'
                                FROM    
                                    sys.columns  c with(nolock)
                                   INNER JOIN 
                                    sys.types t with(nolock) ON c.user_type_id = t.user_type_id
                                   LEFT OUTER JOIN 
                                    sys.index_columns ic with(nolock) ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                                   LEFT OUTER JOIN 
                                    sys.indexes i with(nolock) ON ic.object_id = i.object_id AND ic.index_id = i.index_id
                                WHERE
                                  c.object_id = OBJECT_ID(@TableName)";
                 cmd.Parameters.AddWithValue("@TableName", nombreTabla);
                 da.SelectCommand = cmd;
                 da.Fill(dt);
                 return dt.AsEnumerable().GroupBy(g => g.Field<string>("Column Name")).Select(s => s.First()).ToList();
             }
        }

        /// <summary>
        /// Obtiene los datos de una tabla
        /// </summary>
        /// <param name="nombreTabla"></param>
        /// <param name="BatchSize"></param>
        /// <param name="filtro"></param>
        public List<Registros> ObtenerDatosTabla(string nombreTabla, int BatchSize, string filtro,string ActualAnchor, int batchActual, int totalBatch)
        {

            if (nombreTabla == "ServicioTrayecto_TAR")
            {

            }

            //casos especiales para los filtros con inner join
            //en la tabla tombStone se creó el campo por el cual se filtra en el join, por lo cual el filtro parametrizado en TablasSincronizacion
            //se cambia por un where directo al campo especificado
            //en caso de agregar una nueva tabla con filtros join los triggers de las tablas padre se deben adaptar para insertar el nuevo campo en la tabla tombstone
            switch (nombreTabla)
            {
                case "ServicioTrayecto_TAR_TombStone":
                    //filtro = 
                    string[] str = filtro.Split('=');
                    if (str.Count() >= 2)
                    {
                        filtro = " WHERE TRA_IdLocalidadOrigen=" + str[1].Substring(0, str[1].Length - 1);
                    }
                    break;
                case "CentroServicioServicioDia_PUA_TombStone":
                    //filtro = 
                    string[] strCntServSerDia = filtro.Split('=');
                    if (strCntServSerDia.Count() >= 2)
                    {
                        filtro = " WHERE CSS_IdCentroServicios=" + strCntServSerDia[1].Substring(0, strCntServSerDia[1].Length - 1);
                    }
                    break;
            }

            string query;
            string strCnn = ConfigurationManager.ConnectionStrings["ConSincronizacion"].ConnectionString;

            if (BatchSize <= 0)
                query = "SELECT * FROM " + nombreTabla;
            else
            {
                query = "SELECT TOP(" + BatchSize + ") * FROM " + nombreTabla;

                if (batchActual <= 0)
                {

                    using (SqlConnection cnn = new SqlConnection(strCnn))
                    {
                        SqlCommand cmd = new SqlCommand();
                        SqlDataAdapter da = new SqlDataAdapter();
                        cnn.Open();
                        cmd.Connection = cnn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = @"SELECT COUNT(0) FROM "+nombreTabla;
                        if (!string.IsNullOrWhiteSpace(filtro))
                        {
                            cmd.CommandText = cmd.CommandText + " " + filtro;
                        }
                        var totaRegis = cmd.ExecuteScalar();
                        totalBatch = (Convert.ToInt32(totaRegis) / BatchSize)+1;
                        cnn.Close();
                    }

                }

            }



            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query + " " + filtro;
            }           

           
           
            DataTable dt = new DataTable("Columns");
            using (SqlConnection cnn = new SqlConnection(strCnn))
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();

                cmd.Connection = cnn;
                cmd.CommandType = CommandType.Text;


                if (!string.IsNullOrEmpty(ActualAnchor))
                {

                    if (!string.IsNullOrWhiteSpace(filtro))
                    {
                        query = query + " AND Anchor > @anchor";
                    }
                    else
                    {
                        query = query + " WHERE Anchor > @anchor";
                    }

                  
                    cmd.Parameters.Add("@anchor", SqlDbType.Timestamp, 8).Value = Convert.FromBase64String(ActualAnchor);
                }

                query = query + " ORDER BY Anchor ASC";

                
                cmd.CommandText = query;                
                da.SelectCommand = cmd;
                da.Fill(dt);
                List<Registros> registros = new List<Registros>();
                dt.AsEnumerable().ToList().ForEach(d =>
                    {
                        List<Columnas> columnas = new List<Columnas>();

                        object anchor = null;

                        for (int i = 0; i <= d.ItemArray.Count() - 1; i++)
                        {

                           Columnas col = new Columnas()
                            {
                                NombreColumna = d.Table.Columns[i].ColumnName,
                                TipoDato = d.ItemArray[i].GetType().FullName,
                                
                                Valor = d.ItemArray[i]
                            };

                           if (col.TipoDato == "System.DBNull")
                            {
                                col.Valor = "";
                            }

                           if (col.TipoDato == "System.String")
                           {
                               col.Valor = col.Valor.ToString().Replace("'", "´");
                           }

                            columnas.Add(col);
                            

                            if(d.Table.Columns[i].ColumnName.ToLower() == "anchor")
                            {
                                anchor = d.ItemArray[i];
                            }
                        }                     

                        registros.Add(new Registros()
                        {
                            NombreTabla = nombreTabla,
                            Columnas = columnas,
                            ActualAnchor = Convert.ToBase64String(anchor as byte[]),
                            TotalBatch = totalBatch,
                            BatchActual = batchActual+1                             
                        });

                    });
                
              
                return registros;
            }


        }

        /// <summary>
        /// Obtiene los datos de una tabla
        /// </summary>
        /// <param name="nombreTabla"></param>
        /// <param name="BatchSize"></param>
        /// <param name="filtro"></param>
        public List<Registros> ObtenerDatosTablaWebApi(string nombreTabla, int BatchSize, string filtro, string ActualAnchor, int batchActual, int totalBatch)
        {

            if (nombreTabla == "ServicioTrayecto_TAR")
            {

            }

            //casos especiales para los filtros con inner join
            //en la tabla tombStone se creó el campo por el cual se filtra en el join, por lo cual el filtro parametrizado en TablasSincronizacion
            //se cambia por un where directo al campo especificado
            //en caso de agregar una nueva tabla con filtros join los triggers de las tablas padre se deben adaptar para insertar el nuevo campo en la tabla tombstone
            switch (nombreTabla)
            {
                case "ServicioTrayecto_TAR_TombStone":
                    //filtro = 
                    string[] str = filtro.Split('=');
                    if (str.Count() >= 2)
                    {
                        filtro = " WHERE TRA_IdLocalidadOrigen=" + str[1].Substring(0, str[1].Length - 1);
                    }
                    break;
                case "CentroServicioServicioDia_PUA_TombStone":
                    //filtro = 
                    string[] strCntServSerDia = filtro.Split('=');
                    if (strCntServSerDia.Count() >= 2)
                    {
                        filtro = " WHERE CSS_IdCentroServicios=" + strCntServSerDia[1].Substring(0, strCntServSerDia[1].Length - 1);
                    }
                    break;
            }

            string query;
            string strCnn = ConfigurationManager.ConnectionStrings["ConSincronizacion"].ConnectionString;

            if (BatchSize <= 0)
                query = "SELECT * FROM (SELECT *,CAST((CONVERT(bigint, anchor)) as decimal) anchorNumero FROM " + nombreTabla + "with(nolock))t";
            else
            {
                query = "SELECT TOP(" + BatchSize + ") * FROM (SELECT *,CAST((CONVERT(bigint, anchor)) as decimal) anchorNumero FROM " + nombreTabla + " with(nolock)) t";

                if (batchActual <= 0)
                {

                    using (SqlConnection cnn = new SqlConnection(strCnn))
                    {
                        SqlCommand cmd = new SqlCommand();
                        SqlDataAdapter da = new SqlDataAdapter();
                        cnn.Open();
                        cmd.Connection = cnn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = @"SELECT COUNT(0) FROM " + nombreTabla;
                        if (!string.IsNullOrWhiteSpace(filtro))
                        {
                            cmd.CommandText = cmd.CommandText + " " + filtro;
                        }
                        var totaRegis = cmd.ExecuteScalar();
                        totalBatch = (Convert.ToInt32(totaRegis) / BatchSize) + 1;
                        cnn.Close();
                    }

                }

            }



            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query + " " + filtro;
            }



            DataTable dt = new DataTable("Columns");
            using (SqlConnection cnn = new SqlConnection(strCnn))
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();

                cmd.Connection = cnn;
                cmd.CommandType = CommandType.Text;


                if (!string.IsNullOrEmpty(ActualAnchor))
                {

                    if (!string.IsNullOrWhiteSpace(filtro))
                    {
                        query = query + " AND anchorNumero > @anchor";
                    }
                    else
                    {
                        query = query + " WHERE anchorNumero > @anchor";
                    }


                    //cmd.Parameters.Add("@anchor", SqlDbType.Timestamp, 8).Value = Convert.FromBase64String(ActualAnchor);
                    cmd.Parameters.AddWithValue("@anchor", long.Parse(ActualAnchor));
                }

                query = query + " ORDER BY anchorNumero ASC";


                cmd.CommandText = query;
                da.SelectCommand = cmd;
                da.Fill(dt);
                List<Registros> registros = new List<Registros>();
                dt.AsEnumerable().ToList().ForEach(d =>
                {
                    List<Columnas> columnas = new List<Columnas>();

                    object anchor = null;

                    for (int i = 0; i <= d.ItemArray.Count() - 1; i++)
                    {

                        Columnas col = new Columnas()
                        {
                            NombreColumna = d.Table.Columns[i].ColumnName,
                            TipoDato = d.ItemArray[i].GetType().FullName,

                            Valor = d.ItemArray[i]
                        };

                        if (col.TipoDato == "System.DBNull")
                        {
                            col.Valor = "";
                        }

                        if (col.TipoDato == "System.String")
                        {
                            col.Valor = col.Valor.ToString().Replace("'", "´");
                        }

                        columnas.Add(col);


                        if (d.Table.Columns[i].ColumnName.ToLower() == "anchornumero")
                        {
                            anchor = d.ItemArray[i];
                        }
                    }

                    registros.Add(new Registros()
                    {
                        NombreTabla = nombreTabla,
                        Columnas = columnas,
                        ActualAnchor = anchor.ToString(), // Convert.ToBase64String(anchor as byte[]),
                        TotalBatch = totalBatch,
                        BatchActual = batchActual + 1
                    });

                });


                return registros;
            }


        }

    }
}
