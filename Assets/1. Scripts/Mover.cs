using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Mover
{
    public static Gem SelectedItem;
    
    /// <summary>
    /// Swaps places between two gems and Invokes the GemMovedEvent with themselves as parameter.
    /// </summary>
    /// <param name="gem">Gem to be swaped with</param>
    public static void MoveItemTo(Gem gem)
    {
        if (BejeweldGameManager.Instance.TryMoveGems(SelectedItem, gem))
        {
            BejeweldGameManager.Instance.GemMovedEvent?.Invoke(SelectedItem);
            BejeweldGameManager.Instance.GemMovedEvent?.Invoke(gem);
        }
        
        SelectedItem = null;
        
        
    }
}
