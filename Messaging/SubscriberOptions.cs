using SensorReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging
{
    public class SubscriberOptions:ConnectionOptions
    {
        public IEnumerable<string> Subscriptions { get; set; } = Enumerable.Empty<string>();
    }
}
