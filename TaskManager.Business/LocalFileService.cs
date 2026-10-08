using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Business.IServices;
using TaskManager.Models.ViewModels;

namespace TaskManager.Business
{
    public class LocalFileService : ILocalFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LocalFileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveAttachmentToLocal(IFormFile file)
        {

            if (file != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                // Don't use the OG name alone, could cause duplication
                string fileName = Guid.NewGuid().ToString()
                                    + Path.GetExtension(file.FileName);
                string uploadPath = Path.Combine(wwwRootPath, "uploads");
                // Final path where you want to upload image

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Save the new image
                using (var fileStream = new FileStream(Path.Combine
                (uploadPath, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                
                return fileName;
            }

            return null;
        }

        // GET: File/Download?filename=example.txt
        public async Task<byte[]?> DownloadAttachmentInBytes(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null; 
            }
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return null; 
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return fileBytes;
        }
    }
}
