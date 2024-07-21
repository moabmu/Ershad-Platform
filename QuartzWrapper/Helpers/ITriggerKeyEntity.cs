using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzWrapper.Helpers
{
    public interface ITriggerKeyEntity
    {
        string QuartzJob_CurrentTriggerKeyName { get; set; }
        string QuartzJob_CurrentTriggerKeyGroup { get; set; }
    }
}
