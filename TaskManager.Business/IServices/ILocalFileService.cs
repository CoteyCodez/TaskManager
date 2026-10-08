using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Models.ViewModels;

namespace TaskManager.Business.IServices
{
    public interface ILocalFileService
    {
        Task<string> SaveAttachmentToLocal(IFormFile file);
        Task<byte[]?> DownloadAttachmentInBytes(string fileName);
    }
}