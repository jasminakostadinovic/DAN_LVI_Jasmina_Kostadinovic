using System;

namespace DownloadHTML.DataValidations
{
    class DataValidation
    {
        public static bool CheckURLValid(string strURL)
        {
            return Uri.TryCreate(strURL, UriKind.RelativeOrAbsolute, out Uri uriResult);
        }
    }
}
