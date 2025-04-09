using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MemoryGame.Helpers;

namespace MemoryGame.Services
{
    public class NewUserService
    {
        private List<string> availableImages;
        private int currentIndex;

        public NewUserService()
        {
            LoadAvailableImages();
        }

        private void LoadAvailableImages()
        {
            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Data");
            if (Directory.Exists(imagesFolder))
            {
                availableImages = Directory.GetFiles(imagesFolder, "*.jpg")
                    .Concat(Directory.GetFiles(imagesFolder, "*.png"))
                    .Concat(Directory.GetFiles(imagesFolder, "*.gif"))
                    .OrderBy(f => f)
                    .ToList();
            }
            else
            {
                availableImages = new List<string>();
            }
        }

        public string GetCurrentImagePath()
        {
            if (availableImages.Count == 0)
                return null;
            return availableImages[currentIndex];
        }

        public string GetCurrentImageRelativePath()
        {
            var currentPath = GetCurrentImagePath();
            if (currentPath == null)
                return null;
            return PathHelper.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, currentPath);
        }

        public void MoveLeft()
        {
            if (availableImages.Count > 0)
            {
                currentIndex = (currentIndex - 1 + availableImages.Count) % availableImages.Count;
            }
        }

        public void MoveRight()
        {
            if (availableImages.Count > 0)
            {
                currentIndex = (currentIndex + 1) % availableImages.Count;
            }
        }
    }
}

