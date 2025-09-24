using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Common
{
    [XmlInclude(typeof(DesignState))]
    [XmlInclude(typeof(InProductionState))]
    [XmlInclude(typeof(OutOfProductionState))]
    public abstract class ConcreteState
    {
        
    }
}
