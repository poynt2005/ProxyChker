namespace ProxyChker.NET;

partial class Form1
{
    private static readonly int ClientWidth = 400;
    private static readonly int ClientHeight = 400;
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        webviewComponent = new Microsoft.Web.WebView2.WinForms.WebView2();
        ((System.ComponentModel.ISupportInitialize)webviewComponent).BeginInit();
        SuspendLayout();
        // 
        // webviewComponent
        // 
        webviewComponent.AllowExternalDrop = true;
        webviewComponent.CreationProperties = null;
        webviewComponent.DefaultBackgroundColor = Color.White;
        webviewComponent.Location = new Point(0, 0);
        webviewComponent.Name = "webviewComponent";
        webviewComponent.Size = new Size(ClientWidth, ClientHeight);
        webviewComponent.TabIndex = 0;
        webviewComponent.ZoomFactor = 1D;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(9F, 19F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(ClientWidth, ClientHeight);
        Controls.Add(webviewComponent);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "Form1";
        Text = "ProxyChker.NET";
        Load += Form1_Load;
        ((System.ComponentModel.ISupportInitialize)webviewComponent).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Microsoft.Web.WebView2.WinForms.WebView2 webviewComponent;
}
