using System;
using System.Threading.Tasks;

using NUnit.Framework;

namespace GGUtils.MVVM.Async.Tests
{
    [TestFixture]
    public class AsyncDelegateCommandTests
    {
        private const string CommandResult = "Wololo";
        private static readonly Func<Task<string>> Command = () => Task.FromResult(CommandResult);
        private static readonly Func<Task<string>> UnfinishableCommand = () => Task.Run<string>(async () => { await Task.Delay(-1); return null; });

        [Test]
        public void ShouldThrow_ArgumentNullException_WhenCommandIsNull()
        {
            Func<Task<string>> command = null;

            Assert.Throws<ArgumentNullException>(() => new AsyncDelegateCommand<string>(command));
        }

        [Test]
        public void ShouldCall_CanExecuteFunction_WhenItIsNotNull()
        {
            bool called = false;
            Func<bool> canExecuteFunction = () => { called = true; return true; };

            var asyncCommand = new AsyncDelegateCommand<string>(Command, canExecuteFunction);
            asyncCommand.CanExecute(null);

            Assert.IsTrue(called);
        }

        [Test]
        public void ShouldReturn_TrueCanExecute_WhenNoCanExecuteFunctionIsProvided_And_TheCommandIsNotRunning() =>
            AssertCanExecuteWhenNoCanExecuteFunctionIsProvidedAndTaskIs(Command, true);

        [Test]
        public void ShouldReturn_FalseCanExecute_WhenNoCanExecuteFunctionWasProvided_And_TheCommandIsRunning() =>
            AssertCanExecuteWhenNoCanExecuteFunctionIsProvidedAndTaskIs(UnfinishableCommand, false);

        private static void AssertCanExecuteWhenNoCanExecuteFunctionIsProvidedAndTaskIs(Func<Task<string>> task, bool expectedResult)
        {
            var asyncCommand = new AsyncDelegateCommand<string>(task);

            asyncCommand.Execute(null);
            var canExecute = asyncCommand.CanExecute(null);

            Assert.AreEqual(expectedResult, canExecute);
        }

        [Test]
        public async Task ShouldRaise_PropertyChanged_WhenExecutionStarts_And_WhenItEnds()
        {
            int timesCalled = 0;
            var asyncCommand = new AsyncDelegateCommand<string>(Command);
            asyncCommand.PropertyChanged += (src, args) => timesCalled++;

            await asyncCommand.ExecuteAsync(null);

            Assert.AreEqual(2, timesCalled);
        }
    }
}
