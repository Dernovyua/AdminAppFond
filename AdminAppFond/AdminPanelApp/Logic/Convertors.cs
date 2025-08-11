using ClassControlsAndStyle.Dialogs;
using ETS.Resources;
using Ex.UI.Kit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace AdminPanelApp.Logic
{
    [ValueConversion(typeof(String), typeof(SolidColorBrush))]
    public class ColorForegroundConverter : IValueConverter
    {
        public static readonly SolidColorBrush white = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(ExThemeManager.AllThemes[ExThemeManager.Instance.CurentTheme]["Foreground-Default"].ToString()));
        public static readonly SolidColorBrush green = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(ExThemeManager.AllThemes[ExThemeManager.Instance.CurentTheme]["Foreground-Profit"].ToString()));
        public static readonly SolidColorBrush red = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(ExThemeManager.AllThemes[ExThemeManager.Instance.CurentTheme]["Foreground-Loss"].ToString()));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            try
            {
                if (!String.IsNullOrEmpty(System.Convert.ToString(value)))
                {
                    var obj = System.Convert.ToDecimal(value);
                    if (obj == 0)
                        return white;

                    if (Math.Abs(obj) > 0)
                        return (obj < 0 ? red : green);
                }
            }
            catch (Exception ex)
            {
                new DialogMessage(LanguageModel.GetString(LanguageDialogMessageKeys.CaptionErrorKey) + " ColorConverter " + ex.Message, LanguageModel.GetString(LanguageDialogMessageKeys.CaptionAttentionKey));
            }
            return white;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

}
