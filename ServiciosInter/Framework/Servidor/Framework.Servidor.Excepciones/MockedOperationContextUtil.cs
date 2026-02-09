using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Framework.Servidor.Excepciones
{
  /// <summary>
  /// Interfaz falsa de un servicio web para crear un ObjectContext falso
  /// </summary>
  [ServiceContract(Namespace = "http://contrologis.com")]
  public interface IMockedService
  {
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void DoWork();
  }

  /// <summary>
  /// Clase para construir un ObjectContext falso (Mucked)
  /// </summary>
  public class MockedOperationContextUtil
  {
    /// <summary>
    /// Crea un contexto falso e inicializa el contexto de controller a un valor falseado (Mucked). Sólo debe ser usado en ambiente de pruebas unitarias
    /// </summary>
    /// <remarks>Sólo debe ser usado en ambiente de pruebas unitarias</remarks>
    public static void MockedControllerContext()
    {
#if DEBUG
      if (OperationContext.Current == null)
      {
        //mucked channel
        IContextChannel mockedChannel = (IContextChannel)ChannelFactory<IMockedService>.CreateChannel(
           new CustomBinding(new BinaryMessageEncodingBindingElement(),
             new HttpTransportBindingElement
             {
               MaxBufferSize = 2147483647,
               MaxReceivedMessageSize = 2147483647,
             }),
             new EndpointAddress(new Uri(@"http://localhost/CO.Servidor.Servicios.Web/PAParametrosFW.svc")));

        OperationContext.Current = new OperationContext(mockedChannel);
        OperationContext.Current.Extensions.Add(new ControllerContext());
      }

      if (ControllerContext.Current.Usuario == null)
      {
        ControllerContext.Current.Usuario = "Usuario-Mock";
      }
#endif
    }

    public static void CrearControllerContextTareas()
    {
        IContextChannel mockedChannel = (IContextChannel)ChannelFactory<IMockedService>.CreateChannel(
          new CustomBinding(new BinaryMessageEncodingBindingElement(),
            new HttpTransportBindingElement
            {
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
            }),
            new EndpointAddress(new Uri(@"http://localhost/CO.Servidor.Servicios.Web/PAParametrosFW.svc")));

        OperationContext.Current = new OperationContext(mockedChannel);
        OperationContext.Current.Extensions.Add(new ControllerContext());

    }

  }
}