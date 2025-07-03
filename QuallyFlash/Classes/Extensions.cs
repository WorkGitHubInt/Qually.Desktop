using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace QuallyFlash
{
    public static class Extensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> col)
        {
            return new ObservableCollection<T>(col);
        }

        public static async Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request, CancellationToken ct)
        {
            using (ct.Register(() => request.Abort(), useSynchronizationContext: false))
            {
                try
                {
                    var response = await request.GetResponseAsync();
                    return (HttpWebResponse)response;
                }
                catch (WebException ex)
                {
                    // WebException is thrown when request.Abort() is called,
                    // but there may be many other reasons,
                    // propagate the WebException to the caller correctly
                    if (ct.IsCancellationRequested)
                    {
                        // the WebException will be available as Exception.InnerException
                        throw new OperationCanceledException(ex.Message, ex, ct);
                    }
                    // cancellation hasn't been requested, rethrow the original WebException
                    throw;
                }
            }
        }
    }
}
