using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BitmapOperations.Helper;
using BitmapOperations.Model;

namespace BitmapOperations.Controller
{
    public class BitmapOperationsController
    {
        public string InputPath { get; }
        public string OutputPath { get; }
        public string OperationsPath { get; }
        public FileSystemInfo[] BitmapFilesPath { get; private set; }
        public Operation BitmapOperation { get; set; }

        public BitmapOperationsController(string inputPath = @"Input Files\", string outputPath = @"Output Files\", string operationsPath = @"Operations.txt")
        {
            InputPath = inputPath;
            OutputPath = outputPath;
            OperationsPath = operationsPath;
            CreateOutputFolder();
        }

        /// <summary>Loads the bitmap paths, and fiter in case of illegal file types
        /// </summary>
        public void LoadBitmapPaths()
        {
            DirectoryInfo directoryInfoi = new DirectoryInfo(InputPath);
            FileSystemInfo[] fileSystemInfos = directoryInfoi.GetFileSystemInfos();

            var maxLength = fileSystemInfos.Max(f => f.Name.Length);

            BitmapFilesPath =
                fileSystemInfos.Where(f => f.Extension.Equals(".bmp"))
                    .OrderBy(f => f.Name.PadLeft(maxLength, '0'))
                    .ToArray();

            if (BitmapFilesPath.Length == 0)
            {
                throw new Exception("Invalid files found in input folder or no files found");
            }
        }


        /// <summary>Loads the operation File that contains the pixel marks and rotation angle
        /// </summary>
        public void LoadOperations()
        {
            BitmapOperation = new Operation();

            BitmapOperation.LoadOperations(OperationsPath);
        }

        /// <summary>Retuns an input compressed bitmap destined for UI usage 
        /// <param name="index">Takes a zero based index and points to the position in File Path</param>
        /// </summary>
        public Bitmap GetInputBitmap(int index)
        {
            var path = BitmapFilesPath[index].FullName;
            return new Bitmap(ReadAndConvert(path));
        }

        /// <summary>Retuns an output compressed bitmap destined for UI usage 
        /// <param name="index">Takes a zero based index and points to the position in File Path</param>
        /// <param name="nameInput">Name of the input file</param>
        /// <param name="nameOutput">Name of the output file</param>
        /// </summary>
        public Bitmap GetOutputBitmap(int index, string nameInput = "Input", string nameOutput = "Output")
        {
            var path = BitmapFilesPath[index].FullName.Replace(InputPath, OutputPath).Replace(nameInput, nameOutput);
            return new Bitmap(ReadAndConvert(path));
        }

        /// <summary>Colors and Rotates all Bitmaps that use the specified input files
        /// </summary>
        public void ColorAndRotateAll()
        {
            Parallel.For(0, BitmapFilesPath.Length, ColorAndRotateAtIndex);
        }

        /// <summary>Colors and Rotates a Bitmap using the specified input files
        /// <param name="index">Takes a zero based index and points to the position in File Path</param>
        /// </summary>
        public unsafe void ColorAndRotateAtIndex(int index)
        {
            // Marks are referenced locally for faster access
            var marks = BitmapOperation.Marks.ToArray();

            Bitmap bitmap = new Bitmap(BitmapFilesPath[index].FullName);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int bytesPerPixel = Image.GetPixelFormatSize(PixelFormat.Format24bppRgb) / 8;
            int height = bitmapData.Height;
            int stride = bitmapData.Stride;
            int widthInBytes = bitmapData.Width * bytesPerPixel;

            byte* firstPixel = (byte*)bitmapData.Scan0;

            for (int y = 0; y < height; y++)
            {
                byte* pixelLine = firstPixel + y * stride;

                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    var curentPixel = (pixelLine[x] << 16) | (pixelLine[x + 1] << 8) | pixelLine[x + 2];

                    foreach (Mark pixelMark in marks)
                    {
                        if (pixelMark.SourcePixelInt != curentPixel) continue;

                        pixelLine[x] = pixelMark.TargetPixelBytes[2];     //Blue  0-255
                        pixelLine[x + 1] = pixelMark.TargetPixelBytes[1]; //Green 0-255
                        pixelLine[x + 2] = pixelMark.TargetPixelBytes[0]; //Red   0-255
                        break;
                    }
                }
            }

            bitmap.UnlockBits(bitmapData);

            bitmap.RotateFlip(BitmapOperation.GetRotateFlipType());

            var filename = Environment.CurrentDirectory + @"\" + OutputPath + @"Output-" + (index + 1) + ".bmp";
            bitmap.Save(filename, ImageFormat.Bmp);

            // Memory cleanup
            bitmap.Dispose();
            bitmapData = null;
            bitmap = null;

            GC.Collect();
        }


        /// <summary>Creates the output folder in case it is not created
        /// </summary>
        private void CreateOutputFolder()
        {
            var exists = Directory.Exists(OutputPath);

            if (!exists)
                Directory.CreateDirectory(OutputPath);
        }

        /// <summary>Opens a file stream and converts the image from it to Jpeg. For UI operations
        /// </summary>
        private Stream ReadAndConvert(string path)
        {
            return File.OpenRead(path).ConvertImage(ImageFormat.Jpeg);
        }
    }
}
