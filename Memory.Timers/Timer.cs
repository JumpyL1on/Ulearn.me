using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace Memory.Timers
{
    public class Timer : IDisposable
    {
        public Stopwatch SW { get; } = new Stopwatch();
		public string Name { get; }
        public string Indent { get; private set; }
        public bool IsDisposed { get; private set; } = false;
        public static LinkedList<string> Reports { get; } = new LinkedList<string>();
        public static int PreviousLevelOfNesting { get; private set; }
        public static int CurrentLevelOfNesting { get; private set; }
        public static long WorkTime { get; private set; }
		
	    public Timer(string name) 
        { 
            Name = name; 
            PreviousLevelOfNesting = CurrentLevelOfNesting;
			CurrentLevelOfNesting++;
            Indent = new string(' ', PreviousLevelOfNesting * 4);
            SW.Start(); 
        }
		
		public static Timer Start(string name)
		{ 
			return new Timer(name); 
		}
		
        public static Timer Start()
		{ 
			return new Timer("*"); 
		}
		
        public static string Report
        {
            get
            {
                var totalReport = new StringBuilder();
				foreach (var report in Reports)
					totalReport.Append(report);
				Reports.Clear();
                return totalReport.ToString();
            }
        }
		
        ~Timer()
        {
            Dispose(false);
        }
		
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
		
        protected virtual void Dispose(bool fromDisposeMethod)
        {
            if (!IsDisposed)
            {
                if (fromDisposeMethod)
                {
                    SW.Stop();
                    CurrentLevelOfNesting--;
                    var spaces = new string(' ', 20 - (Indent.Length + Name.Length));
                    if (CurrentLevelOfNesting == PreviousLevelOfNesting)
                    {
                        Reports.AddLast($"{Indent}{Name}{spaces}: {SW.ElapsedMilliseconds}\n");
                        WorkTime += SW.ElapsedMilliseconds;
                    }
                    else
                    {
                        Reports.AddFirst($"{Indent}{Name}{spaces}: {SW.ElapsedMilliseconds}\n");
						spaces = spaces.Remove(0, 7);
						Indent = Indent + "    ";
                        Reports.AddLast($"{Indent}Rest{spaces}: {SW.ElapsedMilliseconds - WorkTime}\n");
                        WorkTime = 0;
                    }
                }
                IsDisposed = true;
            }
        }
    }
}