using System.Windows;
using System.Windows.Controls;
using LittleTips.ViewModels;

namespace LittleTips.CustomStyleSelector
{
    public class ListBoxItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultListBoxItemDataTemplate { get; set; }
        public DataTemplate CategoryListBoxItemDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Shortcut shortcut = (Shortcut) item;
            if (!string.IsNullOrEmpty(shortcut.Category))
            {
                return CategoryListBoxItemDataTemplate;
            }
            else
            {
                return DefaultListBoxItemDataTemplate;
            }
        }
    }
}