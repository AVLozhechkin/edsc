using System;
using System.Collections.Generic;
using System.Text;

namespace EDSc.Common.DTO
{
    public class InstanceDescriptionDTO : InstanceDTO
    {
        public string BuildVersion { get; set; }
        public string TypeVersion { get; set; }
        public string ConfigJson { get; set; }
    }
}
