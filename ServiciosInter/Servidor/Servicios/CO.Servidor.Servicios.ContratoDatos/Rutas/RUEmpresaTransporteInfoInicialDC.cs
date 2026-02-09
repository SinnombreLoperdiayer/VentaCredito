using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas
{
    /// <summary>
    /// Clase que contiene la informacion de las empresas transportadoras
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class RUEmpresaTransporteInfoInicialDC : DataContractBase
    {
        /// <summary>
        /// Lista de Racoles Para inicializar la creacion de una
        /// Empresa Transportadora
        /// </summary>
        [DataMember]
        public List<PURegionalAdministrativa> ListaRacoles { get; set; }

        /// <summary>
        /// Lista de los Medios de Transporte para la
        /// creacion de una empresa
        /// </summary>
        [DataMember]
        public List<PAMedioTransporte> ListaMedioTransporte { get; set; }

        /// <summary>
        /// Lista de los Estados de Una empresa
        /// </summary>
        [DataMember]
        public List<SEEstadoUsuario> ListaEstadosEmpresa { get; set; }

        /// <summary>
        /// Lista de los Tipos de Transporte
        /// </summary>
        [DataMember]
        public List<RUTipoTransporte> ListaTiposTransporte { get; set; }
    }
}