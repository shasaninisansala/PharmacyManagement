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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;


namespace GUIcw
{
    public partial class bill : Form
    {
        public bill()
        {
            InitializeComponent();
            txtBillId.Text = GenerateBillNumber().ToString();
        }
        private int GenerateBillNumber()
        {

            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";

            int billNumber = 1;
            string query = "SELECT MAX(bill_id) FROM tblbill";
            using (SqlConnection conn = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    billNumber = Convert.ToInt32(result) + 1;
                }
            }
            return billNumber;
        }


        private void UpdateNetAmount()
        {
            decimal netAmount = 0;
            foreach (DataGridViewRow row in dgvBill.Rows)
            {
                if (row.Cells["Total"].Value != null)
                {
                    netAmount += Convert.ToDecimal(row.Cells["Total"].Value);
                }
            }
            txtTotal.Text = netAmount.ToString("F2");
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";

            // Check if an item is selected before using SelectedItem
            if (cmbType.SelectedItem == null)
            {
                MessageBox.Show("Please select a valid drug type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedItem = cmbType.SelectedItem.ToString();
            string query = "SELECT price, qty FROM tblstocks WHERE name = @drug_name";
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@drug_name", selectedItem);
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        txtPrice.Text = dr["price"].ToString();
                        txtAQty.Text = dr["qty"].ToString();
                    }
                }
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql = "SELECT DISTINCT name FROM tblstocks";
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    using (SqlDataReader dr = com.ExecuteReader())
                    {
                        cmbType.Items.Clear();
                        while (dr.Read())
                        {
                            cmbType.Items.Add(dr["name"].ToString());
                        }
                    }
                }
            }
        }

        private void btnAdditem_Click(object sender, EventArgs e)
        {
            string itemName = cmbType.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(itemName))
            {
                MessageBox.Show("Please select a drug type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate if available quantity is entered
            if (string.IsNullOrWhiteSpace(txtAQty.Text) || !int.TryParse(txtAQty.Text, out int availableQty) || availableQty <= 0)
            {
                MessageBox.Show("Invalid available quantity for the selected item.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate if entered quantity is a valid positive number
            int enteredQuantity = Convert.ToInt32(txtQty.Value);
            if (enteredQuantity <= 0)
            {
                MessageBox.Show("Please enter a positive quantity.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            decimal price = Convert.ToDecimal(txtPrice.Text);
            int quantity = Convert.ToInt32(txtQty.Value);
            if (quantity > Convert.ToInt32(txtAQty.Text))
            {
                MessageBox.Show(" Unavailable.Out of Stock", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            decimal total = price * quantity;
            dgvBill.Rows.Add(itemName, price, quantity, total);
            UpdateNetAmount();

           
            txtPrice.Clear();
            txtAQty.Clear();
            txtQty.Value = 0;


        }

        private void btnRmv_Click(object sender, EventArgs e)
        {
            if (dgvBill.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvBill.SelectedRows)
                {
                    dgvBill.Rows.Remove(row);
                }
                UpdateNetAmount();
            }
            else
            {
                MessageBox.Show("Please select a row to remove.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter the customer name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidContactNumber(txtContact.Text))
            {
                MessageBox.Show("Please enter a valid 10-digit contact number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvBill.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one item to the bill.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtcash.Text, out decimal cash) || !decimal.TryParse(txtTotal.Text, out decimal netAmount) || cash < netAmount)
            {
                MessageBox.Show("INvalid Cash amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int billId = int.Parse(txtBillId.Text);
            SaveBill();
            SaveBillItems(billId);
            MessageBox.Show("Bill has been saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Store the saved Bill ID for printing
            lastSavedBillId = billId;

            //clear fields
            txtBillId.Text = GenerateBillNumber().ToString();
            txtName.Clear();
            txtContact.Clear();
            txtTotal.Clear();
            txtcash.Clear();
            txtbal.Clear();
            dgvBill.Rows.Clear();
            LoadCustomerDetails();
        }
        private int lastSavedBillId;

        private bool IsValidContactNumber(string contact)
        {
            return contact.All(char.IsDigit) && contact.Length == 10;
        }
        private void SaveBill()
        {
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";

            string query = "INSERT INTO tblbill (bill_id, bill_date, cus_name, contact, net_Amount, cash, balance) VALUES (@bill_id, @bill_date, @customer_name, @customer_phone, @total_amount, @cash, @balance)";
            using (SqlConnection conn = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@bill_id", txtBillId.Text);
                cmd.Parameters.AddWithValue("@bill_date", txtDate.Value);
                cmd.Parameters.AddWithValue("@customer_name", txtName.Text);
                cmd.Parameters.AddWithValue("@customer_phone", txtContact.Text);
                cmd.Parameters.AddWithValue("@total_amount", txtTotal.Text);
                cmd.Parameters.AddWithValue("@cash", txtcash.Text);
                cmd.Parameters.AddWithValue("@balance", txtbal.Text);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        private void SaveBillItems(int billId)
        {
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";
            string query = "INSERT INTO tblbill_item (bill_id, item_name, price, qty, total) VALUES (@bill_id, @item_name, @price, @quantity, @total)";

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();
                foreach (DataGridViewRow row in dgvBill.Rows)
                {
                    if (row.Cells["ItemName"].Value != null)  
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@bill_id", billId);
                            cmd.Parameters.AddWithValue("@item_name", row.Cells["ItemName"].Value.ToString());
                            cmd.Parameters.AddWithValue("@price", Convert.ToDecimal(row.Cells["Price"].Value));
                            cmd.Parameters.AddWithValue("@quantity", Convert.ToInt32(row.Cells["Quantity"].Value));
                            cmd.Parameters.AddWithValue("@total", Convert.ToDecimal(row.Cells["Total"].Value));

                            cmd.ExecuteNonQuery();
                        }
                        UpdateStockQuantity(row.Cells["ItemName"].Value.ToString(), Convert.ToInt32(row.Cells["Quantity"].Value));
                    }
                }
            }
        }
        private void LoadCustomerDetails()
        {
                string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";
                string query = @"
            SELECT 
                b.bill_date AS BillDate, 
                b.cus_name AS CustomerName, 
                b.contact AS Contact, 
                COUNT(bi.item_name) AS TotalItems, 
                SUM(bi.total) AS TotalAmount 
            FROM 
                tblbill b 
            JOIN 
                tblbill_item bi ON b.bill_id = bi.bill_id 
            GROUP BY 
                b.bill_date, b.cus_name, b.contact";

                using (SqlConnection conn = new SqlConnection(cs))
                {
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvcusdetails.DataSource = dt;
                }
        }
        private void UpdateStockQuantity(string itemName, int soldQuantity)
        {
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";

            string updateQuery = "UPDATE tblstocks SET qty = qty - @soldQuantity WHERE name = @item_name";
            using (SqlConnection conn = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@soldQuantity", soldQuantity);
                cmd.Parameters.AddWithValue("@item_name", itemName);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
        }

        private DataTable GetBillData(int billId)
        {
            string cs = "Data Source=DESKTOP-ADM7KIF;Initial Catalog=SNPharmacydb;Integrated Security=True";
            DataTable dt = new DataTable();

            string query = @"
            SELECT 
                b.bill_id, 
                b.bill_date, 
                b.cus_name, 
                b.contact, 
                b.net_Amount, 
                b.cash, 
                b.balance, 
                bi.item_name, 
                bi.price, 
                bi.qty, 
                bi.total 
            FROM 
                tblbill b
            INNER JOIN 
                tblbill_item bi ON b.bill_id = bi.bill_id
            WHERE 
                b.bill_id = @billId";

            using (SqlConnection conn = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@billId", billId);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            return dt;
        }



        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (lastSavedBillId == 0)
                {
                    MessageBox.Show("No recent bill found. Please save a bill before printing.", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                CrystalReport1 myReport = new CrystalReport1();
                DataTable billData = GetBillData(lastSavedBillId);

                if (billData == null || billData.Rows.Count == 0)
                {
                    MessageBox.Show("No data found for the recent Bill ID.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Console.WriteLine("Bill data", billData.Rows.Count);
                myReport.SetDataSource(billData);

                // Use a form to display the Crystal Report Viewer
                using (ReportViewerForm reportForm = new ReportViewerForm())
                {
                    reportForm.CrystalReportViewerControl.ReportSource = myReport;  // Assuming CrystalReportViewerControl is the name of the viewer
                    reportForm.WindowState = FormWindowState.Maximized;  // Optional
                    reportForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtcash_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtcash.Text, out decimal cash) && decimal.TryParse(txtTotal.Text, out decimal netAmount))
            {
                if (cash >= netAmount)
                {
                    txtbal.Text = (cash - netAmount).ToString("F2");
                }
                else
                {
                    txtbal.Text = "0.00";
                }
            }
            else
            {
                txtbal.Text = "0.00";
            }
        }

        private void txtBillId_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtBillId.Text = GenerateBillNumber().ToString();  // Generate a new bill number
            txtName.Clear();
            txtContact.Clear();
            txtTotal.Clear();
            txtcash.Clear();
            txtbal.Clear();
            cmbType.SelectedIndex = -1;  // Reset the combo box
            txtPrice.Clear();
            txtAQty.Clear();
            txtQty.Value = 0;
            dgvBill.Rows.Clear();
        }
    }
}
