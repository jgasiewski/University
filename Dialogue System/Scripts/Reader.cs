using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Reader : IEnumerable<char>
{
    Queue<string> parsedFiles;
    Queue<string> queuedFiles;
    StreamReader sr;

    int rowNumber;
    int columnNumber;
    string fileName;
    bool newLine = false;

    public int RowNumber
    {
        get
        {
            return rowNumber;
        }
    }

    public int ColumnNumber
    {
        get
        {
            return columnNumber;
        }
    }

    public string FileName
    {
        get
        {
            return fileName;
        }
    }

    public Reader()
    {
        parsedFiles = new Queue<string>();
        queuedFiles = new Queue<string>();
    }

    ~Reader()
    {
        if (sr != null)
        {
            sr.Dispose();
        }
    }

    void CarriageUpdate()
    {       
        if (newLine)
        {
            ++rowNumber;
            columnNumber = 1;
            newLine = false;
        }
        else
        {
            ++columnNumber;
        }
    }

    public void QueueFile(string name)
    {
        if (!parsedFiles.Contains(name) && !queuedFiles.Contains(name))
        {
            queuedFiles.Enqueue(name);
        }
    }

    public void Reset()
    {
        if (sr != null)
        {
            sr.Dispose();
        }
        sr = null;
        parsedFiles.Clear();
        queuedFiles.Clear();
    }

    public IEnumerator<char> GetEnumerator()
    {
        while (queuedFiles.Count > 0)
        {
            rowNumber = 1;
            columnNumber = 0;
            fileName = queuedFiles.Dequeue();
            parsedFiles.Enqueue(fileName);

            using (sr = new StreamReader(fileName))
            {
                while (!sr.EndOfStream)
                {
                    CarriageUpdate();

                    char c = (char)sr.Read();
                    if (c == '\n') newLine = true;
                    yield return c;
                }
            }
        }
        rowNumber = 0;
        columnNumber = 0;
        fileName = null;

        yield return (char)0;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        yield return GetEnumerator().Current;
    }
}
