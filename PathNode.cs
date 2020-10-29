using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public float height;
    public bool isWalkable;
    public bool isOccupied;

    public int gCost;
    public int hCost;
    public int fCost;

    public int i;
    public int j;

    public int jumps;
    public int steps;
    public PathNode previousNode;




    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
}
