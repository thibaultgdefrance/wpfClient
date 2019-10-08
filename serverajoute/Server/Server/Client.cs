using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class Client
    {
        public Socket _SocketClient;
        public string _pseudo;
        public Client(Socket sock)
        {
            _SocketClient = sock;
        }
        public void TraitementClient()
        {
            Console.WriteLine("Thread client lancé");
            byte[] Octets = new byte[100000];
            int Recu;
            try
            {
                Recu = _SocketClient.Receive(Octets);
            }
            catch (Exception)
            {
                Console.WriteLine("erreure pendant la réception du pseudo");
                return;
            }
            string Message = System.Text.Encoding.UTF8.GetString(Octets);
            Message = Message.Substring(0,Recu);
            if (Recu>9)
            {
                _pseudo = Message.Substring(0, 9);
            }
            else
            {
                _pseudo = Message.Substring(0, Recu);
            }
            Program prg = new Program();
            prg.Broadcast(_pseudo + "identifié sur le reseau");
            while (_SocketClient.Connected)
            {
                try
                {
                    string Msg;
                    Recu = _SocketClient.Receive(Octets);
                 
                    if (Recu>0)
                    {
                        Msg = System.Text.Encoding.UTF8.GetString(Octets);
                        Msg = Msg.Substring(0, Recu);
                        prg.Broadcast(_pseudo + "a dit:" + Msg);

                    }
                    else
                    {
                        Client clt2 = new Client(_SocketClient);
                        prg.removeClient(clt2);
                        _SocketClient.Close();
                        prg.Broadcast(_pseudo + "déconnecté");
                    }
                }
                catch (Exception)
                {
                    Client clt2 = new Client(_SocketClient);
                    prg.removeClient(clt2);
                    _SocketClient.Close();
                    prg.Broadcast(_pseudo + "déconnecté");
                    throw;
                }
            }
            if (_SocketClient.Connected==false)
            {
                Client clt2 = new Client(_SocketClient);
                prg.removeClient(clt2);
                _SocketClient.Close();
                prg.Broadcast(_pseudo + "déconnecté");
                return;
            }
        }
        public void EnvoiMessage(string Message)
        {
            try
            {
                Byte[] Msg = System.Text.Encoding.UTF8.GetBytes(Message);
                int Envoi = _SocketClient.Send(Msg);
                Console.WriteLine(Envoi + "octets envoyés au client" + _pseudo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Message non remis" + ex.Message);
                _SocketClient.Close();
                Client clt = new Client(_SocketClient);
                Program prg = new Program();
                prg.removeClient(clt);

            }
        }


    }
}
