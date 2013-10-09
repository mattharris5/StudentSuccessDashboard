using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;

namespace SSD.Security
{
    public class DefaultSecurityConfiguration : ISecurityConfiguration
    {
        private const string AdministratorEmailAddressesSettingName = "AdministratorEmailAddresses";

        public IEnumerable<string> AdministratorEmailAddresses
        {
            get
            {
                string settingValue = CloudConfigurationManager.GetSetting(AdministratorEmailAddressesSettingName);
                if (settingValue == null)
                {
                    throw new InvalidOperationException("Cannot get " + AdministratorEmailAddressesSettingName + " because setting was not found.");
                }
                return settingValue.Split(',');
            }
        }
    }
}
