using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json; // required for json serialization and deserialization of musicbrainz

namespace FinalProjectWPF_2
{
    // A client for interacting with the MusicBrainz API
    public class MusicBrainzClient
    {
        // HttpClient instance for sending requests to the MusicBrainz API
        private readonly HttpClient _httpClient;

        // Constructor to initialize the HttpClient with MusicBrainz base URL and user agent
        public MusicBrainzClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://musicbrainz.org/ws/2/")
            };
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "MusicBrainzClient/1.0 ( williamwknotts@gmail.com )");
        }

        // Class to hold the recording data received from MusicBrainz
        public class RecordingData
        {
            public string ArtistName { get; set; }
            public string TrackTitle { get; set; }
            public string AlbumTitle { get; set; }
            public string ReleaseDate { get; set; }
            public IEnumerable<string> Genres { get; set; }
            public IEnumerable<string> Tags { get; set; }
            public string RG { get; set; }
            public string ReleaseGroupId { get; internal set; }
        }

        // retrieves recording data by disc ID
        public async Task<RecordingData> LookupByDiscIdAsync(string discId)
        {
            try
            {
                string url = $"discid/{discId}?fmt=json&inc=artist-credits+labels+recordings+release-groups";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                return ParseDiscResponse(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving data: {ex.Message}");
                throw;
            }
        }

        // Parses the JSON response to a RecordingData
        private RecordingData ParseDiscResponse(string json)
        {
            var result = JsonConvert.DeserializeObject<DiscLookupResult>(json);
            if (result.Releases == null || result.Releases.Count == 0)
            {
                throw new ApplicationException("No releases found.");
            }

            var release = result.Releases.FirstOrDefault();
            return new RecordingData
            {
                ArtistName = release.ArtistCredit?.FirstOrDefault()?.Artist?.Name ?? "Unknown Artist",
                AlbumTitle = release.Title ?? "Unknown Album",
                ReleaseDate = release.Date ?? "Unknown Date",
                Genres = release.Genres?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
                Tags = release.Tags?.Select(t => t.Name) ?? Enumerable.Empty<string>(),
                ReleaseGroupId = release.ReleaseGroup?.Id ?? "Unknown Release Group ID"
            };
        }

        // fetches cover art for a release group
        public async Task<string> FetchCoverArtAsync(string releaseGroupId)
        {
            string url = $"https://coverartarchive.org/release-group/{releaseGroupId}";
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                var coverArtResult = JsonConvert.DeserializeObject<CoverArtResult>(json);
                return coverArtResult.Images.FirstOrDefault()?.Image;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve cover art: {ex.Message}");
                return null;
            }
        }

        //  classes for JSON parsing. Each get/set what their name implies for a media file.
        private class CoverArtResult
        {
            [JsonProperty("images")]
            public List<CoverImage> Images { get; set; }
        }

        private class CoverImage
        {
            [JsonProperty("image")]
            public string Image { get; set; }
        }

        private class DiscLookupResult
        {
            [JsonProperty("releases")]
            public List<Release> Releases { get; set; }
        }

        private class Release
        {
            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("date")]
            public string Date { get; set; }

            [JsonProperty("artist-credit")]
            public List<ArtistCredit> ArtistCredit { get; set; }

            [JsonProperty("genres")]
            public List<Genre> Genres { get; set; }

            [JsonProperty("tags")]
            public List<Tag> Tags { get; set; }

            [JsonProperty("release-group")]
            public ReleaseGroup ReleaseGroup { get; set; }
        }

        private class ReleaseGroup
        {
            [JsonProperty("id")]
            public string Id { get; set; }
        }

        private class ArtistCredit
        {
            [JsonProperty("artist")]
            public Artist Artist { get; set; }
        }

        private class Artist
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }

        private class Genre
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }

        private class Tag
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }
    }
}
