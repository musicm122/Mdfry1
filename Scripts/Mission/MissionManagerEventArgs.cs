namespace Mdfry1.Scripts.Mission;

public class MissionManagerEventArgs
{
    public MissionElement mission;

    public MissionManagerEventArgs(MissionElement mission)
    {
        this.mission = mission;
    }
}