using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;

namespace RxTraining
{
    public interface IRxScheduler
    {
        IScheduler EventLoop { get; }
        IScheduler CurrentThread { get; }
        IScheduler NewThread { get; }
        IScheduler ThreadPool { get; }
        IScheduler TaskPool { get; }
        IScheduler Immediate { get; }
        IScheduler Default { get; }
    }

    public class RxScheduler : IRxScheduler
    {
        public RxScheduler()
        {
            this.EventLoop = new EventLoopScheduler();
        }

        public IScheduler EventLoop { get; private set; }
        public IScheduler CurrentThread { get { return CurrentThreadScheduler.Instance; } }
        public IScheduler NewThread { get { return NewThreadScheduler.Default; } }
        public IScheduler ThreadPool { get { return ThreadPoolScheduler.Instance; } }
        public IScheduler TaskPool { get { return TaskPoolScheduler.Default; } }
        public IScheduler Immediate { get { return ImmediateScheduler.Instance; } }
        public IScheduler Default { get { return DefaultScheduler.Instance; } }
    }
}
