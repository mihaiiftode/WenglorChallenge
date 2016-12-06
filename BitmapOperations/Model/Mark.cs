namespace BitmapOperations.Model
{
    public class Mark
    {
        // Takes the RGB bytes to search for, in total 3 bytes per array
        public byte[] SourcePixelBytes { get; }

        // Shifted SourcePixel array for extra performance when checking
        public int SourcePixelInt { get; private set; }

        // The RGB to replace inside the bmp
        // Target Bytes are not shifted because the pixel its stored in a 24bit space
        public byte[] TargetPixelBytes { get; private set; }

        public Mark(byte[] sourcePixelBytes, byte[] targetPixelBytes)
        {
            SourcePixelBytes = sourcePixelBytes;
            TargetPixelBytes = targetPixelBytes;
        }

        public void ShiftSource()
        {
            // Bytes are shifted from the end to start, because in memory the pixels are stored in BGR format
            // I chose to shift just the source bytes for faster comparison
            SourcePixelInt = (SourcePixelBytes[2] << 16) | (SourcePixelBytes[1] << 8) | SourcePixelBytes[0];
        }
    }
}