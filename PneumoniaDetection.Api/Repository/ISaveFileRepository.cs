using Microsoft.AspNetCore.Http;
using ObjectDetectionWPFML.Model;
using PneumoniaDetection.Api.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PneumoniaDetection.Api.Repository {
    public interface ISaveFileRepository {
        Task<string> SaveImageAsync(IFormFile file);
        Dictionary<string, bool> CheckFileForSave(ModelOutput model, ScoresToKeepOptions scoresToKeep);
        Task<bool> SaveImageToFolderAsync(IFormFile formFile, bool pneumonia, bool normal);
    }
}