using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            // Arrange
            mainWindow.mediaElement.Source = new Uri(filePath, UriKind.Relative);

            // Act
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
