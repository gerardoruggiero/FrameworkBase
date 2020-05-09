using System.Collections.Generic;

namespace Framework.Web.Models.APIModel
{
    public abstract class ApiModelBase<T>
        where T: class, new()
    {
        public T Item { get; set; }

        public string ErrorMsg { get; set; } = string.Empty;

        public List<T> ItemList { get; set; } = new List<T>();

    }
}
