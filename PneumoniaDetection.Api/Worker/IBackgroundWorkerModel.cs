namespace PneumoniaDetection.Api.Worker {
    public interface IBackgroundWorkerModel {
        bool IsProcessing { get; set; }

        void StartTheProcess();
    }
}