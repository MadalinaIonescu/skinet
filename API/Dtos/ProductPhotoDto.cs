using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace API.Dtos
{
    public class ProductPhotoDto
    {
        [MaxFileSize(2 * 1024 * 1024)]
        [AllowedExtensions(new[] {".jpg", ".png", ".jpeg"})]
        public IFormFile Photo { get; set; }
    }
}