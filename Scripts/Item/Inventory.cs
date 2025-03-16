using System;
using System.Collections.Generic;
using System.Linq;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;

namespace Mdfry1.Scripts.Item;

public class Inventory : IInventory
{
    public delegate void AddingItemHandler(object sender, InventoryEventArgs args);

    public delegate void RemovingItemHandler(object sender, InventoryEventArgs args);

    private List<Item> Items { get; } = new();

    public event EventHandler<InventoryEventArgs> AddItemEvent;

    public event EventHandler<InventoryEventArgs> RemoveItemEvent;

    public bool HasResource(string name)
    {
        return Items.Any(item => item.Name == name);
    }

    public bool HasKey(Key key)
    {
        return Items.Any(item => string.Equals(item.Name, key.ToString(), StringComparison.OrdinalIgnoreCase)) || key == Key.None;
    }

    public void Add(string name, int amt)
    {
        for (var i = 0; i < amt; i++)
        {
            var item = new Item(name);
            RaiseAddingItem(item);
            Items.Add(item);
        }
    }

    public void RemoveAmount(string name, int amt = 1)
    {
        Items.RemoveAmt(i => i.Name == name, amt);
        RaiseRemovingItem(new Item(name), amt);
    }

    public void Remove(string name)
    {
        Items.RemoveOne(i => i.Name == name);
        RaiseRemovingItem(new Item(name));
    }

    public IEnumerable<string> Names()
    {
        return Items.Select(i => i.Name);
    }

    public IEnumerable<string> Descriptions()
    {
        return Items.Select(i => i.Description);
    }

    public IEnumerable<string> ImagePaths()
    {
        return Items.Select(i => i.ImagePath);
    }

    public int Count()
    {
        return Items.Count;
    }

    public int CountOfType(string name)
    {
        return Items.Count(i => i.Name.Equals(name));
    }


    public string Display()
    {
        var retval = "Items:\r\n=======\r\n";

        if (Items.Count > 0)
        {
            var distinctItems = DistinctItems();

            for (var i = 0; i < distinctItems.Length; i++)
                retval += $"{distinctItems[i]}: {CountOfType(distinctItems[i])} \r\n";
        }
        else
        {
            retval += "Empty\r\n";
        }

        return retval;
    }

    public bool HasItemWithAtLeast(string itemName, int amt)
    {
        return Items.Count(i => i.Name.Trim().Equals(itemName.Trim())) >= amt;
    }

    public bool HasItemInInventory(string itemName)
    {
        return Items.Any(i => i.Name.Trim().Equals(itemName.Trim()));
    }

    public Item GetItem(string itemName)
    {
        return Items.First(i => i.Name.Trim().Equals(itemName.Trim()));
    }

    public bool HasItem(string itemName)
    {
        return Items.Any(i => i.Name.Trim().Equals(itemName.Trim()));
    }

    public bool RemoveItemIfExists(string itemName)
    {
        if (!Items.Any(i => i.Name.Trim().Equals(itemName.Trim()))) return false;
        var item = Items.First(i => i.Name == itemName);
        RaiseRemovingItem(item);
        Items.Remove(item);
        return true;
    }

    protected virtual void RaiseAddingItem(Item item)
    {
        AddItemEvent?.Invoke(this, new InventoryEventArgs(item));
    }

    protected virtual void RaiseRemovingItem(Item item)
    {
        RemoveItemEvent?.Invoke(this, new InventoryEventArgs(item));
    }

    protected virtual void RaiseRemovingItem(Item item, int amt)
    {
        RemoveItemEvent?.Invoke(this, new InventoryEventArgs(item, amt));
    }

    public string[] DistinctItems()
    {
        return Items.Select(item => item.Name)
            .Distinct()
            .ToArray();
    }
}