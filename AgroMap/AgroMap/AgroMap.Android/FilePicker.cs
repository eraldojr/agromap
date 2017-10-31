
using AgroMap.Droid;
using Android.Graphics;
using Android.Content;
using Android.Net;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(FilePicker))]
namespace AgroMap.Droid
{
    class FilePicker : IFilePicker
    {

        public Task<Stream> GetFileFromLibrary()
        {
            // Define the Intent for getting images
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);

            // Get the MainActivity instance
            MainActivity activity = Forms.Context as MainActivity;

            // Start the picture-picker activity (resumes in MainActivity.cs)
            activity.StartActivityForResult(
                Intent.CreateChooser(intent, "Select Picture"),
                MainActivity.PickImageId);

            // Save the TaskCompletionSource object as a MainActivity property
            activity.PickImageTaskCompletionSource = new TaskCompletionSource<Stream>();

            // Return Task object
            return activity.PickImageTaskCompletionSource.Task;
        }

        public void SaveFile(string fileName, Stream photo_stream)
        {
            fileName = fileName + ".png";
            string imageFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), "agromap");

            if (!System.IO.Directory.Exists(imageFolderPath))
            {
                System.IO.Directory.CreateDirectory(imageFolderPath);
            }
            string imagefilePath = System.IO.Path.Combine(imageFolderPath, fileName);

            try
            {
                var memoryStream = new MemoryStream();
                photo_stream.CopyTo(memoryStream);
                var photo_bytes = memoryStream.ToArray();
                System.IO.File.WriteAllBytes(imagefilePath, photo_bytes);
            }
            catch (System.Exception err)
            {
                Debug.WriteLine("AGROMAP|DROID|FilePicker.cs|SaveFile: " + err.Message);
            }
            return;
        }

        public ImageSource GetFile(string fileName)
        {
            fileName = fileName + ".png";
            try
            {
                string imageFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), "agromap");

                string imagefilePath = System.IO.Path.Combine(imageFolderPath, fileName);

                byte[] imgBytes = System.IO.File.ReadAllBytes(imagefilePath);
                return ImageSource.FromStream(() => new MemoryStream(imgBytes));
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|DROID|FilePicker|GetFile(): " + err.Message);
            }
            return null;
        }

        public string GetFilePath(string uuid)
        {
            string fileName = uuid + ".png";
            string imageFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), "agromap");
            return System.IO.Path.Combine(imageFolderPath, fileName);
        }

        public Boolean DeleteFile(string fileName)
        {
            fileName = fileName + ".png";
            try
            {
                string imageFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), "agromap");

                string imagefilePath = System.IO.Path.Combine(imageFolderPath, fileName);

                System.IO.File.Delete(imagefilePath);
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|DROID|FilePicker|DeleteFile(): " + err.Message);
            }
            return false;
        }

        public void DeleteDirectory()
        {
            string imageFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), "agromap");
            if (System.IO.Directory.Exists(imageFolderPath))
            {
                System.IO.Directory.Delete(imageFolderPath, true);
            }
        }
    }
}