using CSAudioCDRipper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace FinalProjectWPF_2
{
    public class CdRipper
    {
        public AudioCDRipper audioCDRipper;  // Handles the CD ripping process.
        public FreeDB freeDB;                // attempted to use for MusicBranzAPI for metadata

        private readonly HttpClient _httpClient; // Handles HTTP requests

        public CdRipper()
        {
            audioCDRipper = new AudioCDRipper(); //initialize the CD ripper
            freeDB = new FreeDB(); //FreeDB for accessing CD metadata

            _httpClient = new HttpClient(); //HttpClient for web requests

            // audio CD ripper events.
            audioCDRipper.RipProgress += OnRipProgress;
            audioCDRipper.RipError += OnRipError;
            audioCDRipper.RipStart += OnRipStart;
            audioCDRipper.RipDone += OnRipDone;

            // FreeDB events.
            freeDB.FreeDBStatus += OnFreeDBStatus;
            freeDB.FreeDBError += OnFreeDBError;
            freeDB.FreeDBTracks += OnFreeDBTracks;
            freeDB.FreeDBAlbum += OnFreeDBAlbum;
            freeDB.FreeDBDone += OnFreeDBDone;
        }

        // gets album and track information from MusicBrainz API
        public async Task<string> GetAlbumAndTrackInfoAsync(string albumTitle, string trackTitle)
        {
            albumTitle = Uri.EscapeDataString(albumTitle);
            trackTitle = Uri.EscapeDataString(trackTitle);
            string url = $"https://musicbrainz.org/ws/2/recording/?query=release:{albumTitle}+recording:{trackTitle}&fmt=json";//API URL for getting album title and track title 
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }



        // returns a list of available CD drives
        public List<string> GetDevices()
        {
            return audioCDRipper.GetDevices();
        }


        // checks if a CD is ready in the specified drive
        public bool CDIsReady(int driveIndex)
        {
            return audioCDRipper.CDIsReady(driveIndex);
        }


        // gets metadata for the CD in the drive
        public void FetchMetadata(int driveIndex)
        {
            if (CDIsReady(driveIndex))
            {
                freeDB.GetFreeDBInfo(driveIndex);
            }
        }


        // returns a list of track information from the CD
        public List<string> GetTracks(int driveIndex)
        {
            var tracks = new List<string>();
            if (CDIsReady(driveIndex))
            {
                foreach (var track in audioCDRipper.GetTracks(driveIndex))
                {
                    tracks.Add($"Track {track.TrackNumber}: {track.TrackFile}");
                }
            }
            return tracks;
        }


        // Rips the specified track to the designated file path.
        public void RipTrack(string outputFilePath, int trackIndex, int driveIndex)
        {
            if (!CDIsReady(driveIndex))
            {
                MessageBox.Show("The CD is not ready. Please insert an audio CD into the drive.");
                return;
            }
            try
            {
                audioCDRipper.SelectedDriveIndex = driveIndex;
                audioCDRipper.DestinatioFile = outputFilePath;
                audioCDRipper.Format = Format.MP3;
                audioCDRipper.SourceTracks.Clear();
                audioCDRipper.SourceTracks.Add(new Options.Core.SourceTrack(trackIndex));
                audioCDRipper.Rip();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error ripping track: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }

        // for updating the UI during the rip process
        public delegate void UpdateLabelDelegate(string text);
        public event UpdateLabelDelegate UpdateLabelEvent;



        // Event handling for the rip process.
        private void OnRipProgress(object sender, CSAudioCDRipper.PercentArgs e)
        {
            UpdateLabelEvent?.Invoke($"Ripping progress: {e.Number}%");// sends the update work % complete to the lable.
        }

        public event EventHandler RipCompleted;

        protected virtual void OnRipCompleted()
        {
            RipCompleted?.Invoke(this, EventArgs.Empty);// create a rip complete event in order to insert metadata without errors from file still in use
        }

        private void OnRipDone(object sender, EventArgs e)
        {
            UpdateLabelEvent?.Invoke("Ripping completed successfully.");//Updates the label to show ripping is done
            OnRipCompleted();
        }

        //Error handling for ripping track
        private void OnRipError(object sender, CSAudioCDRipper.MessageArgs e)
        {
            UpdateLabelEvent?.Invoke($"Error during ripping: {e.String} ({e.Number})");
        }

        //Update lable to say when the ripping starts
        private void OnRipStart(object sender, EventArgs e)
        {
            UpdateLabelEvent?.Invoke("Ripping started...");
        }



        // FreeDB event handling.
        private void OnFreeDBStatus(object sender, CSFreeDB.Core.MessageArgs e)
        {
            Console.WriteLine(e.String);
        }

        private void OnFreeDBError(object sender, CSFreeDB.Core.MessageArgs e)
        {
            Console.WriteLine($"FreeDB error: {e.String} ({e.Number})");
        }

        private void OnFreeDBTracks(object sender, CSFreeDBLib.FreeDB.TrackInfo e)
        {
            Console.WriteLine($"FreeDB track: {e.TrackName}");
        }

        private void OnFreeDBDone(object sender)
        {
            Console.WriteLine("FreeDB operation completed.");
        }

        private void OnFreeDBAlbum(object sender, CSFreeDBLib.FreeDB.AlbumInfo e)
        {
            Console.WriteLine($"Album: {e.AlbumName} by {e.AlbumArtist}");
        }

        // CD drive test operations.
        public void EjectCD(int driveIndex)
        {
            audioCDRipper.EjectCD(driveIndex);
        }

        public void CloseCD(int driveIndex)
        {
            audioCDRipper.CloseCD(driveIndex);
        }


    }
}
