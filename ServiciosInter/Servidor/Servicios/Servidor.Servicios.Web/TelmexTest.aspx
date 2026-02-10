<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TelmexTest.aspx.cs" Inherits="CO.Servidor.Servicios.Web.TelmexTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Test WebService Telmex</title>
  <link rel="stylesheet" href="styles.css" />

</head>
<body>

  <form id="form1" runat="server">

    <div class="titulo">
      <h1>CONSULTE SU ENVÍO</h1>

    </div>
    <div class="parametros">
      <asp:Label ID="Label1" runat="server">Cuenta</asp:Label>
      <asp:TextBox ID="txtCuenta" runat="server"></asp:TextBox>

      <asp:Label ID="Label2" runat="server" Text="Fecha:"></asp:Label>
      <asp:TextBox ID="txtFecha" runat="server"></asp:TextBox>

    </div>
    <div class="boton">
      <asp:Button ID="Button1" class="btn" runat="server" Text="Test" OnClick="Button1_Click" />

    </div>
    <div class="resultado">
      <div id="tblSection">
        <div class="col1">
          <div class="cell">
            <asp:Label runat="server" Text="Nombre del Cliente:"></asp:Label>
          </div>
          <div class="cell">
            <asp:Label runat="server" Text="" ID="txtNombreCliente"></asp:Label>
          </div>
        </div>
        <div class="col1">
          <div class="cell">
            <asp:Label ID="Label4" class="cell" runat="server" Text="Número de Guía:"></asp:Label>
          </div>
          <div class="cell">
            <asp:Label runat="server" Text="" ID="txtNumGuia"></asp:Label>
          </div>
        </div>
        <div class="col1">
          <div class="cell">
            <asp:Label ID="Label3" runat="server" Text="Direccion del Cliente:"></asp:Label>
          </div>
          <div class="cell">
            <asp:Label runat="server" Text="" ID="txtDireccion"></asp:Label>
          </div>
        </div>
        <div class="col1">
          <div class="cell">
            <asp:Label ID="Label5" class="cell" runat="server" Text="Motivo de devolución:"></asp:Label>
          </div>
          <div class="cell">
            <asp:Label runat="server" Text="" ID="txtMotivoDevolucion"></asp:Label>
          </div>
        </div>
      </div>
      <div class="imagen">
         <asp:Image ID="imgGuia" runat="server" />
    </div></div>
    <div class="error">
      <asp:Label runat="server" ID="lblError"></asp:Label>
    </div
  </form>
</body>
</html>
