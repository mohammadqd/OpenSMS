using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensms
{
    public interface IMessage
    {
        /// <summary>
        /// return a cloned object
        /// </summary>
        /// <returns></returns>
        IMessage Clone();

        /// <summary>
        /// Unique SMS ID
        /// </summary>
        int ID { get; set; }
        
        /// <summary>
        /// message sender phone number
        /// </summary>
        string sender { get; set; }

        /// <summary>
        /// timestamp of message
        /// </summary>
        DateTime time { get; set; }

        /// <summary>
        /// message body (without UDH)
        /// This is the pure message which should be used        
        /// </summary>
        string pureMessage { get; set; }

        /// <summary>
        /// is it an ASCI or Unicode SMS
        /// </summary>
        bool isUnicode { get; set; }

        /// <summary>
        /// is it a complete message (esp. for multi messages)
        /// </summary>
        bool isComplete { get; }

        /// <summary>
        /// User Data Header 
        /// Usually it should be 6
        /// this number of bytes should be removed from the data ti extract the real message
        /// CAUSION: use it only if isCSMS == true
        /// </summary>
        byte[] UDH { get; set; }

        /// <summary>
        /// This is like an identifier which is the same for multi part SMSs
        /// CAUSION: use it only if isCSMS == true
        /// </summary>
        byte CSMSRefNo { get; }

        /// <summary>
        /// No. of parts in a multipart SMS
        /// CAUSION: use it only if isCSMS == true
        /// </summary>
        byte CSMSPartsCount { get; }

        /// <summary>
        /// No. of this SMS in a multipart SMS (starts from 1)
        /// CAUSION: use it only if isCSMS == true
        /// </summary>
        byte CSMSPartNo { get; }

        /// <summary>
        /// True: multipart SMS
        /// False: Single SMS
        /// </summary>
        bool isCSMS { get; set; }

    }
}
