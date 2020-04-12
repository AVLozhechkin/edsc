using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace EDSc.Common.MessageBroker
{
    public abstract class RmqBuilder<T> where T : class
    {
        protected string userName = "guest";
        protected string password = "guest";
        protected string hostName = "localhost";
        protected int port = 5672;
        protected string virtualHost = "/";

        protected IConnectionFactory connectionFactory;
        protected IConnection connection;
        protected IModel amqpChannel;
        public RmqBuilder<T> UsingDefaultConnectionSetting()
        {
            return this;
        }

        public RmqBuilder<T> UsingConfigConnectionSettings(IConfigurationSection configurationSection)
        {
            this.userName = configurationSection.GetSection("UserName").Value;
            this.password = configurationSection.GetSection("Password").Value;
            this.hostName = configurationSection.GetSection("HostName").Value;
            this.port = int.Parse(configurationSection.GetSection("Port").Value);
            this.virtualHost = configurationSection.GetSection("VirtualHost").Value;
            return this;
        }

        public RmqBuilder<T> UsingCustomHost(string hostName)
        {
            this.hostName = hostName;
            return this;
        }

        public RmqBuilder<T> UsingCustomCredentials(string userName, string userPassword)
        {
            this.userName = userName;
            this.password = userPassword;
            return this;
        }

        public abstract T Build();
    }
}
