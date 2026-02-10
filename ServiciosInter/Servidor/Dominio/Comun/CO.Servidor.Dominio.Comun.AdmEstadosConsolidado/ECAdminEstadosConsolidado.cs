using CO.Servidor.Dominio.Comun.AdmEstadosConsolidado.Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Dominio.Comun.AdmEstadosConsolidado
{
    public class ECAdminEstadosConsolidado
    {

        private static readonly ECAdminEstadosConsolidado instancia = new ECAdminEstadosConsolidado();

        public static ECAdminEstadosConsolidado Instancia
        {
            get
            {
                return instancia;
            }
        }

        private ECAdminEstadosConsolidado()
        {

        }


        /// <summary>
        /// graba el estado consolidado
        /// </summary>
        /// <param name="estado"></param>
        /// <param name="inventarioContenedor"></param>
        public static void GuardarEstadoConsolidado(ECEstadoConsolidado estado)
        {
            long inventarioContenedor;
            ECEstadoConsolidado ultimoEstadoConsolidado;
            
            inventarioContenedor = ECRepositorio.Instancia.ObtenerIdConsolidado(estado.NoTula);
            ultimoEstadoConsolidado = ECRepositorio.Instancia.ObtenerUltimoEstadoConsolidado(inventarioContenedor);

            if (ultimoEstadoConsolidado != null && ultimoEstadoConsolidado.Estado == estado.Estado)
                return;

            ECRepositorio.Instancia.GuardarEstadoConsolidado(estado, inventarioContenedor);
        }



        /// <summary>
        /// obtiene el ultimo estado del consolidado
        /// </summary>
        /// <param name="estado"></param>
        /// <param name="inventarioContenedor"></param>
        public static EnumEstadosConsolidados ObtenerUltimoEstadoConsolidado(string noTula)
        {
					long inventarioContenedor;
					inventarioContenedor = ECRepositorio.Instancia.ObtenerIdConsolidado(noTula);
					return ECRepositorio.Instancia.ObtenerUltimoEstadoConsolidado(inventarioContenedor).Estado;    
        }


    }
}
