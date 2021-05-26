using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA.Enums
{
    public enum UpdateMode
    {
        // Replace the entire property list with the new one. Don't key
        // any of the old data around.
        SET,
        // Merge ner property list into the existing one, not replacing
        // any old entries if they share a common key with the new
        // property list.
        MERGE,
        // Merge new property list into the existing one, replacing all
        // old entries that share a common key with the new property
        // list.
        UPDATE_REPLACE,
    }
}
