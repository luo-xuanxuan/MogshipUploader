using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace MogshipUploader.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    public ConfigWindow(Plugin plugin) : base(
        "Mogship Uploader Config", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {

        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 100),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        this.Size = new Vector2(375, 100);
        //this.SizeCondition = ImGuiCond.Always;

        this.Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {

        ImGui.Text("HTTP Endpoint:");
        var target = this.Configuration.DataTarget;
        ImGui.InputText(string.Empty, ref Configuration.DataTarget,200);
        if (ImGui.IsItemDeactivatedAfterEdit())
        {
            this.Configuration.DataTarget = target;
            this.Configuration.Save();
        }
    }
}
