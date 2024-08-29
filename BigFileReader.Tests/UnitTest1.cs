namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GenerateTestFile()
        {
            var r = new Random(DateTime.Now.Microsecond);
            var file = new StreamWriter(@"..\..\..\..\BIG_FILE.TXT");

            for (int i = 0; i < 40000000; i++)
            {


                file.WriteLine($"{i}  0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 -");

            }

            file.Close();
        }
    }
}
