
# Web3.Flashbots

This is the beginning of the Flashbots in dotnet.

Using Nethereum.Web3 (4.5.0)

The Flashbots.Console is a sample project on how to use the Web3.Flashbots. In the sample project 3 transactions will be created as a bundle. The bundle gets simulated and send to flashbots in the next block. The sample project is based on: https://github.com/flashbots/web3-flashbots

The Web3.Flashbots uses a FlashbotsRpcClient for adding the X-Flashbots-Signature. This was not possible with just the Nethereum library.

## Quickstart

In Flashbots.Console add a privatekey and infurakey in launchsettings.json. Add some goerli eth to your account from a fauccet: https://goerlifaucet.com/ . 

## Development

A Nuget package will follow soon.


