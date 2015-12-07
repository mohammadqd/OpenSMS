using GsmComm.GsmCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensms
{
    /// <summary>
    /// Factory to create SimpleMessage or MultiMessage based on DecodedShortMessage 
    /// </summary>
    public class MessageFactory
    {
        public static IMessage CreateMessage(DecodedShortMessage msg)
        {
            SimpleMessage simpleMessage = new SimpleMessage(msg);
            if (!simpleMessage.isCSMS)
            {
                return simpleMessage;
            }
            else
            {
                MultiMessage multiMessage = new MultiMessage(new IMessage[] { simpleMessage });
                return multiMessage;
            }
        }
    }
}
