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
        public List<TranscodeRequestCommandArgs>? CommandArgs { get; set; }
    }
}
