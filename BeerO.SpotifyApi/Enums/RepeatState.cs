using System;

namespace BeerO.SpotifyApi.Enums
{
    [Flags]
    public enum RepeatState
    {
        [String("track")]
        Track = 1,

        [String("context")]
        Context = 2,

        [String("off")]
        Off = 4
    }
}