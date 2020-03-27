namespace WindowsFormsApplication3
{
    partial class ExplorerTreeView
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.treeViewEx1 = new System.Windows.Forms.TreeViewEx();
            this.SuspendLayout();
            // 
            // treeViewEx1
            // 
            this.treeViewEx1.arrowImageList = null;
            this.treeViewEx1.BackColor = System.Drawing.Color.White;
            this.treeViewEx1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewEx1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.treeViewEx1.Font = new System.Drawing.Font("微软雅黑", 9.7F);
            this.treeViewEx1.FullRowSelect = true;
            this.treeViewEx1.HideSelection = false;
            this.treeViewEx1.Indent = 14;
            this.treeViewEx1.Location = new System.Drawing.Point(0, 3);
            this.treeViewEx1.Name = "treeViewEx1";
            this.treeViewEx1.Size = new System.Drawing.Size(240, 311);
            this.treeViewEx1.TabIndex = 0;
            this.treeViewEx1.Text = "None";
            // 
            // ExplorerTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeViewEx1);
            this.Name = "ExplorerTreeView";
            this.Size = new System.Drawing.Size(441, 317);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeViewEx treeViewEx1;
    }
}
