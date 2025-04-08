using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MemoryGame.Views
{
    public partial class NewUserWindow : Window
    {
        public string UserName { get; private set; }
        public string ImagePath { get; private set; }

        private List<string> availableImages;
        private int currentImageIndex = 0;

        public NewUserWindow()
        {
            InitializeComponent();
            LoadAvailableImages();
            DisplayCurrentImage();
        }

        private void LoadAvailableImages()
        {
            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Data");
            if (Directory.Exists(imagesFolder))
            {
                availableImages = Directory.GetFiles(imagesFolder, "*.jpg").ToList();
                availableImages.AddRange(Directory.GetFiles(imagesFolder, "*.png"));
                availableImages.AddRange(Directory.GetFiles(imagesFolder, "*.gif"));
                availableImages = availableImages.OrderBy(x => x).ToList();
            }
            else
            {
                availableImages = new List<string>();
            }
        }

        private void DisplayCurrentImage()
        {
            if (availableImages.Count > 0)
            {
                string currentImagePath = availableImages[currentImageIndex];
                imgSelected.Source = new BitmapImage(new Uri(currentImagePath));
                ImagePath = currentImagePath;
            }
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (availableImages.Count > 0)
            {
                currentImageIndex = (currentImageIndex - 1 + availableImages.Count) % availableImages.Count;
                DisplayCurrentImage();
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (availableImages.Count > 0)
            {
                currentImageIndex = (currentImageIndex + 1) % availableImages.Count;
                DisplayCurrentImage();
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtUserName.Text) && availableImages.Count > 0)
            {
                UserName = txtUserName.Text;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please enter a user name and ensure an image is available.",
                                "Incomplete Data", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
