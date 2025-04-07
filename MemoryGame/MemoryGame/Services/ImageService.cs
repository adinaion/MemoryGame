using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MemoryGame.Helpers;

namespace MemoryGame.Services
{
    public class ImageService
    {
        public List<string> LoadImagePathsFromCategory(string category)
        {
            // Presupunem că numele folderului este format din numele categoriei fără spații
            string folderName = category.Replace(" ", "");
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", folderName);

            if (!Directory.Exists(folderPath))
                return new List<string>();

            var imageFiles = Directory.GetFiles(folderPath, "*.jpg")
                .Concat(Directory.GetFiles(folderPath, "*.png"))
                .Concat(Directory.GetFiles(folderPath, "*.gif"))
                .ToList();

            return imageFiles
                    .Select(path => PathHelper.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, path))
                    .ToList();
        }
    }
}
