using Nethereum.Web3;

namespace Flashbots
{
    /// <summary>
    /// A Web3 with an additional Flashbots to execute flashbots RPC commands.
    /// </summary>
    public interface IFlashbotsWeb3 : IWeb3
    {
        IFlashbots Flashbots { get; }
    }
}