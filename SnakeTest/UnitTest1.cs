using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Snake;

namespace SnakeTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Program.Position position = Program.newRandomPosition(100, 100);
            Program.Position newPosition = Program.newRandomPosition(100, 100); 
            Assert.AreNotEqual(position.col, newPosition.col);
        }

        [TestMethod]
        public void makeNewObstacleTest()
        {
            List<Program.Position> obstacles = new List<Program.Position>
            {       Program.newRandomPosition(22, 40),
                    Program.newRandomPosition(70, 100),
                    Program.newRandomPosition(95, 60),
                    Program.newRandomPosition(120, 80),
                    Program.newRandomPosition(200, 130),};
            Program.Position obstacle = Program.newRandomPosition(56, 150);
            obstacles.Add(obstacle);
            Assert.AreEqual(6, obstacles.Count);
        }
    }
}
