using Godot;

namespace Mdfry1.Scenes.Level
{

    public class DayLabel : Label
    {
        [Export]
        public NodePath DayNightCyclePath = new NodePath("/root/DayNightCycle");
        public DayNightCycle DayNightCycle { get; set; }

        public override void _Ready()
        {
            DayNightCycle = GetNode<DayNightCycle>(DayNightCyclePath);
            DayNightCycle.DayTick += UpdateLabel;
        }

        public void UpdateLabel(int day)
        {
            Text = $"Day {day.ToString()}";
        }

    }
}