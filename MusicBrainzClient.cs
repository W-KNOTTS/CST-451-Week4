using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FinalProjectWPF_2
{
    // Client for interacting with the MusicBrainz API
    public class MusicBrainzClient
    {
        private readonly HttpClient _httpClient;

        // Constructor initializes the HttpClient
        public MusicBrainzClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://musicbrainz.org/ws/2/")
            };
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "MusicBrainzClient/1.0 ( williamwknotts@gmail.com )");
        }

        // Class to hold recording data
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

        // Method to look up recording data by disc ID
        public async Task<RecordingData> LookupByDiscIdAsync(string discId)
        {
            try
            {
                // Construct the URL for the API request
                string url = $"discid/{discId}?fmt=json&inc=artist-credits+labels+recordings+release-groups+genres+tags";
                // Send the GET request
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                // Read the response as a string
                string json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {json}"); // Debugging: Print API response
                // Parse the JSON response
                return ParseDiscResponse(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving data: {ex.Message}");
                throw;
            }
        }

        // Method to parse the JSON response
        private RecordingData ParseDiscResponse(string json)
        {
            var result = JsonConvert.DeserializeObject<DiscLookupResult>(json);
            if (result.Releases == null || result.Releases.Count == 0)
            {
                throw new ApplicationException("No releases found.");
            }

            var release = result.Releases.FirstOrDefault();
            Console.WriteLine($"Genres: {string.Join(", ", release.Genres?.Select(g => g.Name) ?? Enumerable.Empty<string>())}"); //Debugging: Print Genres - not working properly
            Console.WriteLine($"Tags: {string.Join(", ", release.Tags?.Select(t => t.Name) ?? Enumerable.Empty<string>())}"); // Debugging: Print Tags not working properly

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

        // Method to fetch cover art for a release group
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

        // Class for the cover art results
        private class CoverArtResult
        {
            [JsonProperty("images")]
            public List<CoverImage> Images { get; set; }
        }

        // Class for the cover image
        private class CoverImage
        {
            [JsonProperty("image")]
            public string Image { get; set; }
        }

        // Class for the disc lookup results
        private class DiscLookupResult
        {
            [JsonProperty("releases")]
            public List<Release> Releases { get; set; }
        }

        // Class for the release
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

        // Class for the release group
        private class ReleaseGroup
        {
            [JsonProperty("id")]
            public string Id { get; set; }
        }

        // Class for the artist credit
        private class ArtistCredit
        {
            [JsonProperty("artist")]
            public Artist Artist { get; set; }
        }

        // Class for the an artist
        private class Artist
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }

        //Class for the genre
        private class Genre
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }

        // Class for the tag
        private class Tag
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }
    }
}
