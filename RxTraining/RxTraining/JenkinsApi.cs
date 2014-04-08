using System;
using System.Net;
using System.Xml.Linq;

namespace RxTraining
{
    public interface IJenkinsApi
    {
        IJenkinsJob Job(string jobname);
        IJenkinsBuild Build(string jobname, int buildno);
    }

    public class JenkinsApi : IJenkinsApi
    {
        private readonly string endpoint;

        public JenkinsApi(string endpoint)
        {
            this.endpoint = endpoint;
        }

        public IJenkinsJob Job(string jobname)
        {
            var url = string.Format("http://{0}/job/{1}/api/xml", this.endpoint, jobname);

            return JenkinsJob.FromXml(this, new WebClient().DownloadString(url));
        }

        public IJenkinsBuild Build(string jobname, int buildno)
        {
            var url = string.Format("http://{0}/job/{1}/{2}/api/xml", this.endpoint, jobname, buildno);

            return JenkinsBuild.FromXml(new WebClient().DownloadString(url));
        }
    }

    public interface IJenkinsJob
    {
        string Name { get; }
        int HealthScore { get; }
        int LastFailedBuildNo { get; }
        IJenkinsBuild LastFailedBuild { get; }
    }

    public class JenkinsJob : IJenkinsJob
    {
        private readonly IJenkinsApi api;

        private JenkinsJob(IJenkinsApi api)
        {
            this.api = api;
        }

        public static IJenkinsJob FromXml(IJenkinsApi api, string xml)
        {
            var job = new JenkinsJob(api);
            var xdoc = XDocument.Parse(xml);
            var xroot = xdoc.Root;

            job.Name = xroot.Element("name").Value;
            job.HealthScore = Int32.Parse(xroot.Element("healthReport").Element("score").Value);
            job.LastFailedBuildNo = Int32.Parse(xroot.Element("lastFailedBuild").Element("number").Value);

            return job;
        }

        public string Name { get; private set; }
        public int HealthScore { get; private set; }
        public int LastFailedBuildNo { get; private set; }
        public IJenkinsBuild LastFailedBuild { get { return this.api.Build(this.Name, this.LastFailedBuildNo); } }

    }

    public interface IJenkinsBuild
    {
        bool IsBuilding { get; }
        DateTime TimeBuilt { get; }
    }

    public class JenkinsBuild : IJenkinsBuild
    {
        public static IJenkinsBuild FromXml(string xml)
        {
            var build = new JenkinsBuild();
            var xdoc = XDocument.Parse(xml);
            var xroot = xdoc.Root;

            build.IsBuilding = xroot.Element("building").Value == "true";

            var timestamp = UInt64.Parse(xroot.Element("timestamp").Value);
            build.TimeBuilt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(timestamp);

            return build;
        }

        public bool IsBuilding { get; private set; }
        public DateTime TimeBuilt { get; private set; }
    }
}
