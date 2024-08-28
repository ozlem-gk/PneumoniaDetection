using Microsoft.Win32;
using PneumoniaDetection.Models;
using PneumoniaDetection.Repository;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace PneumoniaDetection {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        private readonly IUploadRepository _uploadRepository;

        #region FileName
        private string _FileName = string.Empty;
        public string FilePath {
            get => _FileName;
            set {
                if (_FileName != value) {
                    _FileName = value;
                    OnPropertyChanged(nameof(FilePath));
                }
            }
        }
        #endregion

        #region AddFileVisibility
        private bool _AddFileVisibility;
        public bool AddFileVisibility {
            get => _AddFileVisibility;
            set {
                if (_AddFileVisibility != value) {
                    _AddFileVisibility = value;
                    OnPropertyChanged(nameof(AddFileVisibility));
                }
            }
        }
        #endregion

        #region PredictionResult
        private ModelResult _PredictionResult;
        public ModelResult PredictionResult {
            get => _PredictionResult;
            set {
                if (_PredictionResult != value) {
                    _PredictionResult = value;
                    OnPropertyChanged(nameof(PredictionResult));
                }
            }
        }
        #endregion

        public MainWindow(IUploadRepository uploadRepository) {
            InitializeComponent();
            startButton.IsEnabled = false;
            DataContext = this;
            _uploadRepository = uploadRepository;
            _PredictionResult = new ModelResult();
        }

        private void Canvas_Drop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && e.Data.GetData(DataFormats.FileDrop) is string[] files) {
                FilePath = files[0];
                startButton.IsEnabled = !string.IsNullOrEmpty(FilePath);
                defaultMessage.Visibility = Visibility.Visible;
                resultMessage.Visibility = Visibility.Collapsed;
                startButton.IsEnabled = true;
            }
        }

        #region ToolBarButtons
        private void ButtonMinimizeWindow_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void Header_MouseDown(object sender, RoutedEventArgs e) {
            DragMove();
        }

        private void ButtonCloseWindow_Click(object sender, RoutedEventArgs e) {
            Close();
        }
        #endregion

        private void ChooseImage_Clicked(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                FilePath = openFileDialog.FileName;
                startButton.IsEnabled = !string.IsNullOrEmpty(FilePath);
                defaultMessage.Visibility = Visibility.Visible;
                resultMessage.Visibility = Visibility.Collapsed;
                startButton.IsEnabled = true;
            }
        }

        private async void StartPrediction_Clicked(object sender, RoutedEventArgs e) {
            startButton.IsEnabled = false;
            progressBar.Visibility = Visibility.Visible;

            PredictionResult = await _uploadRepository.GetPredictionResultAsync(FilePath);
            AddFileVisibility = !PredictionResult.AddedToContinous;

            removeFileButton.IsEnabled = PredictionResult.AddedToContinous;
            if (PredictionResult.AddedToContinous) {
                removeFileButton.Visibility = Visibility.Visible;
                removeFail.Visibility = Visibility.Collapsed;
                removeScuces.Visibility = Visibility.Collapsed;
            } else {
                addFileButton.Visibility = Visibility.Visible;
                addScuces.Visibility = Visibility.Collapsed;
                addFail.Visibility = Visibility.Collapsed;
                if (PredictionResult.Prediction.Equals("normal", StringComparison.CurrentCultureIgnoreCase)) {
                    radioNormal.IsChecked = true;
                } else {
                    radioPneumonia.IsChecked = true;
                }
            }

            defaultMessage.Visibility = Visibility.Collapsed;
            resultMessage.Visibility = Visibility.Visible;
            progressBar.Visibility = Visibility.Collapsed;
        }

        private async void RemoveFile_Clicked(object sender, RoutedEventArgs e) {
            removeFileButton.IsEnabled = false;
            var result = await _uploadRepository.RemoveFileAsync(Path.GetFileName(PredictionResult.ImagePath));
            removeFileButton.Visibility = Visibility.Collapsed;
            if (result) {
                removeScuces.Visibility = Visibility.Visible;
            } else {
                removeFail.Visibility = Visibility.Visible;
            }
        }

        private async void AddFile_Clicked(object sender, RoutedEventArgs e) {
            var result = await _uploadRepository.AddFileAsync(FilePath, (bool)radioPneumonia.IsChecked, (bool)radioNormal.IsChecked);
            addFileButton.Visibility = Visibility.Collapsed;
            if (result) {
                addScuces.Visibility = Visibility.Visible;
                addFail.Visibility = Visibility.Collapsed;
            } else {
                addScuces.Visibility = Visibility.Collapsed;
                addFail.Visibility = Visibility.Visible;
            }
        }

        #region Property changed
        /// <summary>
        /// The event that is fired when any child property changes its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// Call this to fire a <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="name"></param>
        public void OnPropertyChanged(string name) {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        private async void TrainModel_Clicked(object sender, RoutedEventArgs e) {
            var result = await _uploadRepository.TrainModel();
            alertIcon.Visibility = result ? Visibility.Visible : Visibility.Collapsed;
        }
    }

}
