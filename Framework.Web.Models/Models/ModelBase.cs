using System.Collections.Generic;

namespace Framework.Web.Models.Model
{
    public class ModelBase<T>
    {
        public int IdItem { get; set; }

        public T Item { get; set; }

        public List<T> ItemList { get; set; }

        public string Action { get; set; }

        public List<string> ErrorMsg { get; set; }
    }
}
