﻿using opensms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;


namespace TestSMS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test SMS Starting ...");
            SMSPort smsPort = new SMSPort("COM25, 19200, 30", "09128448937", true);
            //smsPort.SendSMS(false, "Test 3", new String[] { "09128448937" });
            //smsPort.DelSMS(1);
            //smsPort.DelAllSMS();
            smsPort.ReadSMS();
        }
    }
}
