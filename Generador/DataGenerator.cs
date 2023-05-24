using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockedSensor
{
    public class DataGenerator
    {
        double[] _values;        
        int _currentIndex = -1;        


        public DataGenerator(double[] values) 
        {
            _values = values;
        }

        public double GetValue()
        {
            lock (_values)
            {
                _currentIndex++;
                if (_currentIndex >= _values.Length)
                    _currentIndex = 0;
                return _values[_currentIndex];
            }
        }
    }
}
