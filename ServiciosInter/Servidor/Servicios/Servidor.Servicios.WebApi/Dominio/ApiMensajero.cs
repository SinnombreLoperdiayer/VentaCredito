using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CO.Servidor.Servicios.WebApi.Comun;
using Framework.Servidor.Comun;
using CO.Servidor.Servicios.ContratoDatos.Mensajero;
using CO.Servidor.Servicios.WebApi.ModelosRequest.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiMensajero : ApiDominioBase
    {
        private static readonly ApiMensajero instancia = (ApiMensajero)FabricaInterceptorApi.
            GetProxy(new ApiMensajero(), COConstantesModulos.MENSAJERO);

        public static ApiMensajero Instancia
        {
            get { return ApiMensajero.instancia; }
        }

        public object POEnumEstadosMensajero { get; private set; }

        private ApiMensajero() { }


        public MEMensajero ConsultarMensajero(int idDocumento)
        {
            return FabricaServicios.ServicioMensajero.ConsultarMensajero(idDocumento);
        }

        public bool CrearMensajero(PAMensajero mensajero)
        {
            MEMensajero ouMensajero = new MEMensajero();
            ouMensajero.EsContratista = mensajero.EsContratista;
            ouMensajero.EsMensajeroUrbano = mensajero.EsMensajeroUrbano;
            ouMensajero.IdTipoMensajero = mensajero.IdTipoMensajero;
            ouMensajero.NumeroPase = mensajero.NumeroPase;
            ouMensajero.Telefono2 = mensajero.Telefono2;

            ouMensajero.Agencia = new ContratoDatos.CentroServicios.PUAgenciaDeRacolDC();
            ouMensajero.Agencia.IdCentroServicio = mensajero.Agencia;
            ouMensajero.EstadoRegistro = (EnumEstadoRegistro)Enum.Parse(typeof(EnumEstadoRegistro), mensajero.EstadoRegistro);

            ouMensajero.PersonaInterna = new OUPersonaInternaDC();

            ouMensajero.PersonaInterna.Direccion = mensajero.PersonaInterna.Direccion;
            ouMensajero.PersonaInterna.IdCargo = mensajero.PersonaInterna.IdCargo;
            ouMensajero.PersonaInterna.Regional = mensajero.PersonaInterna.Regional;
            ouMensajero.PersonaInterna.Identificacion = mensajero.PersonaInterna.Identificacion;
            ouMensajero.PersonaInterna.Nombre = mensajero.PersonaInterna.Nombre;
            ouMensajero.PersonaInterna.PrimerApellido = mensajero.PersonaInterna.PrimerApellido;
            ouMensajero.PersonaInterna.SegundoApellido = mensajero.PersonaInterna.SegundoApellido;
            ouMensajero.PersonaInterna.Telefono = mensajero.PersonaInterna.Telefono;
            ouMensajero.PersonaInterna.Regional = mensajero.PersonaInterna.Regional;
            ouMensajero.PersonaInterna.FechaInicioContrato = mensajero.PersonaInterna.FechaInicioContrato;
            ouMensajero.PersonaInterna.FechaTerminacionContrato = mensajero.PersonaInterna.FechaTerminacionContrato;

            ouMensajero.LocalidadMensajero = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC();
            ouMensajero.LocalidadMensajero.IdLocalidad = mensajero.IdLocalidad;

            ouMensajero.CargoMensajero = new Framework.Servidor.Servicios.ContratoDatos.Seguridad.SECargo();
            ouMensajero.CargoMensajero.IdCargo = mensajero.IdCargo;


            ouMensajero.Estado = new OUEstadosMensajeroDC();
            switch (mensajero.IdEstado)
            {
                case "1":
                    mensajero.IdEstado = "ACT";
                    break;
                case "2":
                    mensajero.IdEstado = "INA";
                    break;
                case "3":
                    mensajero.IdEstado = "SUS";
                    break;

            }

            ouMensajero.Estado.IdEstado = mensajero.IdEstado;

            ouMensajero.TipoContrato = new POTipoContrato();
            ouMensajero.TipoContrato.IdTipoContrato = mensajero.IdTipoContrato;

            ouMensajero.TipMensajeros = new OUTipoMensajeroDC();
            ouMensajero.TipMensajeros.IdTipoMensajero = mensajero.IdTipoMensajero;

            return FabricaServicios.ServicioMensajero.CrearMensajero(ouMensajero);
        }
        public List<MEMensajero> ObtenerPersonal(string idTipoUsuario)
        {
            return FabricaServicios.ServicioMensajero.ObtenerPersonal(idTipoUsuario);
        }

        public MEMensajero ObtenerDetalleEmpleadoNovasoft(string idDocumento, string compania)
        {
            return FabricaServicios.ServicioMensajero.ObtenerDetalleEmpleadoNovasoft(idDocumento, compania);
        }

        public bool ObtenerPersonaInternaXId(string idDocumento)
        {
            return FabricaServicios.ServicioMensajero.ObtenerPersonaInternaXId(idDocumento);
        }


        public MEMensajero ObtenerEmpleadoNovasoft(string idDocumento)
        {
            return FabricaServicios.ServicioMensajero.ObtenerEmpleadoNovasoft(idDocumento);
        }

    }
}