using System;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.LogisticaInversa
{
    public class LIGestionesDC
    {
        public long IdGestion { get; set; }
        public long idTrazaGuia { get; set; }
        public long idAdmisionGuia { get; set; }
        public long NumeroGuia { get; set; }
        public DateTime FechaGestion { get; set; }

        //public LIResultadoTelemercadeoDC Resultado { get; set; }
        public string Telefono { get; set; }

        public string NuevoTelefono { get; set; }
        public string Usuario { get; set; }
        public string NuevoContacto { get; set; }
        public string NuevaDireccion { get; set; }
        public string PersonaContesta { get; set; }

        //public PAParienteDC Pariente { get; set; }
        public string Observaciones { get; set; }

        //public LIGestionMotivoBorradoDC MotivoBorrado { get; set; }
        public bool AsignarASupervisor { get; set; }

        //public LIEnumTipoGestionTelemercadeo TipoGestion { get; set; }
        public long IdCentroServicio { get; set; }

        public string Idmensajero { get; set; }
        public string NombreMensajero { get; set; }
        public long IdPlanilla { get; set; }
    }
}