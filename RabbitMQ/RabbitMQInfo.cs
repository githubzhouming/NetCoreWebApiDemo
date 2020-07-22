using System;

public class RabbitMQInfo
{
    public string hostName { get; set; }
    public int port { get; set; }
    public string userName { get; set; }
    public string password { get; set; }
    public string virtualHost { get; set; }

    public string exchange { get; set; }
    public string queueName { get; set; }
    public string routingKey { get; set; }
    public string message { get; set; }
    public string bindingKey { get; set; }
 }