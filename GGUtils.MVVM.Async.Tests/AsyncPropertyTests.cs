using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGUtils.MVVM.Async.Tests
{
    [TestFixture]
    public class AsyncPropertyTests
    {

        private static readonly Task<string> UncompletableTask = Task.Run<string>(async () => { await Task.Delay(-1); return null; });

        private const string CompletedTaskResult = "Wololo";
        private static readonly Task<string> CompletedTask = Task.FromResult(CompletedTaskResult);

        private readonly Task<string> FaultedTask = Task.FromException<string>(new Exception());

        private readonly Task<string> CanceledTask = Task.FromCanceled<string>(new System.Threading.CancellationToken(true));

        [Test]
        public void ShouldThrow_ArgumentNullException_WhenTaskIsNull()
        {
            Task<string> task = null;

            Assert.Throws<ArgumentNullException>(() => new AsyncProperty<string>(task));
        }

        [Test]
        public void ShouldSet_TaskCompletion_WhenTaskIsNotCompleted()
        {
            var task = UncompletableTask;

            var asyncProperty = new AsyncProperty<string>(task);

            Assert.IsNotNull(asyncProperty.TaskCompletion);
        }

        [Test]
        public void ShouldSet_TaskCompletionToACompletedTask_WhenTaskIsCompleted()
        {
            var task = CompletedTask;

            var asyncProperty = new AsyncProperty<string>(task);

            Assert.True(asyncProperty.TaskCompletion.IsCompleted);
        }

        [Test]
        public async Task ShouldRaise_OnCompletionProperties_WhenTaskIsCompleted()
        {
            var task = DelayTask(CompletedTask);

            await WrapTaskAndCheckIfItIsSubset(task, AsyncProperty<object>.OnCompletionChangingProperties);
        }

        [Test]
        public async Task ShouldRaise_OnSuccessProperties_WhenTaskIsSuccessfullyCompleted()
        {
            var task = DelayTask(CompletedTask);

            await WrapTaskAndCheckIfItIsSubset(task, AsyncProperty<object>.OnSuccessChangingProperties);
        }

        [Test]
        public async Task ShouldRaise_OnFaultedProperties_WhenTaskThrowsAnException()
        {
            var task = DelayTask(FaultedTask);

            await WrapTaskAndCheckIfItIsSubset(task, AsyncProperty<object>.OnFaultedChangingProperties);
        }

        [Test]
        public async Task ShouldRaise_OnCanceledProperties_WhenTaskIsCanceled()
        {
            var task = DelayTask(CanceledTask);

            await WrapTaskAndCheckIfItIsSubset(task, AsyncProperty<object>.OnCanceledChangingProperties);
        }

        [Test]
        public async Task ShouldSet_PropertiesValues_WhenTaskIsSuccessfullyCompleted()
        {
            var task = CompletedTask;

            var asyncProperty = new AsyncProperty<string>(task);
            await asyncProperty.TaskCompletion;

            Assert.IsTrue(asyncProperty.IsCompleted);
            Assert.IsFalse(asyncProperty.IsNotCompleted);
            Assert.IsTrue(asyncProperty.IsSuccessfullyCompleted);

            Assert.IsFalse(asyncProperty.IsCanceled);
            Assert.IsFalse(asyncProperty.IsFaulted);

            Assert.IsNull(asyncProperty.Exception);
            Assert.IsNull(asyncProperty.InnerException);
            Assert.IsNull(asyncProperty.ExceptionMessage);

            Assert.AreEqual(CompletedTaskResult, asyncProperty.Result);
        }

        [Test]
        public async Task ShouldSet_PropertiesValues_WhenTaskIsFaulted()
        {
            var task = FaultedTask;

            var asyncProperty = new AsyncProperty<string>(task);
            await asyncProperty.TaskCompletion;

            Assert.IsTrue(asyncProperty.IsCompleted);
            Assert.IsFalse(asyncProperty.IsNotCompleted);
            Assert.IsFalse(asyncProperty.IsSuccessfullyCompleted);

            Assert.IsFalse(asyncProperty.IsCanceled);
            Assert.IsTrue(asyncProperty.IsFaulted);

            Assert.IsNotNull(asyncProperty.Exception);
            Assert.IsNotNull(asyncProperty.InnerException);
            Assert.IsNotNull(asyncProperty.ExceptionMessage);

            Assert.IsNull(asyncProperty.Result);
        }


        [Test]
        public async Task ShouldSet_PropertiesValues_WhenTaskIsCanceled()
        {
            var task = CanceledTask;

            var asyncProperty = new AsyncProperty<string>(task);
            await asyncProperty.TaskCompletion;

            Assert.IsTrue(asyncProperty.IsCompleted);
            Assert.IsFalse(asyncProperty.IsNotCompleted);
            Assert.IsFalse(asyncProperty.IsSuccessfullyCompleted);

            Assert.IsTrue(asyncProperty.IsCanceled);
            Assert.IsFalse(asyncProperty.IsFaulted);

            Assert.IsNull(asyncProperty.Exception);
            Assert.IsNull(asyncProperty.InnerException);
            Assert.IsNull(asyncProperty.ExceptionMessage);

            Assert.IsNull(asyncProperty.Result);
        }

        private static async Task WrapTaskAndCheckIfItIsSubset(Task<string> task, IEnumerable<string> subset)
        {
            var observer = new PropertyChangedObserver();

            var asyncProperty = new AsyncProperty<string>(task);
            asyncProperty.PropertyChanged += observer.EventHandler;
            await asyncProperty.TaskCompletion;

            CollectionAssert.IsSupersetOf(observer.ChangedProperties, subset);
        }

        /// <summary>
        /// Delays a task, allowing to set event handlers before the actual execution.
        /// </summary>
        private async Task<string> DelayTask(Task<string> task)
        {
            await Task.Delay(100);

            return await task;
        }

        private class PropertyChangedObserver
        {
            public List<string> ChangedProperties { get; } = new List<string>();

            public PropertyChangedEventHandler EventHandler { get; }

            public PropertyChangedObserver() =>
                EventHandler = (src, args) => ChangedProperties.Add(args.PropertyName);
        }
    }
}
