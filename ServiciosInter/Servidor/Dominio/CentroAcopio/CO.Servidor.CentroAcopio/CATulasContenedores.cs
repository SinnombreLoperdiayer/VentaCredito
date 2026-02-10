using CO.Servidor.CentroAcopio.Datos;
using CO.Servidor.Servicios.ContratoDatos.CentroAcopio;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CO.Servidor.CentroAcopio
{
    public class CATulasContenedores : MarshalByRefObject
    {
        private static readonly CATulasContenedores instancia = (CATulasContenedores)FabricaInterceptores.GetProxy(new CATulasContenedores(), COConstantesModulos.MODULO_CENTRO_ACOPIO);
        public static CATulasContenedores Instancia
        {
            get { return CATulasContenedores.instancia; }
        }

        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
        private IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        private ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();


        /// <summary>
        /// Método para crear el consolidado Tula o Contenedor
        /// </summary>
        public void InsertarConsolidado(CATipoConsolidado Consolidado)
        {
            CARepositorioTulasContenedores.Instancia.InsertarConsolidado(Consolidado);
        }


        /// <summary>
        /// Consulta las Tulas y Contenedores de un Centro de Servicio
        /// </summary>
        /// <returns></returns>
        public List<CATipoConsolidado> ObtenerConsolidadosCSPropietario()
        {
            return CARepositorioTulasContenedores.Instancia.ObtenerConsolidadosCSPropietario();
        }

        /// <summary>
        /// Actualiza el Centro de Servicio Destino del Contenedor o Tula
        /// </summary>
        public void ModificarCentroServicioDestinoConsolidado(List<CATipoConsolidado> tipoConsolidado)
        {
            CARepositorioTulasContenedores.Instancia.ModificarCentroServicioDestinoConsolidado(tipoConsolidado);
        }
    }
}
