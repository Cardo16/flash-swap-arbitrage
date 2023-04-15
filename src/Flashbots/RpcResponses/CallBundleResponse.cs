using System.Text.Json;

namespace Flashbots.RpcResponses
{
    public class CallBundleResponse
    {
        public string? bundleGasPrice { get; set; }
        public string? bundleHash { get; set; }
        public string? coinbaseDiff { get; set; }
        public string? ethSentToCoinbase { get; set; }
        public string? gasFees { get; set; }
        public List<Result> results { get; set; }
        public int stateBlockNumber { get; set; }
        public int totalGasUsed { get; set; }
        public string? error { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }

        public class Result
        {
            public string coinbaseDiff { get; set; }
            public string ethSentToCoinbase { get; set; }
            public string fromAddress { get; set; }
            public string gasFees { get; set; }
            public string gasPrice { get; set; }
            public int gasUsed { get; set; }
            public string toAddress { get; set; }
            public string txHash { get; set; }
            public string value { get; set; }
        }
    }
}