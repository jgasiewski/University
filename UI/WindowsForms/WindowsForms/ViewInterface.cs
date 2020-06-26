using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsForms
{
    public interface ViewInterface
    {
        void Add(Book book);
        void Edit(Book old, Book edited);
        void Remove(Book book);
    }
}
