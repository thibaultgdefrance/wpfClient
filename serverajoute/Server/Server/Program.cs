using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Program
    {
        public string port = "6060";
        string txt;
        string texte_connexion;
        bool heure_connexion;
        public static List<Client> ListeClients { get; private set; }
        
        static void Main()
        {
            ServeurSocket();
            Console.WriteLine("Hello World!");
        }
        public static void ServeurSocket()
        {
            Socket MonSocketServeur = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint monEp = new IPEndPoint(IPAddress.Any, 6060);
            MonSocketServeur.Bind(monEp);
            MonSocketServeur.Listen(1000);
            Console.WriteLine("Socket serveur initialisé sur le port 6060");
            ListeClients = new List<Client>();
            while(true)
            {
                Console.WriteLine("en attente de connexion");
                Socket SocketEnvoi = MonSocketServeur.Accept();
                Program prg = new Program();
                prg.TraitementConnexion(SocketEnvoi);
            }
        }
        public void TraitementConnexion(Socket SocketEnvoi)
        {
            Console.WriteLine("Socket client connecté,création d'un thread");
            Client clt = new Client(SocketEnvoi);
            ListeClients.Add(clt);
            Thread ThreadClient = new Thread(clt.TraitementClient);
            ThreadClient.Start();
        }
        
        public void Broadcast(string Message)
        {
            Console.WriteLine("Broadcast: " + Message);
            foreach (var ClientConnecte in ListeClients)
            {
                ClientConnecte.EnvoiMessage(Message);
            }
        }
        public void addClient(Client clt)
        {

        }
        public void removeClient(Client clt)
        {
            ListeClients.Remove(clt);
        }
        
    }

}
