using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PneumoniaDetection.Api.Commands.Utils;
using PneumoniaDetection.Api.Dtos;
using PneumoniaDetection.Api.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace PneumoniaDetection.Api.Commands {
    public class UploadImageCommand : IRequest<UploadImageCommandResult> {
        public UploadImageCommand(IFormFile file) {
            File = file;
        }

        public IFormFile File { get; }
    }

    public class UploadImageCommandResult {
        private readonly string _message;
        private readonly string _defaultErrorMessage = "No information to be displayed";

        public UploadImageCommandResult(CommandResult result, PredictionResultDto predictionResult) {
            Result = result;
            Data = predictionResult;
        }

        public UploadImageCommandResult(CommandResult result, string message, PredictionResultDto predictionResult, string imagePath) : this(result, predictionResult) {
            _message = message;
        }

        public CommandResult Result { get; }
        public string Message => JsonConvert.SerializeObject(new { error = _message ?? _defaultErrorMessage });
        public PredictionResultDto Data { get; set; }
    }

    public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, UploadImageCommandResult> {
        private readonly IModelConsumerRepository _consumerRepository;
        private readonly ISaveFileRepository _saveFileRepository;
        private readonly ScoresToKeepOptions _scoresToKeep;

        public UploadImageCommandHandler(IModelConsumerRepository consumerRepository,
                                         ISaveFileRepository saveFileRepository,
                                         IOptionsMonitor<ScoresToKeepOptions> scoresToKeep) {
            _consumerRepository = consumerRepository;
            _saveFileRepository = saveFileRepository;
            _scoresToKeep = scoresToKeep.CurrentValue;
        }

        public async Task<UploadImageCommandResult> Handle(UploadImageCommand request, CancellationToken cancellationToken) {
            var filePath = await _saveFileRepository.SaveImageAsync(request.File);
            var result = _consumerRepository.PredictImage(filePath);
            var resultDictionary = _saveFileRepository.CheckFileForSave(result, _scoresToKeep);
            bool keepFile = false;
            string imagePath = string.Empty;
            foreach (var item in resultDictionary) {
                keepFile = item.Value;
                imagePath = item.Key;
            }
            return new UploadImageCommandResult(
                CommandResult.Succes,
                new PredictionResultDto() { Prediction = result.Prediction, NormalScore = result.Score[0], PneumoniaScore = result.Score[1], AddedToContinous = keepFile, ImagePath = imagePath });
        }
    }
}
