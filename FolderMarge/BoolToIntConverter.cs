// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoolToIntConverter.cs" company="Integra Co" author="Alexander Borovskikh">
//   GNU3 2018
// </copyright>
// <summary>
//   The bool to int converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FolderMarge
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// The bool to int converter.
    /// </summary>
    public class BoolToIntConverter : IValueConverter
    {
        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool copy = (bool?)value ?? true;
            return copy ? 0 : 1;
        }

        /// <summary>
        /// The convert back.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int copy = (int?)value ?? 1;
            return copy == 0;
        }
    }
}
