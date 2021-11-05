using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Wallet
{
    private int voidQuartz = 0;
    private int spaceQuartz = 0;
    private int timeQuartz = 0;
    private int valuableQuartz;
    //Other usable items go here

    public int VoidQuartz { get => voidQuartz; set => voidQuartz = value; }
    public int SpaceQuartz { get => spaceQuartz; set => spaceQuartz = value; }
    public int TimeQuartz { get => timeQuartz; set => timeQuartz = value; }
    public int ValuableQuartz { get => valuableQuartz; set => valuableQuartz = value; }

    public void AddQuartz(ResourceType quartzType, int amount)
    {
        switch (quartzType)
        {
            case ResourceType.Time:
                timeQuartz += amount;
                break;
            case ResourceType.Space:
                spaceQuartz += amount;
                break;
            case ResourceType.Void:
                voidQuartz += amount;
                break;
            case ResourceType.Valuable:
                valuableQuartz += amount;
                break;
        }
    }

    public void RemoveQuartz(ResourceType quartzType, int amount)
    {
        switch (quartzType)
        {
            case ResourceType.Time:
                timeQuartz -= amount;
                break;
            case ResourceType.Space:
                spaceQuartz -= amount;
                break;
            case ResourceType.Void:
                voidQuartz -= amount;
                break;
            case ResourceType.Valuable:
                valuableQuartz -= amount;
                break;
        }
    }

    public int QuartzAmount(ResourceType quartzType)
    {
        switch (quartzType)
        {
            case ResourceType.Time:
                return TimeQuartz;
            case ResourceType.Space:
                return SpaceQuartz;
            case ResourceType.Void:
                return VoidQuartz;
            case ResourceType.Valuable:
                return ValuableQuartz;
            default:
                return -1;
        }
    }
}

