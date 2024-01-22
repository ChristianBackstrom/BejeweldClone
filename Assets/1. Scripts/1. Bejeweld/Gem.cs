using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Gem : GridElement
{
    public GemType gemType;
    [SerializeField] private List<Sprite> sprites = new List<Sprite>(4);

    public override void Initialize()
    {
        int gemTypeIndex = Random.Range(0, 4);
        gemType = (GemType) gemTypeIndex;

        GetComponent<SpriteRenderer>().sprite = sprites[gemTypeIndex];
    }
}
public enum GemType
{
    Ruby,
    Emerald,
    Sapphire,
    Bismuth,
}