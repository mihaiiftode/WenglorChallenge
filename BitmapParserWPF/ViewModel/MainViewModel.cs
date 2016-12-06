using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using BitmapOperations.Controller;

namespace BitmapParserWPF.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public RelayCommand LoadImages { get; private set; }
        public RelayCommand LoadOperations { get; private set; }
        public RelayCommand ExecuteOperations { get; private set; }

        public int SelectedFileIndexIndex
        {
            get { return _selectedFileIndex; }
            set
            {
                _selectedFileIndex = value;
                RaisePropertyChanged();
            }
        }
        public List<string> Files
        {
            get { return _files; }
            set
            {
                _files = value;
                RaisePropertyChanged();
            }
        }
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                RaisePropertyChanged();
            }
        }
        public Bitmap SourceImage
        {
            get { return _sourceImage; }
            set
            {
                _sourceImage = value;
                RaisePropertyChanged();
            }
        }
        public Bitmap OutputImage
        {
            get { return _outputImage; }
            set
            {
                _outputImage = value;
                RaisePropertyChanged();
            }
        }

        private int _selectedFileIndex;
        private List<string> _files;
        private bool _enabled;
        private bool _pathsLoaded;
        private bool _operationsLoaded;
        private Bitmap _outputImage;
        private Bitmap _sourceImage;

        private BitmapOperationsController _operations;
        private int _lastIndex;

        public MainViewModel()
        {
            Files = new List<string>();
            _operations = new BitmapOperationsController();
            LoadImages = new RelayCommand(OnImagePathLoad);
            LoadOperations = new RelayCommand(OnOperationsLoad);
            ExecuteOperations = new RelayCommand(OnExecuteOperations);

            _lastIndex = -1;
        }

        private void OnImagePathLoad()
        {
            try
            {
                _operations.LoadBitmapPaths();

                foreach (var fileSystemInfo in _operations.BitmapFilesPath)
                {
                    Files.Add(fileSystemInfo.Name);
                }

                _pathsLoaded = true;

                EnableOperationExecution();

                SelectedFileIndexIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnOperationsLoad()
        {
            try
            {
                _operations.LoadOperations();
                _operationsLoaded = true;

                EnableOperationExecution();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnExecuteOperations()
        {
            if (SelectedFileIndexIndex == _lastIndex) return;
            _operations.ColorAndRotateAtIndex(SelectedFileIndexIndex);
            SourceImage = _operations.GetInputBitmap(SelectedFileIndexIndex);
            OutputImage = _operations.GetOutputBitmap(SelectedFileIndexIndex);
            _lastIndex = SelectedFileIndexIndex;
        }

        private void EnableOperationExecution()
        {
            Enabled = _operationsLoaded && _pathsLoaded;
        }
    }
}