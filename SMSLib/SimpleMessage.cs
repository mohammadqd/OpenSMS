using System;
using GsmComm.GsmCommunication;
using GsmComm.PduConverter;
using System.Text;

namespace opensms
{
    /// <summary>
    /// Simple Messages (e.g. received SMSs)
    /// </summary>
    public class SimpleMessage : IMessage
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
        /// message body (including UDH)
        /// this is the raw message including some header data in the beginning (usually 5 bytes)
        /// </summary>
        //public string data { get; set; }

        /// <summary>
        /// message body (without UDH)
        /// This is the pure message which should be used        
        /// </summary>
        public string pureMessage { get; set; }

        /// <summary>
        /// is it an ASCI or Unicode SMS
        /// </summary>
        public bool isUnicode { get; set; }

        /// <summary>
        /// User Data Header 
        /// Usually it should be 6
        /// this number of bytes should be removed from the data ti extract the real message
        /// CAUSION: use it only if isCSMS == true
        /// </summary>
        public byte[] UDH { get; set; }

        /// <summary>
        /// This is like an identifier which is the same for multi part SMSs
        /// CAUSION: use it only if isCSMS == true
        /// </summary>
        public byte CSMSRefNo
        {
            get
            {
                if (isCSMS)
                {
                    if (UDH != null && UDH.Length > 3)
                        return UDH[3];
                    else
                        throw new Exception("SimpleMessage Exception: Accessing to CSMSRefNo while UDH is not valid!");
                }
                else
                    throw new Exception("SimpleMessage Exception: Accessing to CSMSRefNo while isCSMS is false!");
            }
        }

        /// <summary>
        /// No. of parts in a multipart SMS
        /// CAUSION: use it only if isCSMS == true
        /// </summary>
        public byte CSMSPartsCount
        {
            get
            {
                if (isCSMS)
                {
                    if (UDH != null && UDH.Length > 4)
                        return UDH[4];
                    else
                        throw new Exception("SimpleMessage Exception: Accessing to CSMSPartsCount while UDH is not valid!");
                }
                else
                    throw new Exception("SimpleMessage Exception: Accessing to CSMSPartsCount while isCSMS is false!");
            }
        }

        /// <summary>
        /// No. of this SMS in a multipart SMS (starts from 1)
        /// CAUSION: use it only if isCSMS == true
        /// </summary>
        public byte CSMSPartNo
        {
            get
            {
                if (isCSMS)
                {
                    if (UDH != null && UDH.Length > 5)
                        return UDH[5];
                    else
                        throw new Exception("SimpleMessage Exception: Accessing to CSMSPartNo while UDH is not valid!");
                }
                else
                    throw new Exception("SimpleMessage Exception: Accessing to CSMSPartNo while isCSMS is false!");
            }
        }

        /// <summary>
        /// True: multipart SMS
        /// False: Single SMS
        /// </summary>
        public bool isCSMS { get; set; }

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

        public bool isComplete
        {
            get
            {
                return true;
            }        
        }

        /// <summary>
        /// Designated Constructor
        /// </summary>
        /// <param name="_time">timestamp of message</param>
        /// <param name="_sender">message sender phone number</param>
        /// <param name="_pureMessage">message body</param>
        protected void Initialize(DateTime _time, string _sender, byte[] _UDH, bool _isUnicode, bool _isCSMS, string _pureMessage)
        {
            time = _time;
            isCSMS = _isCSMS;
            sender = _sender;
            isUnicode = _isUnicode;
            UDH = _UDH;
            pureMessage = _pureMessage;
        }

        /// <summary>
        /// Simple Constructor
        /// </summary>
        /// <param name="_time">timestamp of message</param>
        /// <param name="_sender">message sender phone number</param>
        /// <param name="_pureMessage">message body</param>
        public SimpleMessage(DateTime _time, string _sender, byte[] _UDH, bool _isUnicode, bool _isCSM, string _pureMessage)
        {
            Initialize(_time, _sender, _UDH, _isUnicode, _isCSM, _pureMessage);
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
                bool _isUnicode = (msg.Data.UserDataLength != msg.Data.UserDataText.Length); // check if SMS is unicode
                bool _isCSMS = msg.Data.UserDataHeaderPresent;
                string _pureMessage;
                if (!_isCSMS)
                    _pureMessage = newData.UserDataText;
                else if (_isUnicode)
                    _pureMessage = msg.Data.GetUserDataTextWithoutHeader();
                else
                    _pureMessage = newData.UserDataText.Substring(msg.Data.GetUserDataHeader().Length + 1);
                Initialize(newData.GetTimestamp().ToDateTime(), newData.OriginatingAddress, (_isCSMS) ? msg.Data.GetUserDataHeader() : null, _isUnicode, _isCSMS, _pureMessage);
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

        public IMessage Clone()
        {
            return new SimpleMessage(time, sender, UDH, isUnicode, isCSMS, pureMessage);
        }
    }
}
