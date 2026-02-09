using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos
{
    /// <summary>
    /// Clase genérica a usar cuando se requieran consultas usando paginado, se retorna una lista como resultado de la consulta y
    /// un valor llamado TotalRegistros que indica cuál es el total de registros retornado si no se hace paginación.
    /// </summary>
    /// <typeparam name="T">Tipo de elemento a retornar en la lista</typeparam>
    [DataContract(Namespace = "http://contrologis.com")]
    public class GenericoConsultasFramework<T> where T : class  
    {
        /// <summary>
        /// Información a retornar
        /// </summary>
        [DataMember]
        public IEnumerable<T> Lista { get; set; }

        /// <summary>
        /// Total de registros arrojados por la consulta sin paginar.
        /// </summary>
        [DataMember]
        public int TotalRegistros { get; set; }
    }
}