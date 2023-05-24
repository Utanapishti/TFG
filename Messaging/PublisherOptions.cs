using SensorReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging
{
    public class PublisherOptions:ConnectionOptions
    {
        public string Channel{ get; set; } = "TEST";
    }
}
