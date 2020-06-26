using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsForms
{
    public class BooksList
    {
        List<Book> list;

        public BooksList()
        {
            list = new List<Book>();
        }

        public List<Book> Get()
        {
            return list;
        }

        public bool Add(Book book)
        {
            if (list.Contains(book)) return false;
            else
            {
                list.Add(book);
                return true;
            }
        }

        public bool Edit(Book old, Book edited)
        {
            int oldIndex = list.IndexOf(old);
            int editedIndex = list.IndexOf(edited);
            if (oldIndex >= 0 && (editedIndex == oldIndex || editedIndex == -1))
            {
                list[oldIndex] = edited;
                return true;
            }
            else return false;
        }

        public bool Remove(Book book)
        {
            if (list.Remove(book))
            {
                return true;
            }
            else return false;
        }
    }
}
