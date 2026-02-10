using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    public class RequestVehiculo
    {
        public Vehiculo root { get; set; } 
    }

    public class Vehiculo 
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public VariablesVehiculo variables { get; set; }
    }

    public class VariablesVehiculo 
    {
        public long NUMNITEMPRESATRANSPORTE { get; set; }
        public string NUMPLACA { get; set; }
        public long CODCONFIGURACIONUNIDADCARGA { get; set; }
        public long CODMARCAVEHICULOCARGA { get; set; }
        public long CODLINEAVEHICULOCARGA { get; set; }
        public long ANOFABRICACIONVEHICULOCARGA { get; set; }
        public long CODTIPOCOMBUSTIBLE { get; set; }
        public long PESOVEHICULOVACIO { get; set; }
        public string CODCOLORVEHICULOCARGA { get; set; }
        public long CODTIPOCARROCERIA { get; set; }
        public string CODTIPOIDPROPIETARIO { get; set; }
        public long NUMIDPROPIETARIO { get; set; }
        public string CODTIPOIDTENEDOR { get; set; }
        public long NUMIDTENEDOR { get; set; }
        public string NUMSEGUROSOAT { get; set; }
        public string FECHAVENCIMIENTOSOAT { get; set; }
        public long NUMNITASEGURADORASOAT { get; set; }
        public long CAPACIDADUNIDADCARGA { get; set; }
        public long UNIDADMEDIDACAPACIDAD { get; set; }
    }
}
