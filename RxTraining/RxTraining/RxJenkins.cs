using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace RxTraining
{
    public class RxJenkins
    {
        private readonly IConnectableObservable<IJenkinsBuild> connectable;

        public RxJenkins(IJenkinsApi jenkinsApi, IRxScheduler scheduler)
        {
            // TODO #5 Asynchronous operations can be made from some long running process by using Observable.Start()
            //         The resulting observable only publishes a single result. The process is started on subscribe
            
            this.FailedTrunkBuild = Observable.Start(() => jenkinsApi.Job("Trunk").LastFailedBuild, scheduler.Default);
        }

        //public IObservable<IJenkinsBuild> FailedTrunkBuild { get { return this.connectable; } }
        public IObservable<IJenkinsBuild> FailedTrunkBuild { get; private set; }
    }
}
