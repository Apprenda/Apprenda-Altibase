using System;
using System.Collections.Generic;
using Apprenda.Utility.Extensions;

namespace Apprenda.SaaSGrid.Addons.Altibase
{
    class Program
    {
        static void Main(string[] args)
        {
            var parameters = new List<AddonParameter>
            {
                new AddonParameter
                {
                    Key = "schemaname",
                    Value = ""
                }
            };
            var request = new AddonProvisionRequest
            {
                
                Manifest = new AddonManifest
                {
                    Parameters = new ParameterList(),
                    Properties = new List<AddonProperty>
                    {
                        new AddonProperty
                        {
                            Key = "AltibaseHost",
                            Value = "54.91.16.224"
                        },
                        new AddonProperty
                        {
                            Key = "AltibasePort",
                            Value = "20300"
                        },
                        new AddonProperty
                        {
                            Key = "AltibaseUsername",
                            Value = "sys"
                        },
                        new AddonProperty
                        {
                            Key = "AltibasePassword",
                            Value = "manager"
                        },
                        new AddonProperty
                        {
                            Key="altibasedb",
                            Value="mydb"
                        }
                    },
                    ProvisioningLocation = "",
                    ProvisioningUsername = "",
                    Description = "",
                    Version = "1.0",
                    Name = "Altibase",
                    DeveloperHelp = "",
                    ProvisioningPasswordHasValue = false,
                    AllowUserDefinedParameters = true,
                    ProvisioningPassword = "",
                    Author = "Chris Dutra",
                    ManifestVersionString = "2",
                    Vendor = "Apprenda",
                    DeploymentNotes = "",
                    IsEnabled = true
                },
                DeveloperParameters = new List<AddonParameter>
                {
                    new AddonParameter
                    {
                        Key = "schemaname",
                        Value = "dutra2"
                    }
                }
            };

            request.Manifest.Parameters.Items = parameters.ToArray() as IAddOnParameterDefinition[];
            var output = new AltibaseAddon().Provision(request);
            Console.WriteLine(output.IsSuccess);
            Console.WriteLine(output.EndUserMessage);
            Console.WriteLine(output.ConnectionData);
            Console.ReadKey();
        }
    }
}
