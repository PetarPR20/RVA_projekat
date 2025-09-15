using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
	public class ElectricMotor : EMobility
	{
		private double power;
		private double torque;
		private string cooling;
		private double batteryCapacity;

        public override void EVMotorSpecs()
        {
            throw new NotImplementedException();
        }
    }
}
