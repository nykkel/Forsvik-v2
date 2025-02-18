using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forsvik.Core.Model.External
{
    public class FileModel
    {
        public Guid Id { get; set; }

        public DateTime Created { get; set; }

        public string CreatedDate => Created.ToShortDateString();

        public bool RetrievedCreatedFromImage { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public int Size { get; set; }

        public string SizeDisplay => BytesToString(Size);

        public string Description { get; set; }

        public string Tags { get; set; }

        public string Url { get; set; }

        public bool IsSelected { get; set; }

        public bool Edit { get; set; }

        public string FileName => $"{Name}.{Extension}";

        static string BytesToString(long byteCount)
        {
            string[] suf = { "Kb", "Mb", "Gb", "Tb" }; //Longs run out around EB
            if (byteCount == 0)
                return "0 " + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
        }
    }
}
