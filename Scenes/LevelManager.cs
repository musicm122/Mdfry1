using System.Collections.Generic;
using Godot;

namespace Mdfry1.Scenes
{
    public class LevelManager : Node2D 
    {
        public List<Node2D> Enemies { get; set; }
        
        
        public void AlertEnemies() 
        {
        }

        public override void _Ready()
        {
            base._Ready();
        }
    }
}