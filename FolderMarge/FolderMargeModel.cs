// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderMargeModel.cs" company="Integra Co" author="Alexander Borovskikh">
//   GNU3 2018
// </copyright>
// <summary>
//   Defines the FolderMargeModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FolderMarge
{
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;

    using Prism.Mvvm;

    // TODO: Сделать возможность выбора, что делать с одинаковыми файлами
    // TODO: Приделать прогресс бар статус о перемещении и объединении файла

    /// <inheritdoc />
    /// <summary>
    /// Base logic model.
    /// </summary>
    public class FolderMargeModel : BindableBase
    {
        #region Fields

        /// <summary>
        /// The folders collection list.
        /// </summary>
        private readonly ObservableCollection<string> folders = new ObservableCollection<string>();

        /// <summary>
        /// Progress from merging status.
        /// </summary>
        private int progress = 0;

        #endregion

        #region Construct

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderMargeModel"/> class.
        /// </summary>
        public FolderMargeModel()
        {
            this.Folders = new ReadOnlyObservableCollection<string>(this.folders);
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Gets Access field for folder collection list.
        /// </summary>
        public ReadOnlyObservableCollection<string> Folders { get; }

        /// <summary>
        /// Gets or sets on progress value on merge status
        /// </summary>
        public int Progress
        {
            get => this.progress;
            set => this.SetProperty(ref this.progress, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add new element to collection folders.
        /// </summary>
        /// <param name="list">
        /// List a folders
        /// </param>
        public void Add(IEnumerable list)
        {
            foreach (var obj in list)
            {
                string folder = (string)obj;
                if (!string.IsNullOrEmpty(folder))
                {
                    this.Add(folder);
                }
            }
        }

        /// <summary>
        /// Add new element to collection folders.
        /// </summary>
        /// <param name="folder">
        /// Path to folder
        /// </param>
        public void Add(string folder)
        {
            this.folders.Add(folder);
            this.RaisePropertyChanged(nameof(this.Folders));
        }

        /// <summary>
        /// Clear all collection folder
        /// </summary>
        public void Clear()
        {
            this.folders.Clear();
            this.RaisePropertyChanged(nameof(this.Folders));
        }

        /// <summary>
        /// Merge all selected folder in collection in one new folder
        /// </summary>
        /// <param name="newFolder">
        /// Name of new merge folder
        /// </param>
        /// <param name="isCopy">
        /// Copy all files out base folders to new merge folder or only cut and moving files
        /// </param>
        /// <param name="isDeleted">
        /// Delete all old folders
        /// </param>
        public void Merge(string newFolder, bool isCopy, bool isDeleted)
        {
            // Create new marge folder if she is not exist
            if (!Directory.Exists(newFolder))
            {
                Directory.CreateDirectory(newFolder);
            }

            int step = 100 / this.Folders.Count;

            foreach (string folder in this.Folders)
            {
                var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories).Where(s => !s.EndsWith(".db"));

                foreach (string file in files)
                {
                    var count = 0;

                    string withoutExtension = Path.GetFileNameWithoutExtension(file);
                    string extension = Path.GetExtension(file);
                    string pathWithDuplicateIndex;

                    do
                    {
                        pathWithDuplicateIndex = Path.Combine(newFolder, $"{withoutExtension}_{count}{extension}");
                    }
                    while (File.Exists(pathWithDuplicateIndex));

                    if (isCopy)
                    {
                        File.Copy(file, pathWithDuplicateIndex);
                    }
                    else
                    {
                        File.Move(file, pathWithDuplicateIndex);
                    }
                }

                this.Progress += step;

                if (isDeleted)
                {
                    Directory.Delete(folder, true);
                }
            }

            if (this.Progress < 100)
            {
                this.Progress = 100;
            }
        }
        
        #endregion
    }
}
