using Azure.Messaging.EventHubs.Processor;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using videoprocessor.eventhub.Interfaces;
using videoprocessor.eventhub.Models;

namespace videoprocessor.eventhub.Services
{
    internal class VideoTranscoder : IVideoTranscoder
    {
        private readonly ILogger<VideoTranscoder> _logger;

        public VideoTranscoder(ILogger<VideoTranscoder> logger)
        {
            _logger = logger;
        }

        public async Task TranscodeAsync(TranscodeRequest transcodeRequest)
        {
            _logger.LogInformation($"Transcoding {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}...");
            // TODO: This is fake implemtation to test scaling kindly ignore for now
            int elapsedSeconds = 0;
            while (elapsedSeconds < 60) // TODO: fix here
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                elapsedSeconds += 10;
                _logger.LogInformation($"Still transcoding {elapsedSeconds}... {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}...");
            }
            _logger.LogInformation($"Transcoding completed for {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}.");
        }
    }
}
