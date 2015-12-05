using UnityEngine;
using System.Collections.Generic;

using HexEngine;
using System;

public class Map : MonoBehaviour
{
    [Range(0, 100)]
    public int _wordWidth = 20;
    [Range(0,100)]
    public int _worldHeight = 30;

    [Range(0, 50)]
    public float _updatesPerSecond = 1;
    private float _lastUpdateTime = 0;

    [Range(0.1f, 50)]
    public float _renderedCellSize = 5;

    [Range(1, 50000)]
    public int _maxCreatureDensity = 5000;

    [Range(1, 50000)]
    public int _maxHormoneDensity = 10000;

    private HexWorld _worldModel;

    private Colony _testColony;

    // PROTOTYPE: These are just here to render some world cells with shape and color.
    // Real stuff with particle effects or other wizardry will replace them.
    private IDictionary<Coord, GameObject> _creatureCubes = new Dictionary<Coord, GameObject>();
    private IDictionary<Coord, GameObject> _hormoneCubes = new Dictionary<Coord, GameObject>();

    // Use this for initialization
    void Start()
    {

        _worldModel = new HexWorld(_wordWidth, _worldHeight);

        // add unity objects to each cell
        _worldModel.ForEachPosition((coord) =>
        {
            var cellPosition = GetPosition(coord.Row, coord.Col);

            GameObject creatureCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            creatureCube.transform.localScale = new Vector3(_renderedCellSize / 2, _renderedCellSize / 2, _renderedCellSize);
            creatureCube.transform.position = cellPosition;
            creatureCube.GetComponent<Renderer>().material.color = Color.black;
            
            GameObject hormoneCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hormoneCube.transform.localScale = new Vector3(_renderedCellSize, _renderedCellSize, _renderedCellSize * 0.3f);
            hormoneCube.transform.position = cellPosition;
            hormoneCube.GetComponent<Renderer>().material.color = Color.black;

            _creatureCubes.Add(coord, creatureCube);
            _hormoneCubes.Add(coord, hormoneCube);
        });

        // test dummy data
        _testColony = new Colony(0, "test colony");

        _worldModel.AddColony(_testColony);

        _worldModel.AddCreatures(_testColony, new Coord(3, 0), 4000);
        _worldModel.AddCreatures(_testColony, new Coord(3, 1), 3000);
        _worldModel.AddCreatures(_testColony, new Coord(3, 2), 2000);
        _worldModel.AddCreatures(_testColony, new Coord(3, 3), 1000);

        _worldModel.AddMoveHormone(_testColony, new Coord(5, 4), 9300);
        // end dummy data
    }

    // Update is called once per frame
    void Update()
    {
        if (_worldModel == null)
            return;

        _testColony.MaxCreatureDensity = _maxCreatureDensity;
        _testColony.MaxMoveHormoneDensity = _maxHormoneDensity;

        DrawWorld();

        var secondsSinceUpdate = Time.time - _lastUpdateTime;
        var secondsBetweenUpdates = 1 / _updatesPerSecond;
        if (secondsSinceUpdate > secondsBetweenUpdates)
        {
            Debug.Log("Evolving world.");
            _worldModel.Evolve();
            _lastUpdateTime = Time.time;

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
            foreach (var colonyCell in cellContainer.ColonyCells)
            {
                GameObject colonyCube = _creatureCubes[cellContainer.Coord];
                float creatureRatio = (float)colonyCell.CreatureDensity / (float)colonyCell.Colony.MaxCreatureDensity;
                Color colonyColor = Color.Lerp(Color.black, Color.white, creatureRatio);
                colonyCube.GetComponent<Renderer>().material.color = colonyColor;

                GameObject hormoneCube = _hormoneCubes[cellContainer.Coord];
                float hormoneRatio = (float)colonyCell.MoveHormoneDensity / (float)colonyCell.Colony.MaxMoveHormoneDensity;
                Color hormoneColor = Color.Lerp(Color.black, Color.yellow, hormoneRatio);
                hormoneCube.GetComponent<Renderer>().material.color = hormoneColor;
            }
        }
    }

    private Vector2 GetPosition(int row, int col)
    {
        return new Vector2(col + 0.5f * row, 0.866025404f * row) * _renderedCellSize;
    }
}
