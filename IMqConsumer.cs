using System;
using System.Collections.Generic;
using System.Text;

namespace MessageQueueProvider
{
    public interface IMqConsumer
    {
        T Consumer<T>(string queueName);
    }
}
