using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;

namespace Newbe.Mahua.Greenstal.IGrains
{
    public interface ITestGrain : IGrainWithStringKey
    {
        Task<string> GetId();
    }

    public interface IUser : IGrainWithStringKey
    {
        Task<Result<UserTakeHatResult>> TaskHat();

        Task AddBalance(decimal amount);
    }

    public class UserTakeHatResult
    {
        public decimal Balance { get; set; }
        public decimal HatNowBalance { get; set; }
        public int LeftSeconds { get; set; }
        public string LastUserId { get; set; }
    }

    public interface IGreenHatKeeper : IGrainWithStringKey
    {
        Task<Result<GreenHatKeeperTaskResult>> Take(string userid);
        Task<Result<FinishResult>> Finish();

        [AlwaysInterleave]
        Task<HatInfo> GetHatStatus();
        Task Create(decimal amount);
    }

    public class HatInfo
    {
        public HatStatus HatStatus { get; set; }
        public int LeftSeconds { get; set; }
        public decimal Balance { get; set; }
    }
    public enum HatStatus
    {
        On,
        Finished
    }
    public class FinishResult
    {
        public decimal FinalAmount { get; set; }
        public string LastUserId { get; set; }
        public string[] TopUserIds { get; set; }
    }

    public class GreenHatKeeperTaskResult
    {
        public decimal HatNowBalance { get; set; }
        public int LeftSeconds { get; set; }
        public string LastUserId { get; set; }
    }

    public enum ErrorCode
    {
        Unknow = 0,
        InsufficientBalance = 1,
        HatIsFinished = 2,
    }

    public class Error
    {
        public ErrorCode ErrorCode { get; set; }
        public string[] MessageArgs { get; set; }
    }
    public class Result<TValue>
    {
        public bool IsOk { get; set; }
        public TValue Value { get; set; }
        public Error Error { get; set; }
    }
}
