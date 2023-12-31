﻿using System.Collections.Generic;
using UnityEngine;

public enum NeighborType { Left, Right, Above, Below }

enum CellType { Normal, Start, End }

public class Cell {

    public int cost = 0;
    public bool isVisited = false;

    int _x;
    int _y;
    CellType cellType;
    RoomType _roomType;
    int cellWeight;
    bool _currentlyUsedOnMap;
    public delegate void CellDiscovery();
    public event CellDiscovery onCellDiscover;

    public int X { get => _x; set => _x = value; }
    public int Y { get => _y; set => _y = value; }
    public int CellWeight { get => cellWeight; set => cellWeight = value; }

    public bool CurrentlyUsedOnMap { get => _currentlyUsedOnMap; set => _currentlyUsedOnMap = value; }

    public Dictionary<NeighborType, Cell> NeighborCells { get => neighborCells; set => neighborCells = value; }
    internal CellType CellType { get => cellType; set => cellType = value; }
    public RoomType RoomType { get => _roomType; set => _roomType = value; }

    public Dictionary<NeighborType, Cell> neighborCells = new Dictionary<NeighborType, Cell>();

    public Cell(int x, int y){
        X = x;
        Y = y;
    }

    //Once the node map is done, some branches are generated. Once some branches are generated, meaning the connections need to be updated.
    public void RegenerateConnections(){
        GetConnections();
    }
    
    public List<Connection> GetConnections(){
        List<Connection> connections = new List<Connection>();       
        return connections;
    }

    private Connection FindNeighborFor(int valueToCompareX, int valueToCompareY){
        Connection connection = null;
        if (valueToCompareX == X && valueToCompareY == Y) return null;
        if (ProceduralGeneration.Singleton.CellsTable.ContainsKey(new Vector2(valueToCompareX, valueToCompareY)))
        {
            Cell connectionCell = ProceduralGeneration.Singleton.CellsTable[new Vector2(valueToCompareX, valueToCompareY)];
            connection = new Connection(connectionCell.cellWeight, new Vector2(X, Y), new Vector2(valueToCompareX, valueToCompareY));
        }
        return connection;
    }

    public int getElementValue(){
        return cellWeight;
    }
    //Used to discover adjancent cells when moving to an unexplored cell of the map
    public void InvokeCellDiscover(){
        onCellDiscover?.Invoke();
    }

}
/// <summary>
/// Used by procedural generation cells and rooms. For each room, a location for a 
/// door should be determined using neighbor type.
/// Each room is not required to have all of the connecting doors to be valid.
/// </summary>
