
namespace MogshipUploader.Util
{
    public class UserData
    {
        public Plugin? plugin;

        public string Name;
        public string World;

        public UserData(string Name, string World)
        {
            this.Name = Name;
            this.World = World;
        }

        public UserData(Plugin plugin) 
        {
            this.plugin = plugin;
            this.Name = GetLocalPlayerName();
            this.World = GetLocalPlayerWorld();
        }

        private string GetLocalPlayerName()
        {
            return plugin.ClientState.LocalPlayer?.Name.ToString() ?? string.Empty;
        }

        private string GetLocalPlayerWorld()
        {
            return plugin.ClientState.LocalPlayer?.HomeWorld.GameData?.Name.ToString() ?? string.Empty;
        }
    }
}
