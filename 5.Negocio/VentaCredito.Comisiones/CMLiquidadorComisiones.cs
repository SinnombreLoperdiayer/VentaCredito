using System;
using System.Data.SqlClient;
using Servicio.Entidades.Comisiones;
using VentaCredito.Comisiones.Datos.Repositorio;

namespace VentaCredito.Comisiones
{
   public class CMLiquidadorComisiones 
    {
        private static readonly CMLiquidadorComisiones instancia = new CMLiquidadorComisiones();

        public static CMLiquidadorComisiones Instancia
        {
            get { return CMLiquidadorComisiones.instancia; }
        }

        /// <summary>
        /// Calcula las comisiones por ventas
        /// de un punto, su responsable y de una Agencia.
        /// </summary>
        /// <param name="consulta">Parametros de entrada de idCentroServicio-TipoServicio.</param>
        /// <returns>Las Comisiones del Punto y su responsable - Agencia </returns>
        public CMComisionXVentaCalculadaDC CalcularComisionesxVentas(CMConsultaComisionVenta consulta)
        {
            return CMComisionRepositorio.Instancia.CalcularComisionesxVentas(consulta);
        }

        public void GuardarComision(CMComisionXVentaCalculadaDC comision)
        {
            CMComisionRepositorio.Instancia.GuardarComision(comision);
        }
    }
}