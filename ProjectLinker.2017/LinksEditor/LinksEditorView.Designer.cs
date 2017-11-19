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
namespace ProjectLinker.LinksEditor
{
    partial class LinksEditorView
    {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gridProjectLinks = new System.Windows.Forms.DataGridView();
            this.sourceProjectNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.targetProjectNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.projectLinkItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonUnbind = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridProjectLinks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectLinkItemBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // gridProjectLinks
            // 
            this.gridProjectLinks.AllowUserToAddRows = false;
            this.gridProjectLinks.AllowUserToDeleteRows = false;
            this.gridProjectLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridProjectLinks.AutoGenerateColumns = false;
            this.gridProjectLinks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridProjectLinks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sourceProjectNameDataGridViewTextBoxColumn,
            this.targetProjectNameDataGridViewTextBoxColumn});
            this.gridProjectLinks.DataSource = this.projectLinkItemBindingSource;
            this.gridProjectLinks.Location = new System.Drawing.Point(12, 12);
            this.gridProjectLinks.Name = "gridProjectLinks";
            this.gridProjectLinks.ReadOnly = true;
            this.gridProjectLinks.RowHeadersVisible = false;
            this.gridProjectLinks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridProjectLinks.Size = new System.Drawing.Size(798, 410);
            this.gridProjectLinks.TabIndex = 0;
            this.gridProjectLinks.SelectionChanged += new System.EventHandler(this.gridProjectLinks_SelectionChanged);
            // 
            // sourceProjectNameDataGridViewTextBoxColumn
            // 
            this.sourceProjectNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.sourceProjectNameDataGridViewTextBoxColumn.DataPropertyName = "SourceProjectName";
            this.sourceProjectNameDataGridViewTextBoxColumn.HeaderText = "Source Project";
            this.sourceProjectNameDataGridViewTextBoxColumn.Name = "sourceProjectNameDataGridViewTextBoxColumn";
            this.sourceProjectNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // targetProjectNameDataGridViewTextBoxColumn
            // 
            this.targetProjectNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.targetProjectNameDataGridViewTextBoxColumn.DataPropertyName = "TargetProjectName";
            this.targetProjectNameDataGridViewTextBoxColumn.HeaderText = "Target Project";
            this.targetProjectNameDataGridViewTextBoxColumn.Name = "targetProjectNameDataGridViewTextBoxColumn";
            this.targetProjectNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // projectLinkItemBindingSource
            // 
            this.projectLinkItemBindingSource.DataSource = typeof(ProjectLinkItem);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(765, 428);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(45, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonUnbind
            // 
            this.buttonUnbind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUnbind.Location = new System.Drawing.Point(714, 428);
            this.buttonUnbind.Name = "buttonUnbind";
            this.buttonUnbind.Size = new System.Drawing.Size(45, 23);
            this.buttonUnbind.TabIndex = 1;
            this.buttonUnbind.Text = "Unlink";
            this.buttonUnbind.UseVisualStyleBackColor = true;
            this.buttonUnbind.Click += new System.EventHandler(this.buttonUnbind_Click);
            // 
            // LinksEditorView
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 463);
            this.Controls.Add(this.buttonUnbind);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.gridProjectLinks);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LinksEditorView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Project Links";
            this.Load += new System.EventHandler(this.LinksEditorView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridProjectLinks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectLinkItemBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridProjectLinks;
        private System.Windows.Forms.BindingSource projectLinkItemBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn sourceProjectNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn targetProjectNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonUnbind;
    }
}