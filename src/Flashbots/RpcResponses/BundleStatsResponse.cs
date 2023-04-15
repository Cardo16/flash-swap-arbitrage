using System.Text.Json;

namespace Flashbots.RpcResponses
{
    public class BundleStatsResponse
    {
        public bool isSimulated { get; set; }
        public bool isSentToMiners { get; set; }
        public bool isHighPriority { get; set; }
        public DateTime simulatedAt { get; set; }
        public DateTime submittedAt { get; set; }
        public DateTime sentToMinersAt { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

        }
    }
}