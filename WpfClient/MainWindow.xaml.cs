﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfClient
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Socket MonSocketClient;
        public Thread MonThread;
        public delegate void dEcrit(string texte);
        public delegate void dEnvoi(string texte);
        FlowDocument flowDocument = new FlowDocument();

        public MainWindow()
        {
            InitializeComponent();
            //Pseudo.Text = Environment.UserName.ToUpper();
            Pseudo.Text = "Marlene";

        }

        

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MonSocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            char separator = Convert.ToChar(".");
            string[] numCpte = Ip.Text.Split(separator);
            if (numCpte.Length !=4)
            {
                //EcritureMessage("Adresse Ip non valide");
                return;
            }
            try
            {
                string ip = Ip.Text;
                IPAddress adresse = IPAddress.Parse(ip);
                IPEndPoint MonEp = new IPEndPoint(adresse, Convert.ToInt32(Port.Text));
                MonSocketClient.Connect(MonEp);
            }
            catch (Exception ex)
            {
                //EcritureMessage(ex.Message);
                
            }
            TraitementConnexion();
        }
        public void TraitementConnexion()
        {
            if (Pseudo.Text !=""&& MonSocketClient !=null)
            {
                Byte[] Msg = System.Text.Encoding.UTF8.GetBytes(Pseudo.Text);
                Ip.IsEnabled = false;
                Port.IsEnabled = false;
                txtbComment.IsEnabled = true;
                Pseudo.IsEnabled = true;
                btnConnect.IsEnabled = false;
                btnDeconnexion.IsEnabled = true;
                btnsend.IsEnabled = true;
                //Byte[] Msg = System.Text.Encoding.UTF8.GetBytes(Pseudo.Text);
                int Envoi = MonSocketClient.Send(Msg);
                MonThread = new Thread(ThreadLecture);
                MonThread.Start();
            }
        }


        public void ThreadLecture()
        {
            while (MonSocketClient.Connected)
            {
                Byte[] Octets = new byte[100000];
                int Recu = 0;
                try
                {
                    Recu = MonSocketClient.Receive(Octets);
                }
                catch (Exception)
                {
                    EcritureMessage("Connexion perdue");
                }
                string Message = System.Text.Encoding.UTF8.GetString(Octets);
                Message = Message.Substring(0, Recu);
                EcritureMessage(Message);
            }
        }
        public void Ecrit(string texte)
        {
            
            Paragraph paragraph = new Paragraph();
            string[] result = texte.Split(':');
            if (result.Count() > 1)
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri("C:/Users/afpa/Downloads/kisspng-logo-tuna-fishing-cartoon-fish-5a8b91ac3e8566.9280837215190962362561.png"));

                Bold commentaire = new Bold(new Run(result[1]));
                Run identite = new Run(" " + result[0] + ": ");
                Image smiley = new Image();
                smiley.Source = bitmapImage;
                smiley.Height = 30;
                smiley.Width = 30;
                paragraph.Inlines.Add(smiley);

                paragraph.Inlines.Add(identite);
                paragraph.Inlines.Add(commentaire);

            }
            else
            {
                paragraph.Inlines.Add(new Run(texte));
            }

            paragraph.Margin = new System.Windows.Thickness { Left = 5, Top = 0, Right = 0, Bottom = 0 };
            if (flowDocument.Blocks.FirstBlock != null)
            {
                flowDocument.Blocks.InsertBefore(flowDocument.Blocks.FirstBlock, paragraph);
            }
            else
            {
                flowDocument.Blocks.Add(paragraph);
            }

            affichage.Document = flowDocument;

        }
        public void EcritureMessage(string texte)
        {
            try
            {

                this.Dispatcher.Invoke(new dEcrit(Ecrit), texte);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void Envoi(string texte)
        {
            if (Pseudo.Text != "" && MonSocketClient !=null)
            {
                Byte[] Octets = Encoding.UTF8.GetBytes(txtbComment.Text);
                int Envoi = 0;
                Envoi = MonSocketClient.Send(Octets);
            }
            txtbComment.Text = "";

        }
       /* public void EnvoiMessage()
        {
            string texte2 = txtbComment.Text;
            try
            {
                this.Dispatcher.Invoke(new dEnvoi(Envoi), texte2);
            }
            catch (Exception)
            {

                throw;
            }
        }*/
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string texte = "Je vais faire caca";
            Byte[] Octets = Encoding.UTF8.GetBytes(texte);
            int Envoi = 0;
            Envoi = MonSocketClient.Send(Octets);
           
            MonSocketClient.Disconnect(false);
            btnConnect.IsEnabled = true;
            Ip.IsEnabled = true;
            Port.IsEnabled = true;

        }

        private void Affichage_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*string texte2 = txtbComment.Text ;
            try
            {
                this.Dispatcher.Invoke(new dEnvoi(EcritureMessage), texte2);
            }
            catch (Exception)
            {

                throw;
            }*/

            if (Pseudo.Text != "" && MonSocketClient != null)
            {
                Byte[] Octets = Encoding.UTF8.GetBytes(txtbComment.Text);
                int Envoi = 0;
                Envoi = MonSocketClient.Send(Octets);
            }
            txtbComment.Text = "";
        }
    }
}
