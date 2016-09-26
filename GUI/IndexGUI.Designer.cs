namespace Grimoire.GUI
{
    partial class IndexGUI
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
            this.indexGrid = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.indexGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // indexGrid
            // 
            this.indexGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.indexGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indexGrid.Location = new System.Drawing.Point(0, 0);
            this.indexGrid.Name = "indexGrid";
            this.indexGrid.Size = new System.Drawing.Size(655, 424);
            this.indexGrid.TabIndex = 0;
            // 
            // IndexGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 424);
            this.Controls.Add(this.indexGrid);
            this.Name = "IndexGUI";
            this.Text = "Viewing index... data.000";
            ((System.ComponentModel.ISupportInitialize)(this.indexGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView indexGrid;
    }
}