using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.Windows.Forms;

namespace GUIcw
{
    public partial class ReportViewerForm : Form
    {
        public ReportViewerForm()
        {
            InitializeComponent();
          
        }
        public CrystalReportViewer CrystalReportViewerControl
        {
            get { return crystalReportViewer1; }  
        }

        private void ReportViewerForm_Load(object sender, EventArgs e)
        {

        }
    }
}
