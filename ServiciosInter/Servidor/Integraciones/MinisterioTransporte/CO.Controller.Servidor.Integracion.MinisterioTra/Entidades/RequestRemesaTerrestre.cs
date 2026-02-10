using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    public class RequestRemesaTerrestre
    {
        public RemesaTerrestre root { get; set; }
    }

    public class RemesaTerrestre
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public VariablesRemesaTerrestre variables { get; set; }
    }

    public class VariablesRemesaTerrestre 
    {
       
        public long NUMNITEMPRESATRANSPORTE { get; set; }
        public string CONSECUTIVOREMESA { get; set; }
        public string CONSECUTIVOINFORMACIONCARGA { get; set; }
        public string CONSECUTIVOCARGADIVIDIDA { get; set; }
        public string CODOPERACIONTRANSPORTE { get; set; }
        public string CODNATURALEZACARGA { get; set; }
        public long? CANTIDADCARGADA { get; set; }
        public string UNIDADMEDIDACAPACIDAD { get; set; }
        public string CODTIPOEMPAQUE { get; set; }
        public string PESOCONTENEDORVACIO { get; set; }
        public string MERCANCIAREMESA { get; set; }
        public string DESCRIPCIONCORTAPRODUCTO { get; set; }
        public string CODTIPOIDREMITENTE { get; set; }
        public string NUMIDREMITENTE { get; set; }
        public string CODSEDEREMITENTE { get; set; }
        public string CODTIPOIDDESTINATARIO { get; set; }
        public string NUMIDDESTINATARIO { get; set; }
        public string CODSEDEDESTINATARIO { get; set; }
        public string DUENOPOLIZA { get; set; }
        public long? NUMPOLIZATRANSPORTE { get; set; }
        public string FECHAVENCIMIENTOPOLIZACARGA { get; set; }
        public long? COMPANIASEGURO { get; set; }
        public long? HORASPACTOCARGA { get; set; }
        public long? MINUTOSPACTOCARGA { get; set; }
        public long? HORASPACTODESCARGUE { get; set; }
        public long? MINUTOSPACTODESCARGUE { get; set; }
        public string FECHALLEGADACARGUE { get; set; }
        public string HORALLEGADACARGUEREMESA { get; set; }
        public string FECHAENTRADACARGUE { get; set; }
        public string HORAENTRADACARGUEREMESA { get; set; }
        public string FECHASALIDACARGUE { get; set; }
        public string HORASALIDACARGUEREMESA { get; set; }
        public string CODTIPOIDPROPIETARIO { get; set; }
        public string NUMIDPROPIETARIO { get; set; }
        public string CODSEDEPROPIETARIO { get; set; }
        public string FECHACITAPACTADACARGUE { get; set; }
        public string HORACITAPACTADACARGUE { get; set; }
        public string FECHACITAPACTADADESCARGUE { get; set; }
        public string HORACITAPACTADADESCARGUEREMESA { get; set; }
    }
}
