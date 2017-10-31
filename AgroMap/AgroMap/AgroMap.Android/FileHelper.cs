using Xamarin.Forms;
using AgroMap.Droid;
using System.IO;

[assembly: Dependency(typeof(FileHelper))]

namespace AgroMap.Droid
{
    public class FileHelper: IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }
    }
}
    