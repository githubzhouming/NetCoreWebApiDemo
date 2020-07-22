using System;
using System.Linq;
using RabbitMQ.Client;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

public class RabbitMQSendTopic
{
    private string key = Guid.NewGuid().ToString("N");
    List<string> result = new List<string>();
    public List<string> Run(RabbitMQInfo info, int maxCount = 100)
    {
        if (maxCount == 0 || maxCount > 100) { maxCount = 100; }
        var factory = new ConnectionFactory()
        {
            HostName = info.hostName,
            Port = info.port,
            UserName = info.userName,
            Password = info.password,
            VirtualHost = info.virtualHost
        };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ConfirmSelect();
            var outstandingConfirms = new ConcurrentDictionary<ulong, string>();
            void cleanOutstandingConfirms(ulong sequenceNumber, bool multiple)
            {
                if (multiple)
                {
                    var confirmed = outstandingConfirms.Where(k => k.Key <= sequenceNumber);
                    foreach (var entry in confirmed)
                    {
                        outstandingConfirms.TryRemove(entry.Key, out _);
                    }
                }
                else
                {
                    outstandingConfirms.TryRemove(sequenceNumber, out _);
                }
            }
            channel.ExchangeDeclare(exchange: info.exchange, type: "topic");

            channel.BasicAcks += (sender, ea) =>
            {
                outstandingConfirms.TryGetValue(ea.DeliveryTag, out string body);
                var msg = $"Sent[{ea.DeliveryTag}]Acks,multiple: {ea.Multiple}:hostName:{info.hostName},exchange:{info.exchange},routingKey:{info.routingKey},message:{body}";
                Console.WriteLine(msg);
                result.Add(msg);
                cleanOutstandingConfirms(ea.DeliveryTag, ea.Multiple);
            };
            channel.BasicNacks += (sender, ea) =>
            {
                outstandingConfirms.TryGetValue(ea.DeliveryTag, out string body);
                var msg = $"Sent[{ea.DeliveryTag}]Nacks,multiple: {ea.Multiple}:hostName:{info.hostName},exchange:{info.exchange},routingKey:{info.routingKey},message:{body}";
                Console.WriteLine(msg);
                result.Add(msg);
                cleanOutstandingConfirms(ea.DeliveryTag, ea.Multiple);
            };
            var cnt = 0;
            while (cnt < maxCount)
            {
                cnt++;
                var msg = info.message + cnt;
                var body = Encoding.UTF8.GetBytes(msg);
                var sequenceNumber = channel.NextPublishSeqNo;
                outstandingConfirms.TryAdd(sequenceNumber, msg);
                channel.BasicPublish(exchange: info.exchange,
                                     routingKey: info.routingKey,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine($"Sent[{sequenceNumber}]:hostName:{info.hostName},exchange:{info.exchange},routingKey:{info.routingKey},message:{msg}");
            }
            while (outstandingConfirms.Count > 0)
            {
                Thread.Sleep(1000);
            }
            Console.WriteLine("Send complete");
            return result;
        }
    }


}