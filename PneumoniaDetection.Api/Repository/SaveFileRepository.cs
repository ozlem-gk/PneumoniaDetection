using Microsoft.AspNetCore.Http;
using ObjectDetectionWPFML.Model;
using PneumoniaDetection.Api.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PneumoniaDetection.Api.Repository {
    public class SaveFileRepository : ISaveFileRepository {
        private string filePath;
        public async Task<string> SaveImageAsync(IFormFile file) {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var directory = Directory.CreateDirectory("Images");
            filePath = Path.Combine(directory.FullName, file.FileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create)) {
                await file.CopyToAsync(fileStream);
            }

            return filePath;
        }

        public async Task<bool> SaveImageToFolderAsync(IFormFile formFile, bool pneumonia, bool normal) {
            if (formFile == null)
                throw new ArgumentNullException(nameof(formFile));

            string imagePath;
            filePath = await SaveImageAsync(formFile);
            if (pneumonia) {
                var directory = Directory.CreateDirectory(@"Images/Pneumonia");
                imagePath = Path.Combine(directory.FullName, Path.GetFileName(filePath));
                File.Copy(filePath, imagePath);
                File.Delete(filePath);
                return true;
            }

            if (normal) {
                var directory = Directory.CreateDirectory(@"Images/Normal");
                imagePath = Path.Combine(directory.FullName, Path.GetFileName(filePath));
                File.Copy(filePath, imagePath);
                File.Delete(filePath);
                return true;
            }

            return false;
        }

        public Dictionary<string, bool> CheckFileForSave(ModelOutput model, ScoresToKeepOptions scoresToKeep) {
            if (!IsModelValid(model)) {
                throw new ArgumentException($"{nameof(model)} is not valid");
            }

            bool keepFile = false;
            string imagePath = string.Empty;
            var dictionary = new Dictionary<string, bool>();

            switch (model.Prediction.ToLower().Trim()) {
                case "pneumonia":
                    if (model.Score[1] >= scoresToKeep.PneumoniaScore) {
                        var directory = Directory.CreateDirectory(@"Images/Pneumonia");
                        imagePath = Path.Combine(directory.FullName, Path.GetFileName(filePath));
                        File.Copy(filePath, imagePath);
                        keepFile = true;
                    }
                    break;
                case "normal":
                    if (model.Score[0] >= scoresToKeep.NormalScore) {
                        var directory = Directory.CreateDirectory(@"Images/Normal");
                        imagePath = Path.Combine(directory.FullName, Path.GetFileName(filePath));
                        File.Copy(filePath, imagePath);
                        keepFile = true;
                    }
                    break;
            }
            File.Delete(filePath);
            dictionary.Add(imagePath, keepFile);
            return dictionary;
        }

        private bool IsModelValid(ModelOutput model) {
            return !string.IsNullOrEmpty(model.Prediction);
        }
    }
}
