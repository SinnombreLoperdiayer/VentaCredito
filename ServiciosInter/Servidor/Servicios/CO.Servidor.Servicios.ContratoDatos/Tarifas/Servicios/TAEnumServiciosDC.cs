using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios
{
    /// <summary>
    /// Enumeración con los tipos de servicios que maneja la empresa
    /// </summary>
    public enum TAEnumServiciosDC : short
    {
        RapiHoy = 1,
        RapiAM = 2,
        Mensajería = 3,
        RapiMasivos = 4,
        RapiPromocional = 5,
        RapiCarga = 6,
        RapiCargaContrapago = 7,
        Giros = 8,
        InterViajes = 9,
        Tramites = 10,
        Internacional = 11,
        CentrosdeCorrespon = 12,
        RapiPersonalizado = 13,
        RapiEnvíosContrapago = 14,
        Notificaciones = 15,
        RapiRadicado = 16,
        CargaExpress = 17,
        Komprech = 18
    }
}
