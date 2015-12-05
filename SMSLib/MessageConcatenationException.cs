using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensms
{
    class MessageConcatenationException: Exception
    {
        public MessageConcatenationException(string message)
            :base(message)
        {
        }
    }
}
