namespace ILInject
{
    partial class ILInject
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button_inject = new System.Windows.Forms.Button();
            this.textBox_getpath = new System.Windows.Forms.TextBox();
            this.button_getpath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(776, 401);
            this.textBox1.TabIndex = 0;
            // 
            // button_inject
            // 
            this.button_inject.Location = new System.Drawing.Point(713, 419);
            this.button_inject.Name = "button_inject";
            this.button_inject.Size = new System.Drawing.Size(75, 23);
            this.button_inject.TabIndex = 1;
            this.button_inject.Text = "Inject";
            this.button_inject.UseVisualStyleBackColor = true;
            this.button_inject.Click += new System.EventHandler(this.button_inject_Click);
            // 
            // textBox_getpath
            // 
            this.textBox_getpath.Location = new System.Drawing.Point(13, 420);
            this.textBox_getpath.Name = "textBox_getpath";
            this.textBox_getpath.ReadOnly = true;
            this.textBox_getpath.Size = new System.Drawing.Size(590, 21);
            this.textBox_getpath.TabIndex = 2;
            this.textBox_getpath.Text = "选择文件";
            // 
            // button_getpath
            // 
            this.button_getpath.Location = new System.Drawing.Point(610, 420);
            this.button_getpath.Name = "button_getpath";
            this.button_getpath.Size = new System.Drawing.Size(37, 23);
            this.button_getpath.TabIndex = 3;
            this.button_getpath.Text = "...";
            this.button_getpath.UseVisualStyleBackColor = true;
            this.button_getpath.Click += new System.EventHandler(this.button_getpath_Click);
            // 
            // ILInject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_getpath);
            this.Controls.Add(this.textBox_getpath);
            this.Controls.Add(this.button_inject);
            this.Controls.Add(this.textBox1);
            this.Name = "ILInject";
            this.Text = "ILInject";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button_inject;
        private System.Windows.Forms.TextBox textBox_getpath;
        private System.Windows.Forms.Button button_getpath;
    }
}

