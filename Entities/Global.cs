using Godot;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;
using TinyIoC;

namespace Mdfry1.Entities
{
    public class Global : Node
    {
        public Global()
        {
            Container.Register<ILogger, GDLogger>().AsSingleton();
        }

        public TinyIoCContainer Container { get; } = new();
    }
}