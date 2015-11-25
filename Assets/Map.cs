﻿using UnityEngine;
using System.Collections.Generic;

using HexEngine;
using System;

public class Map : MonoBehaviour
{
    [Range(0,100)]
    public int width;
    [Range(0,100)]
    public int height;

    [Range(1, 1000)]
    public int framesPerUpdate;

    [Range(0.1f, 50)]
    public float cellSize;

    private int _framesSinceUpdate;

    private HexWorld _worldModel;

    private Colony _testColony;

    // PROTOTYPE: These are just here to render some world cells with shape and color.
    // Real stuff with particle effects or other wizardry will replace them.
    private IDictionary<Coord, GameObject> _colonyCubes = new Dictionary<Coord, GameObject>();
    private IDictionary<Coord, GameObject> _hormoneCubes = new Dictionary<Coord, GameObject>();

    // Use this for initialization
    void Start()
    {
        _framesSinceUpdate = 0;

        _worldModel = new HexWorld(width, height);

        // add unity objects to each cell
        _worldModel.ForEachPosition((coord) =>
        {
            var cellPosition = GetPosition(coord.Row, coord.Col);

            GameObject colonyCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            colonyCube.transform.localScale = new Vector3(cellSize, cellSize, cellSize * 0.3f);
            colonyCube.transform.position = cellPosition;
            
            GameObject hormoneCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hormoneCube.transform.localScale = new Vector3(cellSize / 2, cellSize / 2, cellSize);
            hormoneCube.transform.position = cellPosition;

            _colonyCubes.Add(coord, colonyCube);
            _hormoneCubes.Add(coord, hormoneCube);
        });

        // test dummy data
        _testColony = new Colony(0, "test colony");

        _worldModel.AddColony(_testColony);
        
        _worldModel.AddCreatureCells(_testColony, new Coord(1, 1), 30);
        _worldModel.AddCreatureCells(_testColony, new Coord(0, 0), 10);
        _worldModel.AddCreatureCells(_testColony, new Coord(2, 2), 40);
        _worldModel.AddCreatureCells(_testColony, new Coord(3, 3), 30);

        _worldModel.AddMoveHormone(_testColony, new Coord(1, 1), 3);
        _worldModel.AddMoveHormone(_testColony, new Coord(1, 0), 5);
        _worldModel.AddMoveHormone(_testColony, new Coord(2, 0), 35);
        // end dummy data
    }

    // Update is called once per frame
    void Update()
    {
        if (_worldModel == null)
            return;

        _framesSinceUpdate++;
        if (_framesSinceUpdate > framesPerUpdate)
        {
            Debug.Log("Evolving world.");
            _worldModel.Evolve();
            _framesSinceUpdate = 0;

            DrawWorld();
        }
    }

    private void DrawWorld()
    {
        if (_worldModel == null)
            return;

        foreach (var cellContainer in _worldModel.CurrentCells)
        {
            Vector2 position = GetPosition(cellContainer.Row, cellContainer.Col);
            foreach (var colonyCell in cellContainer.Cells)
            {
                GameObject colonyCube = _colonyCubes[cellContainer.Coord];
                if (colonyCell.CreatureCellDensity > 0)
                    colonyCube.GetComponent<Renderer>().material.color = Color.white;
                else
                    colonyCube.GetComponent<Renderer>().material.color = Color.black;

                GameObject hormoneCube = _hormoneCubes[cellContainer.Coord];
                if (colonyCell.MoveHormoneDensity > 0)
                    hormoneCube.GetComponent<Renderer>().material.color = Color.green;
                else
                    hormoneCube.GetComponent<Renderer>().material.color = Color.gray;
            }
        }
    }
    
    public Vector2 GetPosition(int row, int col)
    {
        return new Vector2(col + 0.5f * row, 0.866025404f * row) * cellSize;
    }
}