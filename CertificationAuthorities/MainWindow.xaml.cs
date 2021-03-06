﻿using CertificationAuthorities.Util;
using CertificationAuthorities.Models;
using System;
using System.Windows;

namespace CertificationAuthorities
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string IP = "127.0.0.1";
        private static int HOST = 12000;
        private ServerObject server;
        private CertifacteWindow cw;
        public MainWindow()
        {
            instance = this;
            cw = new CertifacteWindow();
            cw.Activate();
            
            InitializeComponent();
            createServer();
        }

        private void createServer()
        {
        

            server = new ServerObject(IP, HOST);

            server.logger = Log;
            server.intiServer();
            server.startServer();

            server.respose = (s) =>
            {

                string requstType = s.ReadString();
                RequestsManager.ProcessRequst(requstType, s, this);

            };
        }
        public  bool CreateCertificatewindows(DigitalCertificate dc)
        {
            
            return (cw.showCertificate(dc));

        }
        public static MainWindow instance;

        public void Log()
        {
            Log("---------------------------------------------------------------------------------------------------\n");
        }

        public void Log(string logMessage)
        {
            Application.Current.Dispatcher.Invoke((Action)(() => { logTextBox.Text += logMessage + "\n\n"; }));
        }


        public void Log(string messageDescription, string logMessage)
        {
            string format = "{0}:\n{1}\n\n";
            Application.Current.Dispatcher.Invoke((Action)(() => { logTextBox.Text += string.Format(format, messageDescription, logMessage); }));
        }
    }
}
