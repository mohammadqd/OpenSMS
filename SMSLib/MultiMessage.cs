using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensms
{
    class MultiMessage : IMessage
    {
        public IMessage[] messages;

        public byte CSMSPartNo
        {
            get
            {
                return 1;
            }
        }

        public byte CSMSPartsCount { get; set; }

        public byte CSMSRefNo { get; set; }

        public bool isCSMS
        {
            get
            {
                return true;
            }
            set
            {
                throw new NotImplementedException("Cannot set isCSMS in MultiMessage directly!");
            }
        }

        public bool isUnicode { get; set; }

        public string pureMessage
        {
            get
            {
                if (isComplete)
                {
                    string concatenatedPureMessage = "";
                    for (int i = 0; i < messages.Length; i++)
                        concatenatedPureMessage += messages[i];
                    return concatenatedPureMessage;
                }
                else
                    throw new MessageConcatenationException("Trying to get pureMessage of an incomplete MultiMessage");

            }
            set
            {
                throw new NotImplementedException("Cannot set pureMessage in MultiMessage directly!");
            }
        }

        public string sender { get; set; }

        public DateTime time { get; set; }

        public byte[] UDH { get; set; }

        /// <summary>
        /// True: MultiMessage is initialized by the first message Fasle: not initialized
        /// </summary>
        public bool isInitialized { get; set; }
        /// <summary>
        /// True: all message parts are included 
        /// </summary>
        public bool isComplete
        {
            get
            {
                if (isInitialized)
                {
                    bool completeness = true;
                    for (int i = 0; i < messages.Length; i++)
                        if (messages[i] == null)
                            completeness = false;
                    return completeness;
                }
                else
                    return false;
            }
        }

        public int ID
        {
            get
            {
                return CSMSRefNo;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        protected void Initialize(IMessage _message)
        {
            // first initialization
            messages = new IMessage[_message.CSMSPartsCount];
            messages[_message.CSMSPartNo - 1] = _message;
            CSMSPartsCount = _message.CSMSPartsCount;
            CSMSRefNo = _message.CSMSRefNo;
            isUnicode = _message.isUnicode;
            sender = _message.sender;
            time = _message.time;
            UDH = _message.UDH;
            isInitialized = true;
        }

        /// <summary>
        /// To insert a message (simple message) into this multi message
        /// </summary>
        /// <param name="msg"></param>
        public void InsertMessage(IMessage msg)
        {
            if (isInitialized)
            {
                if (msg.CSMSRefNo != CSMSRefNo
                    || msg.CSMSPartsCount != CSMSPartsCount
                    || msg.sender != sender
                    || msg.isUnicode != isUnicode)
                    throw new MessageConcatenationException("Conflict in inserting SimpleMessage into a MultiMessage");
                else
                    messages[msg.CSMSPartNo - 1] = msg;
            }
            else
            {
                throw new MessageConcatenationException("Insert into a MultiMessage before initialization!");
            }
        }

        public MultiMessage()
        {
            isInitialized = false;
        }

        public MultiMessage(IMessage[] _messages)
            :this()
        {
            if (_messages != null && _messages.Length > 0)
            {
                bool needInit = true;
                foreach (IMessage msg in _messages)
                {
                    if (msg != null)
                    {
                        if (needInit)
                        {
                            Initialize(msg);
                            needInit = false;
                        }
                        else
                        {
                            InsertMessage(msg);
                        }
                    }
                }
            }
        }

        public IMessage Clone()
        {
            return new MultiMessage(messages);
        }
    }
}
