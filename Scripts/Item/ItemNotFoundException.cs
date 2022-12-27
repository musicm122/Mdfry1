using System;

namespace Mdfry1.Scripts.Item;

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string itemName)
    {
        ItemName = itemName;
    }

    public ItemNotFoundException(string itemName, string message) : base(message)
    {
        ItemName = itemName;
    }

    public ItemNotFoundException(string itemName, string message, Exception innerException) : base(message,
        innerException)
    {
        ItemName = itemName;
    }

    public ItemNotFoundException()
    {
    }

    public ItemNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public string ItemName { get; set; }
}