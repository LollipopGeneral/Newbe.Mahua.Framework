using Orleans;
using Orleans.Runtime;
using Polly;
using System;
using System.Threading.Tasks;

namespace Newbe.Mahua.Plugins.Greenstal.Services
{
    public interface IClientFactory
    {
        IClusterClient GetClient();
    }

    public class ClientFactory : IClientFactory
    {
        private static Func<IClientBuilder> _builderFunc;
        private static IClusterClient _client;
        private static readonly Policy RetryPolicy = Policy.Handle<SiloUnavailableException>()
            .WaitAndRetryAsync(5, i => TimeSpan.FromSeconds(5));
        public static async Task<IClusterClient> Build(Func<IClientBuilder> builderFunc)
        {
            _builderFunc = builderFunc;
            await RetryPolicy.ExecuteAsync(() =>
            {
                _client?.Dispose();
                _client = builderFunc().Build();
                return _client.Connect();
            });
            return _client;
        }

        readonly object _connectLock = new object();

        public IClusterClient GetClient()
        {
            if (!_client.IsInitialized)
            {
                lock (_connectLock)
                {
                    if (!_client.IsInitialized)
                    {
                        RetryPolicy.ExecuteAsync(() =>
                        {
                            _client?.Dispose();
                            _client = _builderFunc().Build();
                            return _client.Connect();
                        }).GetAwaiter().GetResult();
                    }
                }
            }
            return _client;
        }

    }
}
