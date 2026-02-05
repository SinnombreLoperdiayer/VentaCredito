using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.Planilla
{
    public class Persona_Huella_AUT
    {
        public Int64 IdTercero { get; set; }

        /// <summary>
        /// Identificador de Huella
        /// </summary>
        public Int64 Huella_Id { get; set; }

        /// <summary>
        /// Id de Usuario / Tercero que realizo el proceso de registro en BD
        /// </summary>
        public Int64 Creado_Por { get; set; }

        /// <summary>
        /// Identificador deltipo de discapacidad del Usuario
        /// </summary>
        public Int16? IdDiscapacidad { get; set; }

        /// <summary>
        /// Observaciones adicionales acerca de la discapacidad del tercero
        /// </summary>
        public String Observaciones_discapacidad { get; set; }

        /// <summary>
        /// Indicador de mano derecha (true) o izquierda (false) que debera ser usada en la validación
        /// </summary>
        public bool ManoDerecha { get; set; }

        /// <summary>
        /// Cadena de texto que indica el dedo que debera ser usado en la validación
        /// </summary>
        public string Dedo { get; set; }

        /// <summary>
        /// Fecha de creación del registro en BD de la Biometría
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// XML serializado perteneciente a los Bytes proporcionados por el dispositivo Biométrico
        /// </summary>
        public string SerializadoHuella { get; set; }

        public string Identificación { get; set; }
    }
}
