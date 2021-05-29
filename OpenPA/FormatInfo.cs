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
        public Encoding Encoding { get; init; }
        public PropList? PList {get; init; }

        internal unsafe static FormatInfo Convert(pa_format_info format_info)
        {
            FormatInfo info = new()
            {
                Encoding = format_info.encoding,
                PList = PropList.Convert(format_info.plist)
            };

            return info;
        }
    }
}
