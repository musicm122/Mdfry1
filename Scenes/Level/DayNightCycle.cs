using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Godot;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;

// Adapted from 
//https://github.com/bitbrain/godot-tutorials/blob/main/2d-lighting/demo/DayNightCycle.gd
// and I'm too tired to clean up formatting and casing. I'm sorry.



namespace Mdfry1.Scenes.Level
{
    public class DayNightCycle : CanvasModulate
    {
        private LogLevelOutput _logLevel = LogLevelOutput.Debug; 
        
        private readonly ILogger _logger = new GDLogger
        {
            Level = LogLevelOutput.Debug
        };
        
        [Export]
        public LogLevelOutput LogLevel
        {
            get => _logLevel;
            set
            {
                _logLevel = value;
                this._logger.Level = _logLevel;
            }
        }
        
        public Action<int> DayTick { get; set; }

        public float TotalTime { get; set; } = 0;
        
        [Export]
        private Color _nightColor = new Color("#091d3a");
        
        [Export]
        private Color _dayColor = new Color("#ffffff");
        
        [Export]
        private Color _eveningColor = new Color("#ff3300");

        [Export] 
        public float TimeScale { get; set; } = 0.1f; // 0.1 == Roughly 40 seconds in a day

        public float Time { get; set; } = 0f;
        public int LastDay { get; set; } = 0;

        Color GetSourceColor(float toColorVal) =>
            _nightColor.LinearInterpolate(_eveningColor, toColorVal);

        Color GetTargetColor(float toColorVal) =>
            _eveningColor.LinearInterpolate(_dayColor, toColorVal);

        public int GetDay()
        {
            const float offset = 0.15f;
            var temp = (int) (offset + Mathf.Ceil(0.5f * Time) / Mathf.Pi);
            return temp + 1;
        }

        /// <summary>
        /// if we are at l
        /// </summary>
        /// <returns></returns>
        bool ToggleSpawnCheck()
        {
            const float dayInSec = 40f;
            const float thresholdForSpawning = 0.35f;
            _logger.Debug($" ({TotalTime.ToString(CultureInfo.InvariantCulture)} / {dayInSec.ToString(CultureInfo.InvariantCulture)}) : {(TotalTime / dayInSec)} : > {thresholdForSpawning.ToString(CultureInfo.InvariantCulture)}");
            return (TotalTime / dayInSec) > thresholdForSpawning;
        }

        public override void _Process(float delta)
        {
            if(ToggleSpawnCheck())
            {
                _logger.Debug("Enabling Spawning");
                GetTree().EnableSpawners();
            }else
            {
                _logger.Debug("Disabling Spawning");
                GetTree().DisableSpawners();
            }
            
            Time += delta * TimeScale;
            TotalTime += delta;
            var val = (Mathf.Sin(Time) + 1) / 2;
            Color = GetSourceColor(val).LinearInterpolate(GetTargetColor(val), val);
            var newDay = this.GetDay();
            if (newDay == LastDay) return;
            LastDay = newDay;
            DayTick?.Invoke(newDay);

        }
    }
}