﻿using System;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
{
    [TestFixture]
    public class MethodsInputSampleTests
    {
        private MethodsInputSample _instance;

        [SetUp]
        public void SetUp()
        {
            _instance = new MethodsInputSample();
        }

        [Test]
        public void TestMethodWithNonNullValue()
        {
            Action act = () => _instance.TestMethod("");

            act.ShouldNotThrow();
        }

        [Test]
        public void TestMethodWithNullValue()
        {
            Action act = () => _instance.TestMethod(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void TestMethodWithExplicitCanBeNull()
        {
            Action act = () => _instance.TestMethodWithExplicitCanBeNull(null);

            act.ShouldNotThrow();
        }

        [Test]
        public void TestMethodWithImplicitCanBeNull()
        {
            Action act = () => _instance.TestMethodWithImplicitCanBeNull(nullableInt: null, optional: null);

            act.ShouldNotThrow();
        }

        [Test]
        public void TestMethodWithRefParameter()
        {
            Action act = () =>
            {
                string refString = "";
                _instance.TestMethodWithRefParameter(ref refString);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void TestMethodWithRefParameterWithNullValue()
        {
            Action act = () =>
            {
                string refString = null;
                _instance.TestMethodWithRefParameter(ref refString /* REPORT? constant null value to NotNull parameter */);
            };

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("refString");
        }

        [Test]
        public void TestMethodWithExplicitNotNullRefParameter()
        {
            Action act = () =>
            {
                string refString = "s";
                _instance.TestMethodWithExplicitNotNullRefParameter(ref refString);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void TestMethodWithExplicitNotNullRefParameterWithNullValue()
        {
            Action act = () =>
            {
                string refString = null;
                _instance.TestMethodWithExplicitNotNullRefParameter(ref refString /* REPORT? constant null value to NotNull parameter */);
            };

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("refString");
        }

        [Test]
        public void TestMethodWithCanBeNullRefParameter()
        {
            Action act = () =>
            {
                string refString = null;
                _instance.TestMethodWithCanBeNullRefParameter(ref refString);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void TestMethodWithNullDefaultOfStringDefaultArgument()
        {
            Action act = () => _instance.TestMethodWithNullDefaultOfStringDefaultArgument(optional: null);

            act.ShouldNotThrow();
        }

        [Test]
        public void TestMethodWithNullDefaultArgumentFromConst()
        {
            Action act = () => _instance.TestMethodWithNullDefaultArgumentFromConst(optional: null);

            act.ShouldNotThrow();
        }

        [Test]
        public void TestMethodWithNullDefaultArgumentFromDefaultOfStringConst()
        {
            Action act = () => _instance.TestMethodWithNullDefaultArgumentFromDefaultOfStringConst(optional: null);

            act.ShouldNotThrow();
        }

        [Test]
        public void TestMethodWithNonNullDefaultArgument()
        {
            Action act = () => _instance.TestMethodWithNonNullDefaultArgument(optional: "overridden default");

            act.ShouldNotThrow();
        }

        [Test]
        public void TestMethodWithNonNullDefaultArgumentCalledWithNullValue()
        {
            Action act = () => _instance.TestMethodWithNonNullDefaultArgument(optional: null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("optional");
        }

        [Test]
        public void TestMethodWithNonNullDefaultArgumentFromConst()
        {
            Action act = () => _instance.TestMethodWithNonNullDefaultArgumentFromConst(optional: "overridden default");

            act.ShouldNotThrow();
        }

        [Test]
        public void TestMethodWithNonNullDefaultArgumentFromConstWithNullValue()
        {
            Action act = () => _instance.TestMethodWithNonNullDefaultArgumentFromConst(optional: null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("optional");
        }

        [Test]
        public void TestMethodWithParams()
        {
            Action act = () => _instance.TestMethodWithParams("1", "2", "3");

            act.ShouldNotThrow();
        }

        [Test]
        public void TestMethodWithParamsWithNullValue()
        {
            Action act = () => _instance.TestMethodWithParams(a: null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void StaticTestMethodWithNonNullValue()
        {
            Action act = () => MethodsInputSample.StaticTestMethod("");

            act.ShouldNotThrow();
        }

        [Test]
        public void StaticTestMethodWithNullValue()
        {
            Action act = () => MethodsInputSample.StaticTestMethod(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }
    }
}