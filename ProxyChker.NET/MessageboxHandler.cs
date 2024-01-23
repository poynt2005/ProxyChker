using ProxyChker.NET.DataDefs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChker.NET
{
    public partial class Form1 : Form
    {
        private ResponseMessageBox ProcessMessageBox(string mbType, string title, string message)
        {
            ResponseMessageBox response = new();
            switch (mbType)
            {
                case "Alert":
                    {
                        var result = MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        response.IsSuccess = true;
                        response.IsConfirm = true;
                        response.Target = "MessageBox_Alert";
                        break;
                    }
                case "Confirm":
                    {
                        var result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        response.IsSuccess = true;
                        response.IsConfirm = result == DialogResult.Yes;
                        response.Target = "MessageBox_Confirm";
                        break;
                    }
                case "Info":
                    {
                        var result = MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        response.IsSuccess = true;
                        response.IsConfirm = true;
                        response.Target = "MessageBox_Info";
                        break;
                    }
            }

            return response;
        }
    }
}
