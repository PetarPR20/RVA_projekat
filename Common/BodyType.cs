using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Common
{
	[DataContract]
	public enum BodyType
	{
        [EnumMember] HATCHBACK,
		[EnumMember] SEDAN,
		[EnumMember] SUV,
		[EnumMember] ESTATE,
		[EnumMember] ROADSTER
	}
}
