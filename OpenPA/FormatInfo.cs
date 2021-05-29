using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenPA.Enums;
using OpenPA.Native;

namespace OpenPA
{
    public class FormatInfo
    {
        /// <summary>
        /// Encoding of the format
        /// </summary>
        public Encoding Encoding { get; init; }

        /// <summary>
        /// Properties of this format
        /// </summary>
        public PropList? PList {get; init; }

        /// <summary>
        /// Copies the unmanaged structure into a managed object
        /// </summary>
        /// <param name="format_info">Unamanaged pa_format_info structure to copy</param>
        /// <returns>FormatInfo object</returns>
        internal unsafe static FormatInfo Convert(pa_format_info format_info)
        {
            // Create and populate a FormatInfo object with the data
            // from the unmanaged structure
            FormatInfo info = new()
            {
                Encoding = format_info.encoding,
                PList = PropList.Convert(format_info.plist)
            };

            // Return the FormatInfo object
            return info;
        }
    }
}
