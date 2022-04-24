using System.Windows;
using System.Windows.Controls;
using LittleTips.ViewModels;

namespace LittleTips.CustomStyleSelector
{
    public class ListBoxItemStyleSelector : StyleSelector
    {
        public Style DefaultListboxItemStyle { get; set; }
        public Style CategoryListboxItemStyle { set; get; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            Shortcut shortcut = (Shortcut) item;
            if (!string.IsNullOrEmpty(shortcut.Category))
            {
                return CategoryListboxItemStyle;
            }
            else
            {
                return DefaultListboxItemStyle;
            }
        }
    }
}