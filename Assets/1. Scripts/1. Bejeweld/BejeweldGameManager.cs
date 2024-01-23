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
    
    /// <summary>
    /// When this starts it generates a grid every 1/2 second to show the populating of the grid.
    /// </summary>
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
    
    /// <summary>
    /// Checks if the sent gem has any linear connections with the same gem type and puts them into a list.
    /// </summary>
    /// <param name="sender">Checks connections from this gem</param>
    private void CheckConnectionsWith(Gem sender)
    {
        List<Gem> allMatchedGems = new List<Gem>(20);
        
        foreach (var direction in Directions)
        {
            List<Gem> matchedGems = new List<Gem> { sender };
            matchedGems.AddRange(MatchedGemsInDirection(sender, direction));
            matchedGems.AddRange(MatchedGemsInDirection(sender, -direction));
            
            if (matchedGems.Count < 3) continue;
            
            allMatchedGems.AddRange(matchedGems);
        }

        foreach (var gem in allMatchedGems)
        {
            gem.isMatched = true;
        }
    }

    private List<Gem> MatchedGemsInDirection(Gem startGem, Vector2Int direction)
    {
        List<Gem> matchedGems = new List<Gem>();
        Gem nextGem;
        if (GridElements.TryGetValue(startGem.coordinate + direction, out nextGem))
        {

            if (nextGem.gemType == startGem.gemType)
            {
                print($"Gem: {nextGem.name} : {nextGem.gemType.ToString()}");
                matchedGems.Add(nextGem);   
                matchedGems.AddRange(MatchedGemsInDirection(nextGem, direction));
            }
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
