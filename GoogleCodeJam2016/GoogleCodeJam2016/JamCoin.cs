using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCodeJam2016.JamCoin
{
    public class JamCoinValidator
    {
        public bool Validate(string coin, Dictionary<int, int> divisors)
        {
            foreach (var key in divisors.Keys)
            {
                var numberInNewBase = Convert.ToInt32(coin, key);

                
            }
        }

    }
}
