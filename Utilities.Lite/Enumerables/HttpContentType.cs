using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Enumerables
{
    public static class InternalHttpContent
    {
        internal static Dictionary<HttpContentType, string> httpContentTypeTranslator = new Dictionary<HttpContentType, string>()
        {
            { HttpContentType.application_json,"application/json" },
            { HttpContentType.application_javascript,"application/javascript" },
            { HttpContentType.application_xml,"application/application_xml" },
            { HttpContentType.application_x_www_form_urlencoded,"application/x-www-form-urlencoded" },
            { HttpContentType.text_xml,"text/xml" },
            { HttpContentType.text_html,"text/html"},
            { HttpContentType.text_plain,"text/plain" },
            { HttpContentType.multipart_formdata,"multipart/form-data" }
        };
        /// <summary>
        /// Contain enumerable that describe to MIME type
        /// </summary>
        public enum HttpContentType
        {
            application_json,
            application_javascript,
            application_xml,
            application_x_www_form_urlencoded,
            text_xml,
            text_html,
            text_plain,
            multipart_formdata
        }
    }

}
