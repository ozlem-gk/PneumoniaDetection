using ObjectDetectionWPFML.Model;

namespace PneumoniaDetection.Api.Repository {
    public interface IModelConsumerRepository {
        ModelOutput PredictImage(string filePath);
    }
}