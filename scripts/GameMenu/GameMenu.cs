using Godot;

namespace CardGame.scripts.GameMenu
{
    public class GameMenu : Node2D
    {
        private const int Port = 8080;
        private const int MaxClients = 2;
        private const string Address = "127.0.0.1";

        public override void _Ready()
        {
            GetTree().Connect("network_peer_connected", this, "_connected");
        }

        private void _connected(int clientId)
        {
            Singleton.UserId = clientId;

            var game = GD.Load<PackedScene>("res://scenes/PlaySpace.tscn").Instance();
            GetTree().GetRoot().AddChild(game);
            Hide();
        }

        private void _on_CreateHost_pressed()
        {
            var server = new NetworkedMultiplayerENet();
            server.CreateServer(Port, MaxClients);
            GetTree().SetNetworkPeer(server);
        }

        private void _on_Connect_pressed()
        {
            var client = new NetworkedMultiplayerENet();
            client.CreateClient(Address, Port);
            GetTree().SetNetworkPeer(client);
        }

        private void _on_Back_pressed()
        {
            GetTree().ChangeScene("res://scenes/Control.tscn");
        }
    }
}