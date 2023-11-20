using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace videoprocessor.xabe
{
    public class AssetMessage
    {
        public required string AssetContianerName {get; set;}
        public required string AssetId { get; set; }
        public required string OriginalAssetBlobName { get; set; }
        public required string OutFilePrefix { get; set; }
    }
}
