using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TagLib;


namespace FinalProjectWPF_2
{
    public class ApplicationTests
    {
        private MainWindow mainWindow;
        private DispatcherTimer testTimer;
        private bool isPaused = false;
        private int playbackCounter = 0;
        String filePath = "Resources/Yak.mp3"; // test MP3 file

        public ApplicationTests(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public async Task RunAllTests()
        {
            try
            {
                // Run all test functions
                await TestUpdateArtist();
                await TestUpdateAlbum();
                await TestUpdateTitle();
                await TestUpdateDate();
                await TestMediaPlayback();
                
                MessageBox.Show("All tests passed successfully!", "Test Results", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                //Exceptions shown if test fails
                MessageBox.Show($"Test failed: {ex.Message}", "Test Results", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task TestUpdateArtist()
        {
            // Arrange
            mainWindow.ArtistTextBox.Text = "Test Artist";

            // Act
            await mainWindow.UpdateArtist(filePath);

            // Assert
            using (var file = TagLib.File.Create(filePath))
            {
                if (file.Tag.FirstPerformer != "Test Artist")
                {
                    throw new Exception("UpdateArtist test failed.");
                }
            }
        }

        public async Task TestUpdateAlbum()
        {
            // Arrange
            mainWindow.AlbumTextBox.Text = "Test Album";

            // Act
            await mainWindow.UpdateAlbum(filePath);

            // Assert
            using (var file = TagLib.File.Create(filePath))
            {
                if (file.Tag.Album != "Test Album")
                {
                    throw new Exception("UpdateAlbum test failed.");
                }
            }
        }

        public async Task TestUpdateTitle()
        {
            // Arrange
            mainWindow.TitleTextBox.Text = "Test Title";

            // Act
            await mainWindow.UpdateTitle(filePath);

            // Assert
            using (var file = TagLib.File.Create(filePath))
            {
                if (file.Tag.Title != "Test Title")
                {
                    throw new Exception("UpdateTitle test failed.");
                }
            }
        }

        public async Task TestUpdateDate()
        {
            // Arrange
            mainWindow.ReleaseDateTextBox.Text = "2023";

            // Act
            await mainWindow.UpdateDate(filePath);

            // Assert
            using (var file = TagLib.File.Create(filePath))
            {
                if (file.Tag.Year != 2023)
                {
                    throw new Exception("UpdateDate test failed.");
                }
            }
        }

        public async Task TestMediaPlayback()
        {
            // Arrange: Set the media source to the test file
            mainWindow.mediaElement.Source = new Uri(filePath, UriKind.Relative);

            // Load cover art
            LoadCoverArt(System.IO.Path.GetDirectoryName(filePath));

            // Start media playback
            mainWindow.mediaElement.Play();
            mainWindow.timer.Start();

            // Create and start the playback test timer
            testTimer = new DispatcherTimer();
            testTimer.Interval = TimeSpan.FromSeconds(1);
            testTimer.Tick += TestPlaybackTimer_Tick;
            testTimer.Start();

            // Wait for the test to complete
            await Task.Delay(TimeSpan.FromSeconds(30));

            // Stop media playback after the test
            mainWindow.mediaElement.Stop();
            mainWindow.timer.Stop();

            testTimer.Stop();
        }



        // Method to load cover art from a specified path
        private void LoadCoverArt(string filePath)
        {
            // Path to the album art file
            String artPath = "Resources/AlbumArt.jpg";

            // Check if the album art file exists
            if (System.IO.File.Exists(artPath))
            {
                // Create a new BitmapImage
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(artPath, UriKind.Relative); // Set the UriSource to the album art path
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Ensure the image is loaded immediately
                bitmap.EndInit();

                // Set the CoverArt source to the loaded image
                mainWindow.CoverArt.Source = bitmap;
                Console.WriteLine($"Loaded album art from {artPath}");
            }
            else
            {
                // If the album art file does not exist, clear the CoverArt source
                mainWindow.CoverArt.Source = null;
                Console.WriteLine("No album art found in the directory.");
            }
        }

        private void TestPlaybackTimer_Tick(object sender, EventArgs e)
        {
            playbackCounter++;

            if (playbackCounter == 5)
            {
                if (!isPaused)
                {
                    mainWindow.mediaElement.Pause();
                    isPaused = true;
                }
                else
                {
                    mainWindow.mediaElement.Play();
                    isPaused = false;
                }
                playbackCounter = 0;
            }
        }
    }
}
