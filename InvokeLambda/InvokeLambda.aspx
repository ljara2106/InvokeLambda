<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvokeLambda.aspx.cs" Inherits="InvokeLambda.InvokeLambda" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1"/>

    <title>AWS Lambda Production Recycle</title>

    <style>
      /* Reset some default styles */
      body,
      html {
        margin: 0;
        padding: 0;
        font-family: Arial, sans-serif;
      }
      body {
        background-color: #f8f9fa;
      }
      /* Container for the entire content */
      .container {
        max-width: 600px;
        margin: 0 auto;
        padding: 20px;
        background-color: #fff;
        border-radius: 10px;
        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
      }
      /* Header styles */
      .header {
        text-align: center;
        padding: 20px 0;
      }
      .header h1 {
        font-size: 28px;
        color: #333;
      }
      /* Dropdown styles */
      .form-group {
        margin-bottom: 20px;
      }
      .form-control {
        width: 100%;
        padding: 10px;
        border: 1px solid #ccc;
        border-radius: 5px;
        font-size: 16px;
      }
      /* Button styles */
      .btn-invoke {
        display: block;
        width: 100%;
        padding: 10px;
        background-color: #007bff;
        color: #fff;
        text-align: center;
        text-decoration: none;
        border: none;
        border-radius: 5px;
        font-size: 18px;
        cursor: pointer;
        transition: background-color 0.3s ease;
      }
      .btn-invoke:hover {
        background-color: #0056b3;
      }
      /* Result label styles */
      .result-label {
        text-align: center;
        font-size: 18px;
        margin-top: 20px;
        color: #333;
      }
      /* Mobile responsiveness */
      @media(max-width: 768px) {
        .container {
          padding: 10px;
          border-radius: 0;
        }
        .header h1 {
          font-size: 24px;
        }
        .btn-invoke {
          font-size: 16px;
        }
      }
      /* CSS for JSON response display */
      .json-container {
        max-width: 100%; /* Ensure it doesn't overflow container */
        overflow-x: auto; /* Add horizontal scrollbar when content overflows */
        border: 1px solid #ccc;
        border-radius: 5px;
        padding: 10px;
        font-family: monospace;
      }
      .logo {
        width: 250px;
        height: 150px;
        margin: 0 auto 10px;
      }
      .processing {
        display: none;
        text-align: center;
      }
      .processing img {
        width: 50px;
        height: 50px;
      }
    </style>

      <script>
          var isButtonDisabled = false;
          var countdown = 60; // 60 seconds
      function disableButtonForXSeconds() {
        if (! isButtonDisabled) {
          isButtonDisabled = true;
          var btn = document.getElementById('<%= btnInvokeLambda.ClientID %>');
          btn.disabled = true;
          function updateButtonText() {
            if (countdown === 0) {
              isButtonDisabled = false;
              btn.disabled = false;
              btn.value = 'Invoke Lambda';
            } else {
              btn.value = 'Disabled for ' + countdown + 's';
              countdown--;
              setTimeout(updateButtonText, 1000); // Update every second
            }
          }
          updateButtonText();
        }
      }
          function showProcessing() {
              //disableButtonForXSeconds();
              document
                  .getElementById('processing')
                  .style
                  .display = 'block';
          }
          function hideProcessing() {
              document.getElementById('<%= btnInvokeLambda.ClientID %>').disabled = false;
              document
                  .getElementById('processing')
                  .style
                  .display = 'none';
          }
      </script>
  </head>
  <body>
    <div class="container">
      <div class="header">
        <img class="logo" src="aws-lambda.png" alt="Logo"/>
        <h1>Production Recycle</h1>
      </div>
      <form id="form1" runat="server">
        <div class="form-group">
          <asp:DropDownList id="ddlLambdaOptions" runat="server" cssclass="form-control">
            <asp:ListItem text="---Select Application---" value=""/>
          </asp:DropDownList>
        </div>
        <div class="form-group">
          <asp:Button id="btnInvokeLambda" runat="server" cssclass="btn-invoke" text="Invoke Lambda" onclick="btnInvokeLambda_Click" OnClientClick="showProcessing();" />
        </div>
      </form>
      <div class="result-label">
        <div class="processing" id="processing">
          <img src="pacman-processing.gif" alt="Processing"/>
          <p>Processing...</p>
        </div>
        <asp:Label id="lblResult" runat="server" text=""></asp:Label>
      </div>
    </div>
  </body>
</html>