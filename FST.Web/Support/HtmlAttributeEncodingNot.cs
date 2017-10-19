using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// We're using this to stop the ASP engine from HTMLEncoding the values set for client side control events.
/// This is because somewhere in the code (probably the file uploader grid views) we're setting
/// some javascript into the control onclick client event. This could be conducive to XSS. To fix this,
/// it would probably serve us to either work with registered scripts for the drop downs in the evidence
/// and locus controls.
/// </summary>
public class HtmlAttributeEncodingNot : System.Web.Util.HttpEncoder
{
    protected override void HtmlAttributeEncode(string value, System.IO.TextWriter output)
    {
        output.Write(value);
    }
}
