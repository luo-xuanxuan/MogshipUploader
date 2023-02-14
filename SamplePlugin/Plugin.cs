using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using MogshipUploader.Windows;
using Dalamud.Game;
using System.Text;
using FFXIVClientStructs.FFXIV.Client.Game.Housing;
using System;
using Dalamud.Logging;
using MogshipUploader.Util;
using Dalamud.Game.ClientState;
using System.Net.Http;
using System.Threading;

namespace MogshipUploader
{

    public sealed class Plugin : IDalamudPlugin
    {
        public int[] ReturnTime = new int[4] { 0, 0, 0, 0 };
        public SubmarineData[] Submarines = new SubmarineData[4];

        public string debug = "";

        public string Name => "Mogship Uploader";
        private const string CommandName = "/mogship";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        private Framework Framework;
        public ClientState ClientState { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("MogshipUploader");

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager,
            [RequiredVersion("1.0")] Framework framework,
            [RequiredVersion("1.0")] ClientState clientState)

        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.Framework = framework;
            this.ClientState = clientState;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            WindowSystem.AddWindow(new ConfigWindow(this));
            WindowSystem.AddWindow(new MainWindow(this));

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens main Mogship Window"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            this.Framework.Update += OnFrameworkUpdate;

            Submarines[0] = new SubmarineData();
            Submarines[1] = new SubmarineData();
            Submarines[2] = new SubmarineData();
            Submarines[3] = new SubmarineData();

        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            this.CommandManager.RemoveHandler(CommandName);
            this.Framework.Update -= OnFrameworkUpdate;
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            WindowSystem.GetWindow("My Amazing Window").IsOpen = true;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            WindowSystem.GetWindow("A Wonderful Configuration Window").IsOpen = true;
        }

        public unsafe void sendData(string json)
        {
            PluginLog.Log(json);
            var client = new HttpClient();
            var url = "http://127.0.0.1:5000/";
            var cts = new CancellationTokenSource();
            cts.CancelAfter(60000); //60s

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.PostAsync(url, content, cts.Token);
        }

        public unsafe void OnFrameworkUpdate(Framework framework)
        {

            var workshopTerritory = (HousingWorkshopTerritory*)HousingManager.Instance()->WorkshopTerritory;
            if (workshopTerritory == null) return;

            for (int i = 0; i < 4; i++)
            {
                if (workshopTerritory->SubmersibleList[i].ReturnTime == 0 && ReturnTime[i] != 0)
                {
                    if (ReturnTime[i] <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                    {
                        VoyageDataPacket data = new VoyageDataPacket(workshopTerritory->SubmersibleList[i]);
                        data.VoyageLog.ReturnTime = ReturnTime[i];
                        sendData(data.getJSON());
                    }
                }
                ReturnTime[i] = workshopTerritory->SubmersibleList[i].ReturnTime;
            }
        }
    }
}
