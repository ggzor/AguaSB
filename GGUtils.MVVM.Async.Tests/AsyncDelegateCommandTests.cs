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

        private static readonly (double, string) ProgressReportingCommandValue = (100.0, "Completed");

        private static readonly Func<IProgress<(double, string)>, Task<string>> ProgressReportingCommand = prog => Task.Run(async () =>
        {
            prog.Report((ProgressReportingCommandValue.Item1, ProgressReportingCommandValue.Item2));
            await Task.Delay(100);
            return "Wololo";
        });

        private static readonly Func<IProgress<(double Progress, string ProgressMessage)>, Task<string>> ProgressCommand = _ => Command();

        [Test]
        public void ShouldThrow_ArgumentNullException_WhenCommandIsNull()
        {
            Func<Task<string>> command = null;

            Assert.Throws<ArgumentNullException>(() => new AsyncDelegateCommand<string>(command));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldCall_CanExecuteFunction_WhenItIsNotNull(bool multipleExecutionSupported)
        {
            bool called = false;
            Func<bool> canExecuteFunction = () => { called = true; return true; };

            var asyncCommand = new AsyncDelegateCommand<string>(Command, canExecuteFunction, multipleExecutionSupported);
            asyncCommand.CanExecute(null);

            Assert.IsTrue(called);
        }

        [Test]
        public void ShouldReturn_TrueCanExecute_WhenCanExecuteFunctionIsNull_And_MultipleExecutionIsSupported()
        {
            var asyncCommand = new AsyncDelegateCommand<string>(UnfinishableCommand, multipleExecutionSupported: true);

            asyncCommand.Execute(null);

            Assert.IsTrue(asyncCommand.CanExecute(null));
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

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void ShouldBeAbleTo_Execute_WhenCanExecuteReturnsTrue_And_MultipleExecutionIsSupported__And_TheCommandIsRunning(bool multipleExecutionSupported, bool result)
        {
            var asyncDelegate = new AsyncDelegateCommand<string>(UnfinishableCommand, () => true, multipleExecutionSupported);

            asyncDelegate.Execute(null);

            Assert.AreEqual(result, asyncDelegate.CanExecute(null));
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

        //[Test]: Ignore because causes the runner to block
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

        #region Progress
        [Test]
        public void ShouldThrow_ArgumentNullException_WhenCommandUsesProgressNull()
        {
            var constructors = new TestDelegate[]
            {
                () => new AsyncDelegateCommand<string>(null as Func<IProgress<(double, string)>, Task<string>>),
                () => new AsyncDelegateCommand<string>(null as Func<CancellationToken, IProgress<(double, string)>, Task<string>>)
            };

            foreach (var c in constructors)
                Assert.Throws<ArgumentNullException>(c);
        }

        [Test]
        public void ShouldNotThrowException_WhenCommandUsesProgress_AndCanExecuteFunctionIsNull()
        {
            Func<CancellationToken, IProgress<(double Progress, string ProgressMessage)>, Task<string>> func = (a, b) => Task.FromResult("Wololo");
            var constructors = new TestDelegate[]
            {
                () => new AsyncDelegateCommand<string>(ProgressCommand),
                () => new AsyncDelegateCommand<string>(func)
            };

            foreach (var c in constructors)
                c();
        }


        [Test]
        public async Task ShouldUpdate_ProgressValues_WhenProgressUpdateIsIssued()
        {
            var asyncCommand = new AsyncDelegateCommand<string>(ProgressReportingCommand);

            await asyncCommand.ExecuteAsync(null);

            Assert.AreEqual(ProgressReportingCommandValue, (asyncCommand.Progress, asyncCommand.ProgressMessage));
        }

        [Test]
        public async Task ShouldReport_PropertiesChanged_WhenProgressIsChanged()
        {
            var asyncCommand = new AsyncDelegateCommand<string>(ProgressReportingCommand);
            var observer = new PropertyChangedObserver();
            asyncCommand.PropertyChanged += observer.EventHandler;

            await asyncCommand.ExecuteAsync(null);

            CollectionAssert.IsSubsetOf(
                new[] { nameof(AsyncDelegateCommand<object>.Progress), nameof(AsyncDelegateCommand<object>.ProgressMessage) },
                observer.ChangedProperties);
        }

        [Test]
        public async Task ShouldReport_ProgressWithCancellation_WhenCancelIsExecuted()
        {
            var cancelProgressResult = "Cancelled";
            Func<CancellationToken, IProgress<(double, string)>, Task<string>> function = (ct, prog) => Task.Run(async () =>
            {
                await Task.Run(() => ct.WaitHandle.WaitOne());
                if (ct.IsCancellationRequested)
                {
                    prog.Report((0.0, cancelProgressResult));
                    await Task.Delay(100);
                }
                return "";
            });
            var asyncCommand = new AsyncDelegateCommand<string>(function);

            var task = asyncCommand.ExecuteAsync(null);
            asyncCommand.CancelCommand.Execute(null);

            await task;

            Assert.AreEqual(cancelProgressResult, asyncCommand.ProgressMessage);
        }
        #endregion

    }
}
