using System;
using System.Globalization;
using APKInsight.Logic.ContentParsing.SmaliParser;
using NUnit.Framework;

namespace BinSightTest
{
    [TestFixture]
    public class SmaliParserRegExTest
    {

        #region Line type validators tests

        [TestCase(".class Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskICS;", true)]
        [TestCase(".class Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".classLamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskICS;", false)]
        [TestCase(".super Ljava/lang/Object;", false)]
        public void IsClassNameLine(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsClassNameLine(line), expectedResult);
        }

        [TestCase("    invoke-direct {v0, v1}, Lcom/amazon/dp/logger/DPLogger;-><init>(Ljava/lang/String;)V", true)]
        [TestCase("    invoke-direct {v4, p0}, Lcom/amazon/device/crashmanager/CrashDescriptorStorageUtil$DescriptorCount;-><init>(Lcom/amazon/device/crashmanager/CrashDescriptorStorageUtil;)V", true)]
        [TestCase("    invoke-direct {v4, p0, v0, v1, v2}, Lcom/amazon/device/crashmanager/CrashDescriptorStorageUtil$DescriptorCount;-><init>(Lcom/amazon/device/crashmanager/CrashDescriptorStorageUtil;IJ)V", true)]
        [TestCase("    invoke-interface {v9, v10, v11}, Landroid/content/SharedPreferences;->getString(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;", true)]
        [TestCase("    invoke-virtual {p4}, Ljava/lang/String;->trim()Ljava/lang/String;", true)]
        [TestCase("    invoke-virtual {v2, v3}, Ljava/lang/StringBuilder;->append(Ljava/lang/String;)Ljava/lang/StringBuilder;", true)]
        [TestCase("    invoke-virtual {v9, v10, v11, v12}, Lcom/amazon/dp/logger/DPLogger;->info(Ljava/lang/String;Ljava/lang/String;[Ljava/lang/Object;)V", true)]
        [TestCase("    invoke-static {}, Landroid/os/Environment;->getExternalStorageDirectory()Ljava/io/File;", true)]
        [TestCase("    invoke-static {v14}, Ljava/lang/Integer;->valueOf(I)Ljava/lang/Integer;", true)]
        [TestCase("    invoke-super {p0, p1}, Ljava/lang/Object;->equals(Ljava/lang/Object;)Z", true)]
        [TestCase("    invoke-super {p0}, Ljava/lang/Object;->hashCode()I", true)]
        [TestCase("    invoke-static/range {v0 .. v6}, Lamazon/android/dexload/SupplementalDexLoader;->access$100(Landroid/content/Context;Lamazon/android/dexload/compatibility/DexElementCompatibility;Ljava/lang/Object;Ljava/lang/String;ILamazon/android/dexload/SupplementalDexLoader$DexLocation;Ljava/util/zip/ZipFile;)V", true)]
        [TestCase("    .class Ldagger/ObjectGraph$DaggerObjectGraph;", false)]
        [TestCase("    .super Ldagger/ObjectGraph;", false)]
        [TestCase("    .source \"ObjectGraph.java\"", false)]
        [TestCase("    # annotations", false)]
        [TestCase("    .annotation system Ldalvik/annotation/EnclosingClass;", false)]
        [TestCase("        value = Ldagger/ObjectGraph;", false)]
        [TestCase("    .end annotation", false)]
        [TestCase("    .field private final base:Ldagger/ObjectGraph$DaggerObjectGraph;", false)]
        [TestCase("    .field private final base:Ldagger/ObjectGraph$DaggerObjectGraph;", false)]
        [TestCase("    .locals 1", false)]
        [TestCase(@"    .param p1, ""base""    # Ldagger/ObjectGraph$DaggerObjectGraph;", false)]
        [TestCase("    iput-object p1, p0, Ldagger/ObjectGraph$DaggerObjectGraph;->base:Ldagger/ObjectGraph$DaggerObjectGraph;", false)]
        [TestCase("    const-string v0, \"linker\"", false)]
        [TestCase("    move-result-object v0", false)]
        [TestCase("    check-cast v0, Ldagger/internal/Linker;", false)]
        [TestCase("    new-instance v0, Ljava/lang/NullPointerException;", false)]
        [TestCase("    throw v0", false)]
        [TestCase("    return-object p0", false)]
        public void IsInvokeLine(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsInvokeLine(line), expectedResult);
        }

        [TestCase("    iget-object v4, v0, Lorg/apache/commons/lang3/Range;->minimum:Ljava/lang/Object;", true)]
        [TestCase("    sget-object v4, v0, Lorg/apache/commons/lang3/Range;->minimum:Ljava/lang/Object;", true)]
        public void IsGetLine(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsGetLine(line), expectedResult);
        }

        [TestCase("    iput-object v1, p0, Lorg/apache/commons/lang3/Range;->toString:Ljava/lang/String;", true)]
        [TestCase("    sput-object v0, Lorg/apache/commons/lang3/ArrayUtils;->EMPTY_OBJECT_ARRAY:[Ljava/lang/Object;", true)]
        public void IsPutLine(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsPutLine(line), expectedResult);
        }

        [TestCase(@".source ""SupplementalDexLoader.java""", true)]
        [TestCase(".class Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", false)]
        public void IsSourceFileNameLine(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsSourceFileNameLine(line), expectedResult);
        }

        [TestCase(".super Ljava/lang/Object;", true)]
        [TestCase(".class Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", false)]
        public void IsSuperClassNameLine(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsSuperClassNameLine(line), expectedResult);
        }

        [TestCase(".implements Ljava/lang/Object;", true)]
        [TestCase(".class Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", false)]
        public void IsImplementsInterfaceNameLine(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsImplementsInterfaceNameLine(line), expectedResult);
        }

        [TestCase("Ljava/lang/Object;", false)]
        [TestCase("Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        public void IsInOuterClass(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsInOuterClass(line), expectedResult);
        }

        [TestCase(".field private final eventBus:Lcom/audible/hushpuppy/framework/EventBus;", true)]
        [TestCase(".field protected final globalSyncEventHandler:Lcom/audible/hushpuppy/ApplicationPlugin$GlobalSyncEventHandler;", true)]
        [TestCase(".field private producingState:Z", true)]
        [TestCase(@".field public static final PLUGIN_NAME:Ljava/lang/String; = ""com.audible.hushpuppy.ApplicationPlugin""", true)]
        public void IsFieldLine(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsFieldLine(line), expectedResult);
        }

        [TestCase(".method static toCharArray(Ljava/lang/CharSequence;)[C", true)]
        [TestCase(".method public static subSequence(Ljava/lang/CharSequence;I)Ljava/lang/CharSequence;", true)]
        [TestCase(".method static toCharArray(Ljava/lang/CharSequence;)[C - Used in 5 cases", true)]
        [TestCase(".field private producingState:Z", false)]
        public void IsMethodStartLine(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsMethodStartLine(line), expectedResult);
        }

        [TestCase(".method static toCharArray(Ljava/lang/CharSequence;)[C", false)]
        [TestCase(".end method", true)]
        [TestCase(".end method\r", true)]
        [TestCase(".end method\n", true)]
        public void IsMethodEndLine(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsMethodEndLine(line), expectedResult);
        }

        #endregion


        #region Data extractors tests

        [TestCase(".class Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskICS;", "Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskICS;")]
        [TestCase(".class Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskICS;\r", "Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskICS;")]
        [TestCase(".class Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskICS;\n", "Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskICS;")]
        [TestCase(".class Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskICS;\r\n", "Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskICS;")]
        [TestCase(".class Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", "Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;")]
        [TestCase(".class public Lamazon/android/dexload/SupplementalDexLoader;", "Lamazon/android/dexload/SupplementalDexLoader;")]
        [TestCase(".class Lasdf;", "Lasdf;")]
        [TestCase(".super Ljava/lang/Object;", "Ljava/lang/Object;")]
        public void GetClassName(string line, string expectedName)
        {
            Assert.AreEqual(SmaliParser.GetClassName(line), expectedName);
        }

        [TestCase("    invoke-direct {v0, v1}, Lcom/amazon/dp/logger/DPLogger;-><init>(Ljava/lang/String;)V", "Lcom/amazon/dp/logger/DPLogger;-><init>(Ljava/lang/String;)V")]
        [TestCase("    invoke-direct {v4, p0}, Lcom/amazon/device/crashmanager/CrashDescriptorStorageUtil$DescriptorCount;-><init>(Lcom/amazon/device/crashmanager/CrashDescriptorStorageUtil;)V",
            "Lcom/amazon/device/crashmanager/CrashDescriptorStorageUtil$DescriptorCount;-><init>(Lcom/amazon/device/crashmanager/CrashDescriptorStorageUtil;)V")]
        [TestCase("    invoke-direct {v4, p0, v0, v1, v2}, Lcom/amazon/device/crashmanager/CrashDescriptorStorageUtil$DescriptorCount;-><init>(Lcom/amazon/device/crashmanager/CrashDescriptorStorageUtil;IJ)V",
            "Lcom/amazon/device/crashmanager/CrashDescriptorStorageUtil$DescriptorCount;-><init>(Lcom/amazon/device/crashmanager/CrashDescriptorStorageUtil;IJ)V")]
        [TestCase(
            "    invoke-interface {v9, v10, v11}, Landroid/content/SharedPreferences;->getString(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;",
            "Landroid/content/SharedPreferences;->getString(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;"
            )]
        [TestCase("    invoke-virtual {p4}, Ljava/lang/String;->trim()Ljava/lang/String;",
            "Ljava/lang/String;->trim()Ljava/lang/String;")]
        [TestCase(
            "    invoke-virtual {v2, v3}, Ljava/lang/StringBuilder;->append(Ljava/lang/String;)Ljava/lang/StringBuilder;",
            "Ljava/lang/StringBuilder;->append(Ljava/lang/String;)Ljava/lang/StringBuilder;"
            )]
        [TestCase(
            "    invoke-virtual {v9, v10, v11, v12}, Lcom/amazon/dp/logger/DPLogger;->info(Ljava/lang/String;Ljava/lang/String;[Ljava/lang/Object;)V",
            "Lcom/amazon/dp/logger/DPLogger;->info(Ljava/lang/String;Ljava/lang/String;[Ljava/lang/Object;)V"
            )]
        [TestCase("    invoke-static {}, Landroid/os/Environment;->getExternalStorageDirectory()Ljava/io/File;",
            "Landroid/os/Environment;->getExternalStorageDirectory()Ljava/io/File;")]
        [TestCase("    invoke-static {v14}, Ljava/lang/Integer;->valueOf(I)Ljava/lang/Integer;",
            "Ljava/lang/Integer;->valueOf(I)Ljava/lang/Integer;")]
        [TestCase("    invoke-super {p0, p1}, Ljava/lang/Object;->equals(Ljava/lang/Object;)Z",
            "Ljava/lang/Object;->equals(Ljava/lang/Object;)Z")]
        [TestCase("    invoke-super {p0}, Ljava/lang/Object;->hashCode()I",
            "Ljava/lang/Object;->hashCode()I")]
        [TestCase("    invoke-super {p0}, Ljava/lang/Object;->hashCode()I\r",
            "Ljava/lang/Object;->hashCode()I")]
        [TestCase("    invoke-static/range {v0 .. v6}, Lamazon/android/dexload/SupplementalDexLoader;->access$100(Landroid/content/Context;Lamazon/android/dexload/compatibility/DexElementCompatibility;Ljava/lang/Object;Ljava/lang/String;ILamazon/android/dexload/SupplementalDexLoader$DexLocation;Ljava/util/zip/ZipFile;)V",
            "Lamazon/android/dexload/SupplementalDexLoader;->access$100(Landroid/content/Context;Lamazon/android/dexload/compatibility/DexElementCompatibility;Ljava/lang/Object;Ljava/lang/String;ILamazon/android/dexload/SupplementalDexLoader$DexLocation;Ljava/util/zip/ZipFile;)V")]
        public void GetInvokedMethodFullName(string line, string expectedName)
        {
            Assert.AreEqual(SmaliParser.GetInvokedMethodFullName(line), expectedName);
        }

        [TestCase("    invoke-super {p0}, Ljava/lang/Object;->hashCode()I",
    "Ljava/lang/Object;")]
        [TestCase("    invoke-super {p0}, Ljava/lang/Object;->hashCode()I\r",
    "Ljava/lang/Object;")]
        [TestCase("    invoke-static/range {v0 .. v6}, Lamazon/android/dexload/SupplementalDexLoader;->access$100(Landroid/content/Context;Lamazon/android/dexload/compatibility/DexElementCompatibility;Ljava/lang/Object;Ljava/lang/String;ILamazon/android/dexload/SupplementalDexLoader$DexLocation;Ljava/util/zip/ZipFile;)V",
    "Lamazon/android/dexload/SupplementalDexLoader;")]
        public void GetInvokedMethodTypeInFullName(string line, string expectedName)
        {
            Assert.AreEqual(SmaliParser.GetInvokedMethodTypeInFullName(line), expectedName);
        }

        [TestCase(@".source ""SupplementalDexLoader.java""", "SupplementalDexLoader.java")]
        [TestCase(@".source ""SupplementalDexLoader.java""\r", "SupplementalDexLoader.java")]
        [TestCase(@".source ""фывафыва.java""", "фывафыва.java")]
        public void GetFileName(string line, string expectedResult)
        {
            Assert.IsTrue(string.Compare(SmaliParser.GetFileName(line), expectedResult, StringComparison.Ordinal) == 0);
        }

        [TestCase("Lamazon/android/dexload/SupplementalDexLoader$asdf$SingleDexFileLoadTaskICS;", 2)]
        [TestCase("Lamazon/android/dexload/SupplementalDexLoader$$SingleDexFileLoadTaskPreICS;", 1)]
        [TestCase("Lamazon/android/dexload/SupplementalDexLoader;", 0)]
        public void GetAllOuterClassNames(string line, int classesNo)
        {
            Assert.AreEqual(SmaliParser.GetAllOuterClassNames(line).Count, classesNo);
        }

        [TestCase(".field private final eventBus:Lcom/audible/hushpuppy/framework/EventBus;", "eventBus")]
        [TestCase(".field private kindleReaderSdk:Lcom/amazon/kindle/krx/IKindleReaderSDK;", "kindleReaderSdk")]
        [TestCase(".field private producingState:Z", "producingState")]
        [TestCase(@".field public static final PLUGIN_NAME:Ljava/lang/String; = ""com.audible.hushpuppy.ApplicationPlugin""", "PLUGIN_NAME")]
        public void GetFieldName(string line, string expectedName)
        {
            Assert.AreEqual(SmaliParser.GetFieldName(line), expectedName);
        }

        [TestCase(".field private final eventBus:Lcom/audible/hushpuppy/framework/EventBus;", "Lcom/audible/hushpuppy/ApplicationPlugin;->eventBus:Lcom/audible/hushpuppy/framework/EventBus;")]
        [TestCase(".field private kindleReaderSdk:Lcom/amazon/kindle/krx/IKindleReaderSDK;", "Lcom/audible/hushpuppy/ApplicationPlugin;->kindleReaderSdk:Lcom/amazon/kindle/krx/IKindleReaderSDK;")]
        [TestCase(".field private producingState:Z", "Lcom/audible/hushpuppy/ApplicationPlugin;->producingState:Z")]
        [TestCase(".field private producingState:Z\r", "Lcom/audible/hushpuppy/ApplicationPlugin;->producingState:Z")]
        [TestCase(@".field public static final PLUGIN_NAME:Ljava/lang/String; = ""com.audible.hushpuppy.ApplicationPlugin""", "Lcom/audible/hushpuppy/ApplicationPlugin;->PLUGIN_NAME:Ljava/lang/String;")]
        public void GetFullFieldName(string line, string expectedName)
        {
            Assert.AreEqual(SmaliParser.GetFullFieldName(line, "Lcom/audible/hushpuppy/ApplicationPlugin;"), expectedName);
        }

        [TestCase(".field private final eventBus:Lcom/audible/hushpuppy/framework/EventBus;", "Lcom/audible/hushpuppy/framework/EventBus;")]
        [TestCase(".field private kindleReaderSdk:[Lcom/amazon/kindle/krx/IKindleReaderSDK;\r", "[Lcom/amazon/kindle/krx/IKindleReaderSDK;")]
        [TestCase(".field private producingState:Z", "Z")]
        [TestCase(".field private producingState:Z\n", "Z")]
        [TestCase(@".field public static final PLUGIN_NAME:Ljava/lang/String; = ""com.audible.hushpuppy.ApplicationPlugin""", "Ljava/lang/String;")]
        public void GetFieldTypeName(string line, string expectedName)
        {
            Assert.AreEqual(SmaliParser.GetFieldTypeName(line), expectedName);
        }

        [TestCase(".field private final eventBus:Lcom/audible/hushpuppy/framework/EventBus;\r", "")]
        [TestCase(".field private kindleReaderSdk:[Lcom/amazon/kindle/krx/IKindleReaderSDK;", "")]
        [TestCase(".field private producingState:Z", "")]
        [TestCase(@".field public static final PLUGIN_NAME:Ljava/lang/String; = ""com.audible.hushpuppy.ApplicationPlugin""", @"""com.audible.hushpuppy.ApplicationPlugin""")]
        [TestCase(@".field public static final PLUGIN_NAME:Ljava/lang/String; = ""com.audible.hushpuppy.ApplicationPlugin"""+"\r", @"""com.audible.hushpuppy.ApplicationPlugin""")]
        [TestCase(".field private producingState:I = 1234", "1234")]
        [TestCase(".field private producingState:I = 1234\r", "1234")]
        [TestCase(".field private producingState:I = 0x1234", "0x1234")]
        public void GetFieldInitValue(string line, string expectedName)
        {
            Assert.AreEqual(SmaliParser.GetFieldInitValue(line), expectedName);
        }

        [TestCase(".method static toCharArray(Ljava/lang/CharSequence;)[C", "toCharArray(Ljava/lang/CharSequence;)[C")]
        [TestCase(".method static toCharArray(Ljava/lang/CharSequence;)[C - Used in 5 cases", "toCharArray(Ljava/lang/CharSequence;)[C")]
        [TestCase(".method static toCharArray(Ljava/lang/CharSequence;)[C\r", "toCharArray(Ljava/lang/CharSequence;)[C")]
        [TestCase(".method static toCharArray(Ljava/lang/CharSequence;)[C\r - Used in 5 cases", "toCharArray(Ljava/lang/CharSequence;)[C")]
        [TestCase(".method public static subSequence(Ljava/lang/CharSequence;I)Ljava/lang/CharSequence;", "subSequence(Ljava/lang/CharSequence;I)Ljava/lang/CharSequence;")]
        public void GetMethodName(string line, string expectedResult)
        {
            Assert.AreEqual(SmaliParser.GetMethodName(line), expectedResult);
        }

        [TestCase(".method static toCharArray(Ljava/lang/CharSequence;)[C", ".method static toCharArray(Ljava/lang/CharSequence;)[C")]
        [TestCase(".method static toCharArray(Ljava/lang/CharSequence;)[C - Used in 5 cases", ".method static toCharArray(Ljava/lang/CharSequence;)[C")]
        [TestCase(".method static toCharArray(Ljava/lang/CharSequence;)[C\r", ".method static toCharArray(Ljava/lang/CharSequence;)[C")]
        [TestCase(".method static toCharArray(Ljava/lang/CharSequence;)[C\r - Used in 5 cases", ".method static toCharArray(Ljava/lang/CharSequence;)[C")]
        [TestCase(".method public static subSequence(Ljava/lang/CharSequence;I)Ljava/lang/CharSequence;", ".method public static subSequence(Ljava/lang/CharSequence;I)Ljava/lang/CharSequence;")]
        public void GetMethodNameEndIndex(string line, string expectedResult)
        {
            Assert.AreEqual(SmaliParser.GetMethodNameEndIndex(line), expectedResult.Length);
        }

        [TestCase(".method static toCharArray(Ljava/lang/CharSequence;)[C", "toCharArray")]
        [TestCase(".method static toCharArray(Ljava/lang/CharSequence;)[C\r", "toCharArray")]
        [TestCase(".method public static subSequence(Ljava/lang/CharSequence;I)Ljava/lang/CharSequence;", "subSequence")]
        public void GetShortMethodName(string line, string expectedResult)
        {
            Assert.AreEqual(SmaliParser.GetShortMethodName(line), expectedResult);
        }

        [TestCase(".method static toCharArray(Ljava/lang/CharSequence;)[C", "[C")]
        [TestCase(".method static toCharArray(Ljava/lang/CharSequence;)[C\r", "[C")]
        [TestCase(".method public static subSequence(Ljava/lang/CharSequence;I)Ljava/lang/CharSequence;", "Ljava/lang/CharSequence;")]
        public void GetReturnTypeName(string line, string expectedResult)
        {
            Assert.AreEqual(SmaliParser.GetReturnTypeName(line), expectedResult);
        }

        [TestCase(".method public static subSequence(ZIBI)Ljava/lang/CharSequence;", 4)]
        [TestCase(".method public static subSequence(ZIBI)Ljava/lang/CharSequence;\r", 4)]
        [TestCase(".method public static subSequence([Z[[I[[[B[IZ)Ljava/lang/CharSequence;", 5)]
        [TestCase(".method static toCharArray([Z[[ILjava/lang/CharSequence;)[C", 3)]
        [TestCase(".method public static subSequence([ZI[B[Ljava/lang/CharSequence;I)Ljava/lang/CharSequence;", 5)]
        public void GetInputParametersTypes(string line, int expectedResult)
        {
            Assert.AreEqual(SmaliParser.GetInputParametersTypes(line).Count, expectedResult);
        }

        [TestCase("    iget-object v4, v0, Lorg/apache/commons/lang3/Range;->minimum:Ljava/lang/Object;\r", "Lorg/apache/commons/lang3/Range;->minimum:Ljava/lang/Object;")]
        [TestCase("    iput-object v1, p0, Lorg/apache/commons/lang3/Range;->toString:Ljava/lang/String;\n", "Lorg/apache/commons/lang3/Range;->toString:Ljava/lang/String;")]
        [TestCase("    sput-object v0, Lorg/apache/commons/lang3/ArrayUtils;->EMPTY_OBJECT_ARRAY:[Ljava/lang/Object;\r\n", "Lorg/apache/commons/lang3/ArrayUtils;->EMPTY_OBJECT_ARRAY:[Ljava/lang/Object;")]
        [TestCase("    sget-object v4, v0, Lorg/apache/commons/lang3/Range;->minimum:Ljava/lang/Object;", "Lorg/apache/commons/lang3/Range;->minimum:Ljava/lang/Object;")]
        public void GetAccessedMemberName(string line, string expectedResult)
        {
            Assert.AreEqual(SmaliParser.GetAccessedMemberName(line), expectedResult);
        }

        [TestCase("    iget-object v4, v0, Lorg/apache/commons/lang3/Range;->minimum:Ljava/lang/Object;\r", "Lorg/apache/commons/lang3/Range;")]
        [TestCase("    iput-object v1, p0, Lorg/apache/commons/lang3/Range;->toString:Ljava/lang/String;\n", "Lorg/apache/commons/lang3/Range;")]
        [TestCase("    sput-object v0, Lorg/apache/commons/lang3/ArrayUtils;->EMPTY_OBJECT_ARRAY:[Ljava/lang/Object;\r\n", "Lorg/apache/commons/lang3/ArrayUtils;")]
        [TestCase("    sget-object v4, v0, Lorg/apache/commons/lang3/Range;->minimum:Ljava/lang/Object;", "Lorg/apache/commons/lang3/Range;")]
        public void GetAccessedMemberTypeInName(string line, string expectedResult)
        {
            Assert.AreEqual(SmaliParser.GetAccessedMemberTypeInName(line), expectedResult);
        }

        #endregion


        #region Option checkers Tests

        [TestCase(".class abstract Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public abstract enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", false)]
        public void IsAbstractOptionPresent(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsAbstractOptionPresent(line), expectedResult);
        }

        [TestCase(".class interface Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public interface enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", false)]
        public void IsInterfaceOptionPresent(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsInterfaceOptionPresent(line), expectedResult);
        }

        [TestCase(".class enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public enum interface Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public interface Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", false)]
        public void IsEnumOptionPresent(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsEnumOptionPresent(line), expectedResult);
        }

        [TestCase(".class final Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public final enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", false)]
        public void IsFinalOptionPresent(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsFinalOptionPresent(line), expectedResult);
        }

        [TestCase(".class annotation Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public annotation enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", false)]
        public void IsAnnotationOptionPresent(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsAnnotationOptionPresent(line), expectedResult);
        }

        [TestCase(".class static Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public static enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", false)]
        public void IsStaticOptionPresent(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsStaticOptionPresent(line), expectedResult);
        }

        [TestCase(".class public Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public static enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class private enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", false)]
        public void IsPublicOptionPresent(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsPublicOptionPresent(line), expectedResult);
        }

        [TestCase(".class private Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class private static enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", false)]
        public void IsPrivateOptionPresent(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsPrivateOptionPresent(line), expectedResult);
        }

        [TestCase(".class protected Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class protected static enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", false)]
        public void IsProtectedOptionPresent(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsProtectedOptionPresent(line), expectedResult);
        }

        [TestCase(".class protected volatile Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class protected volatile enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", false)]
        public void IsVolatileOptionPresent(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsVolatileOptionPresent(line), expectedResult);
        }

        [TestCase(".class protected synthetic Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class protected synthetic enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", false)]
        public void IsSyntheticOptionPresent(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsSyntheticOptionPresent(line), expectedResult);
        }

        [TestCase(".class protected constructor synthetic Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class protected constructor synthetic enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", true)]
        [TestCase(".class public enum Lamazon/android/dexload/SupplementalDexLoader$SingleDexFileLoadTaskPreICS;", false)]
        public void IsConstructorOptionPresent(string line, bool expectedResult)
        {
            Assert.AreEqual(SmaliParser.IsConstructorOptionPresent(line), expectedResult);
        }

        #endregion

    }
}