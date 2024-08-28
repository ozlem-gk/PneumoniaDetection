using Newtonsoft.Json;
using PneumoniaDetection.DekstopApp.Models;
using PneumoniaDetection.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PneumoniaDetection.Repository {
    public class UploadRepository : IUploadRepository {
        private readonly string _url = "https://localhost:5001/";
        private readonly HttpClient _client;
        public UploadRepository(IHttpClientFactory httpClientFactory) {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri(_url);
        }

        public async Task<ModelResult> GetPredictionResultAsync(string filePath) {
            if (string.IsNullOrEmpty(filePath)) {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            const string apiPath = "api/upload";

            var fileBytes = File.ReadAllBytes(filePath);
            var multiform = new MultipartFormDataContent() {
                { new ByteArrayContent(fileBytes), "image", $"{Guid.NewGuid()}{Path.GetExtension(filePath)}" }
            };

            var response = await _client.PostAsync(apiPath, multiform);
            var responseAsString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ModelResult>(responseAsString);
        }

        public async Task<bool> RemoveFileAsync(string filePath) {
            if (string.IsNullOrEmpty(filePath)) {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            const string apiPath = "api/remove";
            var content = new MultipartFormDataContent()
            { new StringContent(filePath) };

            var response = await _client.PutAsync(apiPath, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddFileAsync(string filePath, bool pneumonia, bool normal) {
            if (string.IsNullOrEmpty(filePath)) {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            const string apiPath = "api/add";
            var fileBytes = File.ReadAllBytes(filePath);
            var x = new SelectionDTO() { Normal = normal, Pneumonia = pneumonia };
            var data = JsonConvert.SerializeObject(x);
            var multiform = new MultipartFormDataContent() {
                { new ByteArrayContent(fileBytes), "image", $"{Guid.NewGuid()}{Path.GetExtension(filePath)}" },
                { new StringContent(data) }
            };

            var response = await _client.PostAsync(apiPath, multiform);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> TrainModel() {
            const string apiPath = "api/train";
            var result = await _client.PostAsync(apiPath, null);

            return result.IsSuccessStatusCode;
        }
    }
}
