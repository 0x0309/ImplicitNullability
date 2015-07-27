using System;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
{
    [TestFixture]
    public class DelegatesSampleTests
    {
        [Test]
        public void SomeAction()
        {
            Action act = () => DelegatesSample.GetSomeAction()(null);

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void SomeActionWithClosedValues()
        {
            Action act = () => DelegatesSample.GetSomeActionWithClosedValues()(null);

            act.ShouldNotThrow("because the delegate *method*  is an anonymous method (implemented using a display class)");
        }

        [Test]
        public void GetSomeActionToAnonymousMethod()
        {
            Action act = () => DelegatesSample.GetSomeActionToAnonymousMethod()(null);

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void GetSomeDelegateToLambda()
        {
            Action act = () => DelegatesSample.GetSomeDelegate()(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void SomeDelegateToNamedMethod()
        {
            Action act = () => DelegatesSample.GetSomeDelegateToNamedMethod()(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>("because the delegate *method* parameter is implicitly NotNull")
                .And.ParamName.Should().Be("a");
        }

        [Test]
        public void SomeDelegateToNamedMethodWithCanBeNull()
        {
            Action act = () => DelegatesSample.GetSomeDelegateToNamedMethodWithCanBeNull()(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldNotThrow();
        }

        [Test]
        public void SomeDelegateWithCanBeNullToNamedMethod()
        {
            Action act = () => DelegatesSample.GetSomeDelegateWithCanBeNullToNamedMethod()(null);

            act.ShouldThrow<ArgumentNullException>("because the delegate *method* parameter is (implicitly) NotNull")
                .And.ParamName.Should().Be("a");
        }

        [Test]
        public void SomeDelegateWithCanBeNullToNamedMethodWithCanBeNull()
        {
            Action act = () => DelegatesSample.GetSomeDelegateWithCanBeNullToNamedMethodWithCanBeNull()(null);

            act.ShouldNotThrow();
        }

        [Test]
        public void SomeFunctionDelegateWithNotNull()
        {
            Action act = () =>
            {
                var result = DelegatesSample.GetSomeFunctionDelegateWithNotNull()();
                // REPORT? false negative, although input parameters of delegates work (see above) and the [NotNull] is used in the lambda:
                ReSharper.TestValueAnalysis(result, result == null);
            };

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void SomeFunctionDelegate()
        {
            Action act = () =>
            {
                var result = DelegatesSample.GetSomeFunctionDelegate()();
                // This false negative is analog to SomeFunctionDelegateWithNotNull and even requires an exemption for the delegate Invoke() method,
                // but it is necessary because this implicit annotation wouldn't be invertible with [CanBeNull]:
                ReSharper.TestValueAnalysis(result, result == null);
            };

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void SomeFunctionDelegateWithCanBeNull()
        {
            Action act = () =>
            {
                var result = DelegatesSample.GetSomeFunctionDelegateWithCanBeNull()();
                // REPORT? false negative, equivalent to case in SomeFunctionDelegateWithNotNull
                ReSharper.TestValueAnalysis(result, result == null);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void SomeNotNullDelegateOfExternalCode()
        {
            Action act = () => DelegatesSample.GetSomeNotNullDelegateOfExternalCode()(null /*Expect:AssignNullToNotNullAttribute*/);

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void SomeDelegateOfExternalCode()
        {
            Action act = () => DelegatesSample.GetSomeDelegateOfExternalCode()(null /* no warning because the delegate is external */);

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void SomeDelegateWithRefAndOutWithNullValueForRefArgument()
        {
            Action act = () =>
            {
                string refString = null;
                string outString;

                DelegatesSample.GetSomeDelegateWithRefAndOut()(ref refString, out outString);

                ReSharper.TestValueAnalysis(refString, refString == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
                ReSharper.TestValueAnalysis(outString, outString == null /*Expect:ConditionIsAlwaysTrueOrFalse[MOut]*/);
            };

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }
    }
}