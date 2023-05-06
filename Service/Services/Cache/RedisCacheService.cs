using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Service.Extensions;
using Service.Services.Cache.Interfaces;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using System.Net.WebSockets;

namespace Service.Services.Cache
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly ConnectionMultiplexer _client;
        private readonly string _conntectionString;

        public RedisCacheService(IConfiguration configuration)
        {
            //TODO: Redisin bağlanacağı url appsettings.json "localhost:6379" olarak ayarlandı.
            _conntectionString = configuration.GetSection("RedisConfiguration:ConnectionString")?.Value;

            ConfigurationOptions options = new ConfigurationOptions
            {
                EndPoints =
                {
                    _conntectionString //Redis'e bağlanılacak olan url.
                },
                AbortOnConnectFail = false,//Redise bağlanamadığı durumda 
                AsyncTimeout = 10000, //Redis'e async isteklerde 10 sn den geç yanıt verirse timeouta düşmesi için
                ConnectTimeout = 10000, //Redis'e normal isteklerde 10 sn den geç yanıt verirse timeouta düşmesi için
                DefaultDatabase = 10
            };

            _client = ConnectionMultiplexer.Connect(options); // Redise bağlanmak için.            
        }

        public void Set(string key, string value)
        {
            _client.GetDatabase().StringSet(key, value);
        }

        public void Set<T>(string key, T value) where T : class
        {
            _client.GetDatabase().StringSet(key, value.ToJson());
        }

        public Task SetAsync(string key, object value)
        {
            return _client.GetDatabase().StringSetAsync(key, value.ToJson());
        }

        public void Set(string key, object value, TimeSpan expiration)
        {
            _client.GetDatabase().StringSet(key, value.ToJson(), expiration);
        }

        public Task SetAsync(string key, object value, TimeSpan expiration)
        {
            return _client.GetDatabase().StringSetAsync(key, value.ToJson(), expiration);
        }

        public void SetAsync<T>(string key, T value, TimeSpan expiration) where T : class
        {
            _client.GetDatabase().StringSet(key, value.ToJson(), expiration);
        }


        public T Get<T>(string key) where T : class
        {
            string value = _client.GetDatabase().StringGet(key);
            return value.ToObject<T>();
        }

        public string Get(string key)
        {
            return _client.GetDatabase().StringGet(key);
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            string value = await _client.GetDatabase().StringGetAsync(key);
            return value.ToObject<T>();
        }

        public void Remove(string key)
        {
            _client.GetDatabase().KeyDelete(key);
        }

        public void RemoveKeysPattern(string keyPrefix)
        {
            var server = _client.GetServer(_conntectionString);
            foreach (var key in server.Keys(pattern: "*"+keyPrefix+"*"))
            {
                Remove(key);
            }
        }
    }
}
