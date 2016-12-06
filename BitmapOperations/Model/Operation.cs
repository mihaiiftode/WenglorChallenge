using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using BitmapOperations.Helper;

namespace BitmapOperations.Model
{
    public class Operation
    {
        public List<Mark> Marks { get; private set; }

        public int RotationAngle { get; private set; }

        public Operation()
        {
            Marks = new List<Mark>();
        }

        /// <summary>Loads the Marks List and Rotation angle to degrees
        /// <param name="filePath">The file path for the operations file</param>
        /// </summary>
        public void LoadOperations(string filePath)
        {
            var fileOperations = File.ReadAllLines(filePath).ToList();

            List<byte[]> marks = new List<byte[]>();
            int rotationAngle = 0;

            try
            {
                fileOperations.ForEach(operation =>
                {
                    var elements = operation.Split(' ').Where(e => !e.Equals(string.Empty)).ToList();
                    if (elements.Contains("mark"))
                    {
                        elements.Remove("mark");
                        var element = elements.Select(byte.Parse).ToArray();
                        if (element.Length != 6) throw new Exception();
                        marks.Add(element);
                    }
                    else
                    {
                        elements.Remove("rotate");
                        rotationAngle += elements.Select(int.Parse).ElementAt(0);
                    }
                });
            }
            catch (Exception exception)
            {
                throw new Exception("Invalid elements in file", exception);
            }

            RotationAngle = rotationAngle;

            SetMarkObjectList(marks);
        }


        /// <summary>Get rotation type for the Bitmap
        /// </summary>
        public RotateFlipType GetRotateFlipType()
        {
            RotationAngle = RotationAngle % 360;
            if (RotationAngle < 0)
            {
                RotationAngle += 360;
            }

            switch (RotationAngle)
            {
                case 90:
                    return RotateFlipType.Rotate90FlipNone;
                case 180:
                    return RotateFlipType.Rotate180FlipNone;
                case 270:
                    return RotateFlipType.Rotate270FlipNone;
                default:
                    return RotateFlipType.RotateNoneFlipNone;
            }
        }


        /// <summary>Loads the Marks list with the required object and eliminates distinct values based on the source values
        /// </summary>
        private void SetMarkObjectList(List<byte[]> marks)
        {
            Marks =
                marks.ConvertAll(
                    input => new Mark(new[] { input[0], input[1], input[2] }, new[] { input[3], input[4], input[5] }
                        ));
            Marks.ForEach(mark => mark.ShiftSource());
            Marks = Marks.DistinctBy(mark => mark.SourcePixelInt).ToList();
        }
    }
}