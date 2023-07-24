using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.MassTransit.Shared.Messages
{
    public interface IMessage
    {
        public string Text { get; set; }
    }
}
