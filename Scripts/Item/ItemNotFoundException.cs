using System;

namespace Mdfry1.Scripts.Item
{

    public class ItemNotFoundException : Exception
    {
        public string ItemName { get; set; }

        public ItemNotFoundException(string itemName) : base()
        {
            ItemName = itemName;
        }

        public ItemNotFoundException(string itemName, string message) : base(message)
        {
            ItemName = itemName;
        }

        public ItemNotFoundException(string itemName, string message, Exception innerException) : base(message, innerException)
        {
            ItemName = itemName;
        }

        public ItemNotFoundException() : base()
        {
        }

        public ItemNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}