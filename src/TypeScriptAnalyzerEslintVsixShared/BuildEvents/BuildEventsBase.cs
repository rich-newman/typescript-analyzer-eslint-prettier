using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace TypeScriptAnalyzerEslintVsix
{
    public class BuildEventsBase : IVsUpdateSolutionEvents2
    {
        private readonly IVsSolution solution;
        private readonly IVsSolutionBuildManager3 buildManager;
        //private uint cookie1 = VSConstants.VSCOOKIE_NIL;
        private uint cookie2 = VSConstants.VSCOOKIE_NIL;
        //private uint cookie3 = VSConstants.VSCOOKIE_NIL;

        public BuildEventsBase(IServiceProvider serviceProvider)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            this.solution = serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
            if (this.solution == null)
            {
                throw new InvalidOperationException("Cannot get solution service");
            }
            this.buildManager = serviceProvider.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager3;
        }

        public void StartListeningForChanges()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            //ErrorHandler.ThrowOnFailure(this.solution.AdviseSolutionEvents(this, out this.cookie1));
            if (this.buildManager != null)
            {
                if (this.buildManager is IVsSolutionBuildManager2 bm2)
                {
                    ErrorHandler.ThrowOnFailure(bm2.AdviseUpdateSolutionEvents(this, out this.cookie2));
                }
                //ErrorHandler.ThrowOnFailure(this.buildManager.AdviseUpdateSolutionEvents3(this, out this.cookie3));
            }
        }

        public void Dispose()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            // Ignore failures in UnadviseSolutionEvents
            //if (this.cookie1 != VSConstants.VSCOOKIE_NIL)
            //{
            //    this.solution.UnadviseSolutionEvents(this.cookie1);
            //    this.cookie1 = VSConstants.VSCOOKIE_NIL;
            //}
            if (this.cookie2 != VSConstants.VSCOOKIE_NIL)
            {
                ((IVsSolutionBuildManager2)this.buildManager).UnadviseUpdateSolutionEvents(this.cookie2);
                this.cookie2 = VSConstants.VSCOOKIE_NIL;
            }
            //if (this.cookie3 != VSConstants.VSCOOKIE_NIL)
            //{
            //    this.buildManager.UnadviseUpdateSolutionEvents3(this.cookie3);
            //    this.cookie3 = VSConstants.VSCOOKIE_NIL;
            //}
        }

        public virtual int UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int UpdateSolution_StartUpdate(ref int pfCancelUpdate)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int UpdateSolution_Cancel()
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int UpdateProjectCfg_Begin(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int UpdateProjectCfg_Done(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
            return VSConstants.E_NOTIMPL;
        }
    }
}
