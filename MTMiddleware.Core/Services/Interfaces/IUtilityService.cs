namespace MTMiddleware.Core.Services.Interfaces
{
    public interface IUtilityService
    {




      

        //Tools
        Task<Response<string>> EncryptAsync(string TextToEncrypt);
        Task<Response<string>> DecryptAsync(string EncryptedText);

    }
}
