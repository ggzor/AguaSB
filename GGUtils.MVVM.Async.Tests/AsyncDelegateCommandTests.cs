using System;
using System.Threading;
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

        private static readonly Func<CancellationToken, Task<string>> CancelableCommand = _ => Command();
        private static readonly Func<CancellationToken, Task<string>> UnfinishableCancelableCommand = _ => UnfinishableCommand();

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

        #region Cancelation

        [Test]
        public void ShouldThrow_ArgumentNullException_WhenCommandIsCancelableButNull()
        {
            Func<CancellationToken, Task<string>> task = null;

            Assert.Throws<ArgumentNullException>(() => new AsyncDelegateCommand<string>(task));
        }

        [Test]
        public void ShouldNot_CanExecuteCancellationCommand_WhenTaskIsNotRunning()
        {
            var asyncCommand = new AsyncDelegateCommand<string>(CancelableCommand);

            Assert.IsFalse(asyncCommand.CancelCommand.CanExecute(null));
        }

        [Test]
        public void Should_CanExecuteCancellationCommand_WhenTaskIsRunning()
        {
            var asyncCommand = new AsyncDelegateCommand<string>(UnfinishableCancelableCommand);

            asyncCommand.Execute(null);

            Assert.IsTrue(asyncCommand.CancelCommand.CanExecute(null));
        }

        [Test]
        public async Task ShouldNot_CanExecuteCancellation_WhenTaskHasFinished()
        {
            var asyncCommand = new AsyncDelegateCommand<string>(CancelableCommand);

            await asyncCommand.ExecuteAsync(null);

            Assert.IsFalse(asyncCommand.CancelCommand.CanExecute(null));
        }

        [Test]
        public async Task ShouldCall_CanExecuteChangedAtCancelAsyncCommand_WhenExecutionStarts_And_WhenItEnds()
        {
            var timesCalled = 0;
            var asyncCommand = new AsyncDelegateCommand<string>(CancelableCommand);
            asyncCommand.CancelCommand.CanExecuteChanged += (src, args) => timesCalled++;

            await asyncCommand.ExecuteAsync(null);

            Assert.AreEqual(2, timesCalled);
        }

        [Test]
        public void ShouldCancel_CancellationToken_WhenCancelIsExecuted()
        {
            CancellationToken token;
            Func<CancellationToken, Task<string>> command = cts => { token = cts; return UnfinishableCommand(); };
            var asyncCommand = new AsyncDelegateCommand<string>(command);

            asyncCommand.Execute(null);
            asyncCommand.CancelCommand.Execute(null);

            Assert.IsTrue(token.IsCancellationRequested);
        }

        [Test]
        public async Task ShouldReset_CancellationToken_WhenCancelIsExecuted_And_CommandIsRelaunched()
        {
            CancellationToken token = CancellationToken.None; // To make the test fail if not called
            var waitCTS = true;

            Func<CancellationToken, Task<string>> command = cts => Task.Run(async () =>
            {
                token = cts;

                if (waitCTS)
                {
                    waitCTS = false;
                    await Task.Run(() => cts.WaitHandle.WaitOne());
                }

                return CommandResult;
            });
            var asyncCommand = new AsyncDelegateCommand<string>(command);

            asyncCommand.Execute(null);
            asyncCommand.CancelCommand.Execute(null);
            await asyncCommand.ExecuteAsync(null);

            Assert.IsFalse(token.IsCancellationRequested);
        }
        #endregion
    }
}
