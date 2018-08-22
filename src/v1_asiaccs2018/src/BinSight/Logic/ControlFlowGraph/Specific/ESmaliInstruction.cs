﻿namespace APKInsight.Logic.ControlFlowGraph.Specific
{
    public enum ESmaliInstruction
    {
        Unknown,
        NoOperation,
        EntryPoint,
        Line,
        Prologue,
        Param,

        Locals,
        Local,
        LocalEnd,
        LocalRestart,

        Move,
        MoveFrom16,
        Move16,
        MoveWide,
        MoveWideFrom16,
        MoveWide16,
        MoveObject,
        MoveObjectFrom16,
        MoveObject16,
        MoveResult,
        MoveResultWide,
        MoveResultObject,
        MoveException,

        ReturnVoid,
        Return,
        ReturnWide,
        ReturnObject,

        Const4,
        Const16,
        Const,
        ConstHigh16,
        ConstWide16,
        ConstWide32,
        ConstWide,
        ConstWideHigh16,
        ConstString,
        ConstStringJumbo,
        ConstClass,

        MonitorEnter,
        MonitorExit,

        CheckCast,

        InstanceOf,

        ArrayLength,

        NewInstance,
        NewArray,
        FilledNewArray,
        FilledNewArrayRange,
        FillArrayData,

        Throw,
        Catch,
        CatchAll,

        Goto,
        Goto16,
        Goto32,

        PackedSwitch,
        PackedSwitchBegin,
        PackedSwitchEnd,
        SparseSwitch,
        SparseSwitchBegin,
        SparseSwitchEnd,

        CompareLtFloat,
        CompareGtFloat,
        CompareLtDouble,
        CompareGtDouble,
        CompareLong,

        IfEqual,
        IfNotEqual,
        IfLessOrEqual,
        IfLessThan,
        IfGreaterOrEqual,
        IfGreaterThan,
        IfEqualToZero,
        IfNotEqualToZero,
        IfLessOrEqualToZero,
        IfLessThanToZero,
        IfGreaterOrEqualToZero,
        IfGreaterThanToZero,

        ArrayGet,
        ArrayGetWide,
        ArrayGetObject,
        ArrayGetBoolean,
        ArrayGetByte,
        ArrayGetChar,
        ArrayGetShort,
        ArrayPut,
        ArrayPutWide,
        ArrayPutObject,
        ArrayPutBoolean,
        ArrayPutByte,
        ArrayPutChar,
        ArrayPutShort,

        InstanceGet,
        InstanceGetWide,
        InstanceGetObject,
        InstanceGetBoolean,
        InstanceGetByte,
        InstanceGetChar,
        InstanceGetShort,
        InstancePut,
        InstancePutWide,
        InstancePutObject,
        InstancePutBoolean,
        InstancePutByte,
        InstancePutChar,
        InstancePutShort,

        StaticGet,
        StaticGetWide,
        StaticGetObject,
        StaticGetBoolean,
        StaticGetByte,
        StaticGetChar,
        StaticGetShort,
        StaticPut,
        StaticPutWide,
        StaticPutObject,
        StaticPutBoolean,
        StaticPutByte,
        StaticPutChar,
        StaticPutShort,

        InvokeVirtual,
        InvokeSuper,
        InvokeDirect,
        InvokeStatic,
        InvokeInterface,

        InvokeVirtualRange,
        InvokeSuperRange,
        InvokeDirectRange,
        InvokeStaticRange,
        InvokeInterfaceRange,

        // Unary operations
        NegInt,
        NotInt,
        NegLong,
        NotLong,
        NegFloat,
        NegDouble,
        IntToLong,
        IntToFloat,
        IntToDouble,
        LongToInt,
        LongToFloat,
        LongToDouble,
        FloatToInt,
        FloatToLong,
        FloatToDouble,
        DoubleToInt,
        DoubleToLong,
        DoubleToFloat,
        IntToByte,
        IntToChar,
        IntToShort,

        // Binary operation
        AddInt,
        SubInt,
        MulInt,
        DivInt,
        RemInt,
        AndInt,
        OrInt,
        XorInt,
        ShlInt,
        ShrInt,
        UshrInt,
        AddLong,
        SubLong,
        MulLong,
        DivLong,
        RemLong,
        AndLong,
        OrLong,
        XorLong,
        ShlLong,
        ShrLong,
        UshrLong,
        AddFloat,
        SubFloat,
        MulFloat,
        DivFloat,
        RemFloat,
        AddDouble,
        SubDouble,
        MulDouble,
        DivDouble,
        RemDouble,
        AddIntAddr2,
        SubIntAddr2,
        MulIntAddr2,
        DivIntAddr2,
        RemIntAddr2,
        AndIntAddr2,
        OrIntAddr2,
        XorIntAddr2,
        ShlIntAddr2,
        ShrIntAddr2,
        UshrIntAddr2,
        AddLongAddr2,
        SubLongAddr2,
        MulLongAddr2,
        DivLongAddr2,
        RemLongAddr2,
        AndLongAddr2,
        OrLongAddr2,
        XorLongAddr2,
        ShlLongAddr2,
        ShrLongAddr2,
        UshrLongAddr2,
        AddFloatAddr2,
        SubFloatAddr2,
        MulFloatAddr2,
        DivFloatAddr2,
        RemFloatAddr2,
        AddDoubleAddr2,
        SubDoubleAddr2,
        MulDoubleAddr2,
        DivDoubleAddr2,
        RemDoubleAddr2,

        AddIntLit16,
        RSubIntLit16,
        MulIntLit16,
        DivIntLit16,
        RemIntLit16,
        AndIntLit16,
        OrIntLit16,
        XorIntLit16,

        AddIntLit8,
        RSubIntLit8,
        MulIntLit8,
        DivIntLit8,
        RemIntLit8,
        AndIntLit8,
        OrIntLit8,
        XorIntLit8,

        LabelGoto,
        LabelCond,
        LabelTryStart,
        LabelTryEnd,
        LabelCatch,
        LabelCatchAll,
        LabelPSwitch,
        LabelPSwitchData,
        LabelSSwitch,
        LabelSSwitchData,
        LabelArray

    }
}
