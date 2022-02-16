using System;
using System.Collections.Generic;
using System.Text;

namespace Forsvik.Core.Model.External
{
    public class PedigreeOne
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool SendEmail { get; set; }

        public bool StateRightNow1 { get; set; }
        public bool StateRightNow2 { get; set; }
        public bool StateRightNow3 { get; set; }

        public string OtherAssociations { get; set; }

        public bool UsingComputer { get; set; }
        public bool UsingDisgen { get; set; }
        public bool UsingHolger { get; set; }
        public bool UsingOther { get; set; }
        public bool InterestedInProgram { get; set; }
        public string UsingOtherProgramName { get; set; }

        public string KnowMore { get; set; }
        public string Contribute { get; set; }
        public string Suggestions { get; set; }
        public string Parish { get; set; }
        public bool IsSlaktImportant { get; set; }
    }
}
