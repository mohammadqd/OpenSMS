using System;
using System.Linq;
using GsmComm.PduConverter;
using GsmComm.GsmCommunication;


namespace opensms
{
    /// <summary>
    /// CEP Outport for sending email
    /// </summary>
    public class SMSPort
    {
        GsmCommMain comm;
        string[] receiversNo;
        public bool isActive { get; set; }
        public string portName { get; set; }
        public int baudRate;
        public int timeout;

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="_COMCONFIG"> com config including COM port, baud rate and timeout e.g. "COM4,19200,30"</param>
        /// <param name="_receiversNo">list of receivers e.g. 09124568798,09134568798 </param>
        /// <param name="_isActive">true: port will be active false: passive</param>
        public SMSPort(string _COMCONFIG, string _receiversNo, bool _isActive)
        {
            Log("INFO", "SMS port constructed");
            Setup(_COMCONFIG, _receiversNo, _isActive);
        }

        /// <summary>
        /// Setup
        /// </summary>
        /// <param name="_COMCONFIG"> com config including COM port, baud rate and timeout e.g. "COM4,19200,30"</param>
        /// <param name="_receiversNo">list of receivers e.g. 09124568798,09134568798 </param>
        /// <param name="_isActive">true: port will be active false: passive</param>
        public void Setup(string _COMCONFIG, string _receiversNo, bool _isActive)
        {
            try
            {
                isActive = _isActive;
                string[] commConfig = _COMCONFIG.Split(',').Select(sValue => sValue.Trim()).ToArray();
                receiversNo = _receiversNo.Split(',').Select(sValue => sValue.Trim()).ToArray();
                if (commConfig.Length != 3 || receiversNo.Length < 1)
                    throw new Exception("Bad Arguments for constructing SMSPort!!");
                portName = commConfig[0];
                baudRate = 19200;
                timeout = 5000;
                int.TryParse(commConfig[1], out baudRate);
                int.TryParse(commConfig[2], out timeout);
                Connect(portName, baudRate, timeout);
                Log("INFO", String.Format("SMS port setuped to send to port: {0} baudRate: {1} timeout: {2} to notify numbers: {3}", _COMCONFIG[0], _COMCONFIG[1], _COMCONFIG[2], _receiversNo));
            }
            catch (Exception ex)
            {
                Log("ERROR", "Exception in setup port: " + ex.Message);
            }

        }
        public void Dispose()
        {
            if (comm != null && comm.IsOpen())
                comm.Close();
        }


        public static void Log(string level, string message)
        {
            Console.WriteLine("[{0}] [SMS] {1}", level,message);
        }

        //public void OnNext(Events.Message value)
        //{
        //    if (value.majorType == (int)MajorType.Event)
        //    {
        //        try
        //        {
        //            if (isActive)
        //            {
        //                string message = value.ToSMSText();
        //                if (message.Length > 70)
        //                    message = message.Substring(0, 70);
        //                if (comm == null || !comm.IsOpen())
        //                    Connect(portName, baudRate, timeout);
        //                SendSMS(true, message, receiversNo);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log("ERROR","Exception in sending SMS: " + ex.Message);
        //            //HITLog.Write("SMSPort@HITCEPLib", "Exception in sending SMS: " + ex.Message
        //            //    , EventLogEntryType.Error, false);
        //        }
        //    }
        //}

        public void Connect(string _portName, int _baudRate, int _timeout)
        {
            try
            {
                if (isActive)
                {
                    string portName = _portName;
                    int baudRate = _baudRate;
                    int timeout = _timeout;
                    comm = new GsmCommMain(portName, baudRate, timeout);
                    comm.Open();
                }
            }
            catch (Exception e)
            {
                Log("ERROR", "Exception in Connecting to GSM Modem: " + e.Message);
            }
        }

        #region SEND SMS
        public void SendSMS(bool unicode, string message, string[] receiversNo)
        {
            try
            {
                if (isActive)
                {
                    if (comm != null && comm.IsOpen())
                    {
                        if (unicode)
                        {
                            byte dcs = (byte)DataCodingScheme.GeneralCoding.Alpha16Bit;
                            SmsSubmitPdu pdu;
                            foreach (var receiverNo in receiversNo)
                            {
                                pdu = new SmsSubmitPdu(message, receiverNo, dcs);
                                comm.SendMessage(pdu);
                            }
                        }
                        else
                        {
                            SmsSubmitPdu pdu;
                            foreach (var receiverNo in receiversNo)
                            {
                                pdu = new SmsSubmitPdu(message, receiverNo);
                                comm.SendMessage(pdu);
                            }
                        }
                    }
                    else
                    {
                        Log("ERROR", "GSM COM port is not open. Cannot Send SMS!");
                    }
                }
            }
            catch (Exception ex)
            {
                Log("ERROR", "Exception in Sending SMS: " + ex.Message);
            }
        }
        #endregion

        #region RECEIVE SMS
        public void ReadSMS()
        {
            string storage = PhoneStorageType.Sim; // ALTERNATIVE: PhoneStorageType.Phone
            try
            {
                // Read all SMS messages from the storage
                DecodedShortMessage[] messages = comm.ReadMessages(PhoneMessageStatus.All, storage);
                foreach (DecodedShortMessage message in messages)
                {
                    if (message.Data.UserDataText == "شلام")
                        Console.WriteLine("OK) Length: {0} Data: {1}", message.Data.UserDataLength, message.Data.UserDataText);
                    else
                        Console.WriteLine("NOK) Length: {0} Data: {1}", message.Data.UserDataLength, message.Data.UserDataText);
                    //Output(string.Format("Message status = {0}, Location = {1}/{2}",
                    //    StatusToString(message.Status), message.Storage, message.Index));
                    //ShowMessage(message.Data);
                    //Output("");
                }
                //Output(string.Format("{0,9} messages read.", messages.Length.ToString()));
                //Output("");
            }
            catch (Exception ex)
            {
                Log("ERROR", "Exception in Reading SMS from GSM Modem: " + ex.Message);
            }
        }

        public void DelSMS(int index)
        {
            string storage = PhoneStorageType.Sim; // ALTERNATIVE: PhoneStorageType.Phone
            try
            {
                // Delete the message with the specified index from storage
                if (index >= 0)
                    comm.DeleteMessage(index, storage);
            }
            catch (Exception ex)
            {
                Log("ERROR", "Exception in Deleting SMS from GSM Modem: " + ex.Message);
            }
        }

        public void DelAllSMS()
        {
            DelAllSMS(DeleteScope.All);
        }

        protected void DelAllSMS(DeleteScope scope)
        {
            string storage = PhoneStorageType.Sim; // ALTERNATIVE: PhoneStorageType.Phone
            try
            {
                // Delete the message with the specified index from storage
                comm.DeleteMessages(DeleteScope.All, storage);
            }
            catch (Exception ex)
            {
                Log("ERROR", "Exception in Deleting SMS from GSM Modem: " + ex.Message);
            }
        }
        #endregion

        #region GENERAL

        #endregion

    }
}
