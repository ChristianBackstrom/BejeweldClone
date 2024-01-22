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
                gridElement.Initialize();
                
                gridElements.Add(coordinates, gridElement);
            }
        }
    }

    public void DePopulate()
    {
        if (gridElements == null) return;
        
        foreach (var element in gridElements)
        {
            Destroy(element.Value);
        }
        
        gridElements.Clear();
    }

    private Vector2 GeneratePositionFromCoordinates(Vector2Int coordinates)
    {
        return (Vector2)coordinates * gridElementMargin;
    }

    private void OnDrawGizmos()
    {
        if (gridElements == null) return;
        
        foreach (KeyValuePair<Vector2Int,GridElement> keyValuePair in gridElements)
        {
            GridElement gridElement = keyValuePair.Value;
            
            Handles.Label(gridElement.transform.position, keyValuePair.Key.ToString());
        }
    }
}
