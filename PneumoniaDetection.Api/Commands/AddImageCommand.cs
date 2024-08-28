using MediatR;
using Microsoft.AspNetCore.Http;
using PneumoniaDetection.Api.Commands.Utils;
using PneumoniaDetection.Api.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace PneumoniaDetection.Api.Commands {
    public class AddImageCommand : IRequest<AddImageCommandResult> {
        public AddImageCommand(IFormFile formFile, bool pneumonia, bool normal) {
            FormFile = formFile;
            Pneumonia = pneumonia;
            Normal = normal;
        }

        public IFormFile FormFile { get; }
        public bool Pneumonia { get; }
        public bool Normal { get; }
    }

    public class AddImageCommandResult {
        public AddImageCommandResult(CommandResult result) {
            Result = result;
        }

        public CommandResult Result { get; }
    }

    public class AddImageCommandHandler : IRequestHandler<AddImageCommand, AddImageCommandResult> {
        private readonly ISaveFileRepository _saveFileRepository;

        public AddImageCommandHandler(ISaveFileRepository saveFileRepository) {
            _saveFileRepository = saveFileRepository;
        }

        public async Task<AddImageCommandResult> Handle(AddImageCommand request, CancellationToken cancellationToken) {
            if (!IsValidRequest(request)) {
                return new AddImageCommandResult(CommandResult.ValidationError);
            }

            var result = await _saveFileRepository.SaveImageToFolderAsync(request.FormFile, request.Pneumonia, request.Normal);

            if (!result) {
                return new AddImageCommandResult(CommandResult.ValidationError);
            }

            return new AddImageCommandResult(CommandResult.Succes);
        }

        private bool IsValidRequest(AddImageCommand request) {
            if (request.FormFile == null || !request.Normal && !request.Pneumonia) {
                return false;
            }

            return true;
        }
    }
}
