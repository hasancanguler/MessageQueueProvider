using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageQueueProvider
{
    public class RabbitService : IMqConsumer,IMqPublisher
    {
        private readonly ConnectionFactory connectionFactory;
        private static readonly object lockconsume = new object();

        public RabbitService(string hostName, string userName, string password, int port)
        {
            connectionFactory = new ConnectionFactory()
            {
                HostName = hostName,
                UserName = userName,
                Password = password,
                Port = port
            };
        }

        public T Consumer<T>(string queueName)
        {
            T data = default(T);
            string message = "";
            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.BasicQos(0, 1, false);

                    var consumer = new EventingBasicConsumer(channel);
                   
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        message = Encoding.UTF8.GetString(body);                        
                    };
                    channel.BasicConsume(queueName, false, consumer);
                }

            }
            data = JsonConvert.DeserializeObject<T>(message);
            return data;
        }

        public void Publisher<T>(T data,string queueName)
        {
            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queueName, false, false, false, null);

                    var message = JsonConvert.SerializeObject(data);
                    channel.BasicPublish("", queueName, null, Encoding.UTF8.GetBytes(message));
                }
            }
        }
    }
}
