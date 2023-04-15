using Common.Logging;
using Nethereum.BlockchainProcessing.Services;
using Nethereum.Contracts.Services;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC;
using Nethereum.RPC.TransactionManagers;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Net.Http.Headers;

namespace Flashbots
{
    public class FlashbotsWeb3 : IFlashbotsWeb3
    {
        private Web3 Web3 { get; }
        public IFlashbots Flashbots { get; }

        public IClient Client => Web3.Client;

        public IEthApiContractService Eth => Web3.Eth;

        public IBlockchainProcessingService Processing => Web3.Processing;

        public INetApiService Net => Web3.Net;

        public IPersonalApiService Personal => Web3.Personal;

        public IShhApiService Shh => Web3.Shh;

        public ITransactionManager TransactionManager { get => Web3.TransactionManager; set => Web3.TransactionManager = value; }


        public static implicit operator Web3(FlashbotsWeb3 _)
        {
            return _.Web3;
        }

        public FlashbotsWeb3(Web3 web3, string flashbotUrl, string signerKey, ILog log = null, AuthenticationHeaderValue authenticationHeader = null)
        {
            var flashbotsClient = new FlashbotsRpcClient(new Uri(flashbotUrl), signerKey, authenticationHeader, null, null, log);
            Web3 = web3;
            Flashbots = new Flashbots(flashbotsClient);
        }

        public FlashbotsWeb3(Account account, string url, string flashbotUrl, string signerKey, ILog log = null, AuthenticationHeaderValue authenticationHeader = null)
            : this(new Web3(account, url), flashbotUrl, signerKey, log, authenticationHeader)
        {
        }
    }
}