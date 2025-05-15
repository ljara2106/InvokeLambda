using System;
using System.IO;
using System.Web;
using System.Xml;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Amazon.Runtime;
using System.Web.UI;

namespace InvokeLambda
{
    public partial class InvokeLambda : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadLambdaOptions();
            }
        }

        protected void LoadLambdaOptions()
        {
            // Load Lambda options from the XML file
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Server.MapPath("~/LambdaFunctions.xml"));

            foreach (XmlNode node in xmlDoc.SelectNodes("//Function"))
            {
                string name = node.Attributes["Name"].Value;
                string text = node.InnerText;
                ListItem listItem = new ListItem(text, name);
                ddlLambdaOptions.Items.Add(listItem);
            }
        }

        protected void btnInvokeLambda_Click(object sender, EventArgs e)
        {
            string selectedFunctionName = ddlLambdaOptions.SelectedItem.Value;

            // Check if "Select Application" is selected
            if (string.IsNullOrEmpty(selectedFunctionName))
            {
                lblResult.Text = "<span style='color:red;'>Please select an application from the drop-down. </span>";
                return;
            }

            string awsAccessKeyId = WebConfigurationManager.AppSettings["AWSAccessKeyId"];
            string awsSecretAccessKey = WebConfigurationManager.AppSettings["AWSSecretAccessKey"];
            string lambdaRegion = WebConfigurationManager.AppSettings["LambdaRegion"];

            // Show the processing animation
            ScriptManager.RegisterStartupScript(this, GetType(), "disableButtonForXSeconds", "disableButtonForXSeconds();", true);


            try
            {
                
                var lambdaClient = new AmazonLambdaClient(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.GetBySystemName(lambdaRegion));
              
                var request = new InvokeRequest
                {
                    FunctionName = selectedFunctionName, // Use the selected Lambda function name
                    InvocationType = InvocationType.RequestResponse,
                    LogType = LogType.Tail,
                    Payload = "{}" // pass input data as JSON here if needed
                };

                var response = lambdaClient.InvokeAsync(request).Result;

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    string result = System.Text.Encoding.UTF8.GetString(response.Payload.ToArray());

                    // Parse JSON and format it vertically
                    JObject json = JObject.Parse(result);
                    string formattedJson = FormatJsonVertically(json);

                    lblResult.Text = $@"Lambda Response: <span style='color:green;'>Successful 👍 {ddlLambdaOptions.SelectedItem.Text} </span><br />
                                    <pre class='json-container'>{formattedJson}</pre>";

                    // Log the response to a file
                    LogResponse(selectedFunctionName, result);
                }
                else
                {
                    lblResult.Text = $@"Lambda Response: <span style='color:red;'>Error 😒 ({response.HttpStatusCode})</span>";
                }

            }
            catch (AmazonServiceException awsEx)
            {
                // Handle Amazon service exceptions (e.g., invalid credentials)
                string errorMessage = awsEx.Message;
                lblResult.Text = $@"Lambda Response: <span style='color:red;'>Amazon Service Error 😒({errorMessage})</span>";
                LogException("Amazon Service Exception", awsEx.ToString());
            }

            catch (Exception ex)
            {
                lblResult.Text = $@"Lambda Response: <span style='color:red;'> {ddlLambdaOptions.SelectedItem.Text} - Error 😒 ({ex.Message})</span>";
                LogException("Unhandled Exception", ex.ToString());
            }

            finally
            {
                // Hide the processing animation
                ScriptManager.RegisterStartupScript(this, GetType(), "disableButtonForXSeconds", "disableButtonForXSeconds();", true);
            }
        }

        // Helper method to format JSON vertically
        private string FormatJsonVertically(JObject json)
        {
            string formattedJson = "";
            foreach (var property in json.Properties())
            {
                formattedJson += $"{property.Name}: {property.Value}" + System.Environment.NewLine;
            }
            return formattedJson;
        }

        private string GetUserIPAddress()
        {
            string userIPAddress = string.Empty;

            string forwardedFor = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                string[] ipArray = forwardedFor.Split(',');
                userIPAddress = ipArray[0].Trim();
            }
            else
            {
                userIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return userIPAddress;
        }

        private void LogResponse(string functionName, string response)
        {
            string logFolderPath = Server.MapPath("~/log");
            string currentDateString = DateTime.Now.ToString("yyyyMMdd");
            string logFileName = $"{functionName}_{currentDateString}.log";
            string logFilePath = Path.Combine(logFolderPath, logFileName);

            string userIpAddress = GetUserIPAddress();

            // Format the log message
            string logMessage = $"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Function: {functionName} | IP Address: {userIpAddress} | Response: {response}";

            // Check if the log file for the current date exists
            if (!File.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFolderPath);
                File.WriteAllText(logFilePath, ""); 
            }

            File.AppendAllText(logFilePath, logMessage + System.Environment.NewLine + System.Environment.NewLine); 
        }

        // Log exceptions 
        private void LogException(string category, Exception ex)
        {
            string logFolderPath = Server.MapPath("~/log");
            string currentDateString = DateTime.Now.ToString("yyyyMMdd");
            string logFileName = $"{category}_{currentDateString}.log";
            string logFilePath = Path.Combine(logFolderPath, logFileName);

            string userIpAddress = GetUserIPAddress();

            // Format the log message with the exception's message (error message)
            string logMessage = $"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Category: {category} | IP Address: {userIpAddress} | Exception Message: {ex.Message}";

            if (!File.Exists(logFilePath))
            {              
                Directory.CreateDirectory(logFolderPath);
                File.WriteAllText(logFilePath, ""); 
            }

            // Append the log message with a newline to the log file
            File.AppendAllText(logFilePath, logMessage + System.Environment.NewLine + System.Environment.NewLine); 
        }



        // Overloaded method to log exceptions with only a message
        private void LogException(string category, string message)
        {
            string logFolderPath = Server.MapPath("~/log");
            string currentDateString = DateTime.Now.ToString("yyyyMMdd");
            string logFileName = $"{category}_{currentDateString}.log";
            string logFilePath = Path.Combine(logFolderPath, logFileName);

            string userIpAddress = GetUserIPAddress();

            // Format the log message with exception details
            string logMessage = $"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Category: {category} | IP Address: {userIpAddress} | Message: {message}";

            if (!File.Exists(logFilePath))
            {     
                Directory.CreateDirectory(logFolderPath);
                File.WriteAllText(logFilePath, ""); 
            }

            File.AppendAllText(logFilePath, logMessage + System.Environment.NewLine + System.Environment.NewLine); 
        }



    }
}
