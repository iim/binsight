using System;

namespace APKInsight.Logic.ControlFlowGraph.Specific
{
    public class SmaliCfgInstruction: CfgInstruction
    {
        public CfgVertex ParentVertex { get; set; }
        public int ParentIndex { get; set; }
        public CfgVertex ParentEntryPointVertex { get; set; }
        public int ParentEntryPointIndex { get; set; }
        public ESmaliInstruction InstructionType { get; set; }
        public int InstructionIndexInMethod { get; set; }
        public DalvikRegister Dest { get; set; } = null;
        public DalvikRegister Src { get; set; } = null;
        public DalvikRegister Index { get; set; } = null;
        public DalvikRegister SrcB { get; set; } = null;
        public DalvikRegister ArraySizeReg { get; set; } = null;
        public DalvikRegister[] ArrayValueRegs { get; set; } = null;
        public DalvikRegister[] ArgsRegs { get; set; } = null;
        public long? ConstLongValue { get; set; } = null;
        public string ConstStrValue { get; set; } = null;
        public string TypeName { get; set; } = null;
        public int ArrayDimentions { get; set; } = 0;
        public bool HasResultValue { get; set; } = true;
        public string Label { get; set; } = null;
        public string TryStart { get; set; } = null;
        public string TryEnd { get; set; } = null;
        public string Field { get; set; } = null;
        public string Function { get; set; } = null;
        public string ClassName => Function?.Substring(0, Function.IndexOf("->", StringComparison.Ordinal));
        public string CodeLine { get; set; } = null;

        public bool IsMoveResultObject =>
            InstructionType == ESmaliInstruction.MoveResultObject;

        public bool IsMoveResult =>
            InstructionType == ESmaliInstruction.MoveResult ||
            InstructionType == ESmaliInstruction.MoveResultWide;

        public bool IsMoveAnyResult => IsMoveObject || IsMoveResultObject;

        public bool IsBranchingLabel =>
            InstructionType == ESmaliInstruction.LabelCond ||
            InstructionType == ESmaliInstruction.LabelGoto;

        public bool IsBranchingStatement =>
            InstructionType == ESmaliInstruction.IfEqual ||
            InstructionType == ESmaliInstruction.IfNotEqual ||
            InstructionType == ESmaliInstruction.IfLessOrEqual ||
            InstructionType == ESmaliInstruction.IfLessThan ||
            InstructionType == ESmaliInstruction.IfGreaterOrEqual ||
            InstructionType == ESmaliInstruction.IfGreaterThan ||
            InstructionType == ESmaliInstruction.IfEqualToZero ||
            InstructionType == ESmaliInstruction.IfNotEqualToZero ||
            InstructionType == ESmaliInstruction.IfLessOrEqualToZero ||
            InstructionType == ESmaliInstruction.IfLessThanToZero ||
            InstructionType == ESmaliInstruction.IfGreaterOrEqualToZero ||
            InstructionType == ESmaliInstruction.IfGreaterThanToZero;

        public bool IsGotoStatement=>
            InstructionType == ESmaliInstruction.Goto ||
            InstructionType == ESmaliInstruction.Goto16 ||
            InstructionType == ESmaliInstruction.Goto32;

        public bool IsSwitchStatement =>
            InstructionType == ESmaliInstruction.PackedSwitch ||
            InstructionType == ESmaliInstruction.SparseSwitch;

        public bool IsMoveObject =>
            InstructionType == ESmaliInstruction.MoveObject ||
            InstructionType == ESmaliInstruction.MoveObject16 ||
            InstructionType == ESmaliInstruction.MoveObjectFrom16;

        public bool IsConst =>
            InstructionType == ESmaliInstruction.Const ||
            InstructionType == ESmaliInstruction.Const4 ||
            InstructionType == ESmaliInstruction.Const16 ||
            InstructionType == ESmaliInstruction.Const ||
            InstructionType == ESmaliInstruction.ConstHigh16 ||
            InstructionType == ESmaliInstruction.ConstWide16 ||
            InstructionType == ESmaliInstruction.ConstWide32 ||
            InstructionType == ESmaliInstruction.ConstWide ||
            InstructionType == ESmaliInstruction.ConstWideHigh16 ||
            InstructionType == ESmaliInstruction.ConstString ||
            InstructionType == ESmaliInstruction.ConstStringJumbo ||
            InstructionType == ESmaliInstruction.ConstClass;

        public bool IsStrConst =>
            InstructionType == ESmaliInstruction.ConstString ||
            InstructionType == ESmaliInstruction.ConstStringJumbo;

        public bool IsGetObject =>
            InstructionType == ESmaliInstruction.StaticGet ||
            InstructionType == ESmaliInstruction.StaticGetWide ||
            InstructionType == ESmaliInstruction.StaticGetObject ||
            InstructionType == ESmaliInstruction.StaticGetBoolean ||
            InstructionType == ESmaliInstruction.StaticGetByte ||
            InstructionType == ESmaliInstruction.StaticGetChar ||
            InstructionType == ESmaliInstruction.StaticGetShort ||
            InstructionType == ESmaliInstruction.InstanceGet ||
            InstructionType == ESmaliInstruction.InstanceGetWide ||
            InstructionType == ESmaliInstruction.InstanceGetObject ||
            InstructionType == ESmaliInstruction.InstanceGetBoolean ||
            InstructionType == ESmaliInstruction.InstanceGetByte ||
            InstructionType == ESmaliInstruction.InstanceGetChar ||
            InstructionType == ESmaliInstruction.InstanceGetShort;

        public bool IsArrayGet =>
            InstructionType == ESmaliInstruction.ArrayGet ||
            InstructionType == ESmaliInstruction.ArrayGetWide ||
            InstructionType == ESmaliInstruction.ArrayGetObject ||
            InstructionType == ESmaliInstruction.ArrayGetBoolean ||
            InstructionType == ESmaliInstruction.ArrayGetByte ||
            InstructionType == ESmaliInstruction.ArrayGetChar ||
            InstructionType == ESmaliInstruction.ArrayGetShort;

        public bool IsReturn =>
            InstructionType == ESmaliInstruction.Return ||
            InstructionType == ESmaliInstruction.ReturnObject ||
            InstructionType == ESmaliInstruction.ReturnWide;

        public bool IsAnyReturn =>
            InstructionType == ESmaliInstruction.ReturnVoid || IsReturn;

        public bool IsWellKnownApi =>
            Function.EndsWith(";->toString()Ljava/lang/String;") ||
            Function == "Ljava/lang/String;->getBytes()[B" ||
            ClassName.StartsWith("Ljava/") ||
            ClassName.StartsWith("Ljavax/") ||
            ClassName == "Ljava/security/Key;" ||
            ClassName == "Ljava/lang/StringBuffer;" ||
            ClassName == "Ljava/lang/StringBuilder;" ||
            ClassName == "Ljava/lang/String;";

        public bool ShouldFollowTheObject =>
            ClassName == "Ljava/lang/StringBuffer;" ||
            ClassName == "Ljava/lang/StringBuilder;" ||
            ClassName == "Ljava/lang/String;";

        public bool IsArrayCopyInstruction => Function != null && Function == "Ljava/lang/System;->arraycopy(Ljava/lang/Object;ILjava/lang/Object;II)V";

        public bool IsInvoke =>
            InstructionType == ESmaliInstruction.InvokeVirtual ||
            InstructionType == ESmaliInstruction.InvokeSuper ||
            InstructionType == ESmaliInstruction.InvokeDirect ||
            InstructionType == ESmaliInstruction.InvokeStatic ||
            InstructionType == ESmaliInstruction.InvokeInterface;

        public bool IsInstanceInvoke =>
            (IsInvoke || IsInvokeRange) &&
            (!IsStaticInvoke);

        public bool IsStaticInvoke 
            => 
                InstructionType == ESmaliInstruction.InvokeStatic ||
                InstructionType == ESmaliInstruction.InvokeStaticRange;

        public bool IsInvokeRange =>
            InstructionType == ESmaliInstruction.InvokeVirtualRange ||
            InstructionType == ESmaliInstruction.InvokeSuperRange ||
            InstructionType == ESmaliInstruction.InvokeDirectRange ||
            InstructionType == ESmaliInstruction.InvokeStaticRange ||
            InstructionType == ESmaliInstruction.InvokeInterfaceRange;
    }

    public class DalvikRegister
    {
        public DalvikRegister(DalvikRegister reg)
        {
            N = reg.N;
            IsParameter = reg.IsParameter;
        }

        public DalvikRegister(int n = -1, bool isP = false)
        {
            N = n;
            IsParameter = isP;
        }
        public DalvikRegister(string regName)
        {
            N = int.Parse(regName.Substring(1));
            IsParameter = regName.StartsWith("p");
        }

        public int N { get; set; }
        public bool IsParameter { get; set; }
        public bool IsReturnTracking { get; set; } = false;

        public int CompareTo(object obj)
        {
            var reg = obj as DalvikRegister;
            if (reg == null) return -1;
            return reg.IsParameter == IsParameter && reg.N == N ? 0 : -1;
        }

        public DalvikRegister Copy()
        {
            return new DalvikRegister(this);
        }
    }
}
