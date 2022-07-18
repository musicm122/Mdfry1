using Mdfry1.Entities.Components;
using Mdfry1.Scripts.Item;
using Mdfry1.Scripts.Mission;

namespace Mdfry1.Entities
{
    public class PlayerDataStore
    {
        public Health PlayerStatus { get; set; } = new Health();
        
        public MissionManager MissionManager { get; set; } = new MissionManager();

        public Inventory Inventory { get; set; } = new Inventory();

    }
}