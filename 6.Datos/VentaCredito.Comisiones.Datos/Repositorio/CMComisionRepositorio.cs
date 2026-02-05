using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Comisiones.Comun;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Servicio.Entidades.Comisiones;

namespace VentaCredito.Comisiones.Datos.Repositorio
{
    public class CMComisionRepositorio
    {

        private static string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        private static CMComisionRepositorio instancia = new CMComisionRepositorio();

        public static CMComisionRepositorio Instancia
        {
            get
            {
                return instancia;
            }
        }
        /// <summary>
        /// Calcula las comisiones por ventas
        /// de un punto, su responsable y de una Agencia.
        /// </summary>
        /// <param name="consulta">Parametros de entrada de idCentroServicio-TipoServicio.</param>
        /// <returns>Las Comisiones del Punto y su responsable - Agencia </returns>
        public CMComisionXVentaCalculadaDC CalcularComisionesxVentas(CMConsultaComisionVenta consulta)
        {
            CMComisionXVentaCalculadaDC ComisionVenta = new CMComisionXVentaCalculadaDC();

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerComiCentroServicio_COM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroServicio", consulta.IdCentroServicios);
                cmd.Parameters.AddWithValue("@IdServicio", consulta.IdServicio);
                cmd.Parameters.AddWithValue("@tipoComision", consulta.TipoComision);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        
                    }
                }
                return ComisionVenta;
            }
        }
            
        



    /// <summary>
    /// Almacena las comisiones de una venta, una entrega o un pago
    /// </summary>
    /// <param name="comision">valores de la comision ganada por la agencia/punto</param>
    public void GuardarComision(CMComisionXVentaCalculadaDC comision)
    {

    }

    /// <summary>
    /// Llenars los datos de la liquidación para una agencia o para un punto sin comisiones para su agencia
    /// </summary>
    /// <param name="ComisionVenta">Información de la comisión.</param>
    private void LlenarLiquidacionSinCentroServicioAdministrado(CMComisionXVentaCalculadaDC ComisionVenta)
    {
        //si no es Punto retorno valores en 0
        ComisionVenta.IdCentroServicioResponsable = 0000;
        ComisionVenta.NombreCentroServicioResponsable = "N/A";
        ComisionVenta.ValorFijoComisionCentroServicioResponsable = 0;
        ComisionVenta.PorcComisionCentroServicioResponsable = 0;
        ComisionVenta.TotalComisionCentroServicioResponsable = 0;
        ComisionVenta.TotalComisionEmpresa = 0;
    }

}
}
