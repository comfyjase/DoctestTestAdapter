using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Microsoft.VisualStudio.TestWindow.Extensibility.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VS.Common.DoctestTestAdapter;

namespace VS2022.DoctestTestAdapter
{
    public class DoctestTestContainer : ITestContainer
    {
        private ITestContainerDiscoverer discoverer = null;
        private readonly DateTime timeStamp = DateTime.Now;

        public string Source { get; set; }
        public Uri ExecutorUri { get; set; }
        public IEnumerable<Guid> DebugEngines { get; set; }
        public FrameworkVersion TargetFramework { get; set; }
        public Architecture TargetPlatform { get; set; }
        public IDeploymentData DeployAppContainer() { return null; }
        public bool IsAppContainerTestContainer { get { return false; } }
        public ITestContainerDiscoverer Discoverer { get { return discoverer; } }

        public DoctestTestContainer(ITestContainerDiscoverer _discoverer, string _source, Uri _executorUri)
            : this(_discoverer, _source, _executorUri, Enumerable.Empty<Guid>())
        { }

        public DoctestTestContainer(ITestContainerDiscoverer _discoverer, string _source, Uri _executorUri, IEnumerable<Guid> _debugEngines)
        {
            Logger.Instance.WriteLine("Constructor called");

            Source = _source;
            ExecutorUri = _executorUri;
            DebugEngines = _debugEngines;
            discoverer = _discoverer;
            TargetFramework = FrameworkVersion.None;
            TargetPlatform = Architecture.AnyCPU;
            timeStamp = GetTimeStamp();
        }

        private DoctestTestContainer(DoctestTestContainer _copy)
            : this(_copy.discoverer, _copy.Source, _copy.ExecutorUri)
        {
            timeStamp = _copy.timeStamp;
        }

        private DateTime GetTimeStamp()
        {
            if (!string.IsNullOrEmpty(Source) && File.Exists(Source))
            {
                return File.GetLastWriteTime(Source);
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        public override string ToString()
        {
            return ExecutorUri.ToString() + "/" + Source;
        }

        public int CompareTo(ITestContainer _other)
        {
            DoctestTestContainer testContainer = _other as DoctestTestContainer;
            if (testContainer == null)
            {
                return -1;
            }

            int result = string.Compare(Source, testContainer.Source, StringComparison.OrdinalIgnoreCase);
            if (result != 0)
            {
                return result;
            }

            return timeStamp.CompareTo(testContainer.timeStamp);
        }

        public ITestContainer Snapshot()
        {
            return new DoctestTestContainer(this);
        }
    }
}
