using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
