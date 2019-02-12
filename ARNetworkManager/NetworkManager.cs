using System;
using DarkRift;
using DarkRift.Server;

namespace ARNetworkManager
{
    public class NetworkManager : Plugin
    {
        public override bool ThreadSafe => false;
        public override Version Version => new Version(1, 0, 0);
        IClient[] imageTargetClient = new IClient[2];

        public NetworkManager(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            ClientManager.ClientConnected += ClientConnected;
            ClientManager.ClientDisconnected += ClientDisconnected;
        }

        void ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            for (int i=0; i<imageTargetClient.Length; i++)
            {
                if (imageTargetClient[i] == null)
                {
                    imageTargetClient[i] = e.Client;
                    imageTargetClient[i].MessageReceived += InteractionMessageReceived;
                    WriteEvent("New Client with id: " + e.Client.ID + " connected!", LogType.Info);
                    break;
                }                    
            }
        }

        void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            for(int i=0; i<imageTargetClient.Length; i++)
            {
                if(imageTargetClient[i] != null)
                {
                    IClient client = imageTargetClient[i];
                    if(client.ID == e.Client.ID)
                    {
                        imageTargetClient[i] = null;
                        break;
                    }
                }
                
            }
        }

        void InteractionMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage())
            using (DarkRiftReader reader = message.GetReader())
            {
                WriteEvent("button: " + reader.ReadString() + " was activated by client: " + e.Client.ID, LogType.Info);
                foreach(IClient client in imageTargetClient)
                {
                    if(client != e.Client)
                    {
                        client.SendMessage(message, SendMode.Reliable);
                    }
                }
            }
        }
    }
}
