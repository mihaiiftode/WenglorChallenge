using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BitmapOperations.Helper
{
    public static class StreamExtensions
    {
        public static Stream ConvertImage(this Stream originalStream, ImageFormat format)
        {
            var image = Image.FromStream(originalStream);

            var stream = new MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }
    }
}