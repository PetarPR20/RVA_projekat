using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Common
{
    [DataContract]
	public enum EngineConfiguration
	{
        [EnumMember] INLINE,
        [EnumMember] VSHAPED,
        [EnumMember] FLAT
	}
}
