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
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Diagnostics;
using Dalamud.Utility.Signatures;
using System.IO;

namespace MogshipUploader
{

    public sealed class Plugin : IDalamudPlugin
    {
        public ushort[] StatusWatch = new ushort[4] { 0, 0, 0, 0 };
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

        [Signature("48 89 01 48 8B D9 33 C0 48 89 71 18", ScanType = ScanType.StaticAddress)]
        private nint MemoryPointer;

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

            /*
            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            
            WindowSystem.AddWindow(new ConfigWindow(this));
            WindowSystem.AddWindow(new MainWindow(this));

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens main Mogship Window"
            });*/

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            this.Framework.Update += OnFrameworkUpdate;

            

            SignatureHelper.Initialise(this);

            Submarines[0] = new SubmarineData();
            Submarines[1] = new SubmarineData();
            Submarines[2] = new SubmarineData();
            Submarines[3] = new SubmarineData();

        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            //this.CommandManager.RemoveHandler(CommandName);
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
            WindowSystem.GetWindow("Mogship Uploader Config").IsOpen = true;
        }

        public unsafe void sendData(string json)
        {
            //PluginLog.Log(json);
            var client = new HttpClient();
            var url = "https://db.mogship.com/";
            var url2 = "http://127.0.0.1:8000";
            var cts = new CancellationTokenSource();
            cts.CancelAfter(60000); //60s

            File.WriteAllText("C:\\Users\\shade\\Documents\\Python\\MogshipDB\\test.json", json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            

            client.PostAsync(url, content, cts.Token);
        }

        public unsafe void OnFrameworkUpdate(Framework framework)
        {

            //PluginLog.Log($"Housing Manager: {(nint)HousingManager.Instance():X}");

            var workshopTerritory = (HousingWorkshopTerritory*)HousingManager.Instance()->WorkshopTerritory;
            if (workshopTerritory == null) return;

            var inventoryManager = InventoryManager.Instance();
            var subContainer = inventoryManager->GetInventoryContainer(InventoryType.HousingInteriorPlacedItems2);
            if (subContainer == null) return;

            for (int i = 0; i < 4; i++)
            {
                if (StatusWatch[i] != 2 && workshopTerritory->SubmersibleList[i].Status == 2)
                {
                    Submarines[i] = new SubmarineData(workshopTerritory->SubmersibleList[i], subContainer, i);
                    continue;
                }

                if (StatusWatch[i] != 2) continue;
                if (workshopTerritory->SubmersibleList[i].Status != 1) continue;
                if (Submarines[i].ReturnTime > DateTimeOffset.UtcNow.ToUnixTimeSeconds()) continue;
                
                VoyageDataPacket data = new VoyageDataPacket(workshopTerritory->SubmersibleList[i]);
                data.VoyageLog.UpdateSubmarine(Submarines[i]);
                sendData(data.getJSON());

                DamageData damage = new DamageData(workshopTerritory->SubmersibleList[i], Submarines[i].Condition, new ShipCondition(subContainer, i));
                sendData(new DamageDataPacket(damage).getJSON());
            }

            StatusWatch[0] = workshopTerritory->SubmersibleList[0].Status;
            StatusWatch[1] = workshopTerritory->SubmersibleList[1].Status;
            StatusWatch[2] = workshopTerritory->SubmersibleList[2].Status;
            StatusWatch[3] = workshopTerritory->SubmersibleList[3].Status;
        }
    }
}
