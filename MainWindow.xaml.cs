using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Windows.Threading;
using System.Threading.Tasks;
using Hqub.MusicBrainz.Entities;
using DiscId;
using System.Windows.Media.Imaging;

namespace FinalProjectWPF_2
{
    public partial class MainWindow : Window
    {
        //for getting and setting MP3 metadata
        public String Artists { get; set; }
        public String Album { get; set; }
        public String Track { get; set; }
        public String RDate { get; set; }
        public String Genra { get; set; }
        public String Tags { get; set; }

        //for getting and setting disc metadata
        public String dId { get; set; }
        public String freedbId { get; set; }
        public String mcn { get; set; }
        public String MP3OUT { get; set; }
        public int firstTrackNumber { get; set; }
        public int lastTrackNumber { get; set; }
        public int sectors { get; set; }
        public Uri Surl { get; set; }

        // Media player, CDRipper, and MusicBrainzClient
        MediaPlayer mediaPlayer = new MediaPlayer();
        string mediaDirectory;
        private CdRipper cdRipper;
        private MusicBrainzClient musicBrainzclient;
        DispatcherTimer timer;
        private double previousVolume = 0.5;  // Default volume level or last known volume level before mute, needed to reset the slider after and durring mute

        // initialize components and objects
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; 
            InitializeMediaPlayer();
            cdRipper = new CdRipper();
            musicBrainzclient = new MusicBrainzClient();
            PopulateCDrives();
            InitializeTimer();

        }

        // Timer for media run time UI updates
        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
        }

        // method to test MusicBrainz service
        public async void testMBAsync(string dId)
        {
            try
            {
                var data = await musicBrainzclient.LookupByDiscIdAsync(dId);
                if (data != null)
                {
                    ArtistTextBox.Text = data.ArtistName;
                    AlbumTextBox.Text = data.AlbumTitle;
                    TitleTextBox.Text = data.TrackTitle;
                    ReleaseDateTextBox.Text = data.ReleaseDate;
                    GenreTextBox.Text = string.Join(", ", data.Genres);
                    TagsTextBox.Text = string.Join(", ", data.Tags);
                }
                else
                {
                    ArtistTextBox.Text = "No data found.";
                    AlbumTextBox.Text = "No data found.";
                    TitleTextBox.Text = "No data found.";
                    ReleaseDateTextBox.Text = "No data found.";
                    GenreTextBox.Text = "No data found.";
                    TagsTextBox.Text = "No data found.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving CD data: {ex.Message}");
                ArtistTextBox.Text = "No data found.";
                AlbumTextBox.Text = "No data found.";
                TitleTextBox.Text = "No data found.";
                ReleaseDateTextBox.Text = "No data found.";
                GenreTextBox.Text = "No data found.";
                TagsTextBox.Text = "No data found.";
            }
        }

        // Timer event for updating the playback time
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaElement != null && !seekSlider.IsMouseCaptured)
            {
                seekSlider.Value = mediaElement.Position.TotalSeconds;
                string durationText = mediaElement.NaturalDuration.HasTimeSpan
                                      ? mediaElement.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss")
                                      : "Unknown Duration";
                RipLabel.Content = $"{mediaElement.Position.ToString(@"hh\:mm\:ss")} / {durationText}";
            }
        }

        // Initializes the media player and its events
        private void InitializeMediaPlayer()
        {
            mediaPlayer.Volume = 0.5; // Sets the volume level of the media player to 50%
            mediaPlayer.MediaEnded += (s, e) => mediaPlayer.Play(); // Adds an event handler to restart playback when the media ends
            mediaPlayer.MediaFailed += (s, e) => MessageBox.Show("Media failed to load: " + e.ErrorException.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //start dragging the seek slider event
        private void seekSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            mediaElement.Pause();
        }

        //finish dragging the seek slider event
        private void seekSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (mediaElement != null && seekSlider != null)
            {
                mediaElement.Position = TimeSpan.FromSeconds(seekSlider.Value);
                mediaElement.Play();// Resumes media playback after the user finishes dragging the slider.
            }
        }

        // seek slider value changes event
        private void seekSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaElement != null && seekSlider != null && !mediaElement.IsMouseCaptured)
            {
                mediaElement.Position = TimeSpan.FromSeconds(seekSlider.Value);// Updates the position of the media playback as the slider value changes without dragging the slider
            }
        }

        //media is loaded and ready to play event
        public void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                seekSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds; // Sets the maximum value of the seek slider based on the metadata lenght of the media
                timer.Start();
            }
        }

        //media playback ends event
        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();  //stops the media playback
            seekSlider.Value = 0;  //resets the seek slider to the beginning
            timer.Stop();  //stops the timer as there if no there is no playback

            int currentIndex = mediaListBox.SelectedIndex;  //gets the current index of the selected media item
            if (currentIndex < mediaListBox.Items.Count - 1)  //check for more items in the list after the current one
            {
                mediaListBox.SelectedIndex = currentIndex + 1;  //moves the selection to the next media item in the list
                PlaySelectedItem();  // Plays the newly selected media item
            }
        }

        // event for when the repeat play ends
        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            mediaPlayer.Position = TimeSpan.Zero;
            mediaPlayer.Play();
        }

        // play button click
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaElement.Source != null)
            {
                mediaElement.Play();
                timer.Start();

                // Assuming that the file path of the currently playing item is required to check if it's a video or not
                string filePath = mediaElement.Source.LocalPath;  // Get the local path of the file
                if (IsVideoFile(filePath))  // Check if it's a video file
                {
                    HideMetadataTextboxesForVideo();  // Hide metadata textboxes if it's a video
                }
                else
                {
                    ShowMetadataTextboxes();  // Show metadata textboxes if it's not a video
                    DisplayMetadata(filePath);  // Also update the metadata display for non-video files
                }
            }
        }


        //pause button click
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Pause();
            timer.Stop();
        }

        //stop button click
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            timer.Stop();
            seekSlider.Value = 0;
        }

        //adjusting the volume
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaElement != null)
            {
                mediaElement.Volume = e.NewValue;
            }
        }

        // next track button click
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = mediaListBox.SelectedIndex;
            if (currentIndex < mediaListBox.Items.Count - 1)
            {
                mediaListBox.SelectedIndex = currentIndex + 1;
                PlaySelectedItem();
            }
        }

        //previous track button click
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = mediaListBox.SelectedIndex;
            if (currentIndex > 0)
            {
                mediaListBox.SelectedIndex = currentIndex - 1;
                PlaySelectedItem();
            }
        }

        // Play the selected media item
        private void PlaySelectedItem()
        {
            if (mediaListBox.SelectedItem != null && mediaDirectory != null)
            {
                string selectedFileName = mediaListBox.SelectedItem.ToString();
                string filePath = Path.Combine(mediaDirectory, selectedFileName);
                mediaElement.Source = new Uri(filePath);
                mediaElement.Play();

                Console.WriteLine($"Playing: {filePath}"); // Debugging
                bool isVideo = IsVideoFile(filePath);
                Console.WriteLine($"Is video: {isVideo}"); // Debugging

                if (isVideo)
                {
                    HideMetadataTextboxesForVideo();//hide metadata if it is a video file
                    Console.WriteLine("Hiding metadata for video."); // Debugging
                }
                else
                {
                    ShowMetadataTextboxes();
                    DisplayMetadata(filePath); // Update metadata display only for non-video files
                    Console.WriteLine("Showing metadata for audio."); // Debugging
                }
            }
        }



        // Check if a file is a video file
        private bool IsVideoFile(string filePath)
        {
            string[] videoExtensions = { ".mp4", ".mkv", ".avi" }; // seperate video files to hide metadata boxes 
            return videoExtensions.Contains(Path.GetExtension(filePath), StringComparer.OrdinalIgnoreCase);
        }

        // Hide all metadata text boxes when the file is a video
        private void HideMetadataTextboxesForVideo()
        {
            ArtistTextBox.Visibility = Visibility.Collapsed;
            AlbumTextBox.Visibility = Visibility.Collapsed;
            TitleTextBox.Visibility = Visibility.Collapsed;
            ReleaseDateTextBox.Visibility = Visibility.Collapsed;
            GenreTextBox.Visibility = Visibility.Collapsed;
            TagsTextBox.Visibility = Visibility.Collapsed;
            DurationTextBox.Visibility = Visibility.Collapsed;
            SizeTextBox.Visibility = Visibility.Collapsed;
            CoverArt.Visibility = Visibility.Collapsed;
            OkButton.Visibility = Visibility.Collapsed; // hide update Metadata button for audio files
        }

        // Show all metadata text boxes 
        private void ShowMetadataTextboxes()
        {
            ArtistTextBox.Visibility = Visibility.Visible;
            AlbumTextBox.Visibility = Visibility.Visible;
            TitleTextBox.Visibility = Visibility.Visible;
            ReleaseDateTextBox.Visibility = Visibility.Visible;
            GenreTextBox.Visibility = Visibility.Visible;
            TagsTextBox.Visibility = Visibility.Visible;
            DurationTextBox.Visibility = Visibility.Visible;
            SizeTextBox.Visibility = Visibility.Visible;
            CoverArt.Visibility = Visibility.Visible;
            OkButton.Visibility = Visibility.Visible; // show update Metadata button for audio files
        }

        // Opens a dialog to select a media directory and saves it as a string
        private void SelectDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new Microsoft.Win32.OpenFileDialog();
            folderBrowserDialog.ValidateNames = false;
            folderBrowserDialog.CheckFileExists = false;
            folderBrowserDialog.CheckPathExists = true;
            folderBrowserDialog.FileName = "Folder Selection";

            bool? result = folderBrowserDialog.ShowDialog();
            if (result == true)
            {
                mediaDirectory = Path.GetDirectoryName(folderBrowserDialog.FileName);
                PopulateMediaList(mediaDirectory);
            }
        }

        // populates the media list with files from the selected directory
        private void PopulateMediaList(string directoryPath)
        {
            if (directoryPath != null)
            {
                mediaListBox.Items.Clear();
                string[] mediaFiles = Directory.GetFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly).Where(file => IsMediaFile(file)).ToArray();

                foreach (string file in mediaFiles)
                {
                    string fileName = Path.GetFileName(file);
                    mediaListBox.Items.Add(fileName);
                }
            }
        }

        // check if a file is a media file based on its extension
        private bool IsMediaFile(string filePath)
        {
            string[] validExtensions = { ".mp3", ".mp4", ".wav", ".wma", ".mkv", ".cda", ".avi", ".flac" };
            string extension = Path.GetExtension(filePath);
            return validExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        //event for when a media item is selected
        private void MediaListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mediaListBox.SelectedItem != null && mediaDirectory != null)
            {
                string selectedFileName = mediaListBox.SelectedItem.ToString();
                string filePath = Path.Combine(mediaDirectory, selectedFileName);
                mediaElement.Source = new Uri(filePath);
                DisplayMetadata(filePath);
            }
        }

        //populate the CD drive list
        private void PopulateCDrives()
        {
            DriveComboBox.Items.Clear();
            DriveComboBox.ItemsSource = cdRipper.GetDevices();
            if (DriveComboBox.Items.Count > 0)
                DriveComboBox.SelectedIndex = 0;
        }

        // get and displays metadata for the release group information from MusicBrainz
        public async void FetchAndDisplayReleaseGroupInfo()
        {
            try
            {
                var recordingData = await musicBrainzclient.LookupByDiscIdAsync(dId);
                if (recordingData != null)
                {
                    string currentReleaseGroupId = recordingData.ReleaseGroupId;
                    Console.WriteLine("Release Group ID: " + currentReleaseGroupId);
                    LoadCoverArt(currentReleaseGroupId);
                }
                else
                {
                    Console.WriteLine("No recording data found for this disc ID.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching recording data: " + ex.Message);
            }
        }

        //get cover art for the release group
        private async void LoadCoverArt(string releaseGroupId)
        {
            string imageUrl = await musicBrainzclient.FetchCoverArtAsync(releaseGroupId);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imageUrl, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                // Assign the BitmapImage to the Image control
                CoverArt.Source = bitmap;
            }
            else
            {
                // Handle cases where no image is found
                CoverArt.Source = null;
            }
        }

        // Method to display metadata from an MP3 file
        private void DisplayMetadata(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("File does not exist: " + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Additional preliminary check for MPEG headers
            if (!IsLikelyValidMPEG(filePath))
            {
                MessageBox.Show("The file does not appear to be a valid MPEG audio file.", "Invalid File Type", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var file = TagLib.File.Create(filePath)) // Using TagLib to open the file and read metadata
                {
                    // Displaying metadata in the TextBoxes, ignore if blank to avoid invalid tag errors.
                    ArtistTextBox.Text = file.Tag.FirstPerformer ?? "Unknown Artist";
                    AlbumTextBox.Text = file.Tag.Album ?? "Unknown Album";
                    TitleTextBox.Text = file.Tag.Title ?? "Unknown Title";
                    ReleaseDateTextBox.Text = file.Tag.Year > 0 ? file.Tag.Year.ToString() : "Unknown Year";
                    DurationTextBox.Text = file.Properties.Duration.ToString(@"hh\:mm\:ss");
                    SizeTextBox.Text = new FileInfo(filePath).Length.ToString() + " bytes";
                }
            }
            catch (TagLib.CorruptFileException)
            {
                MessageBox.Show("Invalid MPEG audio header. This file may be corrupt or is not a valid audio file.", "Error Loading File", MessageBoxButtons.OK, MessageBoxIcon.Error);//display the ex if the file does not have a valid ID3 header.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Needed to make this check to insure that the file is a valid media file, exception would not catch without this.
        private bool IsLikelyValidMPEG(string filePath)
        {
            const int BufferSize = 3; // mpeg headers are found at the start of the file
            byte[] buffer = new byte[BufferSize];
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (fs.Read(buffer, 0, BufferSize) < BufferSize)
                    return false; //file too short to be a valid MPEG file
            }
            // Check for ID3 tag, 0x49, 0x44, and 0x33 is ASCII characters I, D, and 3 and if that is not there, then it is not valid - HXD used to verify this
            return (buffer[0] == 0x49 && buffer[1] == 0x44 && buffer[2] == 0x33) || (buffer[0] == 0xFF && (buffer[1] & 0xE0) == 0xE0);
        }

        // insert the current metadata from the text boxes into the currently selected audio file
        private async Task insertMetaData(string filePath)
        {
            if (mediaListBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a file from the list.");
                return;
            }

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Selected file does not exist.");
                return;
            }

            try
            {
                //Create seperate functions to maybe add individual buttons for single updates to tracks.
                using (var file = TagLib.File.Create(filePath))
                {
                    Console.WriteLine(filePath);

                    await UpdateArtist(filePath);
                    await UpdateAlbum(filePath);
                    await UpdateTitle(filePath);
                    await UpdateDate(filePath);
                }
                MessageBox.Show("Metadata updated successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update file metadata: {ex.Message}");
            }
        }

        // Updates the artist metadata
        private async Task UpdateArtist(string filePath)
        {
            try
            {
                using (var file = TagLib.File.Create(filePath))
                {
                    Artists = ArtistTextBox.Text;
                    file.Tag.Performers = new[] { Artists };
                    await Task.Delay(100);
                    file.Save();
                    Console.WriteLine("Artist updated successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating artist: {ex.Message}");
            }
        }

        // Updates the album metadata
        public async Task UpdateAlbum(string filePath)
        {
            try
            {
                using (var file = TagLib.File.Create(filePath))
                {
                    Album = AlbumTextBox.Text;
                    file.Tag.Album = Album;
                    await Task.Delay(100);
                    file.Save();
                    Console.WriteLine("Album updated successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating album: {ex.Message}");
            }
        }

        // Updates the title metadata
        public async Task UpdateTitle(string filePath)
        {
            try
            {
                using (var file = TagLib.File.Create(filePath))
                {
                    //Update the metadata from text boxes

                    Title = TitleTextBox.Text;
                    Console.WriteLine(Title);

                    file.Tag.Title = Title;
                    await Task.Delay(100);
                    // Save the changes
                    file.Save();
                }
                Console.WriteLine("Title updated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating artist: {ex.Message}");
            }
        }

        // Updates the title metadata
        public async Task UpdateDate(string filePath)
        {
            try
            {
                // Correctly use TagLib.File to open the file
                using (var file = TagLib.File.Create(filePath))
                {
                    string inputDate = ReleaseDateTextBox.Text;
                    Console.WriteLine(inputDate);

                    if (DateTime.TryParse(inputDate, out DateTime parsedDate))
                    {
                        file.Tag.Year = (uint)parsedDate.Year;
                    }
                    else if (int.TryParse(inputDate, out int year) && year >= 1000 && year <= 9999)
                    {
                        file.Tag.Year = (uint)year;
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format");
                        return;  // Exit if the date is not valid
                    }

                    await Task.Delay(100);
                    file.Save();  // Save the changes
                    Console.WriteLine("Date updated successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating date: {ex.Message}");
            }
        }

        // Button to rip selected media list box item from the loaded cd
        private void RipCDButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaListBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a track first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedTrackInfo = mediaListBox.SelectedItem.ToString().Split(new[] { ':' }, 2);
            if (selectedTrackInfo.Length > 1)
            {
                try
                {
                    int trackNumber = int.Parse(selectedTrackInfo[0].Replace("Track ", "").Trim());
                    int driveIndex = ConvertDriveToIndex(DriveComboBox.SelectedItem.ToString());
                    if (cdRipper.CDIsReady(driveIndex))
                    {
                        var tracks = cdRipper.GetTracks(driveIndex);
                        int totalTracks = tracks.Count;

                        if (tracks != null && totalTracks > 0)
                        {
                            if (trackNumber >= 1 && trackNumber <= totalTracks)
                            {
                                //Oper dialog to select the save directory for ripping tracks
                                var folderBrowserDialog = new FolderBrowserDialog();
                                folderBrowserDialog.Description = "Select a folder for music files";
                                folderBrowserDialog.ShowNewFolderButton = true;
                                folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyMusic;
                                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    string selectedPath = folderBrowserDialog.SelectedPath;//path selected saved to string
                                    Console.WriteLine($"Selected folder: {selectedPath}");

                                    //Update strings with current text box text
                                    Album = AlbumTextBox.Text;
                                    string outputPath = $"{selectedPath}\\Track{trackNumber}.mp3";
                                    MP3OUT = outputPath;
                                    Console.WriteLine($"outputPath folder: {outputPath}");
                                    Title = $"Track{trackNumber}";
                                    TitleTextBox.Text = Title;
                                    //Rip complete event initiated
                                    cdRipper.RipCompleted += CdRipper_RipCompleted;

                                    //start ripping with save path, tracknumber for mp3 name and location of track, cd drive used 
                                    cdRipper.RipTrack(outputPath, trackNumber, driveIndex);
                                    cdRipper.UpdateLabelEvent += UpdateRipLabel;//event to display rip complete and trigger metadata insertion to the newly created mp3 file

                                    Console.WriteLine($"Added Title: {Title}");
                                }
                                Console.WriteLine($"MP3OUT - Insert Meta: {MP3OUT}");
                            }
                            else
                            {
                                MessageBox.Show($"Invalid track number. Available tracks: 1 to {totalTracks}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No tracks found on the CD.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error during operation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Selected track info is in an unexpected format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Triggered when CD ripping is completed
        private void CdRipper_RipCompleted(object sender, EventArgs e)
        {
            _ = insertMetaData(MP3OUT);
            Console.WriteLine("Track rip completed.");
            cdRipper.RipCompleted -= CdRipper_RipCompleted;
        }

        // Converts a drive letter to an index for internal use (Example: Drive J = cd drive 0, So J = 0 / If the J drive was the second cd drive it would be J = 1 and so on)
        private int ConvertDriveToIndex(string driveLetter)
        {
            return DriveComboBox.SelectedIndex; // slected index in my use case is always 0 because I have only 1 CD drive installed.
        }

        // Updates the rip status label.
        public void UpdateRipLabel(string text)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                RipLabel.Content = text;// update the lable ripstatus % on the proper thread
            });
        }

        // Inserts metadata from text boxes if edits were required. Was originally a test button
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaListBox.SelectedItem != null)
            {
                string fullPath = Path.Combine(mediaDirectory, mediaListBox.SelectedItem.ToString());// create a string path by taking the media directory and combining it with the selected list item
                Console.WriteLine("Currently playing media path: " + fullPath);
                _ = insertMetaData(fullPath);
            }
            else
            {
                Console.WriteLine("No media selected or currently playing.");
            }
        }

        // Loads CD tracks into the media list, need to add to a dive event to trigger this when the CD drive is closed, which is why I have tests for open and close
        private void LoadCDTracks()
        {
            if (DriveComboBox.SelectedItem == null) return;
            mediaListBox.Items.Clear();

            int driveIndex = DriveComboBox.SelectedIndex;
            if (!cdRipper.CDIsReady(driveIndex))
            {
                MessageBox.Show("The CD is not ready. Please insert an audio CD into the drive.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //*
            //.NET bindings for MusicBrainz libdiscid
            //https://github.com/phw/dotnet-discid
            //DotNetDiscId.dll
            //
            //simple usage example used in the code because it provided everything that was needed for the disc id.
            //
            //Example was modified to fit my usage but the original usage remained so I wanted to credit the source
            //*
            try
            {
                string device = DiscId.Disc.DefaultDevice;//Save the drive to a string
                using (var disc = DiscId.Disc.Read(device, Features.Mcn | Features.Isrc))
                {
                    //Writes out disc information to the console window
                    Console.Out.WriteLine("DiscId         : {0}", disc.Id);
                    Console.Out.WriteLine("FreeDB ID      : {0}", disc.FreedbId);
                    Console.Out.WriteLine("MCN            : {0}", disc.Mcn);
                    Console.Out.WriteLine("First track no.: {0}", disc.FirstTrackNumber);
                    Console.Out.WriteLine("Last track no. : {0}", disc.LastTrackNumber);
                    Console.Out.WriteLine("Sectors        : {0}", disc.Sectors);
                    Console.Out.WriteLine("Submission URL : {0}", disc.SubmissionUrl);

                    dId = disc.Id;//save the disc id for later use
                    freedbId = disc.FreedbId;// find the free DB id for the disc 
                    mcn = disc.Mcn;
                    firstTrackNumber = disc.FirstTrackNumber;
                    lastTrackNumber = disc.LastTrackNumber;
                    sectors = disc.Sectors;
                    Surl = disc.SubmissionUrl;

                    Console.Out.WriteLine(dId);
                    Console.Out.WriteLine(freedbId);
                    Console.Out.WriteLine(mcn);
                    Console.Out.WriteLine(firstTrackNumber);
                    Console.Out.WriteLine(lastTrackNumber);
                    Console.Out.WriteLine(sectors);
                    Console.Out.WriteLine(Surl);
                }
                testMBAsync(dId);
                FetchAndDisplayReleaseGroupInfo();
            }
            catch (DiscIdException ex)
            {
                Console.Out.WriteLine("Could not read disc: {0}.", ex.Message);// show which metadata could tnot be read
            }
            //end of modified used example

            var tracks = cdRipper.GetTracks(driveIndex);
            foreach (var track in tracks)
            {
                string displayText = $"Track {track}";//list all of the tracks on the loaded album
                mediaListBox.Items.Add(displayText);
            }

            if (mediaListBox.Items.Count > 0)
                mediaListBox.SelectedIndex = 0;
        }

        // Event triggered when a CD drive is selected
        private void DriveComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadCDTracks();

            if (DriveComboBox.SelectedItem != null)
            {
                string selectedDrive = DriveComboBox.SelectedItem.ToString();
                LoadTracksFromCD(ConvertDriveToIndex(selectedDrive));
            }
        }

        // Loads tracks from a CD into the media list
        private void LoadTracksFromCD(int driveIndex)
        {
            if (cdRipper.CDIsReady(driveIndex))
            {
                var tracks = cdRipper.GetTracks(driveIndex);
                mediaListBox.Items.Clear();
                foreach (var track in tracks)
                {
                    string displayText = $"Track {track}";
                    mediaListBox.Items.Add(displayText);
                }

                if (mediaListBox.Items.Count > 0)
                    mediaListBox.SelectedIndex = 0;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("The CD is not ready. Please insert a CD into the drive.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MuteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Store current volume, then mute the media player
            if (mediaElement != null)
            {
                previousVolume = mediaElement.Volume;
                mediaElement.Volume = 0;
                VolumeSlider.Value = 0;  // Move the slider to the left to indicate mute
            }
        }

        private void MuteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Restore the volume from previousVolume
            if (mediaElement != null)
            {
                mediaElement.Volume = previousVolume;
                VolumeSlider.Value = previousVolume;  // Restore the slider position to reflect the current volume
            }
        }

        // Placeholder event handlers for text changes
        private void AlbumTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ReleaseDateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void GenreTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TagsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DurationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
