using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using Microsoft.Reactive.Testing;
using RxTraining;

namespace RxTrainingTests
{
    class TestRxScheduler : IRxScheduler
    {
        public readonly TestScheduler TestEventLoop = new TestScheduler();
        public readonly TestScheduler TestCurrentThread = new TestScheduler();
        public readonly TestScheduler TestNewThread = new TestScheduler();
        public readonly TestScheduler TestThreadPool = new TestScheduler();
        public readonly TestScheduler TestTaskPool = new TestScheduler();
        public readonly TestScheduler TestImmediate = new TestScheduler();
        public readonly TestScheduler TestDefault = new TestScheduler();

        public IScheduler EventLoop { get { return this.TestEventLoop; } }
        public IScheduler CurrentThread { get { return this.TestCurrentThread; } }
        public IScheduler NewThread { get { return this.TestNewThread; } }
        public IScheduler ThreadPool { get { return this.TestThreadPool; } }
        public IScheduler TaskPool { get { return this.TestTaskPool; } }
        public IScheduler Immediate { get { return this.TestImmediate; } }
        public IScheduler Default { get { return this.TestDefault; } }
    }
}
