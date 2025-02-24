﻿using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System;
using System.Collections.Generic;
using VS.Common.DoctestTestAdapter.Packages;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics;
using System.Xml;
using VS.Common.DoctestTestAdapter.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.Threading;

namespace VS.Common.DoctestTestAdapter
{
    public static class VSUtilities
    {
        private static readonly bool shouldAttachDebugger = false;

        /// <summary>
        /// Use this alongside a call to Debugger.Launch();
        /// Will prompt a dialogue box to choose a Visual Studio to attach.
        /// E.g.
        /// if (VSUtilities.ShouldAttachDebugger())
        ///     Debugger.Launch();
        /// </summary>
        /// <returns>bool - true if user wants to attach debugger, false otherwise.</returns>
        public static bool ShouldAttachDebugger()
        {
            return shouldAttachDebugger;
        }

        public static IEnumerable<IVsHierarchy> EnumerateLoadedProjects(this IVsSolution _solution, __VSENUMPROJFLAGS _enumFlags)
        {
            //TODO_comfyjase_24/01/2025: Comment back in once the thread exception has been fixed to fix compiler warning VSTHRD010.
            //ThreadHelper.ThrowIfNotOnUIThread();

            Guid projectType = Guid.Empty;
            IEnumHierarchies ppHierarchy;

            int hr = _solution.GetProjectEnum((uint)_enumFlags, ref projectType, out ppHierarchy);
            if (ErrorHandler.Succeeded(hr) && ppHierarchy != null)
            {
                uint fetched = 0;
                IVsHierarchy[] hierarchies = new IVsHierarchy[1];
                while (ppHierarchy.Next(1, hierarchies, out fetched) == VSConstants.S_OK)
                {
                    yield return hierarchies[0];
                }
            }
        }

        public static IEnumerable<string> GetProjectItems(IVsProject _project)
        {
            //TODO_comfyjase_24/01/2025: Comment back in once the thread exception has been fixed to fix compiler warning VSTHRD010.
            //ThreadHelper.ThrowIfNotOnUIThread();

            // Each item in VS OM is IVSHierarchy. 
            IVsHierarchy hierarchy = (IVsHierarchy)_project;
            return GetProjectItems(hierarchy, VSConstants.VSITEMID_ROOT);
        }

        public static IEnumerable<string> GetProjectItems(IVsHierarchy _project, uint _itemId)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();

            object childPropertyValue = GetPropertyValue((int)__VSHPROPID.VSHPROPID_FirstChild, _itemId, _project);

            uint childId = GetItemId(childPropertyValue);
            while (childId != VSConstants.VSITEMID_NIL)
            {
                string childPath = GetCanonicalName(childId, _project);
                yield return childPath;

                foreach (string childNodePath in GetProjectItems(_project, childId)) 
                    yield return childNodePath;

                childPropertyValue = GetPropertyValue((int)__VSHPROPID.VSHPROPID_NextSibling, childId, _project);
                childId = GetItemId(childPropertyValue);
            }
        }

        public static uint GetItemId(object _propertyValue)
        {
            if (_propertyValue == null)     return VSConstants.VSITEMID_NIL;
            if (_propertyValue is int)      return (uint)(int)_propertyValue;
            if (_propertyValue is uint)     return (uint)_propertyValue;
            if (_propertyValue is short)    return (uint)(short)_propertyValue;
            if (_propertyValue is ushort)   return (uint)(ushort)_propertyValue;
            if (_propertyValue is long)     return (uint)(long)_propertyValue;
            return VSConstants.VSITEMID_NIL;
        }

        public static object GetPropertyValue(int _propertyId, uint _itemId, IVsHierarchy _vsHierarchy)
        {
            //TODO_comfyjase_24/01/2025: Comment back in once the thread exception has been fixed to fix compiler warning VSTHRD010.
            //ThreadHelper.ThrowIfNotOnUIThread();

            if (_itemId == VSConstants.VSITEMID_NIL)
            {
                return null;
            }

            try
            {
                object propertyValue;
                ErrorHandler.ThrowOnFailure(_vsHierarchy.GetProperty(_itemId, _propertyId, out propertyValue));

                return propertyValue;
            }
            catch (System.NotImplementedException)
            {
                return null;
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                return null;
            }
            catch (System.ArgumentException)
            {
                return null;
            }
        }

        public static string GetCanonicalName(uint _itemId, IVsHierarchy _hierarchy)
        {
            //TODO_comfyjase_24/01/2025: Comment back in once the thread exception has been fixed to fix compiler warning VSTHRD010.
            //ThreadHelper.ThrowIfNotOnUIThread();

            string strRet = string.Empty;
            int hr = _hierarchy.GetCanonicalName(_itemId, out strRet);

            if (hr == VSConstants.E_NOTIMPL)
            {
                return string.Empty;
            }
            else
            {
                try
                {
                    ErrorHandler.ThrowOnFailure(hr);
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    strRet = string.Empty;
                }

                return strRet;
            }
        }

        public static T GetOptionValue<T>(string optionsFilePath, string _optionCategoryNodeName, string _optionNodeName)
        {
            object value = null;

            //XmlFile optionsFile = new XmlFile(VS.Common.DoctestTestAdapter.Constants.Options.FilePath);
            XmlFile optionsFile = new XmlFile(optionsFilePath);
            XmlDocument optionsXmlDocument = optionsFile.XmlDocument;
            XmlNode optionNode = optionsXmlDocument.SelectSingleNode("//" + VS.Common.DoctestTestAdapter.Constants.XmlNodeNames.Root + "/" + VS.Common.DoctestTestAdapter.Constants.XmlNodeNames.Options + "/" + _optionCategoryNodeName + "/" + _optionNodeName);

            if (optionNode != null)
            {
                Logger.Instance.WriteLine("Found option " + optionNode.Name + " from category " + _optionCategoryNodeName);

                XmlAttribute optionValueAttribute = optionNode.Attributes["Value"];
                if (optionValueAttribute != null)
                {
                    Logger.Instance.WriteLine("Category: " + _optionCategoryNodeName + " Option: " + optionNode.Name + " Value: " + optionValueAttribute.Value);
                    value = optionValueAttribute.Value;
                }
            }

            /*
             * <General>
             *  <EnableLogging Value=False/>
             *  <CommandArguments Value=""/>
             *  <TestExecutableFilePath Value=""/>
             * </General>
             */

            return (T)value;
        }

        public static T GetTestPropertyValue<T>(TestCase _test, TestProperty _testProperty)
        {
            Debug.Assert(_test != null);
            Debug.Assert(_testProperty != null);
            object testPropertyObject = _test.GetPropertyValue(_testProperty);
            Debug.Assert(testPropertyObject != null);
            return (T)testPropertyObject;
        }

        public static string GetSolutionDirectory()
        {
            string solutionDirectory = string.Empty;

            IVsTask getSolutionDirectoryTask = ThreadHelper.JoinableTaskFactory.RunAsyncAsVsTask(
                VsTaskRunContext.UIThreadBackgroundPriority,
                async cancellationToken =>
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    IVsSolution solution = (IVsSolution)ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution));
                    Debug.Assert(solution != null);

                    solution.GetSolutionInfo(out solutionDirectory, out string solutionName, out string solutionDirectory2);
                    Debug.Assert(!string.IsNullOrEmpty(solutionDirectory));

                    return VSConstants.S_OK;
                });

            getSolutionDirectoryTask.Wait();

            Debug.Assert(!string.IsNullOrEmpty(solutionDirectory));
            return solutionDirectory;

            //string solutionDirectory = ThreadHelper.JoinableTaskFactory.Run(async delegate
            //{
            //    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            //    IVsSolution solution = (IVsSolution)ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution));
            //    Debug.Assert(solution != null);

            //    solution.GetSolutionInfo(out string solutionDirectoryAsync, out string solutionName, out string solutionDirectory2);
            //    Debug.Assert(!string.IsNullOrEmpty(solutionDirectoryAsync));

            //    return solutionDirectoryAsync;
            //});

            //Debug.Assert(!string.IsNullOrEmpty(solutionDirectory));
            //return solutionDirectory;
        }

        public static ITestAdapterPackage GetTestAdapterPackage()
        {
            //TODO_comfyjase_12/02/2025: Comment back in once the thread exception has been fixed to fix compiler warning VSTHRD010.
            //ThreadHelper.ThrowIfNotOnUIThread();

            //ThreadHelper.JoinableTaskFactory.Run();

            ITestAdapterPackage testAdapterPackage = null;

            IVsShell vsShell = (IVsShell)ServiceProvider.GlobalProvider.GetService(typeof(SVsShell));
            Debug.Assert(vsShell != null);

            if (vsShell.IsPackageLoaded(VS.Common.DoctestTestAdapter.Constants.Package.Guid, out IVsPackage myPossiblePackage) == Microsoft.VisualStudio.VSConstants.S_OK)
            {
                testAdapterPackage = (ITestAdapterPackage)myPossiblePackage;
                Debug.Assert(testAdapterPackage != null);
            }

            return testAdapterPackage;
        }
    }
}
