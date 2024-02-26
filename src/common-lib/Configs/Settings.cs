using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.lib.Configs
{
    public class Settings
    {
        // Settings from CH_WIDEMO_CONFIG
        public string? AppConfigEndpoint { get; set; }
        public string? AppConfigLabel { get; set; }
        public string? SharedAppConfiglabel { get; set; }
        public string? KeyVaultName { get; set; }
        public string? AadTenantId { get; set; }

        // App config settings
        public string? DemoSharedConfig1 { get; set; }
        public string? DemoSharedConfig2 { get; set; }
        public string? DemoConfig1 { get; set; }
        public string? DemoConfig2 { get; set; }
        public string? DemoSharedSecret1 { get; set; }
        public string? DemoSharedSecret2 { get; set; }
        public string? DemoBGSecret1 { get; set; }
        public string? DemoBGSecret2 { get; set; }
    }
}
