using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsForms
{
    public class Book : IEquatable<Book>
    {
        String title;
        String author;
        String genre;
        String date;

        public Book(String title, String author, String genre, String date)
        {
            this.title = title;
            this.author = author;
            this.genre = genre;
            this.date = date;
        }

        public String[] ToArray()
        {
            return new String[] { title, author, genre, date };
        }

        public bool Equals(Book other)
        {
            return this.title == other.title &&
                   this.author == other.author;
        }

        public String GetTitle()
        {
            return title;
        }

        public String GetAuthor()
        {
            return author;
        }

        public String GetGenre()
        {
            return genre;
        }

        public String GetDate()
        {
            return date;
        }

        public int GetYear()
        {
            return int.Parse(date.Split('-').First());
        }
    }
}
