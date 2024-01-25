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

    [SerializeField] private float fallTime = .2f;
    
    public GemMoved GemMovedEvent;
    
    private Dictionary<Vector2Int, Gem> GridGemElements => ParseToGem();
    private Dictionary<Vector2Int, GridElement> GridElements => grid.gridElements;

    private Vector2Int GridSize => grid.gridSize;
    
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
        
        MakeGemsFall(allMatchedGems);
    }

    /// <summary>
    /// Checks if the gem has any connections that will make it be matched
    /// </summary>
    /// <param name="sender">Gem to be checked</param>
    /// <returns>bool - if it has valid connections</returns>
    private bool HasValidConnections(Gem sender)
    {
        foreach (var direction in Directions)
        {
            List<Gem> matchedGems = new List<Gem> { sender };
            matchedGems.AddRange(MatchedGemsInDirection(sender, direction));
            matchedGems.AddRange(MatchedGemsInDirection(sender, -direction));
            if (matchedGems.Count >= 3) return true;
        }

        return false;
    }

    private List<Gem> MatchedGemsInDirection(Gem startGem, Vector2Int direction)
    {
        List<Gem> matchedGems = new List<Gem>();
        Gem nextGem;
        if (GridGemElements.TryGetValue(startGem.coordinate + direction, out nextGem))
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
    
    
    
    /// <summary>
    /// Parses the gridElement dictionary to use Gem class instead of GridElement 
    /// </summary>
    /// <returns>Parsed dictionary</returns>
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
    /// Destroyes the matched gems and make all gems above fall down to fill board
    /// </summary>
    /// <param name="destroyedGems">the gems that will be destroyed</param>
    private async void MakeGemsFall(List<Gem> destroyedGems)
    {
        List<Vector2Int> matchedGemCoordinates = destroyedGems.ConvertAll(x => x.coordinate);

        for (int i = matchedGemCoordinates.Count - 1; i >= 0; i--)
        {
            Vector2Int coordinate = matchedGemCoordinates[i];
            if (!GridElements.ContainsKey(coordinate))
            {
                matchedGemCoordinates.Remove(coordinate);
                continue;
            }
            
            Destroy(GridElements[coordinate].gameObject);
            GridElements.Remove(coordinate);
        }
        
        //orders the list in descending order from their y coordinate
        matchedGemCoordinates = matchedGemCoordinates.OrderByDescending(x => x.y).ToList();

        
        for (int i = 0; i < matchedGemCoordinates.Count; i++)
        {
            int x = matchedGemCoordinates[i].x;
            GridElement element;

            for (int y = matchedGemCoordinates[i].y + 1; y < GridSize.y / 2; y++)
            {
                Vector2Int coordinate = new Vector2Int(x, y);
                
                if (!GridElements.TryGetValue(coordinate, out element)) continue;

                GridElements.Remove(coordinate);
                coordinate.y--;
                GridElements.Add(coordinate, element);
                element.coordinate = coordinate;
                Vector2 endPosition = grid.GeneratePositionFromCoordinates(coordinate);
                Vector2 startPosition = element.transform.position;
                
                float time = 0;
                
                while ((Vector2)element.transform.position != endPosition)
                {
                    time += Time.deltaTime * (1f/fallTime);
                    time = Mathf.Clamp01(time);
                    if (element)
                    element.transform.position = Vector2.Lerp(startPosition, endPosition, time);
                    await Task.Yield();
                }
            }

            Vector2Int coordinates = new Vector2Int(x, GridSize.y / 2 - 1);
            Vector2 position = grid.GeneratePositionFromCoordinates(coordinates);
            GridElement gridElement = Instantiate(grid.gridElementPrefab, position, Quaternion.identity);
            gridElement.coordinate = coordinates;
            gridElement.Initialize();
            gridElement.name = $"( {x} , {coordinates.y} )";
                
            GridElements.Add(coordinates, gridElement);
        }
    }
    
    /// <summary>
    /// Swaps the gem in the grid, swaps the coordinates and positions as well. 
    /// </summary>
    /// <param name="gemA"></param>
    /// <param name="gemB"></param>
    /// <returns>if the gems was move or not</returns>
    public bool TryMoveGems(Gem gemA, Gem gemB)
    {
        Vector2Int coordinateA = gemA.coordinate;
        Vector2Int coordinateB = gemB.coordinate;

        gemA.coordinate = coordinateB;
        gemB.coordinate = coordinateA;
        
        grid.gridElements[gemA.coordinate] = gemA;
        grid.gridElements[gemB.coordinate] = gemB;

        if (!HasValidConnections(gemA) && !HasValidConnections(gemB))
        {
            gemA.coordinate = coordinateA;
            gemB.coordinate = coordinateB;
        
            grid.gridElements[gemA.coordinate] = gemA;
            grid.gridElements[gemB.coordinate] = gemB;
            
            return false;
        }

        gemA.transform.position = grid.GeneratePositionFromCoordinates(gemA.coordinate);
        gemB.transform.position = grid.GeneratePositionFromCoordinates(gemB.coordinate);
        return true;
    }
}
