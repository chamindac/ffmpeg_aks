using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace videoprocessor.eventhub.Models
{
    public class TranscodeRequest
    {
        public required string AssetContianerName { get; set; }
        public required string AssetId { get; set; }
        public required string OriginalAssetBlobName { get; set; }
        public required string OutFilePrefix { get; set; }
        public List<TranscodeMessageCommandArgs>? CommandArgs { get; set; }
    }

    public class TranscodeMessageCommandArgs 
    {
        public required string OutFileOptions { get; set; }
        public required string OutFileName { get; set; }
    }
}
