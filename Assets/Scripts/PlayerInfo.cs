using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    private string power;
    private string angle;
    private string distance;
    public PlayerInfo()
    {
    }
    public string Power { get => power; set => power = value; }
    public string Angle { get => angle; set => angle = value; }
    public string Distance { get => distance; set => distance = value; }
}
