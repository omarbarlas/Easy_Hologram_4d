using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Easy_Hologram_4D
{
    class Utility
    {

        public static ImageSource RotateDynaImage(ImageSource imageSrc, double Angle)
        {
            Image image = new Image();
            image.Source = imageSrc;

            CompositeTransform RT = new CompositeTransform();
            RT = (CompositeTransform)image.RenderTransform;
            RT.Rotation += Angle;
            image.RenderTransform = RT;

            return image.Source;
        }


    }


}
