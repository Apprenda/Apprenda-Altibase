using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Apprenda.Utility.Extensions;
using NUnit.Framework;

namespace Apprenda.SaaSGrid.Addons.Altibase
{
    [TestFixture]
    public class UnitTest1
    {
        AddonProvisionRequest _request;

        [SetUp]
        public void SetupTest()
        {
            var manifest = new AddonManifest
            {
                Parameters = new ParameterList(),
                AllowUserDefinedParameters = false,
                Author = "Chris Dutra",
                Properties = new List<AddonProperty>
                {
                    // properties go here
                    new AddonProperty()
                    {
                        Key = "",
                        Value = ""
                    }
                },
                DeploymentNotes = "",
                Description = "",
                DeveloperHelp = "",
                IsEnabled = true,
                ManifestVersionString = "2",
                Name = "Altibase",
                ProvisioningLocation = "",
                ProvisioningPassword = "",
                ProvisioningUsername = "",
                Vendor = "Apprenda",
                Version = "1.0"
            };

            _request = new AddonProvisionRequest
            {
                DeveloperParameters = new List<AddonParameter>
                {
                    new AddonParameter()
                    {
                        Key = "asodfasod",
                        Value = "aodifsad"
                    },
                    new AddonParameter()
                    {
                        Key = "",
                        Value = ""
                    }
                },
                Manifest = manifest
            };

        }

        [Test]
        public void ProvisionTest()
        {
            var output = new AltibaseAddon().Provision(_request);
            // bunch of simple asserts for now
            Assert.NotNull(output);
            Assert.NotNull(output.ConnectionData);
            Assert.NotNull(output.EndUserMessage);
            Assert.NotNull(output.IsSuccess);
            Assert.True(output.IsSuccess);
            Assert.That(output.ConnectionData.Length, Is.GreaterThan(0));
        }

        public void DeProvisionTest()
        {
            
        }

        public void SOCTest()
        {
            
        }
    }
}
