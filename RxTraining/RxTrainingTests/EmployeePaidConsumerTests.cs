using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using NSubstitute;
using NUnit.Framework;
using RxTraining;

namespace RxTrainingTests
{
    [TestFixture]
    class EmployeePaidConsumerTests
    {
        private IEmail mockEmail;

        [SetUp]
        public void Setup()
        {
            this.mockEmail = Substitute.For<IEmail>();
        }

        [Test]
        public void EmailIsSentWhenEmployeePaid()
        {
            // TODO #3.5 Imagine EmployeePaid was an event, what would this test look like?

            var subject = new Subject<EmployeePayment>();

            using (new EmployeePaidConsumer(subject, mockEmail))
            {
                subject.OnNext(new EmployeePayment("aaron", 123.45));
                this.mockEmail.Received(1).Send(Arg.Any<string>());
            }
        }

        [Test]
        public void EmailIsSentIfNoOneRecievesPaymentInThrityDays()
        {
            // TODO #4 Fix the consumer class and uncomment the "using" line below to make this test pass
            //         Problem - The consumer of the observable needs to send an alert email if no one is paid in thirty days
            //                   But the nature of testing timeouts is naturally a burden for any developer (timing, threads...)
            //         ---
            //         The test scheduler uses virtual time so you can programatically advance the clock, executing all actions along the way
            //         No more fiddling with timeout values and using ManualResetEvents! Hazaa!
            //         ---
            //         It's easy to make Rx solve the timeout problem for you
            //         And it's simple to ensure that the time-based operations run on a specified scheduler
            //         Good luck!

            var scheduler = new TestScheduler();
            var observable = Observable.Never<EmployeePayment>(); // Never publishes results, never errors and never completes

            // using (new EmployeePaidConsumer(observable, this.mockEmail, scheduler))
            {
                scheduler.AdvanceBy(TimeSpan.FromDays(30).Ticks);
                this.mockEmail.Received(1).Send(Arg.Is<string>(s => s.Contains("error")));
            }
        }
    }
}
