using Core2D.Effects;

public class BloodSpatter : BloodSpatterFS
{
}
// public class BloodSpatter : CPUParticles2D
// {
//     public Timer Timer { get; set; }
//
//     // Declare member variables here. Examples:
//     // private int a = 2;
//     // private string b = "text";
//
//     // Called when the node enters the scene tree for the first time.
//     public override void _Ready()
//     {
//         Timer = GetNode<Timer>("Timer");
//         Timer.Connect("timeout", this, nameof(OnTimeout));
//         SetDirection();
//         Emitting = true;
//     }
//
//     private void SetDirection()
//     {
//         var playerPos = GetTree().GetPlayerGlobalPosition();
//         if (playerPos.x > GlobalPosition.x) Direction = new Vector2(-1, Direction.y);
//
//         if (playerPos.x < GlobalPosition.x) Direction = new Vector2(1, Direction.y);
//     }
//
//     private void OnTimeout()
//     {
//         QueueFree();
//     }
//
// //  // Called every frame. 'delta' is the elapsed time since the previous frame.
// //  public override void _Process(float delta)
// //  {
// //      
// //  }
// }