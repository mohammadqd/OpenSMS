using opensms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using GsmComm.PduConverter;
using GsmComm.GsmCommunication;

namespace TestSMS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter to Start ...");
            SMSPort smsPort = new SMSPort("COM4, 19200, 30", "09128448937", true);
            Console.ReadLine();

            //smsPort.SendSMS(false, "Test 3", new String[] { "09128448937" });
            //smsPort.DelSMS(1);
            //smsPort.DelAllSMS();

            // READ SMS Loop
            while (true)
            {
                Console.WriteLine("R E A D  ...");
                foreach (DecodedShortMessage msg in smsPort.ReadSMS())
                {
                    SimpleMessage sms = new SimpleMessage(msg);
                    Console.WriteLine("MSG: Sender:{2} Time:{0} Body:{1}", sms.time, sms.data, sms.sender);
                }
                Console.WriteLine("D E L E T E  ...");
                smsPort.DelAllReadSMS();
                Console.ReadLine();
            }
            Console.WriteLine("Finished Press Enter ...");
            Console.ReadLine();
        }
    }
}
