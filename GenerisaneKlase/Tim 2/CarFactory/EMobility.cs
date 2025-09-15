using System;
using System.Collections.Generic;
using System.Text;

namespace Tim 2.CarFactory
{
	public abstract class EMobility
	{
		protected double voltage;
		protected PowerSource source;

		public abstract void EVMotorSpecs();
	}
}
