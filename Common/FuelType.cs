using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Common
{
	[DataContract]
	public enum FuelType
	{
		[EnumMember] GASOLINE,
        [EnumMember] DIESEL,
        [EnumMember] HYBRID
	}
}
