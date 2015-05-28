using System;
using System.Collections.Generic;
using System.Net;
using System.Data.Odbc;
using System.Security;
using Apprenda.Services.Logging;
using Newtonsoft.Json;

namespace Apprenda.SaaSGrid.Addons.Altibase
{
    public class AltibaseAddon : AddonBase
    {
        private static readonly ILogger Log = LogManager.Instance().GetLogger(typeof (AltibaseAddon));

        public override ProvisionAddOnResult Provision(AddonProvisionRequest request)
        {
            try
            {
                var parameters = DeveloperParameters.Parse(request.Manifest, request.DeveloperParameters);
                // todo check parameters
                // creates a secure password of length 16. we can customize this further if needed.
                var secPw = "a" + RandoPW.GetUniqueKey(20);
                var connectionString =
                    $"DSN=altibase-lab;Uid={parameters.AltibaseUsername};Pwd={parameters.AltibasePassword}";
                var queryString = $"create user {parameters.SchemaName} identified by {secPw}";
                var cmd = new OdbcCommand(queryString);
                using (var connection = new OdbcConnection(connectionString))
                {
                    cmd.Connection = connection;
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }

                return new ProvisionAddOnResult("")
                {
                    ConnectionData = (new ConnectionInfo
                    {
                        jdbcConnectionString = $"jdbc:Altibase//{parameters.AltibaseHost}:{parameters.AltibasePort}/{parameters.AltibaseDB}",
                        jdbcUsername = $"{parameters.SchemaName}",
                        jdbcPassword = $"{secPw}",
                        odbcConnectionString = $"Dsn={{ALTIBASE}};UID={parameters.SchemaName};PWD={secPw};Database={parameters.AltibaseDB}",
                        // this is used for deprov (just easier).
                        schemaName = $"{parameters.SchemaName}"
                    }).ToString(),
                    IsSuccess = true,
                    EndUserMessage = "Provisioned OK."
                };
            }
            catch (Exception e)
            {
                Log.Error(e.Message + "\n" + e.InnerException);
                return new ProvisionAddOnResult("")
                {
                    IsSuccess = false,
                    EndUserMessage = e.Message
                };
            }
            
        }

        public override OperationResult Deprovision(AddonDeprovisionRequest request)
        {
            try
            {
                var parameters = DeveloperParameters.Parse(request.Manifest, request.DeveloperParameters);
                // todo check parameters
                // creates a secure password of length 16. we can customize this further if needed
                var connection = new OdbcConnection($"Driver={{Altibase}};Server={parameters.AltibaseHost};UID={parameters.AltibaseUsername};PWD={parameters.AltibasePassword};Database={parameters.AltibaseDB}");
                var queryString = $"drop user {parameters.SchemaName}";
                var cmd = new OdbcCommand(queryString, connection);
                connection.Open();
                connection.Close();
                return new OperationResult()
                {
                    IsSuccess = true,
                    EndUserMessage = "Deprovision OK"
                };
            }
            catch (Exception e)
            {
                Log.Error(e.Data);
                return new OperationResult()
                {
                    
                };
            }
            
        }

        public override OperationResult Test(AddonTestRequest request)
        {
            return new OperationResult()
            {
                EndUserMessage = "Testing complete, but still need to build out a few more things.",
                IsSuccess = true
            };
        }

        private class DeveloperParameters
        {
            internal string SchemaName { get; private set; }
            internal string AltibaseHost { get; private set; }
            internal int AltibasePort { get; private set; }
            internal string AltibaseUsername { get; private set; }
            internal string AltibasePassword { get; private set; }

            internal string AltibaseDB { get; private set; }

            internal static DeveloperParameters Parse(AddonManifest manifest, IEnumerable<AddonParameter> developerParams)
            {
                // construct main model for parsing manifest and developerParams
                var developerParameters = new DeveloperParameters();
                // parse manifest
                foreach (var property in manifest.Properties)
                {
                    switch (property.Key.ToLowerInvariant())
                    {
                        case "altibasehost":
                            developerParameters.AltibaseHost = property.Value;
                            break;
                        case "altibaseport":
                            int tmp;
                            if (int.TryParse(property.Value, out tmp)) developerParameters.AltibasePort = tmp;
                            else
                            {
                                Log.Error($"Port provided ({property.Value}) was invalid format, defaulting to 20300");
                                developerParameters.AltibasePort = 20300;
                            }
                            break;
                        case "altibaseusername":
                            developerParameters.AltibaseUsername = property.Value;
                            break;
                        case "altibasepassword":
                            developerParameters.AltibasePassword = property.Value;
                            break;
                        case "altibasedb":
                            developerParameters.AltibaseDB = property.Value;
                            break;
                        default:
                            Log.Warn($"Unrecognized property: {property.Key}");
                            break;
                    }
                }
                // parse developerParams
                foreach (var addonParam in developerParams)
                {
                    switch (addonParam.Key.ToLowerInvariant())
                    {
                        case "schemaname":
                            developerParameters.SchemaName = addonParam.Value;
                            break;
                        default:
                            Log.Warn($"Unrecognized argument: {addonParam.Key}");
                            break;
                    }
                }

                return developerParameters;
            }
        }
    }

    public class ConnectionInfo
    {
        public string odbcConnectionString { get; set; }
        public string jdbcConnectionString { get; set; }
        public string schemaName { get; set; }
        public string jdbcUsername { get; set; }
        public string jdbcPassword { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
