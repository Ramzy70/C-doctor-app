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
using ClinicManagementSystem;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Clinic_Management_System
{
    public partial class CreateReservation : Form
    {
        int secretary_id, account_type;
        public CreateReservation(int id)
        {
            InitializeComponent();
            this.secretary_id = id;
            SqlCommand cmd = con.CreateCommand();
            con.Open();
            cmd.CommandText = "SELECT account_id , account_type FROM account WHERE account_user_id = @user_id";
            cmd.Parameters.AddWithValue("@user_id", this.secretary_id);
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                if (reader.Read())
                {
                    this.account_type = reader.GetInt32(1);
                    con.Close();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                con.Close(); // Ensure the connection is closed
            }
        }
        SqlConnection con = new SqlConnection(ClinicManagementSystem.Properties.Resources.connectionString);
        SqlCommand command;

        private void updateList(string query)
        {
            command = con.CreateCommand();
            command.CommandText = "SELECT account_id, account_name, account_type FROM account WHERE account_type=2 AND (account_name LIKE @query OR account_phone LIKE @query)";
            command.Parameters.AddWithValue("@query", query + "%");

            con.Open();
            listBox1.Items.Clear();

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
                listBox1.Items.Add(new account(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));

            con.Close();
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            updateList(textBox1.Text);
        }

        private void updateSlots()
        {
            command = con.CreateCommand();
            command.CommandText = "SELECT reservation_visit_slot FROM reservation WHERE reservation_visit_date=@date";
            command.Parameters.AddWithValue("@date", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
            con.Open();

            SqlDataReader reader = command.ExecuteReader();

            Dictionary<int, string> slots = utils.getSlots();

            while (reader.Read())
            {
                slots.Remove(reader.GetInt32(0));
            }
            comboBox1.Items.Clear();
            foreach (object slot in slots.ToArray())
                comboBox1.Items.Add(slot);

            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
            con.Close();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            updateSlots();
        }

        private void CreateReservation_Load_1(object sender, EventArgs e)
        {
            updateList("");
            updateSlots();
            dateTimePicker1.MinDate = DateTime.Today;
            command = con.CreateCommand();
            command.CommandText = "SELECT account_type FROM account WHERE account_id = @id";
            command.Parameters.AddWithValue("@id", this.secretary_id);
            con.Open();
            account_type = (int)command.ExecuteScalar();
            con.Close();
            if (account_type == 1)
            {

            }
            else
            {
                button8.Visible = false; // Make button1 invisible
                button6.Visible = false;
            }
        }

        private void button9_Click(object sender, EventArgs e)
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
                SecretaryPanel secretaryPanel = new SecretaryPanel(this.secretary_id);
                secretaryPanel.ShowDialog();
                Show();
            }
            else if (this.account_type == 1)
            {
                Hide();
                DoctorPanel doctorPanel = new DoctorPanel(this.secretary_id);
                doctorPanel.ShowDialog();
                Show();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Hide();
            EditProfile editProfile = new EditProfile(secretary_id);
            editProfile.ShowDialog();
            Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
            ViewReservations viewReservations = new ViewReservations(secretary_id);
            viewReservations.ShowDialog();
            Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Hide();
            Medicaments medicament = new Medicaments(secretary_id);
            medicament.ShowDialog();
            Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Hide();
            Prescriptions pres = new Prescriptions(secretary_id);
            pres.ShowDialog();
            Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {


            //Inputs Validation
            if (listBox1.SelectedIndex < 0 || listBox1.SelectedIndex >= listBox1.Items.Count)
            {
                MessageBox.Show("Please select a patient!");
                return;
            }
            if (comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a slot!");
                return;
            }

            //Perform the reservation
            int patient_id = ((account)listBox1.SelectedItem).getId();
            int slot = ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key;

            command = con.CreateCommand();
            command.CommandText = "INSERT INTO reservation (reservation_secretary_id, reservation_pationt_id, reservation_visit_date, reservation_visit_slot, reservation_date) VALUES (@secretary_id, @patient_id, @visit_date, @visit_slot, @date)";
            command.Parameters.AddWithValue("@secretary_id", secretary_id);
            command.Parameters.AddWithValue("@patient_id", patient_id);
            command.Parameters.AddWithValue("@visit_date", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@visit_slot", slot);
            command.Parameters.AddWithValue("@date", DateTime.Now);

            con.Open();

            if (command.ExecuteNonQuery() > 0)
            {
                //We successfully performed the reservation
                command.CommandText = "SELECT reservation_id FROM reservation WHERE reservation_visit_date=@visit_date AND reservation_visit_slot=@visit_slot";
                int reservation_id = (int)command.ExecuteScalar();

                MessageBox.Show("Reservation was made!");
                MessageBox.Show("Reservation ID:" + reservation_id.ToString());
            }
            else
                MessageBox.Show("Failed to perform the reservation!");

            con.Close();
            updateSlots();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Hide();
            PatientsProfiles patientProfiles = new PatientsProfiles(this.secretary_id);
            patientProfiles.ShowDialog();
            Show();
        }
    }
}
