using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUIcw
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void billToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (SaleViewerForm reportViewer = new SaleViewerForm())
            {
                reportViewer.WindowState = FormWindowState.Maximized;
                reportViewer.ShowDialog();
            }
        }

        private void billToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bill it = new bill();
            it.MdiParent = this;
            it.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void registerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stock it = new stock();
            it.MdiParent = this;
            it.Show();
        }

        private void stockReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (StockViewerForm reportViewer = new StockViewerForm())
            {
                reportViewer.WindowState = FormWindowState.Maximized;
                reportViewer.ShowDialog();
            }
        }

        private void stockDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewStock it = new ViewStock();
            it.MdiParent = this;
            it.Show();
        }

        private void billDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BillView it = new BillView();
            it.MdiParent = this;
            it.Show();
        }
    }
}
