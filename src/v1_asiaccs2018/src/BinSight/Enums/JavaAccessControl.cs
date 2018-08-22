using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APKInsight.Enums
{
    /// <summary>
    /// This is a set of access control Java supports
    /// From here: http://docs.oracle.com/javase/tutorial/java/javaOO/accesscontrol.html
    /// </summary>
    public enum JavaAccessControl
    {
        Undefined = 0, // This is not the same as no modifier, it means the object is bogus
        Public = 1,
        Private = 2,
        Protected = 3,
        PackagePrivate = 4 // i.e., no modifier

    }
}
