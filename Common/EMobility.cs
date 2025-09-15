using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
	public abstract class EMobility
	{
		protected double voltage;
		protected PowerSource source;

		public abstract void EVMotorSpecs();
	}
}
