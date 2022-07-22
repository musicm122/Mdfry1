using Godot;

namespace Mdfry1.Scenes.Level
{

    public class DayLabel : Label
    {
        [Export]
        public NodePath DayNightCyclePath = new NodePath("/root/DayNightCycle");
        public DayNightCycle DayNightCycle { get; set; }
        private string DayMessage { get; set; }

        public override void _Ready()
        {
            DayNightCycle = GetNode<DayNightCycle>(DayNightCyclePath);
            DayNightCycle.DayTick += UpdateLabel;
        }

        public void UpdateLabel(int day)
        {
            DayMessage = $"Day {day.ToString()}\r\n";
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            Text = $"{DayMessage}Total Time : {DayNightCycle.TotalTime.ToString()}";
        }
    }
}