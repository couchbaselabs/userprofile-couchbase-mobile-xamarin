using System;
using System.IO;
using Xamarin.Forms;

namespace UserProfileDemo.Converters
{
    public class ByteToImageFieldConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource retSource = null;

            try
            {
                if (value != null)
                {
                    retSource = ImageSource.FromStream(() => new MemoryStream((byte[])value));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ByteToImageFieldConverter Exception: {ex.Message}");
            }

            return retSource ?? "profile_placeholder.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) 
            => throw new NotImplementedException();
    }
}
