using Flashbots;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.TransactionTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Numerics;
using static Nethereum.Util.UnitConversion;


string? privateKey = Environment.GetEnvironmentVariable("privatekey");
string? infurakey = Environment.GetEnvironmentVariable("infurakey");

if(string.IsNullOrEmpty(privateKey))
{
    throw(new ArgumentNullException((privateKey),$"{nameof(privateKey)} cannot be null or empty."));
}
if (string.IsNullOrEmpty(infurakey))
{
    throw (new ArgumentNullException((infurakey), $"{nameof(infurakey)} cannot be null or empty."));
}

/**
 *Submitted for verification at Etherscan.io on 2022-09-11
*/
//pragma solidity = 0.8.17;
//contract SendToCoinbase
//{
//    receive() external payable
//    {
//        payable(block.coinbase).transfer(msg.value);
//    }
//}
// this contract will forward all goerli eth as a bribe to the miner
string receiverAddress = "0x3B18d85bE088BC95c14EcEb2958c5702CDbb0894";

string flashbotUrl = "https://relay-goerli.flashbots.net";
string url = $"https://goerli.infura.io/v3/{infurakey}";
int chainId = 5;


Account sender = new(privateKey, chainId);

string signerKey = Nethereum.Signer.EthECKey.GenerateKey()
                                            .GetPrivateKey();

Console.WriteLine($"We generated a signerkey for your flashbots: {signerKey}");


IFlashbotsWeb3 web3 = new FlashbotsWeb3(sender, url, flashbotUrl, signerKey);


var nonce = await web3.TransactionManager.Account.NonceService.GetNextNonceAsync();

var signedBundle = GenerateSignedBundle(web3, sender, receiverAddress, nonce).ToList();

BigInteger oldBlockNo = 0;
var blockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

// Sending bundle until we find the tx hash in a block.
while (true)
{
    Console.WriteLine($"Simulating on block {blockNumber}");
    string txHash;

    try
    {
        var callBundle = await web3.Flashbots.CallBundleAsync(signedBundle, blockNumber);
        txHash = callBundle.results[0].txHash;

        Console.WriteLine($"Simulation succesful. {txHash}");
    }
    catch (Exception e)
    {
        Console.WriteLine($"Simulation error: {e}");
        return;
    }

    HexBigInteger targetBlock = new(blockNumber.Value + 1);
    Console.WriteLine($"Sending bundle targeting block {targetBlock}");

    var task = web3.Flashbots.SendBundleAsync(signedBundle, targetBlock);

    try
    {
        task.Wait();
        var sendBundleResponse = task.Result;
        string bundleHash = sendBundleResponse.bundleHash;

        Console.WriteLine($"Searching for tx hash: {txHash}");

        while (oldBlockNo == blockNumber)
        {
            blockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            Thread.Sleep(1000);
        }

        var tx = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txHash);

        if (tx == null || tx.BlockNumber == null)
        {
            throw new Exception($"Transaction from bundle not found in block {blockNumber}");
        }

        Console.WriteLine($"Bundle {bundleHash} found in block: {tx.BlockNumber}");
        return;
    }
    catch (Exception e)
    {
        Console.WriteLine($"Bundle not found in block {blockNumber.Value}. \n {e.Message}");
    }
    oldBlockNo = blockNumber.Value;
}

// Generates 3 Tx's which send eth to bribe miners as an example
IEnumerable<string> GenerateSignedBundle(IFlashbotsWeb3 web3, Account sender, string receiverAddress, HexBigInteger nonce)
{
    for (int i = 0; i < 3; i++)
    {
        TransactionInput tx = new(
            TransactionType.EIP1559.AsHexBigInteger(),
            "",
            receiverAddress,
            sender.Address,
            new HexBigInteger(42000),
            new HexBigInteger(Web3.Convert.ToWei(0.001, EthUnit.Ether)),
            new HexBigInteger(Web3.Convert.ToWei(200, EthUnit.Gwei)),
            new HexBigInteger(Web3.Convert.ToWei(50, EthUnit.Gwei))
        )
        {
            Nonce = new HexBigInteger(nonce.Value + i),
        };
        yield return web3.Flashbots.SignTransaction(sender, tx).EnsureHexPrefix();
    }
}