// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileExistBehavior.cs" company="Integra Co" aithor="Alexander Borovskikh">
//   GNU3 2018
// </copyright>
// <summary>
//   Defines the FileExistBehavior type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FolderMarge
{
    /// <summary>
    /// File moving operation
    /// </summary>
    public enum FileExistBehavior
    {
        /// <summary>
        ///     None: throw IOException "File exist."
        /// </summary>
        None = 0,

        /// <summary>
        ///     Replace: Replace file in ending directory.
        /// </summary>
        Replace = 1,

        /// <summary>
        ///     Skip: Skip this file.
        /// </summary>
        Skip = 2,

        /// <summary>
        ///     Rename: Rename file
        /// </summary>
        Rename = 3
    }
}
