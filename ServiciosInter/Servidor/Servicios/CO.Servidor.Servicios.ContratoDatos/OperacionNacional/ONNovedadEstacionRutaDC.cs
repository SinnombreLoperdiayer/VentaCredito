using CO.Cliente.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    /// <summary>
    /// Novedad asignada a una Estacion-Ruta
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ONNovedadEstacionRutaDC
    {
        /// <summary>
        /// Es el id de la Novedad Estacion-Ruta (Autonumerico)
        /// </summary>
        [DataMember]
        public long IdNovedadEstacionRuta { get; set; }
        [DataMember]
        [Display(Name = "Nombre Novedad")]
        public string NombreNovedad { get; set; } // No tiene persistencia.

        /// <summary>
        /// Id del Manifiesto de Operacion Nacional
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroManifiesto", Description = "NumeroManifiesto")]
        public long IdManifiestoOpeNal { get; set; }


        /// <summary>
        /// Ciudad de la Ruta donde se Genera la Novedad
        /// </summary>
        [DataMember]
        public string IdLocalidadEstacion { get; set; }
        [DataMember]
        [Display(Name = "Nombre Localidad")]
        public string NombreLocalidad { get; set; } // No tiene persistencia.


        /// <summary>
        /// Id tipo de Novedad
        /// </summary>
        [DataMember]
        public short IdTipoNovedad { get; set; }

        /// <summary>
        /// Novedad seleccionada
        /// </summary>
        [DataMember]
        [Display(Name = "Fecha y hora Novedad")]
        public DateTime FechaNovedad { get; set; }

        /// <summary>
        /// Cantidad Horas de la Novedad
        /// </summary>
        [DataMember]
        public int HorasNovedad { get; set; }

        /// <summary>
        /// Cantidad Minutos de la Novedad
        /// </summary>
        [DataMember]
        public int MinutosNovedad { get; set; }

        /// <summary>
        /// Observaciones de la Novedad
        /// </summary>
        [DataMember]
        public string Observaciones { get; set; }

        [DataMember]
        public string LugarIncidente { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }



        /// <summary>
        /// Opcion Clase de Novedad [ORI]Origen  [DES]Destino  [OPE]Operativo
        /// </summary>
        [DataMember]
        public string ClaseNovedad { get; set; }

        [DataMember]
        [Display(Name = "Clase Novedad")]
        public string stringClaseNovedad
        {
            get
            {
                if (this.ClaseNovedad == "ORI") return "Origen";
                if (this.ClaseNovedad == "DES") return "Destino";
                if (this.ClaseNovedad == "OPE") return "Operativo";

                return this.ClaseNovedad;
            }

            set { }
        } // No Tiene Persistencia




        /// <summary>
        /// Centro de Cervicio COL
        /// </summary>
        [DataMember]
        public long IdCentroServiciosCOL { get; set; }
        [DataMember]
        [Display(Name = "Nombre COL")]
        public string NombreCOL { get; set; } // No tiene persistencia.


        //1
        [DataMember]
        public string IdCiudad { get; set; }
        [DataMember]
        [Display(Name = "Nombre Ciudad")]
        public string NombreCiudad { get; set; } // No tiene persistencia.


        //2
        /// <summary>
        /// Centro de Cervicio Punto
        /// </summary>
        [DataMember]
        public long IdCentroServiciosPunto { get; set; }
        [DataMember]
        [Display(Name = "Punto")]
        public string NombrePunto { get; set; } // No tiene persistencia.


        //3
        /// <summary>
        /// Mensajero
        /// </summary>
        [DataMember]
        public long IdMensajero { get; set; }
        [DataMember]
        [Display(Name = "Nombre Mensajero")]
        public string NombreMensajero { get; set; } // No tiene persistencia.


        //4
        /// <summary>
        /// Cliente
        /// </summary>
        [DataMember]
        public int IdClienteCre { get; set; }
        [DataMember]
        [Display(Name = "Nombre Cliente")]
        public string NombreClienteCre { get; set; } // No tiene persistencia.

        //4.2
        [DataMember]
        public int IdContrato { get; set; }
        [DataMember]
        [Display(Name = "Nombre Contrato")]
        public string NombreContrato { get; set; } // No tiene persistencia.

        //4.3
        [DataMember]
        public int IdSucursalContrato { get; set; }
        [DataMember]
        [Display(Name = "Nombre Sucursal")]
        public string NombreSucursalCon { get; set; } // No tiene persistencia.


        /// <summary>
        /// Impacto Retraso en dias
        /// </summary>
        [DataMember]
        public int Retraso { get; set; }


        [DataMember]
        public ONEnumTipoAfectaNovGuia TipoAfectacion { get; set; }

    }

}