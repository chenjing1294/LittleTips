using System;
using System.ComponentModel;

namespace LittleTips.ViewModels
{
    public class Shortcut : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _id;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Category
        {
            get
            {
                if (LittleTips.Properties.Settings.Default.ChineseMode)
                {
                    return ChineseCategoryName;
                }
                else
                {
                    return EnglishCategoryName;
                }
            }
        }

        private string _chineseCategoryName;

        public string ChineseCategoryName
        {
            get => _chineseCategoryName;
            set
            {
                _chineseCategoryName = value;
                OnPropertyChanged(nameof(ChineseCategoryName));
            }
        }

        private string _englishCategoryName;

        public string EnglishCategoryName
        {
            get => _englishCategoryName;
            set
            {
                _englishCategoryName = value;
                OnPropertyChanged(nameof(EnglishCategoryName));
            }
        }

        private string _shortcutKey;

        public string ShortcutKey
        {
            get => _shortcutKey;
            set
            {
                _shortcutKey = value;
                OnPropertyChanged(nameof(ShortcutKey));
            }
        }

        public string FunctionDescription
        {
            get
            {
                if (LittleTips.Properties.Settings.Default.ChineseMode)
                {
                    return ChineseFunctionDescription;
                }
                else
                {
                    return EnglishFunctionDescription;
                }
            }
        }

        private string _englishFunctionDescription;

        public string EnglishFunctionDescription
        {
            get => _englishFunctionDescription;
            set
            {
                _englishFunctionDescription = value;
                OnPropertyChanged(nameof(EnglishFunctionDescription));
            }
        }

        private string _chineseFunctionDescription;

        public string ChineseFunctionDescription
        {
            get => _chineseFunctionDescription;
            set
            {
                _chineseFunctionDescription = value;
                OnPropertyChanged(nameof(ChineseFunctionDescription));
            }
        }

        private bool _isFavorite;

        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                _isFavorite = value;
                OnPropertyChanged(nameof(IsFavorite));
            }
        }
    }
}