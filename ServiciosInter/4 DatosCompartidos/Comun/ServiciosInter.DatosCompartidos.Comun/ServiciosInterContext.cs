using System.ServiceModel;

namespace ServiciosInter.DatosCompartidos.Comun
{
    public class ServiciosInterContext : IExtension<OperationContext>
    {
        //The "current" custom context
        public static ServiciosInterContext Current
        {
            get
            {
                return OperationContext.Current == null ? null : OperationContext.Current.Extensions.Find<ServiciosInterContext>();
            }
        }

        #region IExtension<OperationContext> Members

        public void Attach(OperationContext owner)
        {
            //no-op
        }

        public void Detach(OperationContext owner)
        {
            //no-op
        }

        #endregion IExtension<OperationContext> Members

        private string usuario;

        private long codigoUsuario;

        private long idCentroServicio;

        private string nombreCentroServicio;

        private int idAplicativoOrigen;

        public int IdAplicativoOrigen
        {
            get { return idAplicativoOrigen; }
            set { idAplicativoOrigen = value; }
        }

        public string NombreCentroServicio
        {
            get { return nombreCentroServicio; }
            set { nombreCentroServicio = value; }
        }

        public long IdCentroServicio
        {
            get { return idCentroServicio; }
            set { idCentroServicio = value; }
        }

        public string Usuario
        {
            get { return this.usuario; }
            set { this.usuario = value; }
        }

        public long CodigoUsuario
        {
            get { return this.codigoUsuario; }
            set { this.codigoUsuario = value; }
        }

        public long Identificacion { get; set; }

        private string identificadorMaquina = "";

        public string IdentificadorMaquina
        {
            get { return identificadorMaquina; }
            set { identificadorMaquina = value; }
        }

        public int IdCaja { get; set; }

        public string TokenSesion { get; set; }
    }
}