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

namespace ProjectLinker.Helper.Design
{
	public partial class SolutionPickerControl
	{
		private System.Windows.Forms.ImageList treeIcons;
		private System.Windows.Forms.TreeView solutionTree;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SolutionPickerControl));
            this.treeIcons = new System.Windows.Forms.ImageList(this.components);
            this.solutionTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeIcons
            // 
            this.treeIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            resources.ApplyResources(this.treeIcons, "treeIcons");
            this.treeIcons.TransparentColor = System.Drawing.Color.Magenta;
            // 
            // solutionTree
            // 
            resources.ApplyResources(this.solutionTree, "solutionTree");
            this.solutionTree.ImageList = this.treeIcons;
            this.solutionTree.Name = "solutionTree";
            this.solutionTree.Sorted = true;
            this.solutionTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.OnBeforeExpand);
            this.solutionTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnSelect);
            // 
            // SolutionPickerControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.solutionTree);
            this.Name = "SolutionPickerControl";
            this.ResumeLayout(false);

		}

		#endregion
	}
}
