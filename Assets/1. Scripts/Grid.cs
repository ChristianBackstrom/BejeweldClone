using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Grid : MonoBehaviour
{
    [SerializeField] private GridElement gridElementPrefab;
    [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
    [SerializeField] private float gridElementMargin = 5;
    
    public Dictionary<Vector2Int, GridElement> gridElements;

    // Creates the gems and runs their initialization function
    public void Populate()
    {
        gridElements = new Dictionary<Vector2Int, GridElement>();

        if (!gridElementPrefab) Application.Quit();

        for (int x = -gridSize.x / 2; x < gridSize.x / 2; x++)
        {
            for (int y = -gridSize.y / 2; y < gridSize.y / 2; y++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);

                Vector2 position = GeneratePositionFromCoordinates(coordinates);
                GridElement gridElement = Instantiate(gridElementPrefab, position, Quaternion.identity);
                gridElement.coordinate = coordinates;
                gridElement.Initialize();
                gridElement.name = $"( {x} , {y} )";
                
                gridElements.Add(coordinates, gridElement);
            }
        }
    }
    
    // Destroys all gems on board and clears the dictionary
    public void DePopulate()
    {
        if (gridElements == null) return; 
        
        foreach (var element in gridElements)
        {
            if (element.Value.gameObject != null)
                Destroy(element.Value.gameObject);
        }
        
        gridElements.Clear();
    }

    
    // to give the gems their correct position in world with their corresponding coordinate in the grid
    public Vector2 GeneratePositionFromCoordinates(Vector2Int coordinates)
    {
        return (Vector2)coordinates * gridElementMargin;
    }

    private void OnDrawGizmos()
    {
        for (int x = -gridSize.x / 2; x < gridSize.x / 2; x++)
        {
            for (int y = -gridSize.y / 2; y < gridSize.y / 2; y++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);

                Vector2 position = GeneratePositionFromCoordinates(coordinates);
                
                Gizmos.DrawWireCube(position, new Vector3(gridElementMargin, gridElementMargin));
            }
        }
    }
}
