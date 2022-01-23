﻿using System.Runtime.Serialization;
using Slithin.VPL.Components.ViewModels;
using Slithin.Core.VPLNodeBuilding;

namespace Slithin.VPL.Components.ViewModels.ControlFlow;

[DataContract(IsReference = true)]
[NodeCategory("Control")]
public class SwitchNode : VisualNode
{
    public SwitchNode() : base("Switch")
    {
    }

    [Pin("Condition", NodeEditor.Model.PinAlignment.Top)]
    public IInputPin ConditionPin { get; set; }

    [Pin("False", NodeEditor.Model.PinAlignment.Right)]
    public IOutputPin FalsePin { get; set; }

    [Pin("Flow Input")]
    public IInputPin FlowInputPin { get; set; }

    [Pin("True", NodeEditor.Model.PinAlignment.Right)]
    public IOutputPin TruePin { get; set; }
}
