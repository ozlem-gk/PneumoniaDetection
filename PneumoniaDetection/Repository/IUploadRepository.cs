using PneumoniaDetection.Models;
using System.Threading.Tasks;

namespace PneumoniaDetection.Repository {
    public interface IUploadRepository {
        Task<ModelResult> GetPredictionResultAsync(string filePath);
        Task<bool> RemoveFileAsync(string filePath);
        Task<bool> AddFileAsync(string filePath, bool pneumonia, bool normal);
        Task<bool> TrainModel();
    }
}