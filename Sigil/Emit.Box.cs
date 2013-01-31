﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Boxes the given value type on the stack, converting it into a reference.
        /// </summary>
        public void Box<ValueType>()
        {
            Box(typeof(ValueType));
        }

        /// <summary>
        /// Boxes the given value type on the stack, converting it into a reference.
        /// </summary>
        public void Box(Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!valueType.IsValueType || valueType.IsByRef || valueType.IsPointer || valueType == typeof(void))
            {
                throw new ArgumentException("Only ValueTypes can be boxed, found " + valueType, "valueType");
            }

            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("Box expects a value on the stack, but found none", Stack);
            }

            var onStack = top.Single();

            var actually32bit = new [] { typeof(Boolean), typeof(Byte), typeof(SByte), typeof(Int16), typeof(UInt16), typeof(Int32), typeof(UInt32) };

            if (actually32bit.Contains(valueType))
            {
                if (onStack != TypeOnStack.Get<int>())
                {
                    throw new SigilException(onStack + " cannot be boxed as an " + valueType, Stack);
                }
            }
            else
            {
                if (onStack != TypeOnStack.Get(valueType))
                {
                    throw new SigilException("Expected " + valueType + " to be on the stack, found " + onStack, Stack);
                }
            }

            UpdateState(OpCodes.Box, valueType, TypeOnStack.Get(typeof(object)), pop: 1);
        }
    }
}
