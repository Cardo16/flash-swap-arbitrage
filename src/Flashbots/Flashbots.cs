using Flashbots.RpcResponses;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3.Accounts;

namespace Flashbots;
/// <summary>
/// Web3.Flashbots
/// </summary>
public class Flashbots : IFlashbots
{
    private FlashbotsRpcClient Client { get; }

    public Flashbots(FlashbotsRpcClient client)
    {
        Client = client;
    }

    public async Task<SendBundleResponse> SendBundleAsync(IList<string> signedBundledTransaction, HexBigInteger targetBlock)
    {
        if (Client == null) throw new NullReferenceException("Client not configured");

        Dictionary<string, object> parameterMap = new()
        {
            { "txs", signedBundledTransaction },
            { "blockNumber", targetBlock.HexValue }
        };

        object[] parameterList = new object[1];
        parameterList[0] = parameterMap;

        var message = new RpcRequestMessage(1, FlashbotApiMethods.eth_sendBundle.ToString(), parameterList);

        var rpcResponseMessage = await Client.SendAsyncPublic(message);
        if (rpcResponseMessage.HasError)
        {
            var error = new Nethereum.JsonRpc.Client.RpcError(
                rpcResponseMessage.Error.Code,
                rpcResponseMessage.Error.Message,
                rpcResponseMessage.Error.Data);

            throw new RpcResponseException(error);
        }

        return rpcResponseMessage.GetResult<SendBundleResponse>();
    }

    public virtual async Task<CallBundleResponse> CallBundleAsync(IList<string> signedBundledTransaction, HexBigInteger evmBlockNumber)
    {
        if (Client == null) throw new NullReferenceException("Client not configured");

        Dictionary<string, object> parameterMap = new()
        {
            { "txs", signedBundledTransaction },
            { "blockNumber", evmBlockNumber },
            { "stateBlockNumber", "latest" }
        };

        object[] parameterList = new object[1];
        parameterList[0] = parameterMap;

        var message = new RpcRequestMessage(1, FlashbotApiMethods.eth_callBundle.ToString(), parameterList);

        var rpcResponseMessage = await Client.SendAsyncPublic(message);

        if (rpcResponseMessage.HasError)
        {
            var error = new Nethereum.JsonRpc.Client.RpcError(
                rpcResponseMessage.Error.Code,
                rpcResponseMessage.Error.Message,
                rpcResponseMessage.Error.Data);

            throw new RpcResponseException(error);
        }
        return rpcResponseMessage.GetResult<CallBundleResponse>();
    }


    public string SignTransaction(Account sender, TransactionInput txInput)
    {
        return new AccountOfflineTransactionSigner().
            SignTransaction(sender, txInput).
            EnsureHexPrefix();
    }

    public async Task<BundleStatsResponse> FlashbotsGetBundleStatsAsync(string bundleHash, HexBigInteger blockNumber)
    {
        if (Client == null) throw new NullReferenceException("Client not configured");

        Dictionary<string, object> parameterMap = new()
        {
            { "bundleHash", bundleHash },
            { "blockNumber", blockNumber }
        };

        object[] parameterList = new object[1];
        parameterList[0] = parameterMap;

        var message = new RpcRequestMessage(1, FlashbotApiMethods.flashbots_getBundleStats.ToString(), parameterList);

        var rpcResponseMessage = await Client.SendAsyncPublic(message);
        if (rpcResponseMessage.HasError)
        {
            var error = new Nethereum.JsonRpc.Client.RpcError(
                rpcResponseMessage.Error.Code,
                rpcResponseMessage.Error.Message,
                rpcResponseMessage.Error.Data);

            throw new RpcResponseException(error);
        }

        return rpcResponseMessage.GetResult<BundleStatsResponse>();
    }

    public async Task<FlashbotsGetUserStatsResponse> FlashbotsGetUserStatsAsync(HexBigInteger blockNumber)
    {
        if (Client == null) throw new NullReferenceException("Client not configured");

        Dictionary<string, object> parameterMap = new()
        {
            { "blockNumber", blockNumber }
        };

        object[] parameterList = new object[1];
        parameterList[0] = parameterMap;

        var message = new RpcRequestMessage(1, FlashbotApiMethods.flashbots_getUserStats.ToString(), parameterList);

        var rpcResponseMessage = await Client.SendAsyncPublic(message);
        if (rpcResponseMessage.HasError)
        {
            var error = new Nethereum.JsonRpc.Client.RpcError(
                rpcResponseMessage.Error.Code,
                rpcResponseMessage.Error.Message,
                rpcResponseMessage.Error.Data);

            throw new RpcResponseException(error);
        }

        return rpcResponseMessage.GetResult<FlashbotsGetUserStatsResponse>();
    }
}
