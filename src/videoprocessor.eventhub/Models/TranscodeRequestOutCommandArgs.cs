using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace videoprocessor.eventhub.Models
{
    public class TranscodeRequestCommandArgs
    {
        public string? InFileOptions { get; set; }
        public string? InFileName { get; set; }
        public required string OutFileOptions { get; set; }
        public required string OutFileName { get; set; }
    }
}
