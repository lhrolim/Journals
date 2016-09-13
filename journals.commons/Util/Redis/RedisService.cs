using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using journals.commons.Model.Entities;
using journals.commons.SimpleInjector;
using log4net;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace journals.commons.Util.Redis {

    public class RedisService : ISingletonComponent {

        private readonly ILog _log = LogManager.GetLogger(typeof(RedisService));

        private readonly NewtonsoftSerializer _serializer;

        public bool ServiceAvailable {get;set;}

        public ConnectionMultiplexer Multiplexer {
            get; set;
        }

        public RedisService() {
            _serializer = new NewtonsoftSerializer();
            try {
                Multiplexer = ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["redisurl"]);
                ServiceAvailable = true;
            } catch (Exception e) {
                ServiceAvailable = false;
                _log.WarnFormat("Redis is not available for pub/sub operations");
            }

        }

        public async Task PublishNewSubscription(Journal journal, string username) {
            await GetSubscriber().PublishAsync("subscriptions/" + username, _serializer.Serialize(journal));
        }

        public async Task SubscriptionAdded(string username, Action<RedisChannel, RedisValue> action) {
            await GetSubscriber().SubscribeAsync("subscriptions/" + username, action);
        }


        public async Task PublishNewUnSubscription(int journalId, string username) {
            await GetSubscriber().PublishAsync("unsubscriptions/" + username, journalId);
        }

        public async Task SubscriptionRemoved(string username, Action<RedisChannel, RedisValue> action) {
            await GetSubscriber().SubscribeAsync("unsubscriptions/" + username, action);
        }


        public async Task PublishIntoJournal(Publication publication, int journalId) {
            await GetSubscriber().PublishAsync("publications/" + journalId, _serializer.Serialize(publication));
        }

        public async Task SubscribeToJournal(int journalid, Action<RedisChannel, RedisValue> action) {
            await GetSubscriber().SubscribeAsync("publications/" + journalid, action);
        }

        public async Task UnSubscribeToJournal(int journalid) {
            await GetSubscriber().UnsubscribeAsync("publications/" + journalid);
        }


        public ISubscriber GetSubscriber() {
            if (Multiplexer != null) {
                return Multiplexer.GetSubscriber();
            }
            return new NullSubscriber();
        }

        #region nullSubscriber
        //doing nothing if Subscriber is not available
        internal class NullSubscriber : ISubscriber {
            public Task PingAsync(CommandFlags flags = CommandFlags.None) {
                return default(Task);
            }

            public ConnectionMultiplexer Multiplexer {
                get;
            }
            public TimeSpan Ping(CommandFlags flags = CommandFlags.None) {
                return default(TimeSpan);
            }

            public T Wait<T>(Task<T> task) {
                return default(T);
            }

            public void WaitAll(params Task[] tasks) {
            }

            public T Wait<T>(Task task) {
                return default(T);
            }

            public void Wait(Task task) {
            }

            Task<TimeSpan> IRedisAsync.PingAsync(CommandFlags flags) {
                return null;
            }

            public bool TryWait(Task task) {
                return false;
            }

            public EndPoint IdentifyEndpoint(RedisChannel channel, CommandFlags flags = CommandFlags.None) {
                return default(EndPoint);
            }

            Task<EndPoint> ISubscriber.IdentifyEndpointAsync(RedisChannel channel, CommandFlags flags) {
                return null;
            }

            public Task IdentifyEndpointAsync(RedisChannel channel, CommandFlags flags = CommandFlags.None) {
                return default(Task);
            }

            public bool IsConnected(RedisChannel channel = new RedisChannel()) {
                return false;
            }

            public long Publish(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None) {
                return 0;
            }

            Task<long> ISubscriber.PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags) {
                return null;
            }

            public void Subscribe(RedisChannel channel, Action<RedisChannel, RedisValue> handler, CommandFlags flags = CommandFlags.None) {
            }

            public Task SubscribeAsync(RedisChannel channel, Action<RedisChannel, RedisValue> handler, CommandFlags flags = CommandFlags.None) {
                return null;
            }

            public Task PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None) {
                return default(Task);
            }

            public EndPoint SubscribedEndpoint(RedisChannel channel) {
                return default(EndPoint);
            }

            public void Unsubscribe(RedisChannel channel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None) {
            }

            public void UnsubscribeAll(CommandFlags flags = CommandFlags.None) {
            }

            public Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None) {
                return default(Task);
            }

            public Task UnsubscribeAsync(RedisChannel channel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None) {
                return null;
            }

            public Task UnsubscribeAsync(RedisChannel channel, Action handler = null, CommandFlags flags = CommandFlags.None) {
                return default(Task);
            }

            public void Unsubscribe(RedisChannel channel, Action handler = null, CommandFlags flags = CommandFlags.None) {
            }

            public Task SubscribeAsync(RedisChannel channel, Action handler, CommandFlags flags = CommandFlags.None) {
                return default(Task);
            }

            public void Subscribe(RedisChannel channel, Action handler, CommandFlags flags = CommandFlags.None) {
            }
        }
        #endregion

    }
}
