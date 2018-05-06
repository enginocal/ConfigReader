using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigReader
{
    public class ConfigurationReader : IConfigReader, IDisposable
    {
        private const string queueConst = "myqueue";
        private readonly string _applicationName;
        private static object _lock = new object();
        private readonly IConfigRepository _connectionString;
        private static ConcurrentDictionary<string, Config> _values;
        private static Timer _timer;

        private ConfigurationReader()
        {

        }
        public ConfigurationReader(string applicationName, IConfigRepository connectionString, int refreshTimerIntervalInMs)
        {
            _connectionString = connectionString;
            _applicationName = applicationName;
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentException($"{applicationName} must be filled.");
            }
            _values = GetConfigValues();

            var factory = new ConnectionFactory() { DispatchConsumersAsync = true };
            string queueName = queueConst + applicationName;

            SubscribeQueue(factory, queueName);

            _timer = new Timer(x =>
            {
                RefreshConfig();
            }, null, refreshTimerIntervalInMs, refreshTimerIntervalInMs);


        }

        private static void SubscribeQueue(ConnectionFactory factory, string queueName)
        {
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += Consumer_Received;
                channel.BasicConsume(queueName, true, consumer);
            }
        }

        internal void RefreshConfig()
        {
            Monitor.Enter(_lock);
            ConcurrentDictionary<string, Config> values = GetConfigValues();

            foreach (KeyValuePair<string, Config> configItem in values)
            {
                Config currentConfig;
                if (_values.TryGetValue(configItem.Key, out currentConfig))
                {
                    _values[configItem.Key] = configItem.Value;
                }
                else
                {
                    _values.TryAdd(configItem.Key, configItem.Value);
                }
            }
            Monitor.Exit(_lock);
        }

        private ConcurrentDictionary<string, Config> GetConfigValues()
        {
            ConcurrentDictionary<string, Config> values = new ConcurrentDictionary<string, Config>();
            List<Config> configValues = _connectionString.GetValues(_applicationName).ToList();

            foreach (Config config in configValues)
            {
                values.TryAdd(config.Name, config);
            }

            return values;
        }

        public T GetValue<T>(string key)
        {
            Config config;
            if (_values.TryGetValue(key, out config))
            {
                return (T)Convert.ChangeType(config.Value, typeof(T));
            }

            throw new Exception("An error occured.");
        }

        private static async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            var message = Encoding.UTF8.GetString(@event.Body);

            Console.WriteLine($"Begin processing {message}");

            await Task.Delay(250);
            ConfigurationReader reader = new ConfigurationReader();
            reader.RefreshConfig();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _values = null;
                }
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
