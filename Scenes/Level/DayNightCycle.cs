using System;
using System.Runtime.InteropServices;
using Godot;

// Adapted from 
//https://github.com/bitbrain/godot-tutorials/blob/main/2d-lighting/demo/DayNightCycle.gd
// and I'm too tired to clean up formatting and casing. I'm sorry.



namespace Mdfry1.Scenes.Level
{
    public class DayNightCycle : CanvasModulate
    {
        public Action<int> DayTick { get; set; }

        public float TotalTime = 0;
        
        [Export]
        private Color NIGHT_COLOR = new Color("#091d3a");
        
        [Export]
        private Color DAY_COLOR = new Color("#ffffff");
        
        [Export]
        private Color EVENING_COLOR = new Color("#ff3300");
        
        [Export]
        public float TIME_SCALE = 0.1f;

        public float time = 0f;
        public int lastDay = 0;


        Color GetSourceColor(float toColorVal) =>
            NIGHT_COLOR.LinearInterpolate(EVENING_COLOR, toColorVal);

        Color GetTargetColor(float toColorVal) =>
            EVENING_COLOR.LinearInterpolate(DAY_COLOR, toColorVal);

        public int GetDay()
        {
            const float offset = 0.15f;
            var temp = (int) (offset + Mathf.Ceil(0.5f * time) / Mathf.Pi);
            return temp + 1;
        }


        public override void _Process(float delta)
        {
            time += delta * TIME_SCALE;
            var val = (Mathf.Sin(time) + 1) / 2;
            Color = GetSourceColor(val).LinearInterpolate(GetTargetColor(val), val);
            var newDay = this.GetDay();
            if (newDay == lastDay) return;
            lastDay = newDay;
            DayTick?.Invoke(newDay);
        }
    }
}