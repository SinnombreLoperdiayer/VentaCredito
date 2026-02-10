using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.ControlCuentas.Comun
{
    /// <summary>
    /// Constantes control de cuentas
    /// </summary>
    public class CCConstantesControlCuentas
    {
        #region Crear Instancia

        private static readonly CCConstantesControlCuentas instancia = new CCConstantesControlCuentas();

        /// <summary>
        /// Retorna instancia de la clase de constantes del modulo control de cuentas
        /// </summary>
        public static CCConstantesControlCuentas Instancia
        {
            get { return CCConstantesControlCuentas.instancia; }
        }

        #endregion Crear Instancia

        /// <summary>
        /// Nomenclatura almacen mensajería
        /// </summary>
        public const string ALMACEN_GUIAS_MENSAJERIA = "MEN";

        /// <summary>
        /// Nomenclatura almacen giros
        /// </summary>
        public const string ALMACEN_GIROS = "GIR";

        /// <summary>
        /// Nomenclatura almacen pagos giros
        /// </summary>
        public const string ALMACEN_PAGOS_GIROS = "PAG";

        /// <summary>
        /// Nomenclatura almacen otros movimientos de caja
        /// </summary>
        public const string ALMACEN_OTROS_MOVIMIENTOS_CAJAS = "MCA";

        /// <summary>
        /// Id tipo operación guías mensajería
        /// </summary>
        public const short ID_TIPO_OPERACION_GUIAS_MENSAJERIA = 1;

        /// <summary>
        /// Id tipo operación giros
        /// </summary>
        public const short ID_TIPO_OPERACION_GIROS = 2;

        /// <summary>
        /// Id tipo operación operación cajas
        /// </summary>
        public const short ID_TIPO_OPERACION_OPERACION_CAJAS = 3;

        /// <summary>
        /// Id tipo operación pagos giros
        /// </summary>
        public const short ID_TIPO_OPERACION_PAGOS_GIROS = 4;

        public const short ID_TIPO_OPERACION_VENTA_PIN_PREPAGO = 5;

        public const short ID_TIPO_OPERACION_RECAUDO_AL_COBRO = 6;

        /// <summary>
        /// Número máximo de posiciones por lote
        /// </summary>
        public const int NUMERO_MAXIMO_POSICIONES_LOTE = 100;

        /// <summary>
        /// Número máximo de lotes por caja
        /// </summary>
        public const int NUMERO_MAXIMO_LOTES_CAJA = 20;

        /// <summary>
        /// Concepto de Caja de Ajuste por Cambio de Destino Guia
        /// </summary>
        public const string CONCEPTO_AJUSTE_X_CAMBIO_DESTINO = "Ajuste x Cambio de Destino";

        /// <summary>
        /// Ajuste x Cambio de Forma Pago AL-CT
        /// </summary>
        public const string CONCEPTO_AJUSTE_X_CAMBIO_FORMA_PAGO_AL_CT = "Ajuste x Cambio de Forma Pago AL-CT";

        /// <summary>
        /// Ajuste x Cambio de Forma Pago AL-CT
        /// </summary>
        public const string CONCEPTO_AJUSTE_X_CAMBIO_FORMA_PAGO_CT_AL = "Ajuste x Cambio de Forma Pago CT-AL";

        /// <summary>
        /// Diferencia por cambio de Tipo de Servicio
        /// </summary>
        public const string CONCEPTO_DIFERENCIA_X_CAMBIO_TIPO_SERVICIO = "Diferencia por cambio de Tipo de Servicio";

        /// <summary>
        /// Ajuste x Cambio de Peso
        /// </summary>
        public const string CONCEPTO_AJUSTE_X_CAMBIO_DE_PESO = "Ajuste x Cambio de Peso";

        /// <summary>
        /// Diferencia por cambio de Valor Total
        /// </summary>
        public const string CONCEPTO_DIFERENCIA_X_CAMBIO_VALOR_TOTAL = "Diferencia por cambio de Valor Total";
    }
}