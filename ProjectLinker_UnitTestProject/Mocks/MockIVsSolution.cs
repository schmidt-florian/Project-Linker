//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProjectLinker.Tests.Mocks
{
    internal class MockIVsSolution : IVsSolution
    {
        public bool AdviseSolutionEventsCalled;
        public List<MockVsHierarchy> Hierarchies = new List<MockVsHierarchy>();

        int IVsSolution.AdviseSolutionEvents(IVsSolutionEvents pSink, out uint pdwCookie)
        {
            AdviseSolutionEventsCalled = true;
            pdwCookie = 1;
            return VSConstants.S_OK;
        }

        int IVsSolution.GetProjectOfGuid(ref Guid rguidProjectID, out IVsHierarchy ppHierarchy)
        {
            ppHierarchy = null;
            foreach (MockVsHierarchy hierarchy in Hierarchies)
            {
                if (hierarchy.GetPropertyProjectIdGuidValue == rguidProjectID)
                {
                    ppHierarchy = hierarchy;
                }
            }
            return VSConstants.S_OK;
        }

        int IVsSolution.GetProjectOfUniqueName(string pszUniqueName, out IVsHierarchy ppHierarchy)
        {
            ppHierarchy = null;
            foreach (MockVsHierarchy hierarchy in Hierarchies)
            {
                if (hierarchy.GetPropertyProjectValue.FullName == pszUniqueName)
                {
                    ppHierarchy = hierarchy;
                }
            }
            return VSConstants.S_OK;
        }

        #region IVsSolution members

        int IVsSolution.GetProjectEnum(uint grfEnumFlags, ref Guid rguidEnumOnlyThisType, out IEnumHierarchies ppenum)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.CreateProject(ref Guid rguidProjectType, string lpszMoniker, string lpszLocation, string lpszName, uint grfCreateFlags,
            ref Guid iidProject, out IntPtr ppProject)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GenerateUniqueProjectName(string lpszRoot, out string pbstrProjectName)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetGuidOfProject(IVsHierarchy pHierarchy, out Guid pguidProjectID)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetSolutionInfo(out string pbstrSolutionDirectory, out string pbstrSolutionFile, out string pbstrUserOptsFile)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.UnadviseSolutionEvents(uint dwCookie)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.SaveSolutionElement(uint grfSaveOpts, IVsHierarchy pHier, uint docCookie)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.CloseSolutionElement(uint grfCloseOpts, IVsHierarchy pHier, uint docCookie)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjectOfProjref(string pszProjref, out IVsHierarchy ppHierarchy, out string pbstrUpdatedProjref,
            VSUPDATEPROJREFREASON[] puprUpdateReason)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjrefOfProject(IVsHierarchy pHierarchy, out string pbstrProjref)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjectInfoOfProjref(string pszProjref, int propid, out object pvar)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.AddVirtualProject(IVsHierarchy pHierarchy, uint grfAddVpFlags)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetItemOfProjref(string pszProjref, out IVsHierarchy ppHierarchy, out uint pitemid, out string pbstrUpdatedProjref,
            VSUPDATEPROJREFREASON[] puprUpdateReason)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjrefOfItem(IVsHierarchy pHierarchy, uint itemid, out string pbstrProjref)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetItemInfoOfProjref(string pszProjref, int propid, out object pvar)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetUniqueNameOfProject(IVsHierarchy pHierarchy, out string pbstrUniqueName)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProperty(int propid, out object pvar)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.SetProperty(int propid, object var)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.OpenSolutionFile(uint grfOpenOpts, string pszFilename)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.QueryEditSolutionFile(out uint pdwEditResult)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.CreateSolution(string lpszLocation, string lpszName, uint grfCreateFlags)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjectFactory(uint dwReserved, Guid[] pguidProjectType, string pszMkProject, out IVsProjectFactory ppProjectFactory)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjectTypeGuid(uint dwReserved, string pszMkProject, out Guid pguidProjectType)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.OpenSolutionViaDlg(string pszStartDirectory, int fDefaultToAllProjectsFilter)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.AddVirtualProjectEx(IVsHierarchy pHierarchy, uint grfAddVpFlags, ref Guid rguidProjectID)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.QueryRenameProject(IVsProject pProject, string pszMkOldName, string pszMkNewName, uint dwReserved, out int pfRenameCanContinue)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.OnAfterRenameProject(IVsProject pProject, string pszMkOldName, string pszMkNewName, uint dwReserved)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.RemoveVirtualProject(IVsHierarchy pHierarchy, uint grfRemoveVpFlags)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.CreateNewProjectViaDlg(string pszExpand, string pszSelect, uint dwReserved)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetVirtualProjectFlags(IVsHierarchy pHierarchy, out uint pgrfAddVpFlags)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GenerateNextDefaultProjectName(string pszBaseName, string pszLocation, out string pbstrProjectName)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjectFilesInSolution(uint grfGetOpts, uint cProjects, string[] rgbstrProjectNames, out uint pcProjectsFetched)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.CanCreateNewProjectAtLocation(int fCreateNewSolution, string pszFullProjectFilePath, out int pfCanCreate)
        {
            throw new NotImplementedException();
        }

        #endregion IVsSolution members
    }
}