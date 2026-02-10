using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PUMunicipioCentroLogisticoDC : DataContractBase
    {
        [DataMember]
        public PALocalidadDC Municipio { get; set; }

        [DataMember]
        [Display(Name = "Centro logistico")]
        public PUCentroServicioApoyo CentroLogistico { get; set; }

        public PUMunicipioCentroLogisticoDC()
        {
            Municipio = new PALocalidadDC();
            CentroLogistico = new PUCentroServicioApoyo();

            //Data original
            MunicipioOriginal = new PALocalidadDC();
            CentroLogisticoOriginal = new PUCentroServicioApoyo();
        }

        #region DataOriginalsinModificar

        [DataMember]
        public PALocalidadDC MunicipioOriginal { get; set; }

        [DataMember]
        [Display(Name = "Centro logistico")]
        public PUCentroServicioApoyo CentroLogisticoOriginal { get; set; }

        #endregion DataOriginalsinModificar
    }
}