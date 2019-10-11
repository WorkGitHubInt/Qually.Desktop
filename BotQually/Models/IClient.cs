using System.Threading;
using System.Threading.Tasks;

namespace BotQually
{
    public interface IClient
    {
        CancellationToken Ct { get; set; }
        string SID { get; set; }
        string BaseAddress { get; set; }
        void SetSID();
        Task<string> PostAsync(string url, string postData);
        Task<string> GetAsync(string url);
    }
}
