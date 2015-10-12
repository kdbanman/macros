using UnityEngine;
using System.Collections;

using HexEngine;

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

    private Faction _testFaction;

    // Use this for initialization
    void Start()
    {
        _framesSinceUpdate = 0;

        _worldModel = new HexWorld(width, height);

        // test dummy data
        _testFaction = new Faction(0, "test faction");

        _worldModel.AddFaction(_testFaction);
        
        _worldModel.AddCreatureCells(_testFaction, new Coord(1, 1), 30);
        _worldModel.AddCreatureCells(_testFaction, new Coord(0, 0), 10);
        _worldModel.AddCreatureCells(_testFaction, new Coord(2, 2), 40);
        _worldModel.AddCreatureCells(_testFaction, new Coord(3, 3), 30);

        _worldModel.AddMoveHormone(_testFaction, new Coord(1, 1), 3);
        _worldModel.AddMoveHormone(_testFaction, new Coord(1, 0), 5);
        _worldModel.AddMoveHormone(_testFaction, new Coord(2, 0), 35);
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
        }
    }

    void OnDrawGizmos()
    {
        if (_worldModel == null)
            return;

        foreach (var cellContainer in _worldModel.CurrentCells)
        {
            Vector2 position = GetPosition(cellContainer.Row, cellContainer.Col);
            foreach (var factionCell in cellContainer.Cells)
            {
                if (factionCell.CreatureCellDensity > 0)
                    Gizmos.color = Color.white;
                else
                    Gizmos.color = Color.black;
                Gizmos.DrawCube(position, new Vector3(cellSize, cellSize, cellSize * 0.3f));

                if (factionCell.MoveHormoneDensity > 0)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.gray;
                Gizmos.DrawCube(position, new Vector3(cellSize / 2, cellSize / 2, cellSize));
            }
        }
    }
    
    public Vector2 GetPosition(int row, int col)
    {
        return new Vector2(col + 0.5f * row, 0.866025404f * row) * cellSize;
    }
}
