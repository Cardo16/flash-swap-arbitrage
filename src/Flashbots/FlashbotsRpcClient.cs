using Common.Logging;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Flashbots
{
    /// <summary>
    /// A Custom RPC client that adds the X-Flashbots-Signature.
    /// </summary>
    public class FlashbotsRpcClient : RpcClient
    {
        /// <summary>
        /// SendAsync is overriden, but also protected. So use this method instead.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public async Task<RpcResponseMessage> SendAsyncPublic(RpcRequestMessage request, string route = null)
        {
            return await SendAsync(request, route);
        }

        private string CreateHashForSignature(string text)
        {
            var ecKey = new EthECKey(account.PrivateKey);
            var keccack = Sha3Keccack.Current.CalculateHash(text).HexToByteArray();

            var signer = new EthereumMessageSigner();
            var hash = signer.EncodeUTF8AndSign(keccack.ToHex(true), ecKey);
            return $"{account.Address}:{hash}";
        }

        protected override async Task<RpcResponseMessage> SendAsync(RpcRequestMessage request, string route = null)
        {
            RpcLogger logger = new RpcLogger(_log);
            try
            {
                HttpClient orCreateHttpClient = GetOrCreateHttpClient();
                string text = JsonConvert.SerializeObject(request, _jsonSerializerSettings);
                StringContent content = new StringContent(text, Encoding.UTF8, "application/json");

                string signature = CreateHashForSignature(text);
                content.Headers.Add("X-Flashbots-Signature", signature);

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(ConnectionTimeout);
                logger.LogRequest(text);
                HttpResponseMessage obj = await orCreateHttpClient.PostAsync(route, content, cancellationTokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
                using StreamReader reader = new StreamReader(await obj.Content.ReadAsStreamAsync());
                using JsonTextReader reader2 = new JsonTextReader(reader);
                obj.EnsureSuccessStatusCode();
                RpcResponseMessage rpcResponseMessage = JsonSerializer.Create(_jsonSerializerSettings).Deserialize<RpcResponseMessage>(reader2);
                logger.LogResponse(rpcResponseMessage);
                return rpcResponseMessage;
            }
            catch (TaskCanceledException innerException)
            {
                RpcClientTimeoutException ex = new RpcClientTimeoutException($"Rpc timeout after {ConnectionTimeout.TotalMilliseconds} milliseconds", innerException);
                logger.LogException(ex);
                throw ex;
            }
            catch (Exception innerException2)
            {
                RpcClientUnknownException ex2 = new RpcClientUnknownException("Error occurred when trying to send rpc requests(s): " + request.Method, innerException2);
                logger.LogException(ex2);
                throw ex2;
            }
        }

        private readonly AuthenticationHeaderValue _authHeaderValue;
        private readonly Account account;
        private readonly Uri _baseUrl;
        private readonly HttpClientHandler _httpClientHandler;
        private readonly ILog _log;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private volatile bool _firstHttpClient;
        private HttpClient _httpClient;
        private HttpClient _httpClient2;
        private bool _rotateHttpClients = true;
        private DateTime _httpClientLastCreatedAt;
        private readonly object _lockObject = new object();

        private const int NUMBER_OF_SECONDS_TO_RECREATE_HTTP_CLIENT = 60;

        private static HttpMessageHandler GetDefaultHandler()
        {
            try
            {
#if NETSTANDARD2_0
                return new HttpClientHandler
                {
                    MaxConnectionsPerServer = MaximumConnectionsPerServer
                };
           
#elif NETCOREAPP2_1 || NETCOREAPP3_1 || NET5_0_OR_GREATER
                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = new TimeSpan(0, NUMBER_OF_SECONDS_TO_RECREATE_HTTP_CLIENT, 0),
                    PooledConnectionIdleTimeout = new TimeSpan(0, NUMBER_OF_SECONDS_TO_RECREATE_HTTP_CLIENT, 0),
                    MaxConnectionsPerServer = MaximumConnectionsPerServer
                };
#else
                return null;
#endif
            }
            catch
            {
                return null;
            }
        }

        private HttpClient GetOrCreateHttpClient()
        {
            if (_rotateHttpClients) //already created if not rotated
            {
                lock (_lockObject)
                {
                    var timeSinceCreated = DateTime.UtcNow - _httpClientLastCreatedAt;
                    if (timeSinceCreated.TotalSeconds > NUMBER_OF_SECONDS_TO_RECREATE_HTTP_CLIENT)
                        CreateNewRotatedHttpClient();
                    return GetClient();
                }
            }
            else
            {
                return GetClient();
            }
        }

        private HttpClient GetClient()
        {
            if (_rotateHttpClients)
            {
                lock (_lockObject)
                {

                    return _firstHttpClient ? _httpClient : _httpClient2;
                }
            }
            else
            {
                return _httpClient;
            }
        }

        private void CreateNewRotatedHttpClient()
        {
            var httpClient = CreateNewHttpClient();
            _httpClientLastCreatedAt = DateTime.UtcNow;

            if (_firstHttpClient)
            {
                lock (_lockObject)
                {
                    _firstHttpClient = false;
                    _httpClient2 = httpClient;
                }
            }
            else
            {
                lock (_lockObject)
                {
                    _firstHttpClient = true;
                    _httpClient = httpClient;
                }
            }
        }

        private HttpClient CreateNewHttpClient()
        {
            HttpClient httpClient = new HttpClient();

            if (_httpClientHandler != null)
            {
                httpClient = new HttpClient(_httpClientHandler);
            }
            else
            {
                var handler = GetDefaultHandler();
                if (handler != null)
                {
                    httpClient = new HttpClient(handler);
                }
            }

            InitialiseHttpClient(httpClient);
            return httpClient;
        }

        private void InitialiseHttpClient(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Authorization = _authHeaderValue;
            httpClient.BaseAddress = _baseUrl;
        }

        public FlashbotsRpcClient(Uri baseUrl, string signerKey, AuthenticationHeaderValue authHeaderValue = null, JsonSerializerSettings jsonSerializerSettings = null, HttpClientHandler httpClientHandler = null, ILog log = null) :
            this(baseUrl, new Account(signerKey), authHeaderValue, jsonSerializerSettings, httpClientHandler, log)
        {

        }

        public FlashbotsRpcClient(Uri baseUrl, Account signer, AuthenticationHeaderValue authHeaderValue = null, JsonSerializerSettings jsonSerializerSettings = null, HttpClientHandler httpClientHandler = null, ILog log = null) :
            base(baseUrl, authHeaderValue, jsonSerializerSettings, httpClientHandler, log)
        {
            account = signer; // we need this account to sign rpc messages
            _baseUrl = baseUrl;

            if (authHeaderValue == null)
            {
                authHeaderValue = BasicAuthenticationHeaderHelper.GetBasicAuthenticationHeaderValueFromUri(baseUrl);
            }

            _authHeaderValue = authHeaderValue;

            if (jsonSerializerSettings == null)
                jsonSerializerSettings = DefaultJsonSerializerSettingsFactory.BuildDefaultJsonSerializerSettings();

            _jsonSerializerSettings = jsonSerializerSettings;
            _httpClientHandler = httpClientHandler;
            _log = log;

#if NETCOREAPP2_1 || NETCOREAPP3_1 || NET5_0_OR_GREATER
            _httpClient = CreateNewHttpClient();
            _rotateHttpClients = false;
#else
            CreateNewRotatedHttpClient();
#endif

        }
    }
}