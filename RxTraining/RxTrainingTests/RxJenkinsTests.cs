using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Reactive.Testing;
using NSubstitute;
using NUnit.Framework;
using RxTraining;

namespace RxTrainingTests
{
    [TestFixture]
    class RxJenkinsTests
    {
        private IJenkinsApi mockApi;
        private IJenkinsJob mockJob;
        private IJenkinsBuild mockBuild;
        private IObserver<IJenkinsBuild> mockObserver;
        private TestRxScheduler scheduler;

        [SetUp]
        public void Setup()
        {
            this.scheduler = new TestRxScheduler();

            this.mockApi = Substitute.For<IJenkinsApi>();
            this.mockJob = Substitute.For<IJenkinsJob>();
            this.mockBuild = Substitute.For<IJenkinsBuild>();
            this.mockObserver = Substitute.For<IObserver<IJenkinsBuild>>();

            this.mockApi.Job(Arg.Any<string>()).Returns(this.mockJob);
            this.mockJob.LastFailedBuild.Returns(this.mockBuild);
        }

        [Test]
        public void SubscribingToFailedTrunkBuildWillAskForLastFailedBuild()
        {
            var rxJenkins = new RxJenkins(this.mockApi, this.scheduler);

            using (rxJenkins.FailedTrunkBuild.Subscribe(this.mockObserver))
            {
                this.scheduler.TestThreadPool.AdvanceBy(1);
                this.scheduler.TestDefault.AdvanceBy(1);

                this.mockApi.Received(1).Job("Trunk");
            }
        }

        [Test]
        public void OperationRepeatsEveryMinute()
        {
            // TODO #6 You decide to create something a little more meaningful
            //         Use a timer observable to repeat the new async action every minute and poll the Jenkins API
            //         Make sure to schedule the timing on the ThreadPool scheduler
            //         You can use linq to make an observable of observables into one observable. Observable. :)
            var rxJenkins = new RxJenkins(this.mockApi, this.scheduler);

            using (rxJenkins.FailedTrunkBuild.Subscribe(this.mockObserver))
            {
                this.scheduler.TestThreadPool.AdvanceBy(1);
                this.scheduler.TestDefault.AdvanceBy(1);
                this.scheduler.TestThreadPool.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);
                this.scheduler.TestDefault.Start();

                this.mockApi.Received(2).Job("Trunk");
            }
        }

        [Test]
        public void OperationIsNotRepeatedOrRestartedIfTakesLongerThanOneMinute()
        {
            // TODO #7 Sometimes Jenkins is slow, and a request takes more than a minute to complete
            //         Throttle the observable to make sure that another async action is not executed 
            //         if the previous action has not completed
            var rxJenkins = new RxJenkins(this.mockApi, this.scheduler);

            using (rxJenkins.FailedTrunkBuild.Subscribe(this.mockObserver))
            {
                this.scheduler.TestThreadPool.AdvanceBy(1);
                this.scheduler.TestThreadPool.AdvanceBy(TimeSpan.FromMinutes(5).Ticks);
                this.scheduler.TestDefault.Start();

                this.mockApi.Received(2).Job("Trunk"); // The exception is the first element in Timer observable (it seems)
            }
        }

        [Test]
        public void ObservableDoesNotPublishResultIfItIsTheSameAsTheLastElement()
        {
            // TODO #8 Ohhh snap. Why would you publish an element if it's the same as the previous one?
            //         Make sure that the resulting observable only publishes an element if it's different than the previous one
            var rxJenkins = new RxJenkins(this.mockApi, this.scheduler);

            using (rxJenkins.FailedTrunkBuild.Subscribe(this.mockObserver))
            {
                this.scheduler.TestThreadPool.AdvanceBy(1);
                this.scheduler.TestDefault.AdvanceBy(1);
                this.scheduler.TestThreadPool.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);
                this.scheduler.TestDefault.AdvanceBy(1);

                this.mockApi.Received(2).Job("Trunk"); // Two calls to the API
                this.mockObserver.Received(1).OnNext(Arg.Any<IJenkinsBuild>()); // Only published once
            }
        }

        [Test]
        public void ObservablePublishesResultsIfTheyAreDifferent()
        {
            var rxJenkins = new RxJenkins(this.mockApi, this.scheduler);

            using (rxJenkins.FailedTrunkBuild.Subscribe(this.mockObserver))
            {
                this.scheduler.TestThreadPool.AdvanceBy(1);
                this.scheduler.TestDefault.AdvanceBy(1);

                this.mockJob.LastFailedBuild.Returns(new JenkinsBuild());
                this.scheduler.TestThreadPool.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);
                this.scheduler.TestDefault.AdvanceBy(1);

                this.mockObserver.Received(2).OnNext(Arg.Any<IJenkinsBuild>());
            }
        }

        [Test]
        public void ObservableCanReplayNewestElementForLateSubscribers()
        {
            // TODO #9 Return the latest element for late subscribers in order finish this beautiful polling thingy ma jiggy
            //         As it stands, for every new subscription a new timer will be made
            //         Which will also end up with more API calls than needed
            //         If we used Publish then we could have one underlying subscription 
            //         that is then used by all other subscriptions, fixing the new timer and uneccessary API calls.
            //         But then new subscribers would have to wait for the next distinct element before the new subscriber gets any data.
            //         --
            //         The Publish extension method returns a ConnectableObservable and the Connect
            //         method will connect the underlying subscription to the observable sequence.
            //         The method needed to fix this test also returns a ConnectableObservable.
            //         Make use of the field already in the RxJenkins class and don't forget to call Connect!
            var rxJenkins = new RxJenkins(this.mockApi, this.scheduler);

            using (rxJenkins.FailedTrunkBuild.Subscribe(this.mockObserver))
            {
                this.scheduler.TestThreadPool.AdvanceBy(1);
                this.scheduler.TestDefault.AdvanceBy(1);
            }

            this.mockJob.LastFailedBuild.Returns(new JenkinsBuild());
            this.scheduler.TestThreadPool.AdvanceBy(TimeSpan.FromSeconds(30).Ticks);

            var newObserver = Substitute.For<IObserver<IJenkinsBuild>>();
            using (rxJenkins.FailedTrunkBuild.Subscribe(newObserver))
            {
                this.scheduler.TestThreadPool.AdvanceBy(1);
                this.scheduler.TestDefault.AdvanceBy(1);
            }

            this.mockObserver.Received(1).OnNext(this.mockBuild);
            newObserver.Received(1).OnNext(this.mockBuild);
            this.mockApi.Received(1).Job("Trunk");
        }
    }
}
