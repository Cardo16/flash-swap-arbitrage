using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using NUnit.Framework;

namespace Flashbots.Tests
{
    [TestFixture]
    internal class GetBundleStatsTests : FlashbotsTestBase
    {
        [Ignore("Integration tests")]
        [Test]
        public async Task GetBundleStats()
        {
            string blockNumber = "15510022";

            var web3 = new Web3(sender, url);
            var sut = new FlashbotsWeb3(web3, flashbotUrl, signerKey);

            const string bundleHash = "0xefd2c77a07c375e8177939ee1c76c111a6daaa6199470779f13bfaacee4c4c81";
            var bundleStats = await sut.Flashbots.FlashbotsGetBundleStatsAsync(bundleHash, new HexBigInteger(blockNumber));

            Assert.NotNull(bundleStats);
            Console.WriteLine(bundleStats);
        }

    }
}
