using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
public class Gem : GridElement
{
    public GemType gemType;
    [HideInInspector] public bool isMatched;
    [SerializeField] private List<Sprite> sprites = new List<Sprite>(6);

    private SpriteRenderer spriteRenderer;
    
    /// <summary>
    /// Used for initialization of the gem for when the grid is created
    /// </summary>
    public override void Initialize()
    {
        int gemTypeIndex = Random.Range(0, 6);
        gemType = (GemType) gemTypeIndex;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[gemTypeIndex];
    }
    
    ///<summary>
    /// Selects a gem to be placed on adjacent tile with effects to show it is selected 
    ///</summary>
    private void OnMouseDown()
    {
        Color color = spriteRenderer.color;

        Gem selectedGem = BejeweldGameManager.Instance.selectedGem;
        if (selectedGem == null)
        {
            BejeweldGameManager.Instance.selectedGem = this;
            color.a /= 2;
            spriteRenderer.color = color;
            return;
        }

        if (selectedGem == this)
        {
            BejeweldGameManager.Instance.selectedGem = null;
            color.a *= 2;
            spriteRenderer.color = color;
            return;
        }
        
        if (Vector2Int.Distance(selectedGem.coordinate, this.coordinate) > 1) return;


        BejeweldGameManager.Instance.selectedGem.GetComponent<SpriteRenderer>().color = color;
        BejeweldGameManager.Instance.MoveGems(this, BejeweldGameManager.Instance.selectedGem);
        BejeweldGameManager.Instance.selectedGem = null;
    }
}
public enum GemType
{
    Ruby,
    Emerald,
    Sapphire,
    Bismuth,
    Diamond,
    Prism,
}