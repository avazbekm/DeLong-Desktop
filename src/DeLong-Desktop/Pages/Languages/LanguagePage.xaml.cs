using System.Windows;
using System.Globalization;
using System.Windows.Controls;

namespace DeLong_Desktop.Pages.Languages
{
    /// <summary>
    /// LanguagePage.xaml uchun logika
    /// </summary>
    public partial class LanguagePage : Page
    {
        private readonly MainWindow _mainWindow;

        public LanguagePage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton selectedRadioButton && selectedRadioButton.Tag != null)
            {
                string selectedLanguage = selectedRadioButton.Tag.ToString();
                string currentLanguage = GetCurrentLanguage();

                if (!string.IsNullOrEmpty(selectedLanguage) && currentLanguage != selectedLanguage)
                {
                    SetLanguage(selectedLanguage);
                    UpdateLanguage();
                }
            }
        }

        // MainWindow'dan currentLanguage olish uchun yordamchi metod
        private string GetCurrentLanguage()
        {
            return _mainWindow.GetType()
                .GetField("_currentLanguage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(_mainWindow)?.ToString() ?? "en";
        }

        // Tilni o'zgartirish
        private void SetLanguage(string language)
        {
            DeLong_Desktop.Resources.Resource.Culture = new CultureInfo(language);
            var mainWindowType = _mainWindow.GetType();
            var setLanguageMethod = mainWindowType.GetMethod("SetLanguage",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            setLanguageMethod?.Invoke(_mainWindow, new object[] { language });

            // _currentLanguage ni yangilash
            mainWindowType.GetField("_currentLanguage",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_mainWindow, language);
        }

        // Interfeysni yangilash
        private void UpdateLanguage()
        {
            var mainWindowType = _mainWindow.GetType();
            var updateLanguageMethod = mainWindowType.GetMethod("UpdateLanguage",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            updateLanguageMethod?.Invoke(_mainWindow, null);
        }
    }
}