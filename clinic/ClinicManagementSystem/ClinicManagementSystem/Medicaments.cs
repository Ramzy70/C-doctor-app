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
using Clinic_Management_System;


namespace ClinicManagementSystem
{
    public partial class Medicaments : Form
    {
        int account_id, account_type;
        public Medicaments(int account_id)
        {
            InitializeComponent();
            this.account_id = account_id;
            SqlCommand cmd = con.CreateCommand();
            con.Open();
            cmd.CommandText = "SELECT account_id , account_type FROM account WHERE account_user_id = @user_id";
            cmd.Parameters.AddWithValue("@user_id", this.account_id);
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
        SqlConnection con = new SqlConnection(Properties.Resources.connectionString);
        SqlCommand command;
        private void updateList(string query)
        {
            command = con.CreateCommand();
            command.CommandText = "SELECT drug_name FROM drugs WHERE  drug_name LIKE @query";
            command.Parameters.AddWithValue("@query", query + "%");
            con.Open();

            SqlDataReader reader = command.ExecuteReader();
            listBox1.Items.Clear();

            while (reader.Read())
            {
                listBox1.Items.Add(reader.GetString(0));
            }

            con.Close();
        }

        private void Medicaments_Load(object sender, EventArgs e)
        {
            updateList("");
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0 || listBox1.SelectedIndex >= listBox1.Items.Count)
                return;
            string drugName = (string)listBox1.SelectedItem;
            command = con.CreateCommand();
            command.CommandText = "SELECT drug_name FROM drugs WHERE  drug_name = @drugname";
            command.Parameters.AddWithValue("@drugname", drugName);
            con.Open();

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                textBox1.Text = reader.GetString(0);
            }

            con.Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            updateList(textBox2.Text);
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

        private void button5_Click(object sender, EventArgs e)
        {
            Hide();
            EditProfile editProfile = new EditProfile(account_id);
            editProfile.ShowDialog();
            Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Hide();
            PatientsProfiles patientProfiles = new PatientsProfiles(this.account_id);
            patientProfiles.ShowDialog();
            Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Hide();
            CreateReservation createReservation = new CreateReservation(account_id);
            createReservation.ShowDialog();
            Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Hide();
            ViewReservations viewReservations = new ViewReservations(account_id);
            viewReservations.ShowDialog();
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

            // Inputs Validation
            if (textBox3.Text == "")
            {
                MessageBox.Show("SVP ajouter le nom du médicament!");
                return;
            }

            // Check if the drug with the same name already exists
            command = con.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM drugs WHERE drug_name = @name";
            command.Parameters.AddWithValue("@name", textBox3.Text);
            con.Open();
            int count = Convert.ToInt32(command.ExecuteScalar());
            con.Close();

            // If a drug with the same name exists, display an error message
            if (count > 0)
            {
                MessageBox.Show("Le médicament avec le même nom existe déjà!");
                return;
            }

            // If no drug with the same name exists, proceed with insertion
            command = con.CreateCommand();
            command.CommandText = "INSERT INTO drugs (drug_name) VALUES (@name)";
            command.Parameters.AddWithValue("@name", textBox3.Text);
            con.Open();
            if (command.ExecuteNonQuery() > 0)
            {
                MessageBox.Show("Médicament ajouté avec succès!");
                textBox3.Text = "";
            }
            else
                MessageBox.Show("Erreur lors de l'ajout du médicament!");
            con.Close();
            updateList("");
        }

        private void button3_Click_1(object sender, EventArgs e)
        {

            try
            {
                // Inputs Validation
                if (textBox4.Text == "")
                {
                    MessageBox.Show("SVP entrer le nouveau nom du médicament");
                    return;
                }

                // Editing the drug name
                using (command = con.CreateCommand())
                {
                    command.CommandText = "UPDATE drugs SET drug_name = @newName WHERE drug_name = @oldName";
                    command.Parameters.AddWithValue("@newName", textBox4.Text);
                    command.Parameters.AddWithValue("@oldName", textBox1.Text); // Use the selected item from listBox1

                    con.Open();

                    if (command.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Le nom du médicament a été modifié !");
                        // Reload the list of drugs

                    }
                    else
                    {
                        MessageBox.Show("Aucune modification n'a été effectuée. Veuillez vérifier le nom du médicament.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur est survenue lors de la modification du nom du médicament : " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                updateList("");
                textBox1.Text = textBox4.Text;
                textBox4.Clear();
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {


            // Inputs Validation
            if (textBox1.Text == "")
            {
                MessageBox.Show("Veuillez sélectionner un médicament à supprimer.");
                return;
            }

            string drugName = textBox1.Text;

            try
            {
                using (command = con.CreateCommand())
                {
                    command.CommandText = "DELETE FROM drugs WHERE drug_name = @name";
                    command.Parameters.AddWithValue("@name", drugName);

                    con.Open();

                    if (command.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Le médicament a été supprimé avec succès !");
                    }
                    else
                    {
                        MessageBox.Show("Échec de la suppression du médicament. Veuillez réessayer.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite lors de la suppression du médicament : " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                updateList("");
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
    }
}
