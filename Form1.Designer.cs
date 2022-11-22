namespace WindowsLocalNetwork
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SplitFiles = new System.Windows.Forms.Button();
            this.MergeFiles = new System.Windows.Forms.Button();
            this.SendFiles = new System.Windows.Forms.Button();
            this.ReceiveFiles = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(24, 67);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(122, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(24, 112);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(229, 134);
            this.listBox1.TabIndex = 1;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Lucida Sans Typewriter", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(33, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Device";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(275, 187);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(195, 16);
            this.progressBar1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(272, 150);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Loading file";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(341, 236);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 5;
            // 
            // SplitFiles
            // 
            this.SplitFiles.Location = new System.Drawing.Point(24, 280);
            this.SplitFiles.Name = "SplitFiles";
            this.SplitFiles.Size = new System.Drawing.Size(229, 23);
            this.SplitFiles.TabIndex = 6;
            this.SplitFiles.Text = "Split file";
            this.SplitFiles.UseVisualStyleBackColor = true;
            this.SplitFiles.Click += new System.EventHandler(this.SplitFiles_Click);
            // 
            // MergeFiles
            // 
            this.MergeFiles.Location = new System.Drawing.Point(24, 320);
            this.MergeFiles.Name = "MergeFiles";
            this.MergeFiles.Size = new System.Drawing.Size(229, 23);
            this.MergeFiles.TabIndex = 7;
            this.MergeFiles.Text = "Merge files";
            this.MergeFiles.UseVisualStyleBackColor = true;
            this.MergeFiles.Click += new System.EventHandler(this.MergeFiles_Click);
            // 
            // SendFiles
            // 
            this.SendFiles.Location = new System.Drawing.Point(24, 361);
            this.SendFiles.Name = "SendFiles";
            this.SendFiles.Size = new System.Drawing.Size(229, 23);
            this.SendFiles.TabIndex = 8;
            this.SendFiles.Text = "Send files";
            this.SendFiles.UseVisualStyleBackColor = true;
            this.SendFiles.Click += new System.EventHandler(this.SendFiles_Click);
            // 
            // ReceiveFiles
            // 
            this.ReceiveFiles.Location = new System.Drawing.Point(24, 402);
            this.ReceiveFiles.Name = "ReceiveFiles";
            this.ReceiveFiles.Size = new System.Drawing.Size(229, 23);
            this.ReceiveFiles.TabIndex = 9;
            this.ReceiveFiles.Text = "Receive files";
            this.ReceiveFiles.UseVisualStyleBackColor = true;
            this.ReceiveFiles.Click += new System.EventHandler(this.ReceiveFiles_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 450);
            this.Controls.Add(this.ReceiveFiles);
            this.Controls.Add(this.SendFiles);
            this.Controls.Add(this.MergeFiles);
            this.Controls.Add(this.SplitFiles);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Files transfer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button SplitFiles;
        private System.Windows.Forms.Button MergeFiles;
        private System.Windows.Forms.Button SendFiles;
        private System.Windows.Forms.Button ReceiveFiles;
    }
}

