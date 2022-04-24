using System;
using System.Collections.Generic;

namespace LittleTips.Dto
{
    public class App
    {
        public String Id { get; set; }
        public String AppName { get; set; }
        public String AppIcon { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
    }

    public class Category
    {
        public string ChineseCategoryName { get; set; }
        public string EnglishCategoryName { get; set; }
        public List<Key> Keys { get; set; } = new List<Key>();
    }

    public class Key
    {
        public string Id { get; set; }
        public string ShortcutKey { get; set; }
        public string ChineseFunctionDescription { get; set; }
        public string EnglishFunctionDescription { get; set; }
    }
}