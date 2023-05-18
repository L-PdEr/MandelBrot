using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MandelBrot;

public partial class MainWindowViewModel : INotifyPropertyChanged
{
    // Define the size and resolution of the image
    private const int width = 1920;
    private const int height = 1080;
    private const double dpi = 96;

    // Define the parameters of the Mandelbrot set
    private const double xmin = -2.5;
    private const double xmax = 1.0;
    private const double ymin = -1.0;
    private const double ymax = 1.0;
    private const int maxIterations = 100;

    // The bitmap that holds the image data
    private WriteableBitmap bitmap;

    // The property that exposes the bitmap to the view
    public WriteableBitmap Bitmap
    {
        get => bitmap;
        set => SetProperty(ref bitmap, value);
    }

    public MainWindowViewModel()
    {
        // Create a new bitmap and assign it to the property
        Bitmap = new WriteableBitmap(width, height, dpi, dpi, PixelFormats.Bgra32, null);
        // Generate the image data and write it to the bitmap
        GenerateImage();

    }
    private void GenerateImage()
    {
        // Create an array to store the pixel colors
        byte[] pixels = new byte[width * height * 4];
        // Loop through each pixel
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Map the pixel coordinates to the complex plane
                double cx = xmin + (xmax - xmin) * x / width;
                double cy = ymin + (ymax - ymin) * y / height;
                // Initialize the complex number z
                double zx = 0;
                double zy = 0;
                // Initialize the iteration count
                int i = 0;
                // Iterate until z escapes the circle of radius 2 or reaches the maximum iterations
                while (zx * zx + zy * zy < 4 && i < maxIterations)
                {
                    // Apply the Mandelbrot formula: z = z^2 + c
                    double temp = zx * zx - zy * zy + cx;
                    zy = 2 * zx * zy + cy;
                    zx = temp;
                    i++;
                }
                // Choose a color based on the iteration count
                Color color = ColorFromIteration(i);
                // Set the pixel color in the array
                int index = (y * width + x) * 4;
                pixels[index] = color.B;
                pixels[index + 1] = color.G;
                pixels[index + 2] = color.R;
                pixels[index + 3] = color.A;
            }
        }

        // Write the pixel array to the bitmap
        Bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
    }
    
    // A helper method to generate a color from an iteration count
    private Color ColorFromIteration(int i)
    {
        if (i == maxIterations)
        {
            // Return black for points inside the set
            return Colors.Black;
        }
        else
        {
            // Return a gradient of colors for points outside the set
            byte r = (byte)(255 * i / maxIterations);
            byte g = (byte)(255 - r);
            byte b = (byte)(255 - r / 2);
            return Color.FromArgb(255, r, g, b);
        }
    }
    // The event that notifies when a property changes
    public event PropertyChangedEventHandler PropertyChanged;
    // A helper method to set a property and raise the event if needed
    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (!Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}


