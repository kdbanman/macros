using System;
using System.Collections.Generic;

using NUnit.Framework;

using HexEngine;

namespace NUnitTests
{
    /// <summary>
    ///     These tests assume rectangular hex grid in the positive quadrant with acute origin at (0, 0).
    /// </summary>
    [TestFixture]
    public class HexWorldTests
    {
        private HexWorld _worldModel;
        private Faction _testFaction;
        private int width = 50;
        private int height = 33;

        [SetUp]
        public void SetUp()
        {
            _testFaction = new Faction(0, "test faction");

            _worldModel = new HexWorld(width, height);

            // test dummy data
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

        [Test]
        public void TestAcuteNeighborComputation()
        {
            // acute corner @ origin
            var nbrs = _worldModel.ComputeNeighborPositions(0, 0);
            Assert.That(nbrs, Is.EquivalentTo(new[] { new Coord(0, 1), new Coord(1, 0) }));
        }

        [Test]
        public void TestObtuseNeighborComputation()
        {
            // interior @ 1,1
            var nbrs = _worldModel.ComputeNeighborPositions(1, 1);
            Assert.That(nbrs, Is.EquivalentTo(new[]
            {
                new Coord(0, 1),
                new Coord(0, 2),
                new Coord(1, 2),
                new Coord(2, 1),
                new Coord(2, 0),
                new Coord(1, 0)
            }));
        }

        [Test]
        public void TestEdgeNeighborComputation1()
        {
            // edge @ left edge middle
            var row = _worldModel.RowCount / 2;
            var nbrs = _worldModel.ComputeNeighborPositions(row, 0);
            Assert.That(nbrs, Is.EquivalentTo(new[]
            {
                new Coord(row - 1, 0),
                new Coord(row - 1, 1),
                new Coord(row, 1),
                new Coord(row + 1, 0)
            }));
        }

        [Test]
        public void TestEdgeNeighborComputation2()
        {
            // edge corner at 0,1
            var nbrs = _worldModel.ComputeNeighborPositions(0, 1);
            Assert.That(nbrs, Is.EquivalentTo(new[]
            {
                new Coord(0, 0),
                new Coord(1, 0),
                new Coord(1, 1),
                new Coord(0, 2)
            }));
        }

        [Test]
        public void TestInteriorNeighborComputation()
        {
            // obtuse corner @ bottom right corner
            var col = _worldModel.ColCount - 1;
            var nbrs = _worldModel.ComputeNeighborPositions(0, col);
            Assert.That(nbrs, Is.EquivalentTo(new[]
            {
                new Coord(0, col - 1),
                new Coord(1, col - 1),
                new Coord(1, col)
            }));
        }
    }
}
