namespace Zermos_Native_Windows;

partial class MainForm
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
        this.SuspendLayout();
        // 
        // MainForm
        // 
        this.ClientSize = new System.Drawing.Size(1200, 800);
        this.Name = "MainForm";
        this.Text = "Zermos";
        try
        {
            this.Icon = new System.Drawing.Icon(@"icon.ico");
        }
        catch
        {
            this.Icon = null;
        }
        this.ResumeLayout(false);

    }
    #endregion
}
