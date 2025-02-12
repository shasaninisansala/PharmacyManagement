using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUIcw
{
    public partial class BillView : Form
    {
        public BillView()
        {
            InitializeComponent();
        }

        private void LoadBills()
        {

            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(cs))
            {
                try
                {
                    connection.Open();
                    // Query to fetch bill data
                    string billQuery = "SELECT [bill_id], [bill_date], [cus_Name], [contact], [net_Amount], [cash], [balance] FROM [dbo].[tblbill]";
                    SqlDataAdapter billAdapter = new SqlDataAdapter(billQuery, connection);
                    DataTable billTable = new DataTable();
                    billAdapter.Fill(billTable);
                    dgvbill.DataSource = billTable;

                    // Set column auto sizing for better display
                    dgvbill.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                  

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading bill data: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadBills();
        }

        private void dgvbill_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvbill.SelectedRows.Count > 0)
            {
                string billId = dgvbill.SelectedRows[0].Cells[0].Value.ToString();
                LoadBillItems(billId);
            }
        }

        private void LoadBillItems(string billId)
        {
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(cs))
            {
                try
                {
                    connection.Open();
                    // Query to fetch bill item details for a specific bill
                    string billItemQuery = $"SELECT [billitem_id], [bill_id], [item_Name], [price], [qty], [total] FROM [dbo].[tblbill_item] WHERE [bill_id] = '{billId}'";
                    SqlDataAdapter billItemAdapter = new SqlDataAdapter(billItemQuery, connection);
                    DataTable billItemTable = new DataTable();
                    billItemAdapter.Fill(billItemTable);
                    dgvbillitem.DataSource = billItemTable;

                    // Set column auto sizing for better display
                    dgvbillitem.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading bill item data: " + ex.Message);
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
