using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridElement : MonoBehaviour
{
    public virtual void Initialize(){}
}

public enum JewelType
{
    Ruby,
    Emerald,
    Sapphire,
    Bismuth,
}