using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Tsr.Web.Common
{
    public static class Others
    {
        public static string GetFullName(string FirstName,string MiddleName,string LastName)
        {
            return FirstName + " "+ MiddleName + " " + LastName;
        }

        public static bool IsFileReady(String sFilename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    if (inputStream.Length > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}