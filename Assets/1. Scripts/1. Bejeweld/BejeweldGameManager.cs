using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BejeweldGameManager : MonoBehaviour
{
    public static BejeweldGameManager Instance;
    
    public delegate void GemMoved(GridElement sender);

    public Grid grid;
    
    public GemMoved GemMovedEvent;

    [SerializeField] private bool showPopulating = true;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Instance = this;
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
        ShowOf();
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

    private void CheckConnectionsWith(GridElement sender)
    {
                
    }
}
