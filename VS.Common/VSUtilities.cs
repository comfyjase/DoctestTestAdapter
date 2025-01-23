using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System;
using System.Collections.Generic;

namespace VS.Common
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

        public static IEnumerable<IVsHierarchy> EnumerateLoadedProjects(this IVsSolution solution, __VSENUMPROJFLAGS enumFlags)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();

            Guid prjType = Guid.Empty;
            IEnumHierarchies ppHier;

            int hr = solution.GetProjectEnum((uint)enumFlags, ref prjType, out ppHier);
            if (ErrorHandler.Succeeded(hr) && ppHier != null)
            {
                uint fetched = 0;
                IVsHierarchy[] hierarchies = new IVsHierarchy[1];
                while (ppHier.Next(1, hierarchies, out fetched) == VSConstants.S_OK)
                {
                    yield return hierarchies[0];
                }
            }
        }

        public static IEnumerable<string> GetProjectItems(IVsProject project)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();

            // Each item in VS OM is IVSHierarchy. 
            IVsHierarchy hierarchy = (IVsHierarchy)project;
            return GetProjectItems(hierarchy, VSConstants.VSITEMID_ROOT);
        }

        public static IEnumerable<string> GetProjectItems(IVsHierarchy project, uint itemId)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();

            object pVar = GetPropertyValue((int)__VSHPROPID.VSHPROPID_FirstChild, itemId, project);

            uint childId = GetItemId(pVar);
            while (childId != VSConstants.VSITEMID_NIL)
            {
                string childPath = GetCanonicalName(childId, project);
                yield return childPath;

                foreach (string childNodePath in GetProjectItems(project, childId)) yield return childNodePath;

                pVar = GetPropertyValue((int)__VSHPROPID.VSHPROPID_NextSibling, childId, project);
                childId = GetItemId(pVar);
            }
        }

        public static uint GetItemId(object pvar)
        {
            if (pvar == null) return VSConstants.VSITEMID_NIL;
            if (pvar is int) return (uint)(int)pvar;
            if (pvar is uint) return (uint)pvar;
            if (pvar is short) return (uint)(short)pvar;
            if (pvar is ushort) return (uint)(ushort)pvar;
            if (pvar is long) return (uint)(long)pvar;
            return VSConstants.VSITEMID_NIL;
        }

        public static object GetPropertyValue(int propid, uint itemId, IVsHierarchy vsHierarchy)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();

            if (itemId == VSConstants.VSITEMID_NIL)
            {
                return null;
            }

            try
            {
                object o;
                ErrorHandler.ThrowOnFailure(vsHierarchy.GetProperty(itemId, propid, out o));

                return o;
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

        public static string GetCanonicalName(uint itemId, IVsHierarchy hierarchy)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();

            string strRet = string.Empty;
            int hr = hierarchy.GetCanonicalName(itemId, out strRet);

            if (hr == VSConstants.E_NOTIMPL)
            {
                // Special case E_NOTIMLP to avoid perf hit to throw an exception.
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

                // This could be in the case of S_OK, S_FALSE, etc.
                return strRet;
            }
        }
    }
}
