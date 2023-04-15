using Flashbots;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Flashbots.Tests
{
    [TestFixture]
    internal class GetUserStatsTests : FlashbotsTestBase
    {
        [Ignore("Integration tests")]
        [Test]
        public async Task GetUserStats()
        {
            var web3 = new Web3(sender, url);
            var sut = new FlashbotsWeb3(web3, flashbotUrl, signerKey);

            var blockNo = await sut.Eth.Blocks.GetBlockNumber.SendRequestAsync();

            Console.WriteLine($"Getting lastest user stats for block: {blockNo}");
            var userStats = await sut.Flashbots.FlashbotsGetUserStatsAsync(blockNo);

            Assert.NotNull(userStats);
            Console.WriteLine(userStats);
        }

    }
}
