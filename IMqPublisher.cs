using System;
using System.Collections.Generic;
using System.Text;

namespace MessageQueueProvider
{
    public interface IMqPublisher
    {
        void Publisher<T>(T data,string queueName);
    }
}
