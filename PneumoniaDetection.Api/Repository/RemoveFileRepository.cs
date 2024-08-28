using System;
using System.IO;

namespace PneumoniaDetection.Api.Repository {
    public class RemoveFileRepository : IRemoveFileRepository {

        public bool RemoveFile(string filePath) {
            if (string.IsNullOrEmpty(filePath)) {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            string pneumoniaPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Pneumonia", filePath);
            string normalPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Normal", filePath);

            if (File.Exists(pneumoniaPath)) {
                File.Delete(pneumoniaPath);
                return true;
            }

            if (File.Exists(normalPath)) {
                File.Delete(normalPath);
                return true;
            }

            return false;
        }
    }
}
