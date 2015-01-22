using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FillingStation.Extensions
{
    public static class ImageExtensions
    {
        public static void Rotate(this Image image, Rotation rotation)
        {
            var transform = image.LayoutTransform as RotateTransform;
            if (transform == null)
            {
                transform = new RotateTransform();
                image.LayoutTransform = transform;
            }

            transform.Angle = rotation.ToAngle();
        }
    }
}