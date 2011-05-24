﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ProtoBuf;

namespace Examples
{
    [TestFixture]
    public class ShadowSetters
    {
        [ProtoContract]
        public class TypeWithShadowSetter
        {
            private int foo, bar, blap;
            [ProtoMember(1)]
            public int NoSetter { get { return foo; } }
            public void SetNoSetter(int value) { this.foo = value; }

            [ProtoMember(2)]
            public int SetterWeDontWantToInvoke { get { return bar; } set { throw new InvalidOperationException("oops, shadow should have won"); } }
            public void SetSetterWeDontWantToInvoke(int value) { this.foo = value; }

            [ProtoMember(3)]
            public int SetterWithUnrelatedSet { get { return foo; } set { blap = value; } }
            public void SetSetterWithUnrelatedSet(string value) { throw new InvalidOperationException("shouldn't have called this"); }
        }
        [Test]
        public void RoundTripWithShadow()
        {
            var orig = new TypeWithShadowSetter();
            orig.SetNoSetter(123);
            orig.SetSetterWeDontWantToInvoke(456);
            orig.SetterWithUnrelatedSet = 789;
            var clone = Serializer.DeepClone(orig);
            Assert.AreEqual(orig.NoSetter, clone.NoSetter);
            Assert.AreEqual(orig.SetterWeDontWantToInvoke, clone.SetterWeDontWantToInvoke);
            Assert.AreEqual(orig.SetterWithUnrelatedSet, clone.SetterWithUnrelatedSet);
        }
    }
}