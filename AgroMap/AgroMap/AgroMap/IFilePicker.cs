
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AgroMap
{
    public interface IFilePicker
    {
        // Recupera arquivo da galeria
        Task<Stream> GetFileFromLibrary();

        // Salva arquivo selecionado para a pasta 'agromap'
        void SaveFile(string fileName, Stream photo_stream);

        // Recupera arquivo salvo na pasta 'agromap' para ser enviado ao S3
        ImageSource GetFile(string fileName);

        // Exclui arquivo da pasta 'agromap'
        Boolean DeleteFile(string fileName);

        // Exclui pasta 'agromap'
        void DeleteDirectory();

        //Retorna caminho absoluto de uma imagem
        string GetFilePath(string uuid);
    }
}
