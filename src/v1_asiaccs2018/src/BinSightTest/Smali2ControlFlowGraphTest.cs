using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using APKInsight.Logic.ContentParsing.SmaliParser;
using APKInsight.Logic.ControlFlowGraph;
using APKInsight.Logic.ControlFlowGraph.Specific;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BinSightTest
{
    [TestFixture]
    class Smali2ControlFlowGraphTest: SmaliParser
    {

        #region Testing parsing Smali(Dalvik) operations

        [TestCase("    move v0, v1", 0, false, ESmaliInstruction.Move, 1, false)]
        [TestCase("    move/from16 v0, v1025", 0, false, ESmaliInstruction.MoveFrom16, 1025, false)]
        [TestCase("    move/16 v2223, v1025", 2223, false, ESmaliInstruction.Move16, 1025, false)]
        [TestCase("    move-wide v1, v5", 1, false, ESmaliInstruction.MoveWide, 5, false)]
        [TestCase("    move-wide/from16 v1, v5000", 1, false, ESmaliInstruction.MoveWideFrom16, 5000, false)]
        [TestCase("    move-wide/16 v10000, v5000", 10000, false, ESmaliInstruction.MoveWide16, 5000, false)]
        [TestCase("    move-object v0, v8", 0, false, ESmaliInstruction.MoveObject, 8, false)]
        [TestCase("    move-object/from16 v3, v8000", 3, false, ESmaliInstruction.MoveObjectFrom16, 8000, false)]
        [TestCase("    move-object/16 v30000, v8000", 30000, false, ESmaliInstruction.MoveObject16, 8000, false)]
        [TestCase("    move-result v200", 200, false, ESmaliInstruction.MoveResult)]
        [TestCase("    move-result-wide v201", 201, false, ESmaliInstruction.MoveResultWide)]
        [TestCase("    move-result-object v202", 202, false, ESmaliInstruction.MoveResultObject)]
        [TestCase("    move-exception v203", 203, false, ESmaliInstruction.MoveException)]
        [TestCase("move v0, p1", 0, false, ESmaliInstruction.Move, 1, true)]
        [TestCase("move/from16 p0, v1025", 0, true, ESmaliInstruction.MoveFrom16, 1025, false)]
        [TestCase("move-wide p1, v5", 1, true, ESmaliInstruction.MoveWide, 5, false)]
        [TestCase("move-wide/from16 p1, v5000", 1, true, ESmaliInstruction.MoveWideFrom16, 5000, false)]
        [TestCase("move-object v0, p8", 0, false, ESmaliInstruction.MoveObject, 8, true)]
        [TestCase("move-object/from16 p3, v8000", 3, true, ESmaliInstruction.MoveObjectFrom16, 8000, false)]
        [TestCase("move-result p200", 200, true, ESmaliInstruction.MoveResult)]
        [TestCase("move-result-wide p201", 201, true, ESmaliInstruction.MoveResultWide)]
        [TestCase("move-result-object p202", 202, true, ESmaliInstruction.MoveResultObject)]
        [TestCase("move-exception p203", 203, true, ESmaliInstruction.MoveException)]
        public void OperationParsing_Moves(string codeline, int destN, bool destIsP, ESmaliInstruction opCode, int srcN = -1, bool srcIsP = false)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseMoveInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
            if (srcN == -1)
            {
                Assert.IsNull(v.Src);
            }
            else
            {
                Assert.AreEqual(v.Src.N, srcN);
                Assert.AreEqual(v.Src.IsParameter, srcIsP);
            }
        }

        [TestCase("    return-void", ESmaliInstruction.ReturnVoid)]
        [TestCase("    return-object v0", ESmaliInstruction.ReturnObject, 0, false)]
        [TestCase("    return-object p1", ESmaliInstruction.ReturnObject, 1, true)]
        [TestCase("    return v2", ESmaliInstruction.Return, 2, false)]
        [TestCase("    return p3", ESmaliInstruction.Return, 3, true)]
        [TestCase("    return-wide v10", ESmaliInstruction.ReturnWide, 10, false)]
        [TestCase("    return-wide p11", ESmaliInstruction.ReturnWide, 11, true)]
        public void OperationParsing_Return(string codeline, ESmaliInstruction opCode, int srcN = -1, bool srcIsP = false)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseReturnInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            if (srcN != -1)
            {
                Assert.AreEqual(v.Src.N, srcN);
                Assert.AreEqual(v.Src.IsParameter, srcIsP);
            }
        }

        [TestCase("    const/4 v0, 0x1", ESmaliInstruction.Const4, 1, 0)]
        [TestCase("    const/4 p1, 0x2", ESmaliInstruction.Const4, 2, 1, true)]
        [TestCase("    const/16 v2, 0x3", ESmaliInstruction.Const16, 3, 2)]
        [TestCase("    const/16 p3, 0x4", ESmaliInstruction.Const16, 4, 3, true)]
        [TestCase("    const v4, 0x102002c", ESmaliInstruction.Const, 0x102002c, 4)]
        [TestCase("    const p5, 0x101030b", ESmaliInstruction.Const, 0x101030b, 5, true)]
        [TestCase("    const/high16 v6, 0x3f800000    # 1.0f", ESmaliInstruction.ConstHigh16, 0x3f800000, 6)]
        [TestCase("    const/high16 p7, 0x3f000000    # 0.5f", ESmaliInstruction.ConstHigh16, 0x3f000000, 7, true)]
        [TestCase("    const-wide/16 v8, 0xdc", ESmaliInstruction.ConstWide16, 0xdc, 8)]
        [TestCase("    const-wide/16 p9, -0x2710", ESmaliInstruction.ConstWide16, -0x2710, 9, true)]
        [TestCase("    const-wide/32 v10, 0xea60", ESmaliInstruction.ConstWide32, 0xea60, 10)]
        [TestCase("    const-wide/32 p11, -0x80000000", ESmaliInstruction.ConstWide32, -0x80000000, 11, true)]
        [TestCase("    const-wide v12, 0x3fde28c7460698c7L    # 0.4712389167638204", ESmaliInstruction.ConstWide, 0x3fde28c7460698c7L, 12)]
        [TestCase("    const-wide p13, 0x4021800000000000L    # 8.75", ESmaliInstruction.ConstWide, 0x4021800000000000L, 13, true)]
        [TestCase("    const-wide/high16 v14, -0x8000000000000000L", ESmaliInstruction.ConstWideHigh16, -0x8000000000000000L, 14)]
        [TestCase("    const-wide/high16 p15, 0x3ff0000000000000L    # 1.0", ESmaliInstruction.ConstWideHigh16, 0x3ff0000000000000L, 15, true)]
        public void OperationParsing_ConstLong(string codeline, ESmaliInstruction opCode, long lValue, int destN, bool destIsP = false)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseConstInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
            Assert.AreEqual(v.ConstLongValue, lValue);
        }

        [TestCase("    const-string v14, \"CAPABILITY_CAN_REQUEST_ENHANCED_WEB_ACCESSIBILITY\"", ESmaliInstruction.ConstString, 14, false, "CAPABILITY_CAN_REQUEST_ENHANCED_WEB_ACCESSIBILITY")]
        [TestCase("    const-string p1, \"FLAG_REQUEST_TOUCH_EXPLORATION_MODE\"", ESmaliInstruction.ConstString, 1, true, "FLAG_REQUEST_TOUCH_EXPLORATION_MODE")]
        [TestCase("    const-string/jumbo v14, \"CAPABILITY_CAN_REQUEST_ENHANCED_WEB_ACCESSIBILITY\"", ESmaliInstruction.ConstStringJumbo, 14, false, "CAPABILITY_CAN_REQUEST_ENHANCED_WEB_ACCESSIBILITY")]
        [TestCase("    const-string/jumbo p1, \"FLAG_REQUEST_TOUCH_EXPLORATION_MODE\"", ESmaliInstruction.ConstStringJumbo, 1, true, "FLAG_REQUEST_TOUCH_EXPLORATION_MODE")]
        [TestCase("    const-class v6, Landroid/app/ActionBar;", ESmaliInstruction.ConstClass, 6, false, "Landroid/app/ActionBar;")]
        [TestCase("    const-class p8, Landroid/graphics/drawable/Drawable;", ESmaliInstruction.ConstClass, 8, true, "Landroid/graphics/drawable/Drawable;")]
        public void OperationParsing_ConstString(string codeline, ESmaliInstruction opCode, int destN, bool destIsP, string sValue)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseConstInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
            Assert.AreEqual(v.ConstStrValue, sValue);
        }

        [TestCase("    monitor-enter p0", ESmaliInstruction.MonitorEnter, 0, true)]
        [TestCase("    monitor-exit p1", ESmaliInstruction.MonitorExit, 1, true)]
        [TestCase("    monitor-enter v2", ESmaliInstruction.MonitorEnter, 2, false)]
        [TestCase("    monitor-exit v3", ESmaliInstruction.MonitorExit, 3, false)]
        public void OperationParsing_Monitor(string codeline, ESmaliInstruction opCode, int srcN, bool srcIsP)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseMonitorInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Src.N, srcN);
            Assert.AreEqual(v.Src.IsParameter, srcIsP);
        }

        [TestCase("    check-cast v0, Ljava/util/Map;", ESmaliInstruction.CheckCast, 0, false, "Ljava/util/Map;")]
        [TestCase("    check-cast v3, Ljava/util/Map$Entry;", ESmaliInstruction.CheckCast, 3, false, "Ljava/util/Map$Entry;")]
        [TestCase("    check-cast p2, Landroid/content/pm/ResolveInfo;", ESmaliInstruction.CheckCast, 2, true, "Landroid/content/pm/ResolveInfo;")]
        public void OperationParsing_CheckCast(string codeline, ESmaliInstruction opCode, int srcN, bool srcIsP, string type)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseCheckCastInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Src.N, srcN);
            Assert.AreEqual(v.Src.IsParameter, srcIsP);
            Assert.AreEqual(v.TypeName, type);
        }

        [TestCase("    instance-of v2, v3, Lcom/amazon/kindle/model/content/SideloadBookID;", ESmaliInstruction.InstanceOf, 2, false, 3, false, "Lcom/amazon/kindle/model/content/SideloadBookID;")]
        [TestCase("    instance-of v0, p1, Lcom/amazon/kindle/event/IBlockingEventHandler;", ESmaliInstruction.InstanceOf, 0, false, 1, true, "Lcom/amazon/kindle/event/IBlockingEventHandler;")]
        [TestCase("    instance-of p5, p1, Ljava/util/Set;", ESmaliInstruction.InstanceOf, 5, true, 1, true, "Ljava/util/Set;")]
        public void OperationParsing_InstanceOf(string codeline, ESmaliInstruction opCode, int destN, bool destIsP, int srcN, bool srcIsP, string type)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseInstanceOfInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
            Assert.AreEqual(v.Src.N, srcN);
            Assert.AreEqual(v.Src.IsParameter, srcIsP);
            Assert.AreEqual(v.TypeName, type);
        }

        [TestCase("    array-length v2, v0", ESmaliInstruction.ArrayLength, 2, false, 0, false)]
        [TestCase("    array-length p4, p3", ESmaliInstruction.ArrayLength, 4, true, 3, true)]
        public void OperationParsing_ArrayLength(string codeline, ESmaliInstruction opCode, int destN, bool destIsP, int srcN, bool srcIsP)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseArrayLengthInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
            Assert.AreEqual(v.Src.N, srcN);
            Assert.AreEqual(v.Src.IsParameter, srcIsP);
        }

        [TestCase("    new-instance v2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileFileDexFile;", ESmaliInstruction.NewInstance, 2, false, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileFileDexFile;")]
        [TestCase("    new-instance v0, Landroid/accounts/IAccountAuthenticatorResponse$Stub$Proxy;", ESmaliInstruction.NewInstance, 0, false, "Landroid/accounts/IAccountAuthenticatorResponse$Stub$Proxy;")]
        [TestCase("    new-instance p3, Landroid/support/v4/app/FragmentState$1;", ESmaliInstruction.NewInstance, 3, true, "Landroid/support/v4/app/FragmentState$1;")]
        public void OperationParsing_NewInstance(string codeline, ESmaliInstruction opCode, int destN, bool destIsP, string type)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseNewInstanceInstruction(codeline, ref v));
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
            Assert.AreEqual(v.TypeName, type);
        }

        [TestCase("    new-array v0, v0, [Lamazon/android/dexload/compatibility/DexElementCompatibility;", ESmaliInstruction.NewArray, 0, false, 0, false, "Lamazon/android/dexload/compatibility/DexElementCompatibility;", 1)]
        [TestCase("    new-array v0, v0, [Ljava/lang/Class;", ESmaliInstruction.NewArray, 0, false, 0, false, "Ljava/lang/Class;", 1)]
        [TestCase("    new-array v1, p7, [B", ESmaliInstruction.NewArray, 1, false, 7, true, "B", 1)]
        [TestCase("    new-array p1, p7, [[[B", ESmaliInstruction.NewArray, 1, true, 7, true, "B", 3)]
        public void OperationParsing_NewArray(string codeline, ESmaliInstruction opCode, int destN, bool destIsP, int srcN, bool srcIsP, string type, int dimentions)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseNewArrayInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
            Assert.AreEqual(v.Src.N, srcN);
            Assert.AreEqual(v.Src.IsParameter, srcIsP);
            Assert.AreEqual(v.TypeName, type);
            Assert.AreEqual(v.ArrayDimentions, dimentions);
        }

        [TestCase("    filled-new-array {v2, v3}, [I", ESmaliInstruction.FilledNewArray, 2, false, 3, false, "I", 1)]
        public void OperationParsing_FilledNewArray(string codeline, ESmaliInstruction opCode, int sizeN, bool sizeIsP, int val1N, bool val1IsP, string type, int dimentions)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseFilledNewArrayInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.ArraySizeReg.N, sizeN);
            Assert.AreEqual(v.ArraySizeReg.IsParameter, sizeIsP);
            Assert.AreEqual(v.ArrayValueRegs[0].N, val1N);
            Assert.AreEqual(v.ArrayValueRegs[0].IsParameter, val1IsP);
            Assert.AreEqual(v.TypeName, type);
            Assert.AreEqual(v.ArrayDimentions, dimentions);
        }

        [TestCase("    fill-array-data v0, :array_0", ESmaliInstruction.FillArrayData, 0, false, ":array_0")]
        public void OperationParsing_FillArrayData(string codeline, ESmaliInstruction opCode, int destN, bool destIsP, string label)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseFillArrayDataInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
            Assert.AreEqual(v.Label, label);
        }

        [TestCase("    throw v1", ESmaliInstruction.Throw, 1, false)]
        [TestCase("    throw p2", ESmaliInstruction.Throw, 2, true)]
        public void OperationParsing_Throw(string codeline, ESmaliInstruction opCode, int destN, bool destIsP)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseThrowInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
        }

        [TestCase("    goto :goto_1", ESmaliInstruction.Goto, ":goto_1")]
        [TestCase("    goto :goto_a", ESmaliInstruction.Goto, ":goto_a")]
        [TestCase("    goto/16 :goto_1", ESmaliInstruction.Goto16, ":goto_1")]
        [TestCase("    goto/32 :goto_1", ESmaliInstruction.Goto32, ":goto_1")]
        public void OperationParsing_Goto(string codeline, ESmaliInstruction opCode, string label)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseGotoInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Label, label);
        }

        [TestCase("    sparse-switch p1, :sswitch_data_0", ESmaliInstruction.SparseSwitch, 1, true, ":sswitch_data_0")]
        [TestCase("    packed-switch p0, :pswitch_data_0", ESmaliInstruction.PackedSwitch, 0, true, ":pswitch_data_0")]
        [TestCase("    packed-switch v6, :pswitch_data_0", ESmaliInstruction.PackedSwitch, 6, false, ":pswitch_data_0")]
        public void OperationParsing_PackedOrSparseSwitch(string codeline, ESmaliInstruction opCode, int srcN, bool srcIsP, string label)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseSwitchInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Src.N, srcN);
            Assert.AreEqual(v.Src.IsParameter, srcIsP);
            Assert.AreEqual(v.Label, label);
        }

        [TestCase("    .packed-switch 0x1", ESmaliInstruction.PackedSwitchBegin, 1)]
        [TestCase("    .end packed-switch", ESmaliInstruction.PackedSwitchEnd, -1)]
        [TestCase("    .sparse-switch", ESmaliInstruction.SparseSwitchBegin, -1)]
        [TestCase("    .end sparse-switch", ESmaliInstruction.SparseSwitchEnd, -1)]
        public void OperationParsing_PackedOrSparseSwitchBlock(string codeline, ESmaliInstruction opCode, long value)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseSwitchBlockInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            if (value != -1)
            {
                Assert.AreEqual(v.ConstLongValue, value);
            }
        }

        [TestCase("    cmpl-float v1, p2, v2", ESmaliInstruction.CompareLtFloat, 1, false, 2, true, 2, false)]
        [TestCase("    cmpg-float v2, p3, p4", ESmaliInstruction.CompareGtFloat, 2, false, 3, true, 4, true)]
        [TestCase("    cmpl-double v1, p2, v2", ESmaliInstruction.CompareLtDouble, 1, false, 2, true, 2, false)]
        [TestCase("    cmpg-double v2, p3, p4", ESmaliInstruction.CompareGtDouble, 2, false, 3, true, 4, true)]
        [TestCase("    cmp-long v2, p3, p4", ESmaliInstruction.CompareGtDouble, 2, false, 3, true, 4, true)]
        public void OperationParsing_CompareSwitch(string codeline, ESmaliInstruction opCode, int destN, bool destIsP, int srcN, bool srcIsP, int srcN2, bool srcIsP2)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseCompareInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
            Assert.AreEqual(v.Src.N, srcN);
            Assert.AreEqual(v.Src.IsParameter, srcIsP);
            Assert.AreEqual(v.SrcB.N, srcN2);
            Assert.AreEqual(v.SrcB.IsParameter, srcIsP2);
        }

        [TestCase("    if-ge v0, v1, :cond_2", ESmaliInstruction.IfGreaterOrEqual, 0, false, 1, false, ":cond_2")]
        [TestCase("    if-ge p0, v2, :cond_0", ESmaliInstruction.IfGreaterOrEqual, 0, true, 2, false, ":cond_0")]
        [TestCase("    if-gt p2, p4, :cond_4", ESmaliInstruction.IfGreaterThan, 2, true, 4, true, ":cond_4")]
        [TestCase("    if-lt v0, v1, :cond_3", ESmaliInstruction.IfLessThan, 0, false, 1, false, ":cond_3")]
        [TestCase("    if-le v2, v3, :cond_4", ESmaliInstruction.IfLessOrEqual, 2, false, 3, false, ":cond_4")]
        [TestCase("    if-ne v0, v1, :cond_1", ESmaliInstruction.IfNotEqual, 0, false, 1, false, ":cond_1")]
        [TestCase("    if-eq p0, p1, :cond_12", ESmaliInstruction.IfEqual, 0, true, 1, true, ":cond_12")]
        public void OperationParsing_IfTest(string codeline, ESmaliInstruction opCode, int srcN1, bool srcIsP1, int srcN2, bool srcIsP2, string label)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseIfTestInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Src.N, srcN1);
            Assert.AreEqual(v.Src.IsParameter, srcIsP1);
            Assert.AreEqual(v.SrcB.N, srcN2);
            Assert.AreEqual(v.SrcB.IsParameter, srcIsP2);
            Assert.AreEqual(v.Label, label);
        }

        [TestCase("    if-gez v0, :cond_2", ESmaliInstruction.IfGreaterOrEqualToZero, 0, false, ":cond_2")]
        [TestCase("    if-gez p0, :cond_0", ESmaliInstruction.IfGreaterOrEqualToZero, 0, true, ":cond_0")]
        [TestCase("    if-gtz p2, :cond_4", ESmaliInstruction.IfGreaterThanToZero, 2, true, ":cond_4")]
        [TestCase("    if-ltz v0, :cond_3", ESmaliInstruction.IfLessThanToZero, 0, false, ":cond_3")]
        [TestCase("    if-lez v2, :cond_4", ESmaliInstruction.IfLessOrEqualToZero, 2, false, ":cond_4")]
        [TestCase("    if-nez v0, :cond_1", ESmaliInstruction.IfNotEqualToZero, 0, false, ":cond_1")]
        [TestCase("    if-eqz p0, :cond_12", ESmaliInstruction.IfEqualToZero, 0, true, ":cond_12")]
        public void OperationParsing_IfTestZero(string codeline, ESmaliInstruction opCode, int srcN1, bool srcIsP1, string label)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseIfTestInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Src.N, srcN1);
            Assert.AreEqual(v.Src.IsParameter, srcIsP1);
            Assert.AreEqual(v.SrcB, null);
            Assert.AreEqual(v.Label, label);
        }

        [TestCase("    aput p2, v0, v1", ESmaliInstruction.ArrayPut, 0, false, 2, true, 1, false)]
        [TestCase("    aput-wide p2, v0, v1", ESmaliInstruction.ArrayPutWide, 0, false, 2, true, 1, false)]
        [TestCase("    aput-object p2, v0, v1", ESmaliInstruction.ArrayPutObject, 0, false, 2, true, 1, false)]
        [TestCase("    aput-boolean p2, v0, v1", ESmaliInstruction.ArrayPutBoolean, 0, false, 2, true, 1, false)]
        [TestCase("    aput-byte p2, v0, v1", ESmaliInstruction.ArrayPutByte, 0, false, 2, true, 1, false)]
        [TestCase("    aput-char p2, v0, v1", ESmaliInstruction.ArrayPutChar, 0, false, 2, true, 1, false)]
        [TestCase("    aput-short p2, v0, v1", ESmaliInstruction.ArrayPutShort, 0, false, 2, true, 1, false)]
        [TestCase("    aget p2, v0, v1", ESmaliInstruction.ArrayGet, 2, true, 0, false, 1, false)]
        [TestCase("    aget-wide p2, v0, v1", ESmaliInstruction.ArrayGetWide, 2, true, 0, false, 1, false)]
        [TestCase("    aget-object p2, v0, v1", ESmaliInstruction.ArrayGetObject, 2, true, 0, false, 1, false)]
        [TestCase("    aget-boolean p2, v0, v1", ESmaliInstruction.ArrayGetBoolean, 2, true, 0, false, 1, false)]
        [TestCase("    aget-byte p2, v0, v1", ESmaliInstruction.ArrayGetByte, 2, true, 0, false, 1, false)]
        [TestCase("    aget-char p2, v0, v1", ESmaliInstruction.ArrayGetChar, 2, true, 0, false, 1, false)]
        [TestCase("    aget-short p2, v0, v1", ESmaliInstruction.ArrayGetShort, 2, true, 0, false, 1, false)]
        public void OperationParsing_ArrayOperation(string codeline, ESmaliInstruction opCode, int destN, bool destIsP, int srcN, bool srcIsP, int srcN2, bool srcIsP2)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseArrayOpInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
            Assert.AreEqual(v.Src.N, srcN);
            Assert.AreEqual(v.Src.IsParameter, srcIsP);
            Assert.AreEqual(v.Index.N, srcN2);
            Assert.AreEqual(v.Index.IsParameter, srcIsP2);
            Assert.AreEqual(v.SrcB, null);
        }

        [TestCase("    iput p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstancePut, 0, false, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    iput-wide p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstancePutWide, 0, false, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    iput-object p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstancePutObject, 0, false, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    iput-boolean p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstancePutBoolean, 0, false, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    iput-byte p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstancePutByte, 0, false, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    iput-char p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstancePutChar, 0, false, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    iput-short p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstancePutShort, 0, false, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    iget p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstanceGet, 2, true, 0, false, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    iget-wide p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstanceGetWide, 2, true, 0, false, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    iget-object p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstanceGetObject, 2, true, 0, false, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    iget-boolean p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstanceGetBoolean, 2, true, 0, false, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    iget-byte p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstanceGetByte, 2, true, 0, false, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    iget-char p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstanceGetChar, 2, true, 0, false, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    iget-short p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstanceGetShort, 2, true, 0, false, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        public void OperationParsing_InstanceOperation(string codeline, ESmaliInstruction opCode, int destN, bool destIsP, int srcN, bool srcIsP, string field)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseInstanceOpInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
            Assert.AreEqual(v.Src.N, srcN);
            Assert.AreEqual(v.Src.IsParameter, srcIsP);
            Assert.AreEqual(v.Field, field);
        }

        [TestCase("    sput p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticPut, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    sput-wide p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticPutWide, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    sput-object p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticPutObject, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    sput-boolean p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticPutBoolean, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    sput-byte p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticPutByte, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    sput-char p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticPutChar, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    sput-short p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticPutShort, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    sget p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticGet, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    sget-wide p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticGetWide, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    sget-object p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticGetObject, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    sget-boolean p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticGetBoolean, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    sget-byte p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticGetByte, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    sget-char p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticGetChar, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        [TestCase("    sget-short p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticGetShort, 2, true, "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;")]
        public void OperationParsing_StaticOperation(string codeline, ESmaliInstruction opCode, int destN, bool destIsP, string field)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseStaticOpInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            if (codeline.Contains("sget"))
            {
                Assert.AreEqual(v.Dest.N, destN);
                Assert.AreEqual(v.Dest.IsParameter, destIsP);
            }
            else
            {
                Assert.AreEqual(v.Src.N, destN);
                Assert.AreEqual(v.Src.IsParameter, destIsP);
            }
            Assert.AreEqual(v.Field, field);
        }

        [TestCase("    invoke-direct {v2}, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileFileDexFile;-><init>()V", ESmaliInstruction.InvokeDirect, 1,  "Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileFileDexFile;-><init>()V", 2, false, -1, false)]
        [TestCase("    invoke-virtual {v0, p0}, Lamazon/android/dexload/compatibility/DexElementCompatibility;->findConstructor(Ljava/lang/Class;)V", ESmaliInstruction.InvokeVirtual, 2, "Lamazon/android/dexload/compatibility/DexElementCompatibility;->findConstructor(Ljava/lang/Class;)V",0, false, 0, true)]
        [TestCase("    invoke-static {v4}, Ljava/lang/Boolean;->valueOf(Z)Ljava/lang/Boolean;", ESmaliInstruction.InvokeStatic, 1, "Ljava/lang/Boolean;->valueOf(Z)Ljava/lang/Boolean;", 4, false, -1, false)]
        [TestCase("    invoke-static {}, Landroid/os/Parcel;->obtain()Landroid/os/Parcel;", ESmaliInstruction.InvokeStatic, 0, "Landroid/os/Parcel;->obtain()Landroid/os/Parcel;", -1, false, -1, false)]
        [TestCase("    invoke-super {p0}, Landroid/graphics/drawable/Drawable;->getConstantState()Landroid/graphics/drawable/Drawable$ConstantState;", ESmaliInstruction.InvokeSuper, 1, "Landroid/graphics/drawable/Drawable;->getConstantState()Landroid/graphics/drawable/Drawable$ConstantState;", 0, true, -1, false)]
        [TestCase("    invoke-super {p0, p1}, Landroid/graphics/drawable/Drawable;->onBoundsChange(Landroid/graphics/Rect;)V", ESmaliInstruction.InvokeSuper, 2, "Landroid/graphics/drawable/Drawable;->onBoundsChange(Landroid/graphics/Rect;)V", 0, true, 1, true)]
        [TestCase("    invoke-interface {v3}, Ljava/util/List;->iterator()Ljava/util/Iterator;", ESmaliInstruction.InvokeInterface, 1, "Ljava/util/List;->iterator()Ljava/util/Iterator;", 3, false, -1, false)]
        [TestCase("    invoke-interface {v8, v10, v11, v12}, Landroid/content/SharedPreferences;->getLong(Ljava/lang/String;J)J", ESmaliInstruction.InvokeInterface, 4, "Landroid/content/SharedPreferences;->getLong(Ljava/lang/String;J)J", 8, false, 10, false)]
        [TestCase("    invoke-virtual {p1}, [B->clone()Ljava/lang/Object;", ESmaliInstruction.InvokeVirtual, 1, "[B->clone()Ljava/lang/Object;", 1, true, -1, false)]
        public void OperationParsing_InvokeOperation(string codeline, ESmaliInstruction opCode, int cnt, string function, int argN, bool argIsP, int argN2, bool argIsP2)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseInvokeInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Function, function);
            Assert.AreEqual(v.ArgsRegs.Length, cnt);
            if (cnt > 0)
            {
                Assert.AreEqual(v.ArgsRegs[0].N, argN);
                Assert.AreEqual(v.ArgsRegs[0].IsParameter, argIsP);
            }
            if (cnt > 1)
            {
                Assert.AreEqual(v.ArgsRegs[1].N, argN2);
                Assert.AreEqual(v.ArgsRegs[1].IsParameter, argIsP2);
            }
        }

        [TestCase("    invoke-direct/range {v0 .. v5}, Lamazon/android/dexload/SupplementalDexLoader;->updatePreICSClassLoader(Landroid/content/Context;ZLjava/util/List;Lamazon/android/dexload/SupplementalDexLoader$DexLocation;Ljava/util/zip/ZipFile;)V", ESmaliInstruction.InvokeDirectRange, "Lamazon/android/dexload/SupplementalDexLoader;->updatePreICSClassLoader(Landroid/content/Context;ZLjava/util/List;Lamazon/android/dexload/SupplementalDexLoader$DexLocation;Ljava/util/zip/ZipFile;)V", 0, false, 5)]
        [TestCase("    invoke-virtual/range {v18 .. v19}, Ljava/lang/reflect/Field;->get(Ljava/lang/Object;)Ljava/lang/Object;", ESmaliInstruction.InvokeVirtualRange, "Ljava/lang/reflect/Field;->get(Ljava/lang/Object;)Ljava/lang/Object;", 18, false, 19)]
        [TestCase("    invoke-super/range {p0 .. p1}, Landroid/app/Activity;->onCreate(Landroid/os/Bundle;)V", ESmaliInstruction.InvokeSuperRange, "Landroid/app/Activity;->onCreate(Landroid/os/Bundle;)V", 0, true, 1)]
        [TestCase("    invoke-static/range {v0 .. v8}, Lamazon/android/dexload/SupplementalDexLoader;->access$000(ILjava/lang/String;[Ljava/lang/String;[Ljava/io/File;[Ljava/util/zip/ZipFile;[Ldalvik/system/DexFile;Landroid/content/Context;Lamazon/android/dexload/SupplementalDexLoader$DexLocation;Ljava/util/zip/ZipFile;)V", ESmaliInstruction.InvokeStaticRange, "Lamazon/android/dexload/SupplementalDexLoader;->access$000(ILjava/lang/String;[Ljava/lang/String;[Ljava/io/File;[Ljava/util/zip/ZipFile;[Ldalvik/system/DexFile;Landroid/content/Context;Lamazon/android/dexload/SupplementalDexLoader$DexLocation;Ljava/util/zip/ZipFile;)V", 0, false, 8)]
        [TestCase("    invoke-interface/range {p3 .. p3}, Ljava/util/List;->size()I", ESmaliInstruction.InvokeInterfaceRange, "Ljava/util/List;->size()I", 3, true, 3)]
        [TestCase("    invoke-virtual/range {p1 .. p1}, [B->clone()Ljava/lang/Object;", ESmaliInstruction.InvokeVirtualRange, "[B->clone()Ljava/lang/Object;", 1, true, 1)]
        public void OperationParsing_InvokeRangedOperation(string codeline, ESmaliInstruction opCode, string function, int argN, bool argIsP, int argN2)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseInvokeRangeInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Function, function);
            Assert.AreEqual(v.ArgsRegs.Length, argN2 - argN + 1);
            foreach (var dalvikRegister in v.ArgsRegs)
            {
                Assert.AreEqual(dalvikRegister.IsParameter, argIsP);
            }
        }

        [TestCase("    neg-int p3, v7", ESmaliInstruction.NegInt, 3, true, 7, false)]
        [TestCase("    not-int p3, v7", ESmaliInstruction.NotInt, 3, true, 7, false)]
        [TestCase("    neg-long p3, v7", ESmaliInstruction.NegLong, 3, true, 7, false)]
        [TestCase("    not-long p3, v7", ESmaliInstruction.NotLong, 3, true, 7, false)]
        [TestCase("    neg-float p3, v7", ESmaliInstruction.NegFloat, 3, true, 7, false)]
        [TestCase("    neg-double p3, v7", ESmaliInstruction.NegDouble, 3, true, 7, false)]
        [TestCase("    int-to-long p3, v7", ESmaliInstruction.IntToLong, 3, true, 7, false)]
        [TestCase("    int-to-float p3, v7", ESmaliInstruction.IntToFloat, 3, true, 7, false)]
        [TestCase("    int-to-double p3, v7", ESmaliInstruction.IntToDouble, 3, true, 7, false)]
        [TestCase("    long-to-int p3, v7", ESmaliInstruction.LongToInt, 3, true, 7, false)]
        [TestCase("    long-to-float p3, v7", ESmaliInstruction.LongToFloat, 3, true, 7, false)]
        [TestCase("    long-to-double p3, v7", ESmaliInstruction.LongToDouble, 3, true, 7, false)]
        [TestCase("    float-to-int p3, v7", ESmaliInstruction.FloatToInt, 3, true, 7, false)]
        [TestCase("    float-to-long p3, v7", ESmaliInstruction.FloatToLong, 3, true, 7, false)]
        [TestCase("    float-to-double p3, v7", ESmaliInstruction.FloatToDouble, 3, true, 7, false)]
        [TestCase("    double-to-int p3, v7", ESmaliInstruction.DoubleToInt, 3, true, 7, false)]
        [TestCase("    double-to-long p3, v7", ESmaliInstruction.DoubleToLong, 3, true, 7, false)]
        [TestCase("    double-to-float p3, v7", ESmaliInstruction.DoubleToFloat, 3, true, 7, false)]
        [TestCase("    int-to-byte p3, v7", ESmaliInstruction.IntToByte, 3, true, 7, false)]
        [TestCase("    int-to-char p3, v7", ESmaliInstruction.IntToChar, 3, true, 7, false)]
        [TestCase("    int-to-short p3, v7", ESmaliInstruction.IntToShort, 3, true, 7, false)]
        public void OperationParsing_UnaryOperation(string codeline, ESmaliInstruction opCode, int destN, bool destIsP, int srcN, bool srcIsP)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseUnaryOperationInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
            Assert.AreEqual(v.Src.N, srcN);
            Assert.AreEqual(v.Src.IsParameter, srcIsP);
        }

        [TestCase("    add-int p3, v7, p15", ESmaliInstruction.AddInt, 3, true, 7, false, 15, true)]
        [TestCase("    sub-int p3, v7, p15", ESmaliInstruction.SubInt, 3, true, 7, false, 15, true)]
        [TestCase("    mul-int p3, v7, p15", ESmaliInstruction.MulInt, 3, true, 7, false, 15, true)]
        [TestCase("    div-int p3, v7, p15", ESmaliInstruction.DivInt, 3, true, 7, false, 15, true)]
        [TestCase("    rem-int p3, v7, p15", ESmaliInstruction.RemInt, 3, true, 7, false, 15, true)]
        [TestCase("    and-int p3, v7, p15", ESmaliInstruction.AndInt, 3, true, 7, false, 15, true)]
        [TestCase("    or-int p3, v7, p15", ESmaliInstruction.OrInt, 3, true, 7, false, 15, true)]
        [TestCase("    xor-int p3, v7, p15", ESmaliInstruction.XorInt, 3, true, 7, false, 15, true)]
        [TestCase("    shl-int p3, v7, p15", ESmaliInstruction.ShlInt, 3, true, 7, false, 15, true)]
        [TestCase("    shr-int p3, v7, p15", ESmaliInstruction.ShrInt, 3, true, 7, false, 15, true)]
        [TestCase("    ushr-int p3, v7, p15", ESmaliInstruction.UshrInt, 3, true, 7, false, 15, true)]
        [TestCase("    add-long p3, v7, p15", ESmaliInstruction.AddLong, 3, true, 7, false, 15, true)]
        [TestCase("    sub-long p3, v7, p15", ESmaliInstruction.SubLong, 3, true, 7, false, 15, true)]
        [TestCase("    mul-long p3, v7, p15", ESmaliInstruction.MulLong, 3, true, 7, false, 15, true)]
        [TestCase("    div-long p3, v7, p15", ESmaliInstruction.DivLong, 3, true, 7, false, 15, true)]
        [TestCase("    rem-long p3, v7, p15", ESmaliInstruction.RemLong, 3, true, 7, false, 15, true)]
        [TestCase("    and-long p3, v7, p15", ESmaliInstruction.AndLong, 3, true, 7, false, 15, true)]
        [TestCase("    or-long p3, v7, p15", ESmaliInstruction.OrLong, 3, true, 7, false, 15, true)]
        [TestCase("    xor-long p3, v7, p15", ESmaliInstruction.XorLong, 3, true, 7, false, 15, true)]
        [TestCase("    shl-long p3, v7, p15", ESmaliInstruction.ShlLong, 3, true, 7, false, 15, true)]
        [TestCase("    shr-long p3, v7, p15", ESmaliInstruction.ShrLong, 3, true, 7, false, 15, true)]
        [TestCase("    ushr-long p3, v7, p15", ESmaliInstruction.UshrLong, 3, true, 7, false, 15, true)]
        [TestCase("    add-float p3, v7, p15", ESmaliInstruction.AddFloat, 3, true, 7, false, 15, true)]
        [TestCase("    sub-float p3, v7, p15", ESmaliInstruction.SubFloat, 3, true, 7, false, 15, true)]
        [TestCase("    mul-float p3, v7, p15", ESmaliInstruction.MulFloat, 3, true, 7, false, 15, true)]
        [TestCase("    div-float p3, v7, p15", ESmaliInstruction.DivFloat, 3, true, 7, false, 15, true)]
        [TestCase("    rem-float p3, v7, p15", ESmaliInstruction.RemFloat, 3, true, 7, false, 15, true)]
        [TestCase("    add-double p3, v7, p15", ESmaliInstruction.AddDouble, 3, true, 7, false, 15, true)]
        [TestCase("    sub-double p3, v7, p15", ESmaliInstruction.SubDouble, 3, true, 7, false, 15, true)]
        [TestCase("    mul-double p3, v7, p15", ESmaliInstruction.MulDouble, 3, true, 7, false, 15, true)]
        [TestCase("    div-double p3, v7, p15", ESmaliInstruction.DivDouble, 3, true, 7, false, 15, true)]
        [TestCase("    rem-double p3, v7, p15", ESmaliInstruction.RemDouble, 3, true, 7, false, 15, true)]
        public void OperationParsing_BinaryOperation(string codeline, ESmaliInstruction opCode, int destN, bool destIsP, int srcN, bool srcIsP, int srcN2, bool srcIsP2)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseBinaryOperationInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
            Assert.AreEqual(v.Src.N, srcN);
            Assert.AreEqual(v.Src.IsParameter, srcIsP);
            Assert.AreEqual(v.SrcB.N, srcN2);
            Assert.AreEqual(v.SrcB.IsParameter, srcIsP2);
        }

        [TestCase("    add-int/addr2 p3, v7", ESmaliInstruction.AddIntAddr2, 3, true, 7, false)]
        [TestCase("    sub-int/addr2 p3, v7", ESmaliInstruction.SubIntAddr2, 3, true, 7, false)]
        [TestCase("    mul-int/addr2 p3, v7", ESmaliInstruction.MulIntAddr2, 3, true, 7, false)]
        [TestCase("    div-int/addr2 p3, v7", ESmaliInstruction.DivIntAddr2, 3, true, 7, false)]
        [TestCase("    rem-int/addr2 p3, v7", ESmaliInstruction.RemIntAddr2, 3, true, 7, false)]
        [TestCase("    and-int/addr2 p3, v7", ESmaliInstruction.AndIntAddr2, 3, true, 7, false)]
        [TestCase("    or-int/addr2 p3, v7", ESmaliInstruction.OrIntAddr2, 3, true, 7, false)]
        [TestCase("    xor-int/addr2 p3, v7", ESmaliInstruction.XorIntAddr2, 3, true, 7, false)]
        [TestCase("    shl-int/addr2 p3, v7", ESmaliInstruction.ShlIntAddr2, 3, true, 7, false)]
        [TestCase("    shr-int/addr2 p3, v7", ESmaliInstruction.ShrIntAddr2, 3, true, 7, false)]
        [TestCase("    ushr-int/addr2 p3, v7", ESmaliInstruction.UshrIntAddr2, 3, true, 7, false)]
        [TestCase("    add-long/addr2 p3, v7", ESmaliInstruction.AddLongAddr2, 3, true, 7, false)]
        [TestCase("    sub-long/addr2 p3, v7", ESmaliInstruction.SubLongAddr2, 3, true, 7, false)]
        [TestCase("    mul-long/addr2 p3, v7", ESmaliInstruction.MulLongAddr2, 3, true, 7, false)]
        [TestCase("    div-long/addr2 p3, v7", ESmaliInstruction.DivLongAddr2, 3, true, 7, false)]
        [TestCase("    rem-long/addr2 p3, v7", ESmaliInstruction.RemLongAddr2, 3, true, 7, false)]
        [TestCase("    and-long/addr2 p3, v7", ESmaliInstruction.AndLongAddr2, 3, true, 7, false)]
        [TestCase("    or-long/addr2 p3, v7", ESmaliInstruction.OrLongAddr2, 3, true, 7, false)]
        [TestCase("    xor-long/addr2 p3, v7", ESmaliInstruction.XorLongAddr2, 3, true, 7, false)]
        [TestCase("    shl-long/addr2 p3, v7", ESmaliInstruction.ShlLongAddr2, 3, true, 7, false)]
        [TestCase("    shr-long/addr2 p3, v7", ESmaliInstruction.ShrLongAddr2, 3, true, 7, false)]
        [TestCase("    ushr-long/addr2 p3, v7", ESmaliInstruction.UshrLongAddr2, 3, true, 7, false)]
        [TestCase("    add-float/addr2 p3, v7", ESmaliInstruction.AddFloatAddr2, 3, true, 7, false)]
        [TestCase("    sub-float/addr2 p3, v7", ESmaliInstruction.SubFloatAddr2, 3, true, 7, false)]
        [TestCase("    mul-float/addr2 p3, v7", ESmaliInstruction.MulFloatAddr2, 3, true, 7, false)]
        [TestCase("    div-float/addr2 p3, v7", ESmaliInstruction.DivFloatAddr2, 3, true, 7, false)]
        [TestCase("    rem-float/addr2 p3, v7", ESmaliInstruction.RemFloatAddr2, 3, true, 7, false)]
        [TestCase("    add-double/addr2 p3, v7", ESmaliInstruction.AddDoubleAddr2, 3, true, 7, false)]
        [TestCase("    sub-double/addr2 p3, v7", ESmaliInstruction.SubDoubleAddr2, 3, true, 7, false)]
        [TestCase("    mul-double/addr2 p3, v7", ESmaliInstruction.MulDoubleAddr2, 3, true, 7, false)]
        [TestCase("    div-double/addr2 p3, v7", ESmaliInstruction.DivDoubleAddr2, 3, true, 7, false)]
        [TestCase("    rem-double/addr2 p3, v7", ESmaliInstruction.RemDoubleAddr2, 3, true, 7, false)]
        public void OperationParsing_BinaryOperationAddr2(string codeline, ESmaliInstruction opCode, int destN, bool destIsP, int srcN, bool srcIsP)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseBinaryOperationAddr2Instruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
            Assert.AreEqual(v.Src.N, srcN);
            Assert.AreEqual(v.Src.IsParameter, srcIsP);
        }

        [TestCase("    add-int/lit16 p0, v1, 0x353", ESmaliInstruction.AddIntLit16, 0, true, 1, false, (long)0x353)]
        [TestCase("    rsub-int/lit16 p0, v1, 0x353", ESmaliInstruction.RSubIntLit16, 0, true, 1, false, (long)0x353)]
        [TestCase("    mul-int/lit16 p0, v1, 0x353", ESmaliInstruction.MulIntLit16, 0, true, 1, false, (long)0x353)]
        [TestCase("    div-int/lit16 p0, v1, 0x353", ESmaliInstruction.DivIntLit16, 0, true, 1, false, (long)0x353)]
        [TestCase("    rem-int/lit16 p0, v1, 0x353", ESmaliInstruction.RemIntLit16, 0, true, 1, false, (long)0x353)]
        [TestCase("    and-int/lit16 p0, v1, 0x353", ESmaliInstruction.AndIntLit16, 0, true, 1, false, (long)0x353)]
        [TestCase("    or-int/lit16 p0, v1, 0x353", ESmaliInstruction.OrIntLit16, 0, true, 1, false, (long)0x353)]
        [TestCase("    xor-int/lit16 p0, v1, 0x353", ESmaliInstruction.XorIntLit16, 0, true, 1, false, (long)0x353)]
        [TestCase("    add-int/lit8 p0, v1, -0x353", ESmaliInstruction.AddIntLit8, 0, true, 1, false, -(long)0x353)]
        [TestCase("    rsub-int/lit8 p0, v1, -0x353", ESmaliInstruction.RSubIntLit8, 0, true, 1, false, -(long)0x353)]
        [TestCase("    mul-int/lit8 p0, v1, -0x353", ESmaliInstruction.MulIntLit8, 0, true, 1, false, -(long)0x353)]
        [TestCase("    div-int/lit8 p0, v1, -0x353", ESmaliInstruction.DivIntLit8, 0, true, 1, false, -(long)0x353)]
        [TestCase("    rem-int/lit8 p0, v1, -0x353", ESmaliInstruction.RemIntLit8, 0, true, 1, false, -(long)0x353)]
        [TestCase("    and-int/lit8 p0, v1, -0x353", ESmaliInstruction.AndIntLit8, 0, true, 1, false, -(long)0x353)]
        [TestCase("    or-int/lit8 p0, v1, -0x353", ESmaliInstruction.OrIntLit8, 0, true, 1, false, -(long)0x353)]
        [TestCase("    xor-int/lit8 p0, v1, -0x353", ESmaliInstruction.XorIntLit8, 0, true, 1, false, -(long)0x353)]
        public void OperationParsing_BinaryOperationLit(string codeline, ESmaliInstruction opCode, int destN, bool destIsP, int srcN, bool srcIsP, long value)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseBinaryOperationLiteralInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Dest.N, destN);
            Assert.AreEqual(v.Dest.IsParameter, destIsP);
            Assert.AreEqual(v.Src.N, srcN);
            Assert.AreEqual(v.Src.IsParameter, srcIsP);
            Assert.AreEqual(v.ConstLongValue, value);
        }

        [TestCase("    :goto_0", ESmaliInstruction.LabelGoto, ":goto_0")]
        [TestCase("    :try_start_0", ESmaliInstruction.LabelTryStart, ":try_start_0")]
        [TestCase("    :cond_0", ESmaliInstruction.LabelCond, ":cond_0")]
        [TestCase("    :cond_4", ESmaliInstruction.LabelCond, ":cond_4")]
        [TestCase("    :pswitch_0", ESmaliInstruction.LabelPSwitch, ":pswitch_0")]
        [TestCase("    :pswitch_data_0", ESmaliInstruction.LabelPSwitchData, ":pswitch_data_0")]
        [TestCase("    :try_end_0", ESmaliInstruction.LabelTryEnd, ":try_end_0")]
        [TestCase("    :catch_0", ESmaliInstruction.LabelCatch, ":catch_0")]
        [TestCase("    :catchall_0", ESmaliInstruction.LabelCatchAll, ":catchall_0")]
        [TestCase("    :sswitch_0", ESmaliInstruction.LabelSSwitch, ":sswitch_0")]
        [TestCase("    :sswitch_data_0", ESmaliInstruction.LabelSSwitchData, ":sswitch_data_0")]
        [TestCase("    :array_0", ESmaliInstruction.LabelArray, ":array_0")]
        public void OperationParsing_Label(string codeline, ESmaliInstruction opCode, string label)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseLabel(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.Label, label);
        }

        [TestCase("        0x1 -> :sswitch_0", 1, ":sswitch_0")]
        [TestCase("        0x2 -> :sswitch_2", 2, ":sswitch_2")]
        [TestCase("        0x4 -> :sswitch_1", 4, ":sswitch_1")]
        [TestCase("        0x8 -> :sswitch_4", 8, ":sswitch_4")]
        [TestCase("        0x10 -> :sswitch_3", 16, ":sswitch_3")]
        public void OperationParsing_SSwitchLabel(string codeline, long value, string label)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseSSwitchLabel(codeline, ref v));
            Assert.AreEqual(v.InstructionType, ESmaliInstruction.LabelSSwitch);
            Assert.AreEqual(v.Label, label);
            Assert.AreEqual(v.ConstLongValue, value);
        }

        [TestCase("    .catch Ljava/lang/SecurityException; {:try_start_0 .. :try_end_0} :catch_1", ESmaliInstruction.Catch, "Ljava/lang/SecurityException;", ":try_start_0", ":try_end_0", ":catch_1")]
        [TestCase("    .catch Ljava/lang/NoSuchMethodException; {:try_start_0 .. :try_end_0} :catch_0", ESmaliInstruction.Catch, "Ljava/lang/NoSuchMethodException;", ":try_start_0", ":try_end_0", ":catch_0")]
        [TestCase("    .catch Landroid/content/pm/PackageManager$NameNotFoundException; {:try_start_0 .. :try_end_0} :catch_0", ESmaliInstruction.Catch, "Landroid/content/pm/PackageManager$NameNotFoundException;", ":try_start_0", ":try_end_0", ":catch_0")]
        [TestCase("    .catch Ljava/lang/InterruptedException; {:try_start_0 .. :try_end_0} :catch_0", ESmaliInstruction.Catch, "Ljava/lang/InterruptedException;", ":try_start_0", ":try_end_0", ":catch_0")]
        [TestCase("    .catchall {:try_start_0 .. :try_end_0} :catchall_0", ESmaliInstruction.CatchAll, null, ":try_start_0", ":try_end_0", ":catchall_0")]
        public void OperationParsing_Catch(string codeline, ESmaliInstruction opCode, string type, string start, string end, string label)
        {
            var v = new SmaliCfgInstruction();
            Assert.True(ParseCatchInstruction(codeline, ref v));
            Assert.AreEqual(v.InstructionType, opCode);
            Assert.AreEqual(v.TryStart, start);
            Assert.AreEqual(v.TryEnd, end);
            Assert.AreEqual(v.TypeName, type);
            Assert.AreEqual(v.Label, label);
        }

        [TestCase("    move v0, v1", ESmaliInstruction.Move)]
        [TestCase("    move/from16 v0, v1025", ESmaliInstruction.MoveFrom16)]
        [TestCase("    move/16 v2223, v1025", ESmaliInstruction.Move16)]
        [TestCase("    move-wide v1, v5", ESmaliInstruction.MoveWide)]
        [TestCase("    move-wide/from16 v1, v5000", ESmaliInstruction.MoveWideFrom16)]
        [TestCase("    move-wide/16 v10000, v5000", ESmaliInstruction.MoveWide16)]
        [TestCase("    move-object v0, v8", ESmaliInstruction.MoveObject)]
        [TestCase("    move-object/from16 v3, v8000", ESmaliInstruction.MoveObjectFrom16)]
        [TestCase("    move-object/16 v30000, v8000", ESmaliInstruction.MoveObject16)]
        [TestCase("    move-result v200", ESmaliInstruction.MoveResult)]
        [TestCase("    move-result-wide v201", ESmaliInstruction.MoveResultWide)]
        [TestCase("    move-result-object v202", ESmaliInstruction.MoveResultObject)]
        [TestCase("    move-exception v203", ESmaliInstruction.MoveException)]
        [TestCase("move v0, p1", ESmaliInstruction.Move)]
        [TestCase("move/from16 p0, v1025", ESmaliInstruction.MoveFrom16)]
        [TestCase("move-wide p1, v5", ESmaliInstruction.MoveWide)]
        [TestCase("move-wide/from16 p1, v5000", ESmaliInstruction.MoveWideFrom16)]
        [TestCase("move-object v0, p8", ESmaliInstruction.MoveObject)]
        [TestCase("move-object/from16 p3, v8000", ESmaliInstruction.MoveObjectFrom16)]
        [TestCase("move-result p200", ESmaliInstruction.MoveResult)]
        [TestCase("move-result-wide p201", ESmaliInstruction.MoveResultWide)]
        [TestCase("move-result-object p202", ESmaliInstruction.MoveResultObject)]
        [TestCase("move-exception p203", ESmaliInstruction.MoveException)]
        [TestCase("    return-void", ESmaliInstruction.ReturnVoid)]
        [TestCase("    return-object v0", ESmaliInstruction.ReturnObject)]
        [TestCase("    return-object p1", ESmaliInstruction.ReturnObject)]
        [TestCase("    return v2", ESmaliInstruction.Return)]
        [TestCase("    return p3", ESmaliInstruction.Return)]
        [TestCase("    return-wide v10", ESmaliInstruction.ReturnWide)]
        [TestCase("    return-wide p11", ESmaliInstruction.ReturnWide)]
        [TestCase("    const/4 v0, 0x1", ESmaliInstruction.Const4)]
        [TestCase("    const/4 p1, 0x2", ESmaliInstruction.Const4)]
        [TestCase("    const/16 v2, 0x3", ESmaliInstruction.Const16)]
        [TestCase("    const/16 p3, 0x4", ESmaliInstruction.Const16)]
        [TestCase("    const v4, 0x102002c", ESmaliInstruction.Const)]
        [TestCase("    const p5, 0x101030b", ESmaliInstruction.Const)]
        [TestCase("    const/high16 v6, 0x3f800000    # 1.0f", ESmaliInstruction.ConstHigh16)]
        [TestCase("    const/high16 p7, 0x3f000000    # 0.5f", ESmaliInstruction.ConstHigh16)]
        [TestCase("    const-wide/16 v8, 0xdc", ESmaliInstruction.ConstWide16)]
        [TestCase("    const-wide/16 p9, -0x2710", ESmaliInstruction.ConstWide16)]
        [TestCase("    const-wide/32 v10, 0xea60", ESmaliInstruction.ConstWide32)]
        [TestCase("    const-wide/32 p11, -0x80000000", ESmaliInstruction.ConstWide32)]
        [TestCase("    const-wide v12, 0x3fde28c7460698c7L    # 0.4712389167638204", ESmaliInstruction.ConstWide)]
        [TestCase("    const-wide p13, 0x4021800000000000L    # 8.75", ESmaliInstruction.ConstWide)]
        [TestCase("    const-wide/high16 v14, -0x8000000000000000L", ESmaliInstruction.ConstWideHigh16)]
        [TestCase("    const-wide/high16 p15, 0x3ff0000000000000L    # 1.0", ESmaliInstruction.ConstWideHigh16)]
        [TestCase("    const-string v14, \"CAPABILITY_CAN_REQUEST_ENHANCED_WEB_ACCESSIBILITY\"", ESmaliInstruction.ConstString)]
        [TestCase("    const-string p1, \"FLAG_REQUEST_TOUCH_EXPLORATION_MODE\"", ESmaliInstruction.ConstString)]
        [TestCase("    const-string/jumbo v14, \"CAPABILITY_CAN_REQUEST_ENHANCED_WEB_ACCESSIBILITY\"", ESmaliInstruction.ConstStringJumbo)]
        [TestCase("    const-string/jumbo p1, \"FLAG_REQUEST_TOUCH_EXPLORATION_MODE\"", ESmaliInstruction.ConstStringJumbo)]
        [TestCase("    const-class v6, Landroid/app/ActionBar;", ESmaliInstruction.ConstClass)]
        [TestCase("    const-class p8, Landroid/graphics/drawable/Drawable;", ESmaliInstruction.ConstClass)]
        [TestCase("    monitor-enter p0", ESmaliInstruction.MonitorEnter)]
        [TestCase("    monitor-exit p1", ESmaliInstruction.MonitorExit)]
        [TestCase("    monitor-enter v2", ESmaliInstruction.MonitorEnter)]
        [TestCase("    monitor-exit v3", ESmaliInstruction.MonitorExit)]
        [TestCase("    check-cast v0, Ljava/util/Map;", ESmaliInstruction.CheckCast)]
        [TestCase("    check-cast v3, Ljava/util/Map$Entry;", ESmaliInstruction.CheckCast)]
        [TestCase("    check-cast p2, Landroid/content/pm/ResolveInfo;", ESmaliInstruction.CheckCast)]
        [TestCase("    instance-of v2, v3, Lcom/amazon/kindle/model/content/SideloadBookID;", ESmaliInstruction.InstanceOf)]
        [TestCase("    instance-of v0, p1, Lcom/amazon/kindle/event/IBlockingEventHandler;", ESmaliInstruction.InstanceOf)]
        [TestCase("    instance-of p5, p1, Ljava/util/Set;", ESmaliInstruction.InstanceOf)]
        [TestCase("    array-length v2, v0", ESmaliInstruction.ArrayLength)]
        [TestCase("    array-length p4, p3", ESmaliInstruction.ArrayLength)]
        [TestCase("    new-instance v2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileFileDexFile;", ESmaliInstruction.NewInstance)]
        [TestCase("    new-instance v0, Landroid/accounts/IAccountAuthenticatorResponse$Stub$Proxy;", ESmaliInstruction.NewInstance)]
        [TestCase("    new-instance p3, Landroid/support/v4/app/FragmentState$1;", ESmaliInstruction.NewInstance)]
        [TestCase("    new-array v0, v0, [Lamazon/android/dexload/compatibility/DexElementCompatibility;", ESmaliInstruction.NewArray)]
        [TestCase("    new-array v0, v0, [Ljava/lang/Class;", ESmaliInstruction.NewArray)]
        [TestCase("    new-array v1, p7, [B", ESmaliInstruction.NewArray)]
        [TestCase("    new-array p1, p7, [[[B", ESmaliInstruction.NewArray)]
        [TestCase("    filled-new-array {v2, v3}, [I", ESmaliInstruction.FilledNewArray)]
        [TestCase("    fill-array-data v0, :array_0", ESmaliInstruction.FillArrayData)]
        [TestCase("    throw v1", ESmaliInstruction.Throw)]
        [TestCase("    throw p2", ESmaliInstruction.Throw)]
        [TestCase("    goto :goto_1", ESmaliInstruction.Goto)]
        [TestCase("    goto :goto_a", ESmaliInstruction.Goto)]
        [TestCase("    goto/16 :goto_1", ESmaliInstruction.Goto16)]
        [TestCase("    goto/32 :goto_1", ESmaliInstruction.Goto32)]
        [TestCase("    sparse-switch p1, :sswitch_data_0", ESmaliInstruction.SparseSwitch)]
        [TestCase("    packed-switch p0, :pswitch_data_0", ESmaliInstruction.PackedSwitch)]
        [TestCase("    packed-switch v6, :pswitch_data_0", ESmaliInstruction.PackedSwitch)]
        [TestCase("    cmpl-float v1, p2, v2", ESmaliInstruction.CompareLtFloat)]
        [TestCase("    cmpg-float v2, p3, p4", ESmaliInstruction.CompareGtFloat)]
        [TestCase("    cmpl-double v1, p2, v2", ESmaliInstruction.CompareLtDouble)]
        [TestCase("    cmpg-double v2, p3, p4", ESmaliInstruction.CompareGtDouble)]
        [TestCase("    cmp-long v2, p3, p4", ESmaliInstruction.CompareGtDouble)]
        [TestCase("    if-ge v0, v1, :cond_2", ESmaliInstruction.IfGreaterOrEqual)]
        [TestCase("    if-ge p0, v2, :cond_0", ESmaliInstruction.IfGreaterOrEqual)]
        [TestCase("    if-gt p2, p4, :cond_4", ESmaliInstruction.IfGreaterThan)]
        [TestCase("    if-lt v0, v1, :cond_3", ESmaliInstruction.IfLessThan)]
        [TestCase("    if-le v2, v3, :cond_4", ESmaliInstruction.IfLessOrEqual)]
        [TestCase("    if-ne v0, v1, :cond_1", ESmaliInstruction.IfNotEqual)]
        [TestCase("    if-eq p0, p1, :cond_12", ESmaliInstruction.IfEqual)]
        [TestCase("    if-gez v0, :cond_2", ESmaliInstruction.IfGreaterOrEqualToZero)]
        [TestCase("    if-gez p0, :cond_0", ESmaliInstruction.IfGreaterOrEqualToZero)]
        [TestCase("    if-gtz p2, :cond_4", ESmaliInstruction.IfGreaterThanToZero)]
        [TestCase("    if-ltz v0, :cond_3", ESmaliInstruction.IfLessThanToZero)]
        [TestCase("    if-lez v2, :cond_4", ESmaliInstruction.IfLessOrEqualToZero)]
        [TestCase("    if-nez v0, :cond_1", ESmaliInstruction.IfNotEqualToZero)]
        [TestCase("    if-eqz p0, :cond_12", ESmaliInstruction.IfEqualToZero)]
        [TestCase("    if-gez v0, :cond_2", ESmaliInstruction.IfGreaterOrEqualToZero)]
        [TestCase("    if-gez p0, :cond_0", ESmaliInstruction.IfGreaterOrEqualToZero)]
        [TestCase("    if-gtz p2, :cond_4", ESmaliInstruction.IfGreaterThanToZero)]
        [TestCase("    if-ltz v0, :cond_3", ESmaliInstruction.IfLessThanToZero)]
        [TestCase("    if-lez v2, :cond_4", ESmaliInstruction.IfLessOrEqualToZero)]
        [TestCase("    if-nez v0, :cond_1", ESmaliInstruction.IfNotEqualToZero)]
        [TestCase("    if-eqz p0, :cond_12", ESmaliInstruction.IfEqualToZero)]
        [TestCase("    aput p2, v0, v1", ESmaliInstruction.ArrayPut)]
        [TestCase("    aput-wide p2, v0, v1", ESmaliInstruction.ArrayPutWide)]
        [TestCase("    aput-object p2, v0, v1", ESmaliInstruction.ArrayPutObject)]
        [TestCase("    aput-boolean p2, v0, v1", ESmaliInstruction.ArrayPutBoolean)]
        [TestCase("    aput-byte p2, v0, v1", ESmaliInstruction.ArrayPutByte)]
        [TestCase("    aput-char p2, v0, v1", ESmaliInstruction.ArrayPutChar)]
        [TestCase("    aput-short p2, v0, v1", ESmaliInstruction.ArrayPutShort)]
        [TestCase("    aget p2, v0, v1", ESmaliInstruction.ArrayGet)]
        [TestCase("    aget-wide p2, v0, v1", ESmaliInstruction.ArrayGetWide)]
        [TestCase("    aget-object p2, v0, v1", ESmaliInstruction.ArrayGetObject)]
        [TestCase("    aget-boolean p2, v0, v1", ESmaliInstruction.ArrayGetBoolean)]
        [TestCase("    aget-byte p2, v0, v1", ESmaliInstruction.ArrayGetByte)]
        [TestCase("    aget-char p2, v0, v1", ESmaliInstruction.ArrayGetChar)]
        [TestCase("    aget-short p2, v0, v1", ESmaliInstruction.ArrayGetShort)]
        [TestCase("    iput p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstancePut)]
        [TestCase("    iput-wide p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstancePutWide)]
        [TestCase("    iput-object p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstancePutObject)]
        [TestCase("    iput-boolean p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstancePutBoolean)]
        [TestCase("    iput-byte p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstancePutByte)]
        [TestCase("    iput-char p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstancePutChar)]
        [TestCase("    iput-short p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstancePutShort)]
        [TestCase("    iget p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstanceGet)]
        [TestCase("    iget-wide p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstanceGetWide)]
        [TestCase("    iget-object p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstanceGetObject)]
        [TestCase("    iget-boolean p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstanceGetBoolean)]
        [TestCase("    iget-byte p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstanceGetByte)]
        [TestCase("    iget-char p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstanceGetChar)]
        [TestCase("    iget-short p2, v0, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.InstanceGetShort)]
        [TestCase("    sput p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticPut)]
        [TestCase("    sput-wide p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticPutWide)]
        [TestCase("    sput-object p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticPutObject)]
        [TestCase("    sput-boolean p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticPutBoolean)]
        [TestCase("    sput-byte p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticPutByte)]
        [TestCase("    sput-char p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticPutChar)]
        [TestCase("    sput-short p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticPutShort)]
        [TestCase("    sget p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticGet)]
        [TestCase("    sget-wide p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticGetWide)]
        [TestCase("    sget-object p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticGetObject)]
        [TestCase("    sget-boolean p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticGetBoolean)]
        [TestCase("    sget-byte p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticGetByte)]
        [TestCase("    sget-char p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticGetChar)]
        [TestCase("    sget-short p2, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileBooleanFileDexFile;->mElementConstructor:Ljava/lang/reflect/Constructor;", ESmaliInstruction.StaticGetShort)]
        [TestCase("    invoke-direct {v2}, Lamazon/android/dexload/compatibility/DexElementCompatibility$ElementObjectFileFileDexFile;-><init>()V", ESmaliInstruction.InvokeDirect)]
        [TestCase("    invoke-virtual {v0, p0}, Lamazon/android/dexload/compatibility/DexElementCompatibility;->findConstructor(Ljava/lang/Class;)V", ESmaliInstruction.InvokeVirtual)]
        [TestCase("    invoke-static {v4}, Ljava/lang/Boolean;->valueOf(Z)Ljava/lang/Boolean;", ESmaliInstruction.InvokeStatic)]
        [TestCase("    invoke-static {}, Landroid/os/Parcel;->obtain()Landroid/os/Parcel;", ESmaliInstruction.InvokeStatic)]
        [TestCase("    invoke-super {p0}, Landroid/graphics/drawable/Drawable;->getConstantState()Landroid/graphics/drawable/Drawable$ConstantState;", ESmaliInstruction.InvokeSuper)]
        [TestCase("    invoke-super {p0, p1}, Landroid/graphics/drawable/Drawable;->onBoundsChange(Landroid/graphics/Rect;)V", ESmaliInstruction.InvokeSuper)]
        [TestCase("    invoke-interface {v3}, Ljava/util/List;->iterator()Ljava/util/Iterator;", ESmaliInstruction.InvokeInterface)]
        [TestCase("    invoke-interface {v8, v10, v11, v12}, Landroid/content/SharedPreferences;->getLong(Ljava/lang/String;J)J", ESmaliInstruction.InvokeInterface)]
        [TestCase("    invoke-direct/range {v0 .. v5}, Lamazon/android/dexload/SupplementalDexLoader;->updatePreICSClassLoader(Landroid/content/Context;ZLjava/util/List;Lamazon/android/dexload/SupplementalDexLoader$DexLocation;Ljava/util/zip/ZipFile;)V", ESmaliInstruction.InvokeDirectRange)]
        [TestCase("    invoke-virtual/range {v18 .. v19}, Ljava/lang/reflect/Field;->get(Ljava/lang/Object;)Ljava/lang/Object;", ESmaliInstruction.InvokeVirtualRange)]
        [TestCase("    invoke-super/range {p0 .. p1}, Landroid/app/Activity;->onCreate(Landroid/os/Bundle;)V", ESmaliInstruction.InvokeSuperRange)]
        [TestCase("    invoke-static/range {v0 .. v8}, Lamazon/android/dexload/SupplementalDexLoader;->access$000(ILjava/lang/String;[Ljava/lang/String;[Ljava/io/File;[Ljava/util/zip/ZipFile;[Ldalvik/system/DexFile;Landroid/content/Context;Lamazon/android/dexload/SupplementalDexLoader$DexLocation;Ljava/util/zip/ZipFile;)V", ESmaliInstruction.InvokeStaticRange)]
        [TestCase("    invoke-interface/range {p3 .. p3}, Ljava/util/List;->size()I", ESmaliInstruction.InvokeInterfaceRange)]
        [TestCase("    neg-int p3, v7", ESmaliInstruction.NegInt)]
        [TestCase("    not-int p3, v7", ESmaliInstruction.NotInt)]
        [TestCase("    neg-long p3, v7", ESmaliInstruction.NegLong)]
        [TestCase("    not-long p3, v7", ESmaliInstruction.NotLong)]
        [TestCase("    neg-float p3, v7", ESmaliInstruction.NegFloat)]
        [TestCase("    neg-double p3, v7", ESmaliInstruction.NegDouble)]
        [TestCase("    int-to-long p3, v7", ESmaliInstruction.IntToLong)]
        [TestCase("    int-to-float p3, v7", ESmaliInstruction.IntToFloat)]
        [TestCase("    int-to-double p3, v7", ESmaliInstruction.IntToDouble)]
        [TestCase("    long-to-int p3, v7", ESmaliInstruction.LongToInt)]
        [TestCase("    long-to-float p3, v7", ESmaliInstruction.LongToFloat)]
        [TestCase("    long-to-double p3, v7", ESmaliInstruction.LongToDouble)]
        [TestCase("    float-to-int p3, v7", ESmaliInstruction.FloatToInt)]
        [TestCase("    float-to-long p3, v7", ESmaliInstruction.FloatToLong)]
        [TestCase("    float-to-double p3, v7", ESmaliInstruction.FloatToDouble)]
        [TestCase("    double-to-int p3, v7", ESmaliInstruction.DoubleToInt)]
        [TestCase("    double-to-long p3, v7", ESmaliInstruction.DoubleToLong)]
        [TestCase("    double-to-float p3, v7", ESmaliInstruction.DoubleToFloat)]
        [TestCase("    int-to-byte p3, v7", ESmaliInstruction.IntToByte)]
        [TestCase("    int-to-char p3, v7", ESmaliInstruction.IntToChar)]
        [TestCase("    int-to-short p3, v7", ESmaliInstruction.IntToShort)]
        [TestCase("    add-int p3, v7, p15", ESmaliInstruction.AddInt)]
        [TestCase("    sub-int p3, v7, p15", ESmaliInstruction.SubInt)]
        [TestCase("    mul-int p3, v7, p15", ESmaliInstruction.MulInt)]
        [TestCase("    div-int p3, v7, p15", ESmaliInstruction.DivInt)]
        [TestCase("    rem-int p3, v7, p15", ESmaliInstruction.RemInt)]
        [TestCase("    and-int p3, v7, p15", ESmaliInstruction.AndInt)]
        [TestCase("    or-int p3, v7, p15", ESmaliInstruction.OrInt)]
        [TestCase("    xor-int p3, v7, p15", ESmaliInstruction.XorInt)]
        [TestCase("    shl-int p3, v7, p15", ESmaliInstruction.ShlInt)]
        [TestCase("    shr-int p3, v7, p15", ESmaliInstruction.ShrInt)]
        [TestCase("    ushr-int p3, v7, p15", ESmaliInstruction.UshrInt)]
        [TestCase("    add-long p3, v7, p15", ESmaliInstruction.AddLong)]
        [TestCase("    sub-long p3, v7, p15", ESmaliInstruction.SubLong)]
        [TestCase("    mul-long p3, v7, p15", ESmaliInstruction.MulLong)]
        [TestCase("    div-long p3, v7, p15", ESmaliInstruction.DivLong)]
        [TestCase("    rem-long p3, v7, p15", ESmaliInstruction.RemLong)]
        [TestCase("    and-long p3, v7, p15", ESmaliInstruction.AndLong)]
        [TestCase("    or-long p3, v7, p15", ESmaliInstruction.OrLong)]
        [TestCase("    xor-long p3, v7, p15", ESmaliInstruction.XorLong)]
        [TestCase("    shl-long p3, v7, p15", ESmaliInstruction.ShlLong)]
        [TestCase("    shr-long p3, v7, p15", ESmaliInstruction.ShrLong)]
        [TestCase("    ushr-long p3, v7, p15", ESmaliInstruction.UshrLong)]
        [TestCase("    add-float p3, v7, p15", ESmaliInstruction.AddFloat)]
        [TestCase("    sub-float p3, v7, p15", ESmaliInstruction.SubFloat)]
        [TestCase("    mul-float p3, v7, p15", ESmaliInstruction.MulFloat)]
        [TestCase("    div-float p3, v7, p15", ESmaliInstruction.DivFloat)]
        [TestCase("    rem-float p3, v7, p15", ESmaliInstruction.RemFloat)]
        [TestCase("    add-double p3, v7, p15", ESmaliInstruction.AddDouble)]
        [TestCase("    sub-double p3, v7, p15", ESmaliInstruction.SubDouble)]
        [TestCase("    mul-double p3, v7, p15", ESmaliInstruction.MulDouble)]
        [TestCase("    div-double p3, v7, p15", ESmaliInstruction.DivDouble)]
        [TestCase("    rem-double p3, v7, p15", ESmaliInstruction.RemDouble)]
        [TestCase("    add-int/addr2 p3, v7", ESmaliInstruction.AddIntAddr2)]
        [TestCase("    sub-int/addr2 p3, v7", ESmaliInstruction.SubIntAddr2)]
        [TestCase("    mul-int/addr2 p3, v7", ESmaliInstruction.MulIntAddr2)]
        [TestCase("    div-int/addr2 p3, v7", ESmaliInstruction.DivIntAddr2)]
        [TestCase("    rem-int/addr2 p3, v7", ESmaliInstruction.RemIntAddr2)]
        [TestCase("    and-int/addr2 p3, v7", ESmaliInstruction.AndIntAddr2)]
        [TestCase("    or-int/addr2 p3, v7", ESmaliInstruction.OrIntAddr2)]
        [TestCase("    xor-int/addr2 p3, v7", ESmaliInstruction.XorIntAddr2)]
        [TestCase("    shl-int/addr2 p3, v7", ESmaliInstruction.ShlIntAddr2)]
        [TestCase("    shr-int/addr2 p3, v7", ESmaliInstruction.ShrIntAddr2)]
        [TestCase("    ushr-int/addr2 p3, v7", ESmaliInstruction.UshrIntAddr2)]
        [TestCase("    add-long/addr2 p3, v7", ESmaliInstruction.AddLongAddr2)]
        [TestCase("    sub-long/addr2 p3, v7", ESmaliInstruction.SubLongAddr2)]
        [TestCase("    mul-long/addr2 p3, v7", ESmaliInstruction.MulLongAddr2)]
        [TestCase("    div-long/addr2 p3, v7", ESmaliInstruction.DivLongAddr2)]
        [TestCase("    rem-long/addr2 p3, v7", ESmaliInstruction.RemLongAddr2)]
        [TestCase("    and-long/addr2 p3, v7", ESmaliInstruction.AndLongAddr2)]
        [TestCase("    or-long/addr2 p3, v7", ESmaliInstruction.OrLongAddr2)]
        [TestCase("    xor-long/addr2 p3, v7", ESmaliInstruction.XorLongAddr2)]
        [TestCase("    shl-long/addr2 p3, v7", ESmaliInstruction.ShlLongAddr2)]
        [TestCase("    shr-long/addr2 p3, v7", ESmaliInstruction.ShrLongAddr2)]
        [TestCase("    ushr-long/addr2 p3, v7", ESmaliInstruction.UshrLongAddr2)]
        [TestCase("    add-float/addr2 p3, v7", ESmaliInstruction.AddFloatAddr2)]
        [TestCase("    sub-float/addr2 p3, v7", ESmaliInstruction.SubFloatAddr2)]
        [TestCase("    mul-float/addr2 p3, v7", ESmaliInstruction.MulFloatAddr2)]
        [TestCase("    div-float/addr2 p3, v7", ESmaliInstruction.DivFloatAddr2)]
        [TestCase("    rem-float/addr2 p3, v7", ESmaliInstruction.RemFloatAddr2)]
        [TestCase("    add-double/addr2 p3, v7", ESmaliInstruction.AddDoubleAddr2)]
        [TestCase("    sub-double/addr2 p3, v7", ESmaliInstruction.SubDoubleAddr2)]
        [TestCase("    mul-double/addr2 p3, v7", ESmaliInstruction.MulDoubleAddr2)]
        [TestCase("    div-double/addr2 p3, v7", ESmaliInstruction.DivDoubleAddr2)]
        [TestCase("    rem-double/addr2 p3, v7", ESmaliInstruction.RemDoubleAddr2)]
        [TestCase("    add-int/lit16 p0, v1, 0x353", ESmaliInstruction.AddIntLit16)]
        [TestCase("    rsub-int/lit16 p0, v1, 0x353", ESmaliInstruction.RSubIntLit16)]
        [TestCase("    mul-int/lit16 p0, v1, 0x353", ESmaliInstruction.MulIntLit16)]
        [TestCase("    div-int/lit16 p0, v1, 0x353", ESmaliInstruction.DivIntLit16)]
        [TestCase("    rem-int/lit16 p0, v1, 0x353", ESmaliInstruction.RemIntLit16)]
        [TestCase("    and-int/lit16 p0, v1, 0x353", ESmaliInstruction.AndIntLit16)]
        [TestCase("    or-int/lit16 p0, v1, 0x353", ESmaliInstruction.OrIntLit16)]
        [TestCase("    xor-int/lit16 p0, v1, 0x353", ESmaliInstruction.XorIntLit16)]
        [TestCase("    add-int/lit8 p0, v1, -0x353", ESmaliInstruction.AddIntLit8)]
        [TestCase("    rsub-int/lit8 p0, v1, -0x353", ESmaliInstruction.RSubIntLit8)]
        [TestCase("    mul-int/lit8 p0, v1, -0x353", ESmaliInstruction.MulIntLit8)]
        [TestCase("    div-int/lit8 p0, v1, -0x353", ESmaliInstruction.DivIntLit8)]
        [TestCase("    rem-int/lit8 p0, v1, -0x353", ESmaliInstruction.RemIntLit8)]
        [TestCase("    and-int/lit8 p0, v1, -0x353", ESmaliInstruction.AndIntLit8)]
        [TestCase("    or-int/lit8 p0, v1, -0x353", ESmaliInstruction.OrIntLit8)]
        [TestCase("    xor-int/lit8 p0, v1, -0x353", ESmaliInstruction.XorIntLit8)]
        [TestCase("    :goto_0", ESmaliInstruction.LabelGoto)]
        [TestCase("    :try_start_0", ESmaliInstruction.LabelTryStart)]
        [TestCase("    :cond_0", ESmaliInstruction.LabelCond)]
        [TestCase("    :cond_4", ESmaliInstruction.LabelCond)]
        [TestCase("    :pswitch_0", ESmaliInstruction.LabelPSwitch)]
        [TestCase("    :pswitch_data_0", ESmaliInstruction.LabelPSwitchData)]
        [TestCase("    :try_end_0", ESmaliInstruction.LabelTryEnd)]
        [TestCase("    :catch_0", ESmaliInstruction.LabelCatch)]
        [TestCase("    :catchall_0", ESmaliInstruction.LabelCatchAll)]
        [TestCase("    :sswitch_0", ESmaliInstruction.LabelSSwitch)]
        [TestCase("    :sswitch_data_0", ESmaliInstruction.LabelSSwitchData)]
        [TestCase("    .catch Ljava/lang/SecurityException; {:try_start_0 .. :try_end_0} :catch_1", ESmaliInstruction.Catch)]
        [TestCase("    .catch Ljava/lang/NoSuchMethodException; {:try_start_0 .. :try_end_0} :catch_0", ESmaliInstruction.Catch)]
        [TestCase("    .catch Landroid/content/pm/PackageManager$NameNotFoundException; {:try_start_0 .. :try_end_0} :catch_0", ESmaliInstruction.Catch)]
        [TestCase("    .catch Ljava/lang/InterruptedException; {:try_start_0 .. :try_end_0} :catch_0", ESmaliInstruction.Catch)]
        [TestCase("    .catchall {:try_start_0 .. :try_end_0} :catchall_0", ESmaliInstruction.CatchAll)]
        public void GetInstruction_Test(string codeline, ESmaliInstruction opCode)
        {
            var instr = GetInstruction(codeline);
            Assert.IsNotNull(instr);
            Assert.AreEqual(instr.InstructionType, opCode);
        }

        #endregion

    }


}
