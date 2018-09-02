using Newbe.Mahua.Greenstal.IGrains;
using Newbe.Mahua.MahuaEvents;
using Newbe.Mahua.Plugins.Greenstal.Services;

namespace Newbe.Mahua.Plugins.Greenstal.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupMessageReceivedMahuaEvent
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;
        private readonly IClientFactory _clientFactory;

        public GroupMessageReceivedMahuaEvent(
            IMahuaApi mahuaApi,
            IClientFactory clientFactory)
        {
            _mahuaApi = mahuaApi;
            _clientFactory = clientFactory;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            var group = "610394020";
            if (context.FromGroup == group)
            {
                if (context.Message.TrimStart().StartsWith("##"))
                {
                    var clusterClient = _clientFactory.GetClient();
                    var greenHatKeeper = clusterClient.GetGrain<IGreenHatKeeper>("0");
                    var hatStatus = greenHatKeeper.GetHatStatus().GetAwaiter().GetResult();
                    if (hatStatus == HatStatus.On)
                    {
                        var result = clusterClient.GetGrain<IUser>(context.FromQq).TaskHat().GetAwaiter().GetResult();
                        if (result.IsOk)
                        {
                            _mahuaApi.SendGroupMessage(group)
                                .At(context.FromQq)
                                .Text($"成功从{result.Value.LastUserId}手上抢到了原谅帽，现帽价值为{result.Value.HatNowBalance}，用户余额为{result.Value.Balance}，还剩余{result.Value.LeftSeconds}秒")
                                .Done();
                        }
                        else
                        {
                            _mahuaApi.SendGroupMessage(group)
                                .At(context.FromQq)
                                .Text($"因为{result.Error.ErrorCode}，没有没能抢到哟")
                                .Done();
                        }
                    }
                }
            }
        }
    }
}
