using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class BejeweldGameManager : MonoBehaviour
{
    public static BejeweldGameManager Instance;
    
    public delegate void GemMoved(Gem sender);

    private Grid grid;
    
    public GemMoved GemMovedEvent;

    [SerializeField] private bool showPopulating = true;
    
    private Dictionary<Vector2Int, Gem> GridElements => ParseToGem();
    
    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        
        grid = FindObjectOfType<Grid>();
    }

    private void OnEnable()
    {
        GemMovedEvent += CheckConnectionsWith;
    }

    private void OnDisable()
    {
        GemMovedEvent -= CheckConnectionsWith;
    }

    private void Start()
    {
        grid.Populate();
    }

    private async void ShowOf()
    {
        while(showPopulating && Application.isPlaying)
        {
            grid.Populate();
            await Task.Delay(500);
            grid.DePopulate();
        }
    }
    private static readonly Vector2Int[] Directions =
    {
        new Vector2Int(1,0),
        new Vector2Int(0,1),
    };
    private void CheckConnectionsWith(Gem sender)
    {
        for (int i = 0; i < Directions.Length; i++)
        {
            List<Gem> matchedGems = MatchedGemsInPlane(sender, Directions[i]);

            if (matchedGems.Count < 3) continue;

            for (int j = 0; j < matchedGems.Count; j++)
            {
                if (matchedGems[j] == null) continue;
                Destroy(matchedGems[j]);
            }
        }
        
        
    }

    private List<Gem> MatchedGemsInPlane(Gem startGem, Vector2Int direction)
    {
        List<Gem> matchedGems = new List<Gem>();
        matchedGems.Add(startGem);
        
        Gem nextGem = startGem;

        for (int x = -1; x < 2; x += 2)
        {
            do
            {
                if (!GridElements.TryGetValue(nextGem.coordinate + (direction * x), out nextGem))
                {
                    nextGem = startGem;
                    break;
                }
                
                if (nextGem.gemType == startGem.gemType && !matchedGems.Contains(nextGem))
                    matchedGems.Add(nextGem);
                
            } while (nextGem != null);
        }

        return matchedGems;
    }

    private Dictionary<Vector2Int, Gem> ParseToGem()
    {
        Dictionary<Vector2Int, Gem> gems = new Dictionary<Vector2Int, Gem>();

        foreach (var element in grid.gridElements)
        {
            gems.Add(element.Key, (Gem)element.Value);
        }

        return gems;
    }
    
    /// <summary>
    /// Swaps the gem in the grid, swaps the coordinates and positions as well. 
    /// </summary>
    /// <param name="gemA"></param>
    /// <param name="gemB"></param>
    public void MoveGems(Gem gemA, Gem gemB)
    {
        Vector2Int coordinateA = gemA.coordinate;
        Vector2Int coordinateB = gemB.coordinate;

        gemA.coordinate = coordinateB;
        gemB.coordinate = coordinateA;
        
        grid.gridElements[gemA.coordinate] = gemA;
        grid.gridElements[gemB.coordinate] = gemB;

        gemA.transform.position = grid.GeneratePositionFromCoordinates(gemA.coordinate);
        gemB.transform.position = grid.GeneratePositionFromCoordinates(gemB.coordinate);
    }
}
