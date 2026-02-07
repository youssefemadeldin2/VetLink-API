using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Polly.Caching;
using StackExchange.Redis;

namespace VetLink.Services.Services.CachedService
{
    public class CachedService : ICachedService
    {
        private readonly IDatabase _database;
        public CachedService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task<string> GetCacheResponseAsync(string Key)
        {
            var cachedResponse = await _database.StringGetAsync(Key);

			if (!cachedResponse.HasValue || cachedResponse.IsNullOrEmpty)
				return null;

			return cachedResponse;
        }
		public async Task SetCacheResponseWithSerializAsync(string Key, object Response, TimeSpan TimeToLive)
        {
            if (Response is null)
                return;
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(Response, options);
            await _database.StringSetAsync(Key, json, TimeToLive);
        }
		public async Task SetCacheResponseWithNoSerializAsync(string key, string value, TimeSpan ttl)
        {
			if(string.IsNullOrWhiteSpace(value))
				return;

			await _database.StringSetAsync(key, value, ttl);
		}
		public async Task<IEnumerable<string>> GetKeysAsync(string pattern)
		{
			var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints()[0]);
			return server.Keys(pattern: pattern).Select(k => k.ToString());
		}


		public async Task<long> IncrementCounterAsync(string key, TimeSpan timeToLive)
		{
			var count = await _database.StringIncrementAsync(key);
			if (count == 1)
			{
				await _database.KeyExpireAsync(key, timeToLive);
			}

			return count;
		}

		public async Task<TimeSpan?> GetTimeToLiveAsync(string key)
		{
			return await _database.KeyTimeToLiveAsync(key);
		}

		public async Task<bool> RemoveCacheAsync(string key)
		{
			return await _database.KeyDeleteAsync(key);
		}
	}
}
