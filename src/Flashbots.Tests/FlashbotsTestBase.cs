using Nethereum.Web3.Accounts;
using NUnit.Framework;

namespace Flashbots.Tests
{
    internal abstract class FlashbotsTestBase
    {
        protected int chainId;
        protected string flashbotUrl;
        protected string url;
        protected string senderKey;
        protected string signerKey;
        protected Account sender;
        
        [SetUp]
        public void Init()
        {
            chainId = 1;
            flashbotUrl = "https://relay.flashbots.net";
            string infurakey = "";
            
            url = $"https://mainnet.infura.io/v3/{infurakey}";

            senderKey = "";
            signerKey = "";

            sender = new Account(senderKey, chainId);
        }
    }
}