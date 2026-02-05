using System;
using Framework.Servidor.Comun;
using System.ServiceModel;
using VentaCredito.Clientes.Comun;
using VentaCredito.Clientes.Datos.Repositorio;
using Framework.Servidor.Excepciones;

namespace VentaCredito.Clientes
{
    public class CLContrato
    {
        private static CLContrato instancia = new CLContrato();

        public static CLContrato Instancia
        {
            get
            {
                return instancia;
            }
        }

        /// <summary>te fuiste blake
        /// Valida el estado del Contrato
        /// </summary>
        /// <returns>"True" si se superó el porcentaje mínimo de aviso</returns>
        public bool ValidarCupoContrato(int idContrato, decimal valorTransaccion)
        {
            decimal consumoContrato = ObtenerConsumoContrato(idContrato);
            decimal presupuestoContrato = ObtenerValorPresupuestoContrato(idContrato);
            decimal consumoTotal = consumoContrato + valorTransaccion;
            if (presupuestoContrato < consumoTotal)
            {
                var x = new ControllerException
                     (
                     COConstantesModulos.CLIENTES,
                     CLEnumTipoErrorCliente.EX_FALLO_VALOR_CONTRATO.ToString(),
                     CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_FALLO_VALOR_CONTRATO)
                     );
                throw new FaultException<ControllerException>(x);
            }
            else
            {
                decimal porcentajeAviso = ObtenerPorcentajeAvisoContrato(idContrato);
                decimal porcentajeConsumoActual = consumoTotal * 100 / presupuestoContrato;
                return porcentajeConsumoActual >= porcentajeAviso;
            }
        }

        private decimal ObtenerPorcentajeAvisoContrato(int idContrato)
        {
            return CLContratoRepositorio.Instancia.ObtenerPorcentajeAvisoContrato(idContrato);
        }

        /// <summary>
        /// Metodo que obtiene el acumulado del consumo del contrato
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns>decimal con el valor</returns>
        private decimal ObtenerConsumoContrato(int idContrato)
        {
            return CLContratoRepositorio.Instancia.ObtenerConsumoContrato(idContrato);
            
        }

        /// <summary>
        /// Metodo que obtiene el valor del presupuesto del contrato
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns>decimal con el valor</returns>
        private decimal ObtenerValorPresupuestoContrato(int idContrato)
        {
            return CLContratoRepositorio.Instancia.ObtenerValorPresupuestoContrato(idContrato) +
              CLContratoRepositorio.Instancia.ObtenerValorPresupuestoOtrosi(idContrato);
        }
    }
}