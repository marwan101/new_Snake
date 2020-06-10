using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snake;
using System.Collections.Generic;

namespace SnakeStoredScoreTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void snakeStoredScoreTest()
        {
            Queue<Program.Position> snakeBody;
            snakeBody = new Queue<Program.Position>();
            for (int i = 0; i <= 3; i++)
            {
                snakeBody.Enqueue(new Program.Position(1, i));
            }
            Assert.AreEqual(300, Program.calculateUserPoints(snakeBody, 0, 1));
        }
    }
}
