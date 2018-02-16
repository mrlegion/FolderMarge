// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Integra Co" author="Alexander Borovskikh">
//   GNU3 2018
// </copyright>
// <summary>
//   Base View Model Class for MainWindow View.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FolderMarge
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows;

    using GongSolutions.Wpf.DragDrop;

    using Microsoft.WindowsAPICodePack.Dialogs;

    using Prism.Commands;
    using Prism.Mvvm;

    /// <inheritdoc />
    /// <summary>
    /// Base View Model Class for MainWindow View.
    /// </summary>
    public class MainWindowViewModel : BindableBase, IDropTarget
    {
        #region Fields

        /// <summary>
        /// The model.
        /// </summary>
        private readonly FolderMargeModel model = new FolderMargeModel();

        /// <summary>
        /// The background worker.
        /// </summary>
        private readonly BackgroundWorker worker;

        /// <summary>
        /// Check work on
        /// </summary>
        private bool notWork;

        /// <summary>
        /// The parent folder.
        /// </summary>
        private string parent;

        /// <summary>
        /// Name of new marge folder.
        /// </summary>
        private string folderName;

        /// <summary>
        /// Need or not deleted old folders.
        /// </summary>
        private bool deleted;

        /// <summary>
        /// Need copy files in new marge folder or only moving.
        /// </summary>
        private bool isCopy = true;

        /// <summary>
        /// Check collection list on selected folders.
        /// </summary>
        private bool isFolderListEmpty;

        /// <summary>
        /// The progress bar show.
        /// </summary>
        private Visibility progressShow;

        #endregion

        #region Construct

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.NotWork = true;

            // Worker initialize
            this.worker = new BackgroundWorker();
            this.worker.DoWork += (sender, args) =>
                {
                    this.NotWork = false;
                    this.ProgressShow = Visibility.Visible;
                    this.model.Merge(Path.Combine(this.parent, this.FolderName), this.IsCopy, this.IsDeleted);
                };
            this.worker.RunWorkerCompleted += (sender, args) =>
                {
                    if (!args.Cancelled)
                    {
                        this.NotWork = true;
                        this.ProgressShow = Visibility.Collapsed;
                        this.model.Progress = 0;
                        this.FolderName = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Work is Fail!");
                    }
                };

            this.model.PropertyChanged += (sender, args) => { this.RaisePropertyChanged(args.PropertyName); };

            this.IsFolderListEmpty = true;
            this.ProgressShow = Visibility.Collapsed;

            this.OpenDialogCommand = new DelegateCommand(this.OpenFolderDialog);
            this.ClearCommand = new DelegateCommand(() =>
                {
                    this.model.Clear();
                    this.IsFolderListEmpty = true;
                });
            this.MargeCommand = new DelegateCommand(() =>
                {
                    this.worker.RunWorkerAsync();
                });
            this.DragAndDropCommand = new DelegateCommand(() => { MessageBox.Show("Drag work"); });
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Gets or sets show progress bar on UI
        /// </summary>
        public Visibility ProgressShow
        {
            get => this.progressShow;
            set => this.SetProperty(ref this.progressShow, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether not work.
        /// </summary>
        public bool NotWork
        {
            get => this.notWork;
            set => this.SetProperty(ref this.notWork, value);
        }

        /// <summary>
        /// Gets or sets the folder name.
        /// </summary>
        public string FolderName
        {
            get => this.folderName;
            set => this.SetProperty(ref this.folderName, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether is deleted.
        /// </summary>
        public bool IsDeleted
        {
            get => this.deleted;
            set => this.SetProperty(ref this.deleted, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether is copy.
        /// </summary>
        public bool IsCopy
        {
            get => this.isCopy;
            set => this.SetProperty(ref this.isCopy, value);
        }

        /// <summary>
        /// Gets progress operation
        /// </summary>
        public int Progress => this.model.Progress;

        /// <summary>
        /// The folders.
        /// </summary>
        public ReadOnlyObservableCollection<string> Folders => this.model.Folders;

        /// <summary>
        /// Gets or sets a value indicating whether is folder list empty.
        /// </summary>
        public bool IsFolderListEmpty
        {
            get => this.isFolderListEmpty;
            set => this.SetProperty(ref this.isFolderListEmpty, value);
        }

        /// <summary>
        /// Gets the open dialog command.
        /// </summary>
        public DelegateCommand OpenDialogCommand { get; }

        /// <summary>
        /// Gets the clear command.
        /// </summary>
        public DelegateCommand ClearCommand { get; }

        /// <summary>
        /// Gets the marge command.
        /// </summary>
        public DelegateCommand MargeCommand { get; }

        /// <summary>
        /// Gets command for drag and drop function
        /// </summary>
        public DelegateCommand DragAndDropCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// The drag over.
        /// </summary>
        /// <param name="dropInfo">
        /// The drop info.
        /// </param>
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = dragFileList.Any(Directory.Exists) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        /// <summary>
        /// The drop.
        /// </summary>
        /// <param name="dropInfo">
        /// The drop info.
        /// </param>
        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>().ToList();
            dropInfo.Effects = dragFileList.Any(Directory.Exists) ? DragDropEffects.Copy : DragDropEffects.None;
            this.model.Add(dragFileList);
            this.IsFolderListEmpty = false;
            this.parent = Path.GetDirectoryName(dragFileList.First());
        }

        /// <summary>
        /// The open folder select dialog.
        /// </summary>
        private void OpenFolderDialog()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                AllowNonFileSystemItems = true,
                Multiselect = true,
                IsFolderPicker = true,
                Title = "Select the folders that you want to merge",
                InitialDirectory = !string.IsNullOrEmpty(this.parent) ? this.parent : Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                MessageBox.Show("No Folder selected");
                return;
            }

            this.model.Add(dialog.FileNames);
            this.IsFolderListEmpty = false;
            this.parent = Path.GetDirectoryName(dialog.FileNames.First());
        }

        #endregion
    }
}
