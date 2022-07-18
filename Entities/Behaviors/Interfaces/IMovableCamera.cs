using Godot;

namespace Mdfry1.Entities.Behaviors.Interfaces
{
    public interface IMovableCamera
    {
        void SetZoom(float amount);
        void SetPan(Vector2 newOffset);
        void ResetCamera();
    }
}