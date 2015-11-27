using System;
using GsmComm.GsmCommunication;
using GsmComm.PduConverter;

namespace opensms
{
    /// <summary>
    /// Simple Messages (e.g. received SMSs)
    /// </summary>
    public class SimpleMessage
    {
        /// <summary>
        /// message sender phone number
        /// </summary>
        public string sender { get; set; }
        /// <summary>
        /// timestamp of message
        /// </summary>
        public DateTime time { get; set; }
        /// <summary>
        /// message body
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// Designated Constructor
        /// </summary>
        /// <param name="_time">timestamp of message</param>
        /// <param name="_sender">message sender phone number</param>
        /// <param name="_data">message body</param>
        protected void Initialize(DateTime _time, string _sender, string _data)
        {
            time = _time;
            sender = _sender;
            data = _data;
        }

        /// <summary>
        /// Simple Constructor
        /// </summary>
        /// <param name="_time">timestamp of message</param>
        /// <param name="_sender">message sender phone number</param>
        /// <param name="_data">message body</param>
        public SimpleMessage(DateTime _time, string _sender, string _data)
        {
            Initialize(_time, _sender, _data);
        }

        /// <summary>
        /// Construct a SimpleMessage from DecodedShortMessage
        /// </summary>
        /// <param name="msg">input message</param>
        public SimpleMessage(DecodedShortMessage msg)
        {
            var originalData = msg.Data;
            if (originalData is SmsDeliverPdu)
            {
                SmsDeliverPdu newData = (SmsDeliverPdu)originalData;
                Initialize(newData.GetTimestamp().ToDateTime(), newData.OriginatingAddress, newData.UserDataText);
            }
        }

        /// <summary>
        /// Online Convert DecodedShortMessage --> SimpleMessage
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static SimpleMessage ToSimpleMessage(DecodedShortMessage msg)
        {
            return new SimpleMessage(msg);
        }
    }
}
