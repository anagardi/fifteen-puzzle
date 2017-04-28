using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fifteen;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace FifteenTests
{
    [TestClass]
    public class MovementHelperTests
    {
        private TestContext _testContextInstance;

        private Canvas _testCanvas;
        private Collection<int> _testPuzzle;
        private Collection<int>[] _solvableCollections, _unSolvableCollections;
        private Collection<int> _orderedCollection, _disOrderdCollection;

        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value; 
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _testCanvas = new Canvas();
            _testPuzzle = MovementHelper.GeneratePuzzle();

            _solvableCollections = new Collection<int>[] {
                new Collection<int> { 8, 12, 6, 13, 14, 0, 15, 9, 7, 2, 11, 10, 1, 5, 3, 4 },
                new Collection<int> { 1, 13, 8, 6, 2, 9, 7, 5, 15, 3, 14, 10, 0, 11, 4, 12 },
                new Collection<int> { 13, 0, 9, 4, 12, 1, 8, 6, 14, 11, 15, 7, 3, 5, 2, 10 },
                new Collection<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 15, 13, 14, 0 },
                new Collection<int> { 10, 3, 15, 0, 11, 1, 2, 14, 13, 4, 8, 9, 12, 6, 7, 5 },
                new Collection<int> { 5, 12, 11, 9, 3, 0, 4, 15, 1, 7, 13, 8, 14, 10, 6, 2 }
            };

            _unSolvableCollections = new Collection<int>[] {
                new Collection<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 14, 0 },
                new Collection<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 13, 15, 0 },
                new Collection<int> { 3, 9, 1, 15, 14, 11, 4, 6, 13, 0, 10, 12, 2, 7, 8, 5 },
                new Collection<int> { 0, 2, 1, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 13, 15 },
                new Collection<int> { 0, 1, 3, 2, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 13, 15 },
                new Collection<int> { 0, 1, 2, 4, 3, 5, 7, 6, 9, 8, 10, 11, 12, 14, 13, 15 }
            };

            _orderedCollection = new Collection<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0 };
            _disOrderdCollection = new Collection<int> { 3, 9, 1, 14, 15, 11, 7, 6, 13, 0, 10, 12, 2, 4, 8, 5 };
        }

        [TestMethod()]
        public void GeneratePuzzleTest()
        {          
           
            Assert.IsNotNull(_testPuzzle);
            
            Assert.IsTrue(_testPuzzle.Count == MovementHelper.GridWidth * MovementHelper.GridWidth);
            
            for (int i = 0; i < _testPuzzle.Count; i++)
            {
                Assert.IsTrue(_testPuzzle.Contains(i));
            }

            Assert.IsTrue(MovementHelper.IsSolvable(_testPuzzle));
        }

        [TestMethod()]
        public void IsSolvableTest()
        {
            foreach (Collection<int> collection in _solvableCollections)
            {
                Assert.IsTrue(MovementHelper.IsSolvable(collection));
            }
        }

        [TestMethod()]
        public void IsNotSolvableTest()
        {
            foreach (Collection<int> collection in _unSolvableCollections)
            {
                Assert.IsFalse(MovementHelper.IsSolvable(collection));
            }
        }

        [TestMethod()]
        public void ElementsAreOrderedTest()
        {
            MovementHelper.AddElementsToTarget(_orderedCollection, _testCanvas);
            Assert.IsTrue(MovementHelper.ElementsAreOrdered(_testCanvas));

            MovementHelper.AddElementsToTarget(_disOrderdCollection, _testCanvas);
            Assert.IsFalse(MovementHelper.ElementsAreOrdered(_testCanvas));
        }        
    }   
}
