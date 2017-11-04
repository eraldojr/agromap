using AgroMap.Resources;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;
using Android;
using Java.IO;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AgroMap.Services
{
    class PhotoService
    {        
        private static CognitoAWSCredentials Credentials;
        private static IAmazonS3 S3Client;
        private static RegionEndpoint REGION = RegionEndpoint.USEast2;
        public const HttpStatusCode NO_SUCH_BUCKET_STATUS_CODE = HttpStatusCode.NotFound;
        public const HttpStatusCode BUCKET_ACCESS_FORBIDDEN_STATUS_CODE = HttpStatusCode.Forbidden;
        public const HttpStatusCode BUCKET_REDIRECT_STATUS_CODE = HttpStatusCode.Redirect;
        private static string camera = Manifest.Permission.Camera;

        private static void GetCredentials()
        {
            try
            {
                if (Credentials == null)
                    Credentials = new CognitoAWSCredentials(Strings.AWS_COGNITO_POOL_ID, REGION);
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|PhotoService.cs|GetCredentials(): " + err.Message);
            }
        }

        public static void GetS3Client()
        {
            try
            {
                if (S3Client != null)
                    return;
                GetCredentials();
                S3Client = new AmazonS3Client(Credentials, REGION);
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|PhotoService.cs|GetS3Client(): " + err.Message);
            }
        }


        public static async Task<bool> UploadFile(string fileName, int inspection_id)
        {
            
            if (S3Client == null)
                GetS3Client();
            try
            {

                string filePath = DependencyService.Get<IFilePicker>().GetFilePath(fileName);
                if (filePath == null)
                    return false;

                File file = new File(filePath);
                if (!file.IsFile)
                    return true;

                var response = await S3Client.PutObjectAsync(new PutObjectRequest()
                {
                    BucketName = Strings.AWS_BUCKET_NAME,
                    FilePath = file.Path,
                    Key = inspection_id.ToString() + "/" + file.Name
                });

                if(response.HttpStatusCode == HttpStatusCode.OK)
                {
                    DeleteFile(fileName);
                    return true;
                }
            }
            catch (AmazonS3Exception err)
            {
                Debug.WriteLine("AGROMAP|PhotoService.cs|UploadFile: " + err.Message);
            }
            return false;
        }

        // Recupera foto da pasta do app
        public static ImageSource RetrievePhoto(string uuid)
        {
            CheckCameraPermission();
            return DependencyService.Get<IFilePicker>().GetFile(uuid);
        }

        //Recupera e retorna foto da galeria
        public static async Task<ImageSource> AddPhoto(string uuid)
        {
            CheckCameraPermission();

            if(uuid == null)
                uuid = InspectionService.GetNextID();

            Stream stream = await DependencyService.Get<IFilePicker>().GetFileFromLibrary();
            DependencyService.Get<IFilePicker>().SaveFile(uuid, stream);
            return DependencyService.Get<IFilePicker>().GetFile(uuid);
        }

        private static Boolean CheckCameraPermission()
        {
            try
            {
                DependencyService.Get<IGetPermission>().CheckPermission(camera);
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|NewEventScreen.cs|CheckCameraPermission: " + err.Message);
            }
            return false;

        }

        public static void DeleteFile(string fileName)
        {
            try
            {
                DependencyService.Get<IFilePicker>().DeleteFile(fileName);
            }
            catch (Exception err)
            {

                Debug.WriteLine("AGROMAP|PhotoService.cs|DeleteFile(): " + err.Message);
            }
        }

    }
}
