using System.Threading;
using System.Threading.Tasks;

namespace QuallyFlash
{
    public interface IClient
    {
        CancellationToken Ct { get; set; }
        string SID { get; set; }
        Account Account { get; set; }
        void SetSID();
        Task<string> PostAsync(string url, string postData);
        Task<string> GetAsync(string url);
    }
}
