﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorReader
{
    public class ConnectionOptions
    {        
        public string Host { get; set; }
        public int Port { get; set; }
        public string Exchange { get; set; } = "SENSOR_NET";
    }
}