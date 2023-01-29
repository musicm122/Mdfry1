using System;
using Mdfry1.Logic.Constants;
using Mdfry1.Scripts.Constants;

namespace Mdfry1.Scripts.Item;

[Serializable]
public class Item
{
    public Item()
    {
    }

    public Item(string name)
    {
        var temp = MasterItemList.GetItemByName(name);
        Name = name;
        Description = temp.Description;
        ImagePath = temp.ImagePath;
    }

    public Item(string name, string description, string imagePath) : this()
    {
        Name = name;
        Description = description;
        ImagePath = imagePath;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }

    public string ImagePathLookup(string name) =>
        ItemConstants.ItemImagePaths[name.ToLowerInvariant()];
    
}