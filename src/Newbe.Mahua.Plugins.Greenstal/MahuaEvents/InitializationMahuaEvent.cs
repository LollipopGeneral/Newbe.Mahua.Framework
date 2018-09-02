using Newbe.Mahua.Logging;
using Newbe.Mahua.MahuaEvents;
using Newbe.Mahua.Plugins.Greenstal.Services;
using NLog.Extensions.Logging;
using Orleans;

namespace Newbe.Mahua.Plugins.Greenstal.MahuaEvents
{
    /// <summary>
    /// 插件初始化事件
    /// </summary>
    public class InitializationMahuaEvent
        : IInitializationMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;
        private readonly ILog _logger = LogProvider.For<InitializationMahuaEvent>();
        public InitializationMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void Initialized(InitializedContext context)
        {
            ClientFactory.Build(() =>
            {
                var builder = new ClientBuilder()
                    .UseLocalhostClustering()
                    .ConfigureLogging(b => b.AddNLog());
                return builder;
            }).GetAwaiter().GetResult();

            _logger.Info("Client successfully connect to silo host");
        }
    }
}
