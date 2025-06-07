using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public class QuestionFile
    {
        public List<CategoryFile> Categories { get; set; } = new();
    }

    public class CategoryFile
    {
        public string Name { get; set; } = string.Empty;
        public List<QuestionFile_Item> Questions { get; set; } = new();
    }

    public class QuestionFile_Item
    {
        public string Text { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public int Price { get; set; }
    }
}
