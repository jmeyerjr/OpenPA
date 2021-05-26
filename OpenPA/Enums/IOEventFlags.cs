using System;

namespace OpenPA
{
    /// <summary>
    /// A bitmask for IO events
    /// </summary>
    [Flags]
    public enum IOEventFlags
    {
        NULL = 0,
        Input = 1,
        Output = 2,
        HangUp = 4,
        Error = 8,
    }
}