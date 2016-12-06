using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BitmapParserWPF.Converters
{
    [ValueConversion(typeof(Image), typeof(BitmapSource))]
    public class ImageToBitmapSourceConverter : IValueConverter
    {
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object result = null;
            if (value == null) return null;

            try
            {
                Image myImage = (Image)value;

                var bitmap = new Bitmap(myImage);
                IntPtr bmpPt = bitmap.GetHbitmap();
                BitmapSource bitmapSource =
                    System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        bmpPt,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                //freeze bitmapSource and clear memory to avoid memory leaks
                bitmapSource.Freeze();
                DeleteObject(bmpPt);

                result = bitmapSource;
            }
            catch (Exception)
            {
                // ignored
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
