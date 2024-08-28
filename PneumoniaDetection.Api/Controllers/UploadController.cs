using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ObjectDetectionWPFML.Model.FileHandler;
using PneumoniaDetection.Api.Commands;
using PneumoniaDetection.Api.Commands.Utils;
using PneumoniaDetection.Api.Dtos;
using PneumoniaDetection.Api.Worker;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PneumoniaDetection.Api.Controllers {
    [ApiController]
    [Route("api/")]
    public class UploadController : ControllerBase {
        private readonly IMediator _mediator;
        private IBackgroundWorkerModel _backgroundWorkerModel;

        public UploadController(IMediator mediator, IBackgroundWorkerModel backgroundWorkerModel) {
            _mediator = mediator;
            _backgroundWorkerModel = backgroundWorkerModel;
        }

        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadImage(IFormFile file) {
            if (!HttpContext.Request.Form.Files.Any()) {
                return BadRequest();
            }
            file = HttpContext.Request.Form.Files.First();

            var result = await _mediator.Send(new UploadImageCommand(file));

            return result.Result switch {
                CommandResult.Succes => Ok(result.Data),
                CommandResult.InternalError => Conflict(result.Message),
                CommandResult.ValidationError => BadRequest(result.Message),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [HttpPut("remove")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveImage([FromForm] string file) {
            if (string.IsNullOrEmpty(file)) {
                return BadRequest();
            }

            var result = await _mediator.Send(new RemoveImageCommand(file));

            return result.Result switch {
                CommandResult.Succes => Ok(),
                CommandResult.ValidationError => BadRequest(),
                CommandResult.InternalError => Conflict(),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddImage([FromForm] IFormFile file, [FromForm] SelectionDto data) {
            if (!HttpContext.Request.Form.Files.Any()) {
                return BadRequest();
            }

            file = HttpContext.Request.Form.Files.First();

            data = JsonConvert.DeserializeObject<SelectionDto>(HttpContext.Request.Form.First().Value);
            var result = await _mediator.Send(new AddImageCommand(file, (bool)data.Pneumonia, (bool)data.Normal));

            return result.Result switch {
                CommandResult.Succes => Ok(),
                CommandResult.ValidationError => BadRequest(),
                CommandResult.InternalError => Conflict(),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [HttpPost("train")]
        public IActionResult TrainModel() {
            try {
                TsvFileHandler test = new TsvFileHandler();
                test.CreateTsvFile();
                if (_backgroundWorkerModel.IsProcessing) {
                    return Ok("processing");
                }

                _backgroundWorkerModel.StartTheProcess();
            }
            catch (Exception e) {
                return Conflict(e.Message);
            }

            return Ok();
        }
    }
}
