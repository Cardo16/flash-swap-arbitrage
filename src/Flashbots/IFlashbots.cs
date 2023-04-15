using Flashbots.RpcResponses;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3.Accounts;

public interface IFlashbots
{

    /// <summary>
    /// Simulates the whole bundle.
    /// Throws an exception when the request has errors.
    /// </summary>
    /// <param name="bundle">IList of bundles to include.</param>
    /// <param name="blockNumber">in Hex</param>
    /// <returns>Returns a Task with CallBundleResponse</returns>
    Task<CallBundleResponse> CallBundleAsync(IList<string> bundle, HexBigInteger blockNumber);

    /// <summary>
    /// Send the flashbot bundle to the Flashbot Relay
    /// </summary>
    /// <param name="bundle"></param>
    /// <param name="targetBlock"></param>
    /// <returns>A bundlehash.</returns>
    Task<SendBundleResponse> SendBundleAsync(IList<string> bundle, HexBigInteger targetBlock);

    /// <summary>
    /// Needs to be signed with your signer key.
    /// </summary>
    /// <param name="bundleHash"></param>
    /// <param name="blockNumber"></param>
    /// <returns>Bundle stats.</returns>
    Task<BundleStatsResponse> FlashbotsGetBundleStatsAsync(string bundleHash, HexBigInteger blockNumber);

    /// <summary>
    /// Gets User stats.
    /// </summary>
    /// <param name="blockNumber"></param>
    /// <returns></returns>
    Task<FlashbotsGetUserStatsResponse> FlashbotsGetUserStatsAsync(HexBigInteger blockNumber);

    /// <summary>
    /// Signs a transaction with the sender account provided.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="txInput"></param>
    /// <returns></returns>
    string SignTransaction(Account sender, TransactionInput txInput);
}