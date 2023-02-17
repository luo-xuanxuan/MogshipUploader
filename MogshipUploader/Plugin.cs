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
        public uint[] ReturnTime = new uint[4] { 0, 0, 0, 0 };
        public ushort[] StatusWatch = new ushort[4] { 0, 0, 0, 0 };
        public SubmarineData[] Submarines = new SubmarineData[4];
        public ShipCondition[] ShipConditions = new ShipCondition[4];

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
            ShipConditions[0] = new ShipCondition();
            ShipConditions[1] = new ShipCondition();
            ShipConditions[2] = new ShipCondition();
            ShipConditions[3] = new ShipCondition();

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
            var cts = new CancellationTokenSource();
            cts.CancelAfter(60000); //60s


            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.PostAsync(url, content, cts.Token);
        }

        public unsafe void OnFrameworkUpdate(Framework framework)
        {

            //damage data can be flawed if player swaps parts and sends on voyage and doesnt leave the room until finalizing

            //debug = "";

            //PluginLog.Log(this.MemoryPointer:X8);
            //PluginLog.Log($"Housing Manager: {(nint)HousingManager.Instance():X}");

            var workshopTerritory = (HousingWorkshopTerritory*)HousingManager.Instance()->WorkshopTerritory;
            if (workshopTerritory == null)
            {
                //fixes character swapping holding previous ship conditions
                for (int i = 0; i < 4; i++)
                {
                    ShipConditions[i] = new ShipCondition();
                    ReturnTime[i] = 0;
                }
                return;
            }

            var inventoryManager = InventoryManager.Instance();
            var subContainer = inventoryManager->GetInventoryContainer(InventoryType.HousingInteriorPlacedItems2);
            if (subContainer == null) return;

            for (int i = 0; i < 4; i++)
            {
                if (workshopTerritory->SubmersibleList[i].ReturnTime == 0 && ReturnTime[i] != 0)
                {
                    if (ReturnTime[i] <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                    {
                        VoyageDataPacket data = new VoyageDataPacket(workshopTerritory->SubmersibleList[i]);
                        data.VoyageLog.ReturnTime = ReturnTime[i];

                        ShipCondition newCondition = new ShipCondition(subContainer, i);
                        DamageData damage = new DamageData(workshopTerritory->SubmersibleList[i], ShipConditions[i], newCondition);

                        sendData(new DamageDataPacket(damage).getJSON());

                        sendData(data.getJSON());

                        ShipConditions[i] = newCondition;
                    }
                }

                if(StatusWatch[i] != 2 && workshopTerritory->SubmersibleList[i].Status == 2)
                    ShipConditions[i] = new ShipCondition(subContainer, i);

                StatusWatch[i] = workshopTerritory->SubmersibleList[i].Status;

                ReturnTime[i] = workshopTerritory->SubmersibleList[i].ReturnTime;
            }
        }
    }
}
