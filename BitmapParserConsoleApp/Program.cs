using BitmapOperations.Controller;

namespace BitmapParserConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            BitmapOperationsController controller = new BitmapOperationsController();
            controller.LoadBitmapPaths();
            controller.LoadOperations();
            controller.ColorAndRotateAll();
        }
    }
}
