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
using Clinic_Management_System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace ClinicManagementSystem
{
    public partial class PatientsProfiles : Form
    {
        int user_id, account_type;
        public PatientsProfiles(int id)
        {
            InitializeComponent();
            user_id = id;
        }
        SqlConnection con = new SqlConnection(ClinicManagementSystem.Properties.Resources.connectionString);
        SqlCommand command;
        private void updateList(string query)
        {
            command = con.CreateCommand();
            command.CommandText = "SELECT account_id, account_name, account_type FROM account WHERE account_type=2 AND (account_name LIKE @query OR account_phone LIKE @query)";
            command.Parameters.AddWithValue("@query", query + "%");
            con.Open();

            SqlDataReader reader = command.ExecuteReader();
            listBox1.Items.Clear();

            while (reader.Read())
            {
                listBox1.Items.Add(new account(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
            }

            con.Close();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            updateList(textBox6.Text);
        }

        private void PatientsProfiles_Load(object sender, EventArgs e)
        {
            updateList("");
            updateSlots();
            command = con.CreateCommand();
            command.CommandText = "SELECT account_type FROM account WHERE account_id = @id";
            command.Parameters.AddWithValue("@id", user_id);
            con.Open();
            account_type = (int)command.ExecuteScalar();
            con.Close();
            if (account_type == 1) { 
                button3.Enabled = true;
            }
            else
            {
                button10.Visible = false; // Make button1 invisible
                button6.Visible = false;
                button3.Enabled = false; // Make button2 invisible
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0 || listBox1.SelectedIndex >= listBox1.Items.Count)
                return;
            int account_id = ((account)listBox1.SelectedItem).getId();
            command = con.CreateCommand();
            command.CommandText = "SELECT account_name, account_dob, account_phone, account_note, account_creation_date FROM account WHERE account_id=@id";
            command.Parameters.AddWithValue("@id", account_id);

            con.Open();

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                textBox4.Text = account_id.ToString();
                textBox7.Text = reader.GetString(0);

                DateTime dob = new DateTime();
                if (DateTime.TryParse(reader.GetValue(1).ToString(), out dob))
                    dateTimePicker1.Value = dob;
                textBox9.Text = reader.GetString(2);
                textBox11.Text = reader.GetString(3);
                textBox12.Text = reader.GetValue(4).ToString();
            }

            con.Close();
        }

        private void updateSlots()
        {
            command = con.CreateCommand();
            command.CommandText = "SELECT reservation_visit_slot FROM reservation WHERE reservation_visit_date=@date";
            command.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
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

        private void button11_Click(object sender, EventArgs e)
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
                SecretaryPanel secretaryPanel = new SecretaryPanel(this.user_id);
                secretaryPanel.ShowDialog();
                Show();
            }
            else if (this.account_type == 1)
            {
                Hide();
                DoctorPanel doctorPanel = new DoctorPanel(this.user_id);
                doctorPanel.ShowDialog();
                Show();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Hide();
            EditProfile editProfile = new EditProfile(user_id);
            editProfile.ShowDialog();
            Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Hide();
            CreateReservation createReservation = new CreateReservation(user_id);
            createReservation.ShowDialog();
            Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Hide();
            ViewReservations viewReservations = new ViewReservations(user_id);
            viewReservations.ShowDialog();
            Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Hide();
            Medicaments medicament = new Medicaments(user_id);
            medicament.ShowDialog();
            Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Hide();
            Prescriptions pres = new Prescriptions(user_id);
            pres.ShowDialog();
            Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            //Inputs Validation
            if (textBox1.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Please check the inputs!");
                return;
            }

            //Account Creation
            command = con.CreateCommand();
            command.CommandText = "INSERT INTO account (account_name, account_phone, account_note, account_type, account_creation_date) VALUES (@name, @phone, @notes, 2, @date)";
            command.Parameters.AddWithValue("@name", textBox3.Text);
            command.Parameters.AddWithValue("@phone", textBox1.Text);
            command.Parameters.AddWithValue("@notes", textBox13.Text);
            command.Parameters.AddWithValue("@date", DateTime.Now);
            con.Open();

            if (command.ExecuteNonQuery() > 0)
                MessageBox.Show("Account was created!");
            else
                MessageBox.Show("Failed to create the account!");
            con.Close();
            updateList("");
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

            //Inputs Validation
            if (textBox7.Text == "" || textBox9.Text == "")
            {
                MessageBox.Show("Please check the inputs!");
                return;
            }

            //Editing the account
            command = con.CreateCommand();
            command.CommandText = "UPDATE account SET account_name = @name, account_phone = @phone, account_dob = @dob, account_note = @notes WHERE account_id = @id";
            command.Parameters.AddWithValue("@name", textBox7.Text);
            command.Parameters.AddWithValue("@phone", textBox9.Text);
            command.Parameters.AddWithValue("@dob", dateTimePicker1.Value);
            command.Parameters.AddWithValue("@notes", textBox11.Text);
            command.Parameters.AddWithValue("@id", textBox4.Text);

            con.Open();

            if (command.ExecuteNonQuery() > 0)
                MessageBox.Show("Account was updated!");
            else
                MessageBox.Show("Failed to updated the account!");

            con.Close();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {

            // Inputs Validation
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

            // Perform the reservation
            int patient_id = ((account)listBox1.SelectedItem).getId();
            int slot = ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key;

            using (SqlConnection con = new SqlConnection(Properties.Resources.connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandText = "INSERT INTO reservation (reservation_secretary_id, reservation_pationt_id, reservation_visit_date, reservation_visit_slot, reservation_date) VALUES (@secretary_id, @patient_id, @visit_date, @visit_slot, @date)";
                    command.Parameters.AddWithValue("@secretary_id", user_id);
                    command.Parameters.AddWithValue("@patient_id", patient_id);
                    command.Parameters.AddWithValue("@visit_date", DateTime.Now.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@visit_slot", slot);
                    command.Parameters.AddWithValue("@date", DateTime.Now);

                    con.Open();
                    int reservation_id;

                    // Inside the button3_Click event handler after successful reservation
                    if (command.ExecuteNonQuery() > 0)
                    {
                        // We successfully performed the reservation
                        command.CommandText = "SELECT reservation_id FROM reservation WHERE reservation_visit_date=@visit_date AND reservation_visit_slot=@visit_slot";
                        reservation_id = (int)command.ExecuteScalar();

                        MessageBox.Show("Reservation was made!");
                        MessageBox.Show("Reservation ID:" + reservation_id.ToString());

                        // Retrieve secretary and patient names
                        string secretary_name = string.Empty;
                        string patient_name = string.Empty;

                        // Retrieve secretary name
                        command.CommandText = "SELECT account_name FROM account WHERE account_id = @secretary_id";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@secretary_id", user_id);
                        secretary_name = command.ExecuteScalar()?.ToString();

                        // Retrieve patient name
                        command.CommandText = "SELECT account_name FROM account WHERE account_id = @patient_id";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@patient_id", patient_id);
                        patient_name = command.ExecuteScalar()?.ToString();

                        // Now, navigate to the page
                        reservation res = new reservation(reservation_id, patient_id, patient_name, user_id, secretary_name, slot, DateTime.Now, DateTime.Now);
                        Hide();
                        Visits visits = new Visits(user_id, patient_id, res.id);
                        visits.ShowDialog();
                        Show();
                    }

                    else
                        MessageBox.Show("Failed to perform the reservation!");
                }
            }

            updateSlots();
        }
    }
}
