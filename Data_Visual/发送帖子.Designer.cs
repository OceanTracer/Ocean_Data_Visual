namespace Data_Visual
{
    partial class 发送帖子
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(发送帖子));
            this.richTextBoxTitle = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.richTextBoxContent = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbSection = new System.Windows.Forms.ComboBox();
            this.buttonLaunch = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBoxTitle
            // 
            this.richTextBoxTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxTitle.Font = new System.Drawing.Font("张海山锐线体简", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBoxTitle.Location = new System.Drawing.Point(99, 92);
            this.richTextBoxTitle.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBoxTitle.Name = "richTextBoxTitle";
            this.richTextBoxTitle.Size = new System.Drawing.Size(348, 32);
            this.richTextBoxTitle.TabIndex = 0;
            this.richTextBoxTitle.Text = "请输入标题";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("张海山锐线体简", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(37, 97);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "标题";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("张海山锐线体简", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(37, 137);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "内容";
            // 
            // richTextBoxContent
            // 
            this.richTextBoxContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxContent.Font = new System.Drawing.Font("张海山锐线体简", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBoxContent.Location = new System.Drawing.Point(99, 137);
            this.richTextBoxContent.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBoxContent.Name = "richTextBoxContent";
            this.richTextBoxContent.Size = new System.Drawing.Size(348, 178);
            this.richTextBoxContent.TabIndex = 3;
            this.richTextBoxContent.Text = "请输入内容";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("张海山锐线体简", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(37, 54);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "版块";
            // 
            // cbSection
            // 
            this.cbSection.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbSection.Font = new System.Drawing.Font("张海山锐线体简", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbSection.FormattingEnabled = true;
            this.cbSection.Items.AddRange(new object[] {
            "知道问答",
            "科普讨论",
            "交流杂谈",
            "版务意见"});
            this.cbSection.Location = new System.Drawing.Point(99, 53);
            this.cbSection.Margin = new System.Windows.Forms.Padding(2);
            this.cbSection.Name = "cbSection";
            this.cbSection.Size = new System.Drawing.Size(92, 25);
            this.cbSection.TabIndex = 5;
            this.cbSection.Text = "请选择版块";
            // 
            // buttonLaunch
            // 
            this.buttonLaunch.BackColor = System.Drawing.Color.White;
            this.buttonLaunch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonLaunch.Font = new System.Drawing.Font("张海山锐线体简", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonLaunch.Location = new System.Drawing.Point(482, 214);
            this.buttonLaunch.Margin = new System.Windows.Forms.Padding(2);
            this.buttonLaunch.Name = "buttonLaunch";
            this.buttonLaunch.Size = new System.Drawing.Size(88, 35);
            this.buttonLaunch.TabIndex = 6;
            this.buttonLaunch.Text = "发送";
            this.buttonLaunch.UseVisualStyleBackColor = false;
            this.buttonLaunch.Click += new System.EventHandler(this.buttonLaunch_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.White;
            this.buttonCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonCancel.Font = new System.Drawing.Font("张海山锐线体简", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonCancel.Location = new System.Drawing.Point(482, 279);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(88, 35);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // 发送帖子
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Data_Visual.Properties.Resources.bg8;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(600, 360);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonLaunch);
            this.Controls.Add(this.cbSection);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBoxContent);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.richTextBoxTitle);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "发送帖子";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "发送帖子";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxTitle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox richTextBoxContent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbSection;
        private System.Windows.Forms.Button buttonLaunch;
        private System.Windows.Forms.Button buttonCancel;
    }
}