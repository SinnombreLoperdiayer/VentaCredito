using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO.Servidor.Integraciones
{
    public enum SIEnumEstadosGuiasMasivos
    {
        [Description("Envío Admitido")]
        Sinsalir = 0,

        [Description("En Distribución Urbana")]
        Enzona =1,
        
        [Description("En Proceso de Devolución")]
        Devuelto=2,

        [Description("En Proceso de Devolución")]
        DevolucionInicial = 6,

        [Description("Entrega Exitosa")]
        Entregado = 3,

        [Description("Viajando En Ruta Nacional")]
        Transito= 4,

        [Description("No Llego el Envío Físico")]
        Guiasinfisico = 5,

        [Description("En Confirmación Telefónica")]
        Telemercadeo = 8,

        [Description("Documento Anulado")]
        Anulada = 10,

        [Description("Caso Fortuito")]
        Siniestro    = 11,

        [Description("Devol. x Confirmar Cliente")]
        Retencion = 12,
        
    }
}
