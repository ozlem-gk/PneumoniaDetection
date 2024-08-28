using ObjectDetectionWPFML.Model;
using System;
using System.ComponentModel;
using System.IO;

namespace PneumoniaDetection.Api.Worker {
    public class BackgroundWorkerModel : IBackgroundWorkerModel {
        private BackgroundWorker backgroundWorker;
        private const string tsvFileName = "tsvFile.tsv";
        public bool IsProcessing { get; set; }

        public void StartTheProcess() {
            IsProcessing = true;
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            IsProcessing = false;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
            var rootPath = Directory.GetCurrentDirectory();
            ModelHandler(rootPath);
            var tsvFilePath = Path.Combine(rootPath, tsvFileName);
            ModelBuilder.CreateModel(tsvFilePath);
        }

        private void ModelHandler(string rootPath) {
            var modelPath = Path.Combine(rootPath, "MLModel.zip");
            if (File.Exists(modelPath)) {
                var nameOfBackupFolder = Path.Combine("ModelBackups", DateTime.Now.ToString("ddMMyyyyhhm"));
                var directoryInfo = Directory.CreateDirectory(nameOfBackupFolder);
                File.Copy(modelPath, Path.Combine(directoryInfo.FullName, "MLModel.zip"));
                //File.Delete(modelPath);
            }
        }
    }
}
