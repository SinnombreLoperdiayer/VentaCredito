using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Datos
{
    public partial class LIRepositorioTelemercadeo
    {
       
        #region custodia

        public void AdicionarEvidenciasCustodia(long numeroEvidencia, long idBodega, string ruta, bool sincronizado,long idEstadoGuiaLog, string descripcionTipoEvidencia, Int16 idTipoEvidenciaDevolucion)
        {
            using (SqlConnection con = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarEvidenciaCustodia_MEN", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroEvidencia", numeroEvidencia);
                cmd.Parameters.AddWithValue("@IdCentroLogistico", idBodega);
                cmd.Parameters.AddWithValue("@RutaArchivo", ruta);
                cmd.Parameters.AddWithValue("@Sincronizado", sincronizado);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@IdEstadoGuiaLog", idEstadoGuiaLog);
                cmd.Parameters.AddWithValue("@EstaDigitalizado", true);
                cmd.Parameters.AddWithValue("@DescripcionTipoEvidenciaDevolucion", descripcionTipoEvidencia);
                cmd.Parameters.AddWithValue("@IdTipoEvidenciaDevolucion", idTipoEvidenciaDevolucion);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        } 

        #endregion
    }
}
