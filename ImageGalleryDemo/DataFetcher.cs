using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageGalleryDemo
{
    // Helps to fetch the data from server, parse it and provide the data back to the application.
    class DataFetcher
    {
        // Helps to get image data.
        async Task<string> GetDatafromService(string searchstring)
        {
            string readText = null;
            try
            {
                // Sets the URL of the server api.
                string url = @"https://imagefetcherapi.azurewebsites.net/api/fetch_images?query=" +
               searchstring + "&max_count=5";
                // Uses HttpClient class to fetch the JSON data from the server.
                using (HttpClient c = new HttpClient())
                {
                    readText = await c.GetStringAsync(url);
                }
            }
            catch
            {
                // If no data received (null) use the sample data.
                readText = File.ReadAllText(@"Data/sampleData.json");
            }
            return readText;
        }

        public async Task<List<ImageItem>> GetImageData(string search)
        {
            // Calls the data fetcher function.
            string data = await GetDatafromService(search);
            // Deserialization performed to parse the json data into a valid instance of ImageItem.
            return JsonConvert.DeserializeObject<List<ImageItem>>(data);
        }
    }
}
