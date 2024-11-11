using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using Clinic_Management_System;

namespace ClinicManagementSystem
{
    public partial class ViewReservations : Form
    {
        int account_id , account_type;
        public ViewReservations(int id)
        {
            InitializeComponent();
            account_id = id;
        }

        SqlConnection con = new SqlConnection(Properties.Resources.connectionString);
        SqlCommand command;

        private void updateList()
        {
            command = con.CreateCommand();

            if (radioButton1.Checked)
                command.CommandText = "SELECT reservation_id, reservation_pationt_id, patient.account_name, reservation_secretary_id, secretary.account_name, reservation_visit_date, reservation_visit_slot, reservation_date FROM reservation, account as patient, account as secretary WHERE reservation_pationt_id = patient.account_id AND reservation_secretary_id = secretary.account_id AND reservation_visit_date=@date";
            else if (radioButton2.Checked)
                command.CommandText = "SELECT reservation_id, reservation_pationt_id, patient.account_name, reservation_secretary_id, secretary.account_name, reservation_visit_date, reservation_visit_slot, reservation_date FROM reservation, account as patient, account as secretary WHERE reservation_pationt_id = patient.account_id AND reservation_secretary_id = secretary.account_id AND (patient.account_name LIKE @query OR patient.account_phone LIKE @query OR reservation_id LIKE @query)";
            else
                command.CommandText = "SELECT reservation_id, reservation_pationt_id, patient.account_name, reservation_secretary_id, secretary.account_name, reservation_visit_date, reservation_visit_slot, reservation_date FROM reservation, account as patient, account as secretary WHERE reservation_pationt_id = patient.account_id AND reservation_secretary_id = secretary.account_id AND (patient.account_name LIKE @query OR patient.account_phone LIKE @query OR reservation_id LIKE @query) AND reservation_visit_date=@date";

            command.Parameters.AddWithValue("@date", dateTimePicker2.Value.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@query", textBox8.Text + "%");

            con.Open();

            SqlDataReader reader = command.ExecuteReader();
            listBox1.Items.Clear();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int patient_id = reader.GetInt32(1);
                string patient_name = reader.GetString(2);
                int secretary_id = reader.GetInt32(3);
                string secretary_name = reader.GetString(4);
                DateTime visit_date = new DateTime();
                DateTime.TryParse(reader.GetValue(5).ToString(), out visit_date);

                int slot = reader.GetInt32(6);

                DateTime date = new DateTime();
                DateTime.TryParse(reader.GetValue(7).ToString(), out date);

                listBox1.Items.Add(new reservation(id, patient_id, patient_name, secretary_id, secretary_name, slot, visit_date, date));
            }

            con.Close();
        }

        private void ViewReservations_Load(object sender, EventArgs e)
        {
            updateList();
            command = con.CreateCommand();
            command.CommandText = "SELECT account_type FROM account WHERE account_id = @id";
            command.Parameters.AddWithValue("@id", account_id);
            con.Open();
            account_type = (int)command.ExecuteScalar();
            con.Close();
            if (account_type == 1)
            {
                
            }
            else
            {
                button9.Visible = false; // Make button1 invisible
                button6.Visible = false;
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            updateList();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            updateList();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            updateList();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            updateList();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            updateList();
        }


        private void button10_Click(object sender, EventArgs e)
        {
            // Display a confirmation message box
            DialogResult result = MessageBox.Show("Voulez-vous vraiment fermer l'application ?",
                "Confirmation de fermeture", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // If the user confirms, close the application
            if (result == DialogResult.Yes)
            {
                // Close the main form, which will close the application
                Application.Exit();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (this.account_type == 0)
            {
                //Secratery Pan
                Hide();
                SecretaryPanel secretaryPanel = new SecretaryPanel(this.account_id);
                secretaryPanel.ShowDialog();
                Show();
            }
            else if (this.account_type == 1)
            {
                Hide();
                DoctorPanel doctorPanel = new DoctorPanel(this.account_id);
                doctorPanel.ShowDialog();
                Show();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Hide();
            EditProfile editProfile = new EditProfile(this.account_id);
            editProfile.ShowDialog();
            Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Hide();
            PatientsProfiles patientProfiles = new PatientsProfiles(this.account_id);
            patientProfiles.ShowDialog();
            Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Hide();
            CreateReservation createReservation = new CreateReservation(account_id);
            createReservation.ShowDialog();
            Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Hide();
            Medicaments medicament = new Medicaments(account_id);
            medicament.ShowDialog();
            Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Hide();
            Prescriptions pres = new Prescriptions(account_id);
            pres.ShowDialog();
            Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            if (listBox1.SelectedIndex < 0 || listBox1.SelectedIndex >= listBox1.Items.Count)
            {
                MessageBox.Show("Please select a reservation!");
                return;
            }

            reservation res = (reservation)listBox1.SelectedItem;

            Hide();
            EditReservation editReservation = new EditReservation(res);
            editReservation.ShowDialog();
            updateList();
            Show();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0 || listBox1.SelectedIndex >= listBox1.Items.Count)
            {
                MessageBox.Show("Please select a reservation!");
                return;
            }

            reservation res = (reservation)listBox1.SelectedItem;
            Hide();
            Visits visits = new Visits(account_id, res.patient.Key, res.id);
            visits.ShowDialog();
            Show();
        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            updateForm();
        }

        private void updateForm()
        {
            if (listBox1.SelectedIndex < 0 || listBox1.SelectedIndex >= listBox1.Items.Count)
            {
                MessageBox.Show("Please select a reservation!");
                return;
            }

            reservation res = (reservation)listBox1.SelectedItem;
            textBox4.Text = res.id.ToString();
            textBox5.Text = res.patient.ToString();
            textBox10.Text = res.secretary.ToString();
            textBox9.Text = res.visit_date.Date.ToString();
            textBox11.Text = utils.getSlots()[res.slot];
            textBox7.Text = res.date.ToString();

            if ((account_type == 0 || account_type==1) && res.visit_date >= DateTime.Today)
                button1.Enabled = true;
            else
                button1.Enabled = false;

            if (account_type == 1)
                button2.Enabled = true; 
            else
                button2.Enabled = false;
        }
    }
}
