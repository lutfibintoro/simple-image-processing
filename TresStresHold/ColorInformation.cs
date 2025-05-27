
namespace TresStresHold
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    public class ColorInformation
    {
        public string? Path { get; set; }
        public ImageRGB[][]? DefaultColor { get; set; }
        public int[][]? GrayScaleColor { get; set; }
    }

    public class ImageRGB
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
    }
}
