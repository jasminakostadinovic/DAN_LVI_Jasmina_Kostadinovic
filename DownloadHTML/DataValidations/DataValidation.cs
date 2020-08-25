using System;

namespace DownloadHTML.DataValidations
{
    class DataValidation
    {
        public static bool CheckURLValid(string uriName)
        {
            Uri uriResult;
            return Uri.TryCreate(uriName, UriKind.Absolute, out uriResult)
                     && (uriResult.Scheme == Uri.UriSchemeHttp
                     || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}