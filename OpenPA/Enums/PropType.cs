using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA.Enums
{
    public enum PropType
    {
        // Integer property
        INT,
        // Integer range property
        INT_RANGE,
        // Integer array property
        INT_ARRAY,
        // String property
        STRING,
        // String array property
        STRING_ARRAY,
        // Represents an invalid type
        INVALID = -1,
    }
}
