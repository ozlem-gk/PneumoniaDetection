using System;

namespace PneumoniaDetection.Models {
    public class ModelResult {
        public string Prediction { get; set; }

        private float normalScore;
        public float NormalScore {
            get => (float)Math.Round(normalScore, 3) * 100;
            set { normalScore = value; }
        }

        private float pneumoniaScore;
        public float PneumoniaScore {
            get => (float)Math.Round(pneumoniaScore, 3) * 100;
            set { pneumoniaScore = value; }
        }
        public bool AddedToContinous { get; set; }
        public string ImagePath { get; set; }
    }
}
