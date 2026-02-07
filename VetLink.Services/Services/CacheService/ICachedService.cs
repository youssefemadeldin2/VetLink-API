using System;
using System.Collections.Generic;
using System.Text;

namespace VetLink.Services.Services.CachedService
{
    public interface ICachedService
    {
        Task SetCacheResponseWithSerializAsync(string Key, Object Response, TimeSpan TimeToLive);
        Task<string> GetCacheResponseAsync(string Key);
		Task<long> IncrementCounterAsync(string key, TimeSpan timeToLive);
		Task<TimeSpan?> GetTimeToLiveAsync(string key);
		Task<bool> RemoveCacheAsync(string key);
		Task SetCacheResponseWithNoSerializAsync(string Key, string Response, TimeSpan TimeToLive);
		Task<IEnumerable<string>> GetKeysAsync(string pattern);

	}
}
