using Azure.Messaging.EventHubs.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using videoprocessor.eventhub.Models;

namespace videoprocessor.eventhub.Interfaces
{
    internal interface IVideoTranscoder
    {
        Task TranscodeAsync(TranscodeMessage transcodeMessage);
    }
}
