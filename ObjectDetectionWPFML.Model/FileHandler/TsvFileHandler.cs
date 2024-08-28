using System.IO;
using System.Text;

namespace ObjectDetectionWPFML.Model.FileHandler {
    public class TsvFileHandler {

        public void CreateTsvFile() {
            var tsvBuilder = new StringBuilder();
            tsvBuilder.AppendLine($"Label \tImageSource");
            var rootDirectory = Directory.GetCurrentDirectory();
            var imagesPath = Path.Combine(rootDirectory, "Images");
            if (Directory.Exists(imagesPath)) {
                var normalPath = Path.Combine(imagesPath, "Normal");
                var pneumoniaPath = Path.Combine(imagesPath, "Pneumonia");

                foreach (var normalLabel in Directory.GetFiles(normalPath)) {
                    tsvBuilder.AppendLine($"NORMAL \t{normalLabel}");
                }

                foreach (var pneumoniaLabel in Directory.GetFiles(pneumoniaPath)) {
                    tsvBuilder.AppendLine($"PNEUMONIA \t{pneumoniaLabel}");
                }

                var tsvFile = Path.Combine(rootDirectory, "tsvFile.tsv");
                if (File.Exists(tsvFile)) {
                    File.Delete(tsvFile);
                }
                File.AppendAllText(tsvFile, tsvBuilder.ToString());
            }
        }
    }
}
