using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PR2_SMS
{
    class InternalException: Exception
    {
        private string p;

        public InternalException(string p):base(p)
        {
            // TODO: Complete member initialization
            this.p = p;
        }
        
    }
}
