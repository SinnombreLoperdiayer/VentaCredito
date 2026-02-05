using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Cajas.Comun
{
    /// <summary>
    /// Clase que contiene las constantes de Caja.
    /// </summary>
    public class CAConstantesCaja
    {
        /// <summary>
        /// Fecha Estandar Inicial
        /// </summary>
        public const string VALOR_VACIO = "00";

        /// <summary>
        /// Id de la forma de pago Efectivo
        /// </summary>
        public const short FORMA_PAGO_EFECTIVO = 1;

        /// <summary>
        /// Id de la forma de pago Efectivo
        /// </summary>
        public const short FORMA_PAGO_PREPAGO = 4;

        /// <summary>
        /// Operador Logico IGUAL =
        /// </summary>
        public const string OPERADOR_LOGICO_IGUAL = "=";

        /// <summary>
        /// Operador Logico DIFERENCIA <>
        /// </summary>
        public const string OPERADOR_LOGICO_DIFERENCIA = "<>";

        /// <summary>
        /// Valor cero Long
        /// </summary>
        public const long VALOR_CERO_LONG = 0;

        /// <summary>
        /// Constante de id del Punto
        /// </summary>
        public const string PUNTO = "PTO";

        /// <summary>
        /// Constante de id del Punto
        /// </summary>
        public const string RACOL = "RAC";

        /// <summary>
        /// Estado de la facturacion pendietne por facturar
        /// </summary>
        public const string PENDIENTE_POR_FACTURAR = "FAC";

        /// <summary>
        /// Cliente Credito
        /// </summary>
        public const string CLIENTE_CREDITO = "CRE";

        /// <summary>
        /// Esta Caja Abierta
        /// </summary>
        public const bool CAJA_ABIERTA = true;

        /// <summary>
        /// descripcion de la transaccion de Caja Aux a Caja Ppal
        /// </summary>
        public const string CAJA_NUMERO = "Caja N° ";

        /// <summary>
        /// descripcion de la transaccion de Caja Ppal a Caja Aux
        /// </summary>
        public const string TRANS_DINERO_CAJAPPAL_CAJAAUX = "Translado de Dinero de la Caja Principal a la Caja Auxiliar";

        /// <summary>
        /// descripcion de la transaccion de Caja Aux a Caja Ppal
        /// </summary>
        public const string TRANS_DINERO_CAJAAUX_CAJAPPAL = "Translado de Dinero de la Caja Auxiliar a la Caja Principal";

        /// <summary>
        /// Valor Cero
        /// </summary>
        public const string VALOR_CERO = "0";

        /// <summary>
        /// Valor Cero decimal para validación
        /// </summary>
        public const decimal VALOR_CERO_DECIMAL = 0;
    }
}
