using MediatR;
using PneumoniaDetection.Api.Commands.Utils;
using PneumoniaDetection.Api.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace PneumoniaDetection.Api.Commands {
    public class RemoveImageCommand : IRequest<RemoveImageCommandResult> {
        public RemoveImageCommand(string file) {
            File = file;
        }

        public string File { get; }
    }

    public class RemoveImageCommandResult {
        public RemoveImageCommandResult(CommandResult result) {
            Result = result;
        }

        public CommandResult Result { get; }
    }

    public class RemoveImageCommandHandler : IRequestHandler<RemoveImageCommand, RemoveImageCommandResult> {
        private readonly IRemoveFileRepository _removeFileRepository;

        public RemoveImageCommandHandler(IRemoveFileRepository removeFileRepository) {
            _removeFileRepository = removeFileRepository;
        }

        public async Task<RemoveImageCommandResult> Handle(RemoveImageCommand request, CancellationToken cancellationToken) {
            if (!IsValidRequest(request)) {
                return new RemoveImageCommandResult(CommandResult.ValidationError);
            }

            var result = await Task.Run(() => _removeFileRepository.RemoveFile(request.File));

            if (!result) {
                return new RemoveImageCommandResult(CommandResult.InternalError);
            }

            return new RemoveImageCommandResult(CommandResult.Succes);
        }

        private bool IsValidRequest(RemoveImageCommand request) {
            if (string.IsNullOrEmpty(request.File)) {
                return false;
            }
            return true;
        }
    }
}
