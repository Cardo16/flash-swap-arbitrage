using System.Text.Json;

namespace Flashbots.RpcResponses
{
    public class FlashbotsGetUserStatsResponse
    {
        public bool is_high_priority { get; set; }
        public string all_time_miner_payments { get; set; }
        public string all_time_gas_simulated { get; set; }
        public string last_7d_miner_payments { get; set; }
        public string last_7d_gas_simulated { get; set; }
        public string last_1d_miner_payments { get; set; }
        public string last_1d_gas_simulated { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}