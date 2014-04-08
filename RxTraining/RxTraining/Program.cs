using System;
using System.Reactive.Linq;

namespace RxTraining
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new JenkinsApi("rbrjenkins");
            var rx = new RxJenkins(api, new RxScheduler());

            using (rx.FailedTrunkBuild.Select(j => j.TimeBuilt.ToString()).Subscribe(Console.WriteLine))
            {
                Console.ReadLine();
            }
        }
    }
}
