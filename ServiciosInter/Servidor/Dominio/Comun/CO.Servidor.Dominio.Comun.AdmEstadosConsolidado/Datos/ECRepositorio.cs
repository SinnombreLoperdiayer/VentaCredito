using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CO.Servidor.Dominio.Comun.AdmEstadosConsolidado.Datos
{
    public class ECRepositorio
    {
        #region Atributos

        private string NombreModelo = "ModeloEstadosConsolidados";

        #endregion Atributos

        #region Instancia singleton de la clase

        private static readonly ECRepositorio instancia = new ECRepositorio();

        public static ECRepositorio Instancia
        {
            get
            {
                return instancia;
            }
        }

        #endregion Instancia singleton de la clase


        /// <summary>
        /// graba el estado consolidado
        /// </summary>
        /// <param name="estado"></param>
        /// <param name="inventarioContenedor"></param>
        public void GuardarEstadoConsolidado(ECEstadoConsolidado estado, long inventarioContenedor )
        {
            using (Modelo.ModeloEstadosConsolidados contexto = new Modelo.ModeloEstadosConsolidados(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paGrabarEstadoConsolidado_OPU(inventarioContenedor, estado.Estado.ToString(), estado.Observaciones, estado.IdCentroServicios, ControllerContext.Current.Usuario);
                contexto.SaveChanges();
            }

        }

        public long ObtenerIdConsolidado(string noTula)
        {
            using (Modelo.ModeloEstadosConsolidados contexto = new Modelo.ModeloEstadosConsolidados(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
							var inventario = contexto.paObtenerInventarioPorCodigoInventario_OPU(noTula).FirstOrDefault();
                if (inventario != null)
                {

                    return inventario.INT_IdInventarioConsolidado;
                }
                else
                {
									ControllerException excepcion = new ControllerException(COConstantesModulos.MENSAJERIA, ETipoErrorFramework.EX_DIFERENTE_CENTRO_SERVICIO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_DIFERENTE_CENTRO_SERVICIO));  
									//ControllerException excepcion = new ControllerException(COConstantesModulos.MENSAJERIA, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

            }

        }

        

        /// <summary>
        /// Método para obtener el ultimo estado de un consolidado
        /// </summary>
        /// <param name="idInventario"></param>
        /// <returns></returns>
        public ECEstadoConsolidado ObtenerUltimoEstadoConsolidado(long idInventario)
        {
            using (Modelo.ModeloEstadosConsolidados contexto = new Modelo.ModeloEstadosConsolidados(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var inventario = contexto.EstadoConsolidado_OPU.Where(est => est.ECO_IdInvContenedor == idInventario).OrderByDescending(id=>id.ECO_IdEstContenedor).FirstOrDefault();
                if (inventario != null)
                {
                    return new ECEstadoConsolidado
                    {
                        Estado = (EnumEstadosConsolidados)Enum.Parse(typeof(EnumEstadosConsolidados), inventario.ECO_Estado),
                        IdEstadoConsolidado = inventario.ECO_IdEstContenedor
                    };

                }
                else
                    return null;

            }

        }




    }
}
