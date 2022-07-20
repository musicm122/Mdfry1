using Godot;
using Mdfry1.Entities.Behaviors.Interfaces;
using Mdfry1.Scripts.GDUtils;
using Mdfry1.Scripts.Mission;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.UI;

namespace Mdfry1.Entities.Behaviors
{
    public class UiBehavior : Node2D, IDebuggable<Node>, IUiBehavior
    {
        public PlayerDataStore DataStore { get; set; }

        public PauseMenu PauseMenu { get; set; }

        private Hud Hud { get; set; }

        [Export]
        public bool IsDebugging { get; set; } = false;

        public bool IsDebugPrintEnabled() => IsDebugging;

        public override void _Ready()
        {
            Hud = GetNode<Hud>("./Camera2D/CanvasLayer/Hud");
            PauseMenu = GetNode<PauseMenu>("./CanvasLayer/PauseMenu");
        }

        private void UpdateMissions(object sender, MissionManagerEventArgs args)
        {
            RefreshUI();
        }

        public void AddMission(string title)
        {
            var mission = MasterMissionList.GetMissionByTitle(title);
            if (mission != null)
            {
                this.Print($"AddMission called with mission title: {title}");

                DataStore.MissionManager.AddIfDNE(mission);
                RefreshUI();
            }
        }

        public void RemoveMission(MissionElement mission)
        {
            this.Print($"RemoveMission called with mission : {mission}");
            DataStore.MissionManager.Remove(mission);
            RefreshUI();
        }

        public void RemoveMissionByTitle(string missionTitle)
        {
            this.Print($"RemoveMission called with mission : {missionTitle}");
            DataStore.MissionManager.RemoveByTitle(missionTitle);
            RefreshUI();
        }

        public void EvaluateMissions(PlayerDataStore playerDataStore)
        {
            DataStore.MissionManager.EvaluateMissionsState(playerDataStore);
            RefreshUI();
        }

        public void AddItem(string name, int amt = 1)
        {
            this.Print($"AddItem called with name:{name}, amt:{amt} ");

            DataStore.Inventory.Add(name, amt);
            RefreshUI();
        }

        public void RemoveItem(string name)
        {
            this.Print($"RemoveItem called with name:{name}");
            DataStore.Inventory.Remove(name);
            RefreshUI();
        }

        public void RemoveItems(string name, int amt)
        {
            this.Print($"RemoveItem called with name:{name}");
            DataStore.Inventory.RemoveAmount(name, amt);
            RefreshUI();
        }

        public override void _Process(float delta)
        {
            if (DataStore.PlayerStatus.IsDead())
            {
                PauseMenu.IsPauseOptionEnabled = false;
                if (InputUtils.IsAnyKeyPressed())
                {
                    this.Print("Reloading Scene");
                    GetTree().ReloadCurrentScene();
                }
            }
        }

        public void RefreshUI()
        {
            Hud.RefreshUI(DataStore);
            PauseMenu.RefreshUI(DataStore.Inventory, DataStore.MissionManager);
        }

        public void Init(PlayerDataStore dataStore)
        {
            DataStore = dataStore;

            DataStore.PlayerStatus.MaxHealthChangedCallback += (amt) => RefreshUI();
            DataStore.PlayerStatus.HealthChangedCallback += (amt) => RefreshUI();
            DataStore.PlayerStatus.EmptyHealthBarCallback += () => this.Print("Player Dead");

            DataStore.MissionManager.AddMissionEvent += UpdateMissions;
            DataStore.MissionManager.RemoveMissionEvent += UpdateMissions;
            RefreshUI();
        }
    }
}