using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus
{
    public delegate void TailLineEmitterCallback(string line);

    /**
     * Opens a given File for read even if it is currently written by a different process and
     * emits each line to the given TailLineEmitterCallback in correct order. When the end of
     * the file is reached it pauses until ReadMore() is called again and it tries to continues
     * reading from the spot it paused at.
     */
    public class TailLineEmitter
    {
        StreamReader StreamReader { get; set; }
        TailLineEmitterCallback Callback { get; set; }
        int CurrentLine { get; set; } = 0;
        private string file;
        private bool closeOnEndOfFile;
        private bool firstRead = true;
        private bool initialized = false;
        private bool closed = false;

        public TailLineEmitter(string file, bool closeOnEndOfFile, TailLineEmitterCallback callback)
        {
            this.file = file;
            this.Callback = callback;
            this.closeOnEndOfFile = closeOnEndOfFile;
        }

        ~TailLineEmitter()
        {
            Close();
        }

        public void Close()
        {
            if (!closed)
            {
                closed = true;
                if (this.StreamReader != null)
                    this.StreamReader.Close();
            }
        }

        /**
         * Read and emit each line from last read point on until current end of file is reached
         */
        public void ReadMore()
        {
            if (!initialized)
            {
                var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var bufferedFs = new BufferedStream(fs, 128 * 1024);
                this.StreamReader = new StreamReader(bufferedFs);
                initialized = true;
            }
            if (!closeOnEndOfFile ||firstRead)
            {
                firstRead = false;
                try
                {
                    string? line = "";
                    do
                    {
                        line = this.StreamReader.ReadLine();
                        if (line != null)
                        {
                            this.Callback(line);
                        }
                    } while (line != null);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                if (closeOnEndOfFile)
                    Close();
            }
        }

    }
}
