using Microsoft.Extensions.Logging;
using ObjectDetectionWPFML.Model;
using System;

namespace PneumoniaDetection.Api.Repository {
    public class ModelConsumerRepository : IModelConsumerRepository {
        private readonly ILogger<ModelConsumerRepository> _logger;

        public ModelConsumerRepository(ILogger<ModelConsumerRepository> logger) {
            _logger = logger;
        }

        public ModelOutput PredictImage(string filePath) {
            if (string.IsNullOrEmpty(filePath)) {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty", nameof(filePath));
            }
            try {
                ModelInput modelInput = new ModelInput() {
                    ImageSource = filePath
                };

                return ConsumeModel.Predict(modelInput);
            }
            catch (Exception ex) {
                _logger.LogInformation(ex.Message);
            }
            return new ModelOutput();
        }
    }
}
