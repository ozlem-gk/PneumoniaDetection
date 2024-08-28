namespace PneumoniaDetection.Api.Dtos {
    public class ScoresToKeepOptions {
        public const string ScoresToKeep = "ScoresToKeep";
        public double NormalScore { get; set; }
        public double PneumoniaScore { get; set; }
    }
}
