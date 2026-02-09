using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class SEUbicacionAutorizada : DataContractBase
    {
        /// <summary>
        /// Id del punto/Agencia/Col/Racol/gestion o sucursal  con el que ingresó el usuario
        /// </summary>
        [DataMember]
        public string IdLocacion
        {
            get;
            set;
        }

        /// <summary>
        /// El tipo indica si es un punto/Agencia/Col/Racol/gestion o sucursal
        /// </summary>
        [DataMember]
        public TipoLocacionAutorizada TipoLocacion
        {
            get;
            set;
        }

        [DataMember]
        public string DescripcionLocacion
        {
            get;
            set;
        }

        /// <summary>
        /// Cuando la ubicacion es de tipo centro de servicios se debe asociar este número de caja
        /// </summary>
        [DataMember]
        public int IdCaja
        {
            get;
            set;
        }

        /// <summary>
        /// Ciudad de la ubicación autorizada
        /// </summary>
        [DataMember]
        public string IdCiudad
        {
            get;
            set;
        }


        /// <summary>
        /// Ciudad de la ubicación autorizada
        /// </summary>
        [DataMember]
        public string DescripcionCiudad
        {
            get;
            set;
        }



        [DataMember]
        public string IdCentroCostos
        {
            get;
            set;
        }


        [DataMember]
        public bool ImpresionPos
        {
            get;
            set;
        }

        [DataMember]
        public bool Operacional
        {
            get;
            set;
        }


        [DataMember]
        public string TipoCentroServicio { get; set; }

        [DataMember]
        public long IdCentroServiciosOrigen { get; set; }
        
        /// <summary>
        /// Es custodia para el centro servicio (especial para bodega)
        /// </summary>
        [DataMember]
        public bool EsCustodia { get; set; }
    }

    [DataContract(Namespace = "http://contrologis.com")]
    public enum TipoLocacionAutorizada
    {
        /// <summary>
        /// Incluye punto/Agencia/Col/Racol
        /// </summary>
        [EnumMember]
        CentroServicios,

        /// <summary>
        /// Areas de la compañia
        /// </summary>
        [EnumMember]
        Gestion,

        /// <summary>
        /// Sucursal de un cliente crédito
        /// </summary>
        [EnumMember]
        Sucursal
    }
}