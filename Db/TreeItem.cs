using System;
using System.Collections.Generic;
using System.Text;

namespace Mio.Core.Dal.Db
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeItem<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public T Item { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TreeItem<T>> Children { get; set; }
    }
}
