using Newbe.Mahua.Greenstal.IGrains;
using Newbe.Mahua.MahuaEvents;
using Newbe.Mahua.Plugins.Greenstal.Services;
using System;

namespace Newbe.Mahua.Plugins.Greenstal.MahuaEvents
{
    /// <summary>
    /// 来自好友的私聊消息接收事件
    /// </summary>
    public class PrivateMessageFromFriendReceivedMahuaEvent
        : IPrivateMessageFromFriendReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;
        private readonly IClientFactory _clientFactory;

        public PrivateMessageFromFriendReceivedMahuaEvent(
            IMahuaApi mahuaApi,
            IClientFactory clientFactory)
        {
            _mahuaApi = mahuaApi;
            _clientFactory = clientFactory;
        }

        private static IDisposable timmer;
        public void ProcessFriendMessage(PrivateMessageFromFriendReceivedContext context)
        {
            var group = "610394020";
            if (context.FromQq == "472158246")
            {
                if (context.Message.StartsWith("#"))
                {
                    var clusterClient = _clientFactory.GetClient();
                    var greenHatKeeper = clusterClient.GetGrain<IGreenHatKeeper>("0");
                    var amount = decimal.Parse(context.Message.Substring(1));
                    greenHatKeeper.Create(amount);
                    _mahuaApi.SendGroupMessage("610394020")
                        .Text($"新的原谅帽已经生成，价值：{amount} 原谅水晶")
                        .Done();
                    _mahuaApi.SendGroupMessage(group)
                        .Text($"群主创建了一个原谅帽，价值：{amount} 原谅水晶，发送 ## 来抢吧!");
                }
            }
        }
    }
}
