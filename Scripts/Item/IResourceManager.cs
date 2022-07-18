using System.Collections.Generic;
using Mdfry1.Scripts.Enum;

namespace Mdfry1.Scripts.Item
{
    public interface IResourceManager<T>
    {
        List<T> Items { get; set; }

        bool HasResource(string name);

        bool HasKey(Key key);

        string InventoryDisplay();

        void RemoveItem(string name, int amt = 1);
    }
}