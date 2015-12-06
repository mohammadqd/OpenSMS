using System.Collections.Generic;
using System.Linq;

namespace opensms
{
    class MessageStorage
    {
        protected Dictionary<int, IMessage> storage;

        public MessageStorage()
        {
            storage = new Dictionary<int, IMessage>(256);
        }

        /// <summary>
        /// To insert a new message (simple or multi) to storage
        /// It could be also a part of an incomplete MultiMessage
        /// </summary>
        /// <param name="msg">IMessage to insert</param>
        /// <returns>If Conflict: new message will be replaced and returns old one No Conflict: null</returns>
        public IMessage InsertMessage(IMessage msg)
        {
            if (storage.Keys.Contains(msg.ID))
            {
                IMessage oldElement = storage[msg.ID];
                if (oldElement is MultiMessage && !((MultiMessage)oldElement).isComplete) // incomplete MultiMessage 
                {
                    ((MultiMessage)oldElement).InsertMessage(msg); // try to complete it using this new part
                    return null;
                }
                else // conflict
                {
                    SMSPort.Log("WARNING", "SMS Conflict in MessageStorage");
                    IMessage oldMsg = oldElement.Clone();
                    storage[msg.ID] = msg;
                    return oldMsg;
                }
            }
            else // new element
            {
                storage[msg.ID] = msg;
                return null;
            }
        }

        /// <summary>
        /// To fetch a message and remove it from storage (optional)
        /// </summary>
        /// <param name="_remove">True: remove it after fetch False: no removal</param>
        /// <returns>fetched message (null if no message)</returns>
        public IMessage FetchMessage(bool _remove)
        {
            IMessage result;
            foreach (var element in storage)
            {
                if (element.Value.isComplete)
                {
                    result = element.Value;
                    if (_remove)
                        storage.Remove(element.Key);
                    return result;
                }
            }
            return null;
        }
    }
}
