using System;

namespace BeerO.SlackConnector.Models
{
    public class SlackThumbnail
    {
        public Uri Thumb64 { get; }
        public Uri Thumb80 { get; }
        public Uri Thumb360 { get; }
        public int Thumb360Width { get; }
        public int Thumb360Height { get; }
        public Uri Thumb160 { get; }
        public Uri Thumb360Gif { get; }

        public SlackThumbnail(
            Uri thumb64,
            Uri thumb80,
            Uri thumb360,
            int thumb360Width,
            int thumb360Height,
            Uri thumb160,
            Uri thumb360Gif)
        {
            this.Thumb64 = thumb64;
            this.Thumb80 = thumb80;
            this.Thumb360 = thumb360;
            this.Thumb360Width = thumb360Width;
            this.Thumb360Height = thumb360Height;
            this.Thumb160 = thumb160;
            this.Thumb360Gif = thumb360Gif;
        }
    }
}