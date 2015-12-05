using UnityEngine;
using System.Collections.Generic;

using HexEngine;
using System;

public class Map : MonoBehaviour
{
    [Range(1, 100)]
    public int _wordWidth = 20;
    [Range(1,100)]
    public int _worldHeight = 30;

    [Range(0, 50)]
    public float _generationsPerSecond = 1;
    private float _lastEvolveTime = 0;
    private float _lastUpdateTime = 0;

    [Range(0.1f, 50)]
    public float _renderedCellSize = 5;

    // test colony props
    [Range(1, 50000)]
    public int _maxCreatureDensity = 5000;
    [Range(1, 50000)]
    public int _maxHormoneDensity = 10000;

    [Range(0, 10)]
    public double _creatureMoveRate = 2.0;
    [Range(0, 2)]
    public double _creatureReboundRatio = 0.2;
    [Range(0, 0.1f)]
    public double _creatureMultiplicationRate = 0.005;

    [Range(0, 5)]
    public double _hormoneEvaporationRatio = 0.05;
    [Range(0, 1)]
    public double _hormoneDissipationRatio = 0.5;
    [Range(0, 1)]
    public double _hormoneLeavingCreatureDepositionRatio = 0.3;
    [Range(0, 1)]
    public double _hormoneArrivingCreatureDepositionRatio = 0.3;

    [Range(0, 10000)]
    public int _hormoneTotemDepositionRate = 200;
    // end test colony props

    private HexWorld _worldModel;

    private Colony _playerColony;

    // PROTOTYPE: These are just here to render some world cells with shape and color.
    // Real stuff with particle effects or other wizardry will replace them.
    private IDictionary<Coord, GameObject> _creatureCubes = new Dictionary<Coord, GameObject>();
    private IDictionary<Coord, GameObject> _hormoneCubes = new Dictionary<Coord, GameObject>();

    // Use this for initialization
    void Start()
    {

        _worldModel = new HexWorld(_wordWidth, _worldHeight);

        _playerColony = new Colony(0, "test colony");

        _worldModel.AddColony(_playerColony);

        // add unity objects to each cell
        _worldModel.ForEachPosition((coord) =>
        {

            GameObject creatureCube = CreateCube(coord, new Vector3(_renderedCellSize / 2, _renderedCellSize / 2, _renderedCellSize));
            GameObject hormoneCube = CreateCube(coord, new Vector3(_renderedCellSize, _renderedCellSize, _renderedCellSize * 0.3f));

            _creatureCubes.Add(coord, creatureCube);
            _hormoneCubes.Add(coord, hormoneCube);
        });

        // test dummy data
        _worldModel.AddCreatures(_playerColony, new Coord(3, 0), 4000);
        _worldModel.AddCreatures(_playerColony, new Coord(3, 1), 3000);
        _worldModel.AddCreatures(_playerColony, new Coord(3, 2), 2000);
        _worldModel.AddCreatures(_playerColony, new Coord(3, 3), 1000);

        _playerColony.HormoneTotemPosition = new Coord(6, 4);
        // end dummy data
    }

    private GameObject CreateCube(Coord coord, Vector3 size)
    {
        var cellPosition = GetHexGridPosition(coord.Row, coord.Col);

        GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.localScale = size;
        Cube.transform.position = cellPosition;
        Cube.GetComponent<Renderer>().material.color = Color.black;

        var totemPositioner = Cube.AddComponent<OnClickTotemPositioner>();
        totemPositioner.Coord = coord;
        totemPositioner.Colony = _playerColony;

        return Cube;
    }

    // Update is called once per frame
    void Update()
    {
        if (_worldModel == null)
            return;

        UpdateTestColonyProps();

        DepositFromTotem();

        EvolveWorld();

        DrawWorld();

        _lastUpdateTime = Time.time;
    }

    private void DepositFromTotem()
    {
        if (!Input.GetKey(KeyCode.Space))
            return;

        // Deposit this frame's fraction of the totem's hormone for this generation
        var secondsSinceUpdate = Time.time - _lastUpdateTime;
        var secondsBetweenGenerations = 1 / _generationsPerSecond;
        var totemDeposition = _playerColony.HormoneTotemDepositionRate * secondsSinceUpdate / secondsBetweenGenerations;
        _worldModel.AddMoveHormone(_playerColony, _playerColony.HormoneTotemPosition, (int)totemDeposition);
    }

    private void UpdateTestColonyProps()
    {
        _playerColony.MaxCreatureDensity = _maxCreatureDensity;
        _playerColony.MaxMoveHormoneDensity = _maxHormoneDensity;

        _playerColony.CreatureMoveRate = _creatureMoveRate;
        _playerColony.CreatureReboundRatio = _creatureReboundRatio;
        _playerColony.CreatureMultiplicationRate = _creatureMultiplicationRate;

        _playerColony.HormoneEvaporationRatio = _hormoneEvaporationRatio;
        _playerColony.HormoneDissipationRatio = _hormoneDissipationRatio;
        _playerColony.HormoneLeavingCreatureDepositionRatio = _hormoneLeavingCreatureDepositionRatio;
        _playerColony.HormoneArrivingCreatureDepositionRatio = _hormoneArrivingCreatureDepositionRatio;

        _playerColony.HormoneTotemDepositionRate = _hormoneTotemDepositionRate;
    }

    private void EvolveWorld()
    {
        var secondsSinceEvolution = Time.time - _lastEvolveTime;
        var secondsBetweenGenerations = 1 / _generationsPerSecond;
        if (secondsSinceEvolution > secondsBetweenGenerations) {
            Debug.Log("Evolving world.");
            _worldModel.Evolve();
            _lastEvolveTime = Time.time;
        }
    }

    private void DrawWorld()
    {
        if (_worldModel == null)
            return;

        foreach (var cellContainer in _worldModel.CurrentCells)
        {
            foreach (var colonyCell in cellContainer.ColonyCells)
            {
                GameObject colonyCube = _creatureCubes[cellContainer.Coord];
                float creatureRatio = (float)colonyCell.CreatureDensity / (float)colonyCell.Colony.MaxCreatureDensity;
                Color colonyColor;
                if (creatureRatio < 1)
                    colonyColor = Color.Lerp(Color.black, Color.white, creatureRatio);
                else
                    colonyColor = Color.Lerp(Color.white, Color.green, creatureRatio - 1);
                colonyCube.GetComponent<Renderer>().material.color = colonyColor;

                GameObject hormoneCube = _hormoneCubes[cellContainer.Coord];
                float hormoneRatio = (float)colonyCell.MoveHormoneDensity / (float)colonyCell.Colony.MaxMoveHormoneDensity;
                Color hormoneColor = Color.Lerp(Color.black, Color.yellow, hormoneRatio);
                hormoneCube.GetComponent<Renderer>().material.color = hormoneColor;
            }
        }
    }

    private Vector2 GetHexGridPosition(int row, int col)
    {
        return new Vector2(col + 0.5f * row, 0.866025404f * row) * _renderedCellSize;
    }

    private class OnClickTotemPositioner : MonoBehaviour
    {
        public OnClickTotemPositioner(Coord coord, Colony colony)
        {
            Coord = coord;
            Colony = colony;
        }

        public Coord Coord { get; set; }
        public Colony Colony { get; set; }

        void OnMouseOver()
        {
            if (Colony != null)
                Colony.HormoneTotemPosition = Coord;
        }
    }
}
