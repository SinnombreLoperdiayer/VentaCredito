using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.ParametrosOperacion
{
    public class PAPersonaInterna
    {
        /// <summary>
        /// retorna o asigna la cedula de la persona
        /// </summary>
        public string Identificacion { get; set; }

        /// <summary>
        /// retorna o asigna el tipo de identificacion de la persona
        /// </summary>
        public string IdTipoIdentificacion { get; set; }

        /// <summary>
        /// retorna o asigna el tipo de identificacion de la persona
        /// </summary>
        public string IdMunicipio { get; set; }

        /// <summary>
        /// retorna o asigna el tipo de identificacion de la persona
        /// </summary>
        public long IdPersonaInterna { get; set; }

        /// <summary>
        /// Retorna o asigna el id del cargo de la persona
        /// </summary>
        public int IdCargo { get; set; }

        /// <summary>
        /// Retorna o asigna el cargo de la persona
        /// </summary>
        //[Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string Cargo { get; set; }

        /// <summary>
        /// retorna o asigna el nombre de la persona
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Retorna o Asigna el primer apellido de la persona
        /// </summary>
       public string PrimerApellido { get; set; }

        /// <summary>
        /// Retorna o asigna el segundo apellido de la persona
        /// </summary>
        public string SegundoApellido { get; set; }

        /// <summary>
        /// retorna o asigna la direccion de la persona
        /// </summary>
          public string Direccion { get; set; }

        /// <summary>
        /// Retorna o asigna el municipio de la persona
        /// </summary>
          public string Municipio { get; set; }

        /// <summary>
        /// retorna o asigna el telefono de la persona
        /// </summary>
        public string Telefono { get; set; }

     
     
        /// <summary>
        /// retorna o asigna el codigo de la regional de  la persona
        /// </summary>
        //[Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string TipoContrato { get; set; }

        /// <summary>
        /// REtorna o asigana la fecha de inicio de contrato
        /// </summary>
          public DateTime FechaInicioContrato { get; set; }

        /// <summary>
        /// Retorna o asigna la fecha de terminacion del contrato
        /// </summary>
        public DateTime FechaTerminacionContrato { get; set; }

    
        public long Regional { get; set; }


    }
}