using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsnowFramework
{
    public class LockedValue<T> 
    {
        private T _objectValue;
        private readonly object objectLock = new object();

        public LockedValue(T initialValue)
        {
            Value = initialValue;
        }

        public T Value
        {
            get
            {
                T result;
                lock (objectLock)
                {
                    result = _objectValue;
                }
                return result;
            }
            set
            {
                lock (objectLock)
                {
                    _objectValue = value;
                }
            }
        }

        public static implicit operator LockedValue<T>(T value)
        {
            return new LockedValue<T>(value);
        }


    }
}
