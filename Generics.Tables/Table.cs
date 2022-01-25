using System;
using System.Collections.Generic;

namespace Generics.Tables
{
    public class Table<T1, T2, T3>
    {
        public bool IfOpen { get; set; } = false;
        public bool IfExisted { get; set; } = false;
        public Table<T1, T2, T3> Open 
        { 
            get 
            {
                IfOpen = true;
                return this; 
            } 
        }
        public Table<T1, T2, T3> Existed 
        { 
            get 
            {
                IfExisted = true;
                return this; 
            } 
        }
        public HashSet<T1> Rows { get; set; } = new HashSet<T1>();
        public HashSet<T2> Columns { get; set; } = new HashSet<T2>();
        public Dictionary<Tuple<T1, T2>, T3> Values { get; set; } = new Dictionary<Tuple<T1, T2>, T3>();
        public T3 this[T1 row, T2 column]
        {
            get
            {
                if (IfOpen)
                {
                    IfOpen = false;
					if (Rows.Contains(row) && Columns.Contains(column))
                        return Values[Tuple.Create(row, column)];
                    else if (!Rows.Contains(row) && !Columns.Contains(column))
                        return default(T3);
                    else throw new ArgumentException();
                }
                else
                {
                    IfExisted = false;
                    if (Rows.Contains(row) && Columns.Contains(column))
                        return Values[Tuple.Create(row, column)];
                    else throw new ArgumentException();
                }
            }
            set
            {
                if (IfOpen)
                {
                    IfOpen = false;
                    AddRow(row);
                    AddColumn(column);
                    Values[Tuple.Create(row, column)] = value;
                }
                else
                {
                    IfExisted = false;
                    if (Rows.Contains(row) && Columns.Contains(column))
                        Values[Tuple.Create(row, column)] = value;
                    else throw new ArgumentException();
                }
            }
        }
        public void AddRow(T1 row)
        {
            if (!Rows.Contains(row))
            {
                Rows.Add(row);
                foreach (var e in Columns)
                    Values[Tuple.Create(row, e)] = default(T3);
            }
        }
		
        public void AddColumn(T2 column)
        {
            if (!Columns.Contains(column))
            {
                Columns.Add(column);
                foreach (var e in Rows)
                    Values[Tuple.Create(e, column)] = default(T3);
            }
        }
    }    
}
