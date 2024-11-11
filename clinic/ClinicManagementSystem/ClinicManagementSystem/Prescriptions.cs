using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing.Printing;
using System.Xml.Linq;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Geom;
using iText.Layout.Borders;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;

namespace ClinicManagementSystem
{
    public partial class Prescriptions : Form
    {
        int prescriptionId = -1 , account_id;
        public Prescriptions(int account_id)
        {
            InitializeComponent();
            this.account_id = account_id;
        }
        public Prescriptions(int account_id,int newPrescriptionId)
        {
            InitializeComponent();
            this.account_id = account_id;
            prescriptionId = newPrescriptionId;
        }
        SqlConnection con = new SqlConnection(Properties.Resources.connectionString);
        SqlCommand command;

        private void updateList()
        {
            command = con.CreateCommand();

            if (radioButton1.Checked)
                command.CommandText = "SELECT prescriptions.prescription_id, prescriptions.patient_id, prescriptions.doctor_id, prescriptions.prescription_date, prescriptions.prescription_lieu, patient.account_name, patient.account_dob, patient.account_phone, doctor.account_name, doctor.account_phone, doctor.doctor_specialty, doctor.doctor_email, doctor.CabinetName, doctor.Cabinet_Address FROM prescriptions , account as patient , account as doctor WHERE patient.account_id = prescriptions.patient_id AND doctor.account_id = prescriptions.doctor_id AND prescription_date=@date";
            else if (radioButton2.Checked)
                command.CommandText = "SELECT prescriptions.prescription_id, prescriptions.patient_id, prescriptions.doctor_id, prescriptions.prescription_date, prescriptions.prescription_lieu, patient.account_name, patient.account_dob, patient.account_phone, doctor.account_name, doctor.account_phone, doctor.doctor_specialty, doctor.doctor_email, doctor.CabinetName, doctor.Cabinet_Address FROM prescriptions , account as patient , account as doctor WHERE patient.account_id = prescriptions.patient_id AND doctor.account_id = prescriptions.doctor_id AND ( patient.account_name LIKE @query OR patient.account_phone LIKE @query OR prescription_id LIKE @query )";
            else
                command.CommandText = "SELECT prescriptions.prescription_id, prescriptions.patient_id, prescriptions.doctor_id, prescriptions.prescription_date, prescriptions.prescription_lieu, patient.account_name, patient.account_dob, patient.account_phone, doctor.account_name, doctor.account_phone, doctor.doctor_specialty, doctor.doctor_email, doctor.CabinetName, doctor.Cabinet_Address FROM prescriptions , account as patient , account as doctor WHERE patient.account_id = prescriptions.patient_id AND doctor.account_id = prescriptions.doctor_id AND ( patient.account_name LIKE @query OR patient.account_phone LIKE @query OR prescription_id LIKE @query ) AND prescription_date=@date";

            command.Parameters.AddWithValue("@date", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@query", textBox1.Text + "%");

            con.Open();

            SqlDataReader reader = command.ExecuteReader();
            listBox1.Items.Clear();
            while (reader.Read())
            {
                int id = reader.IsDBNull(0) ? -1 : reader.GetInt32(0);
                prescriptionId = id;
                int patient_id = reader.IsDBNull(1) ? -1 : reader.GetInt32(1);
                int doctor_id = reader.IsDBNull(2) ? -1 : reader.GetInt32(2);
                DateTime prescription_date;
                DateTime.TryParse(reader.IsDBNull(3) ? string.Empty : reader.GetValue(3).ToString(), out prescription_date);
                string prescription_lieu = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                string patient_name = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                DateTime patient_dob;
                DateTime.TryParse(reader.IsDBNull(6) ? string.Empty : reader.GetValue(6).ToString(), out patient_dob);
                string patient_phone = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);
                string doctor_name = reader.IsDBNull(8) ? string.Empty : reader.GetString(8);
                string doctor_phone = reader.IsDBNull(9) ? string.Empty : reader.GetString(9);
                string doctor_specialty = reader.IsDBNull(10) ? string.Empty : reader.GetString(10);
                string doctor_email = reader.IsDBNull(11) ? string.Empty : reader.GetString(11);
                string CabinetName = reader.IsDBNull(12) ? string.Empty : reader.GetString(12);
                string Cabinet_Address = reader.IsDBNull(13) ? string.Empty : reader.GetString(13);

                listBox1.Items.Add(new prescription(id, patient_id, doctor_id, prescription_date, prescription_lieu, patient_name, patient_dob, patient_phone, doctor_name, doctor_phone, doctor_specialty, doctor_email, CabinetName, Cabinet_Address));
            }

            con.Close();

        }

        private void Prescriptions_Load(object sender, EventArgs e)
        {
            updateList();
            updateListMed("");
            updateListPatient("");

            if(prescriptionId != -1)
            {
                foreach (prescription item in listBox1.Items)
                {
                    if (item.id == prescriptionId)
                    {
                        listBox1.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            updateList();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
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



        private void updateForm()
        {
            if (listBox1.SelectedItem != null)
            {
                prescription pres = (prescription)listBox1.SelectedItem;
                textBox11.Text = pres.id.ToString();
                prescriptionId = pres.id;
                dateTimePicker2.Text = pres.prescription_date.Date.ToString("dd/MM/yyyy");
                textBox3.Text = pres.prescription_lieu.ToString();
                textBox5.Text = pres.patient_name.ToString();
                textBox6.Text = pres.patient_dob.Date.ToString("dd/MM/yyyy");
                textBox10.Text = pres.patient_phone.ToString();
                textBox7.Text = pres.doctor_name.ToString();
                textBox9.Text = pres.doctor_phone.ToString();
                textBox8.Text = pres.doctor_specialty.ToString();
                textBox13.Text = pres.doctor_email.ToString();
                textBox2.Text = pres.CabinetName.ToString();
                textBox12.Text = pres.Cabinet_Address.ToString();

                con.Open();

                SqlCommand drugCommand = con.CreateCommand();
                drugCommand.CommandText = "SELECT drugs.drug_name, drugs.last_dosage, drugs.last_periode FROM drugs , prescription_details WHERE prescription_details.prescription_id = @prescriptionId AND prescription_details.drug_name=drugs.drug_name";
                drugCommand.Parameters.AddWithValue("@prescriptionId", prescriptionId);

                SqlDataReader drugReader = drugCommand.ExecuteReader();
                while (drugReader.Read())
                {
                    string drugName = drugReader.IsDBNull(0) ? string.Empty : drugReader.GetString(0);
                    string lastDosage = drugReader.IsDBNull(1) ? string.Empty : drugReader.GetString(1);
                    string lastPeriode = drugReader.IsDBNull(2) ? string.Empty : drugReader.GetString(2);

                    listBox2.Items.Add($"{drugName} - Dose: {lastDosage} - Période: {lastPeriode}");
                }

                drugReader.Close();

                con.Close();
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (listBox2.SelectedItem != null)
            {
                // Assuming the format of listBox2 item is "DrugName - Last Dosage: Dosage - Last Periode: Periode"
                string selectedItem = listBox2.SelectedItem.ToString();

                // Split the selected item by '-' to separate drug name from dosage and periode
                string[] parts = selectedItem.Split('-');

                // Get the drug name (first part)
                string drugName = parts[0].Trim();

                // Extract dosage and periode from the second and third parts respectively
                string[] dosageParts = parts[1].Trim().Split(':');
                string dosage = dosageParts[1].Trim();

                string[] periodeParts = parts[2].Trim().Split(':');
                string periode = periodeParts[1].Trim();

                // Display drug information in textBox14, textBox15, and textBox16
                textBox14.Text = drugName;
                textBox15.Text = dosage;
                textBox16.Text = periode;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                string selectedDrug = listBox2.SelectedItem.ToString();

                string drugName = selectedDrug.Split('-')[0].Trim();

                string checkQuery = "SELECT COUNT(*) FROM prescription_details WHERE prescription_id = @prescriptionId AND drug_name = @drugName";

                SqlCommand checkCommand = new SqlCommand(checkQuery, con);

                checkCommand.Parameters.AddWithValue("@prescriptionId", prescriptionId);
                checkCommand.Parameters.AddWithValue("@drugName", drugName);

                try
                {
                    con.Open();

                    int count = (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        string deleteQuery = "DELETE FROM prescription_details WHERE prescription_id = @prescriptionId AND drug_name = @drugName";

                        SqlCommand deleteCommand = new SqlCommand(deleteQuery, con);
                        deleteCommand.Parameters.AddWithValue("@prescriptionId", prescriptionId);
                        deleteCommand.Parameters.AddWithValue("@drugName", drugName);
                        int rowsAffected = deleteCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Médicament supprimé de l'ordonnance.");
                        }
                        else
                        {
                            MessageBox.Show("Échec de suppression du médicament de l'ordonnance.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Le médicament sélectionné n'est pas associé à l'ordonnance.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Une erreur s'est produite : " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un médicament à supprimer.");
            }
            listBox2.Items.Clear();
            updateForm();
            textBox14.Clear();
            textBox15.Clear();
            textBox16.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox14.Text))
            {
                string newDrugName = textBox14.Text;
                string newDosage = textBox15.Text;
                string newPeriode = textBox16.Text;

                if (listBox2.SelectedItem != null)
                {
                    string selectedDrug = listBox2.SelectedItem.ToString();
                    string[] drugInfo = selectedDrug.Split('-');
                    string existingDrugName = drugInfo[0].Trim();

                    // Check if any changes are made
                    bool nameChanged = newDrugName != existingDrugName;
                    bool dosageChanged = newDosage != drugInfo[1].Trim();
                    bool periodeChanged = newPeriode != drugInfo[2].Trim();

                    if (nameChanged || dosageChanged || periodeChanged)
                    {
                        try
                        {
                            con.Open();

                            // Check for duplicates if the drug name is changed
                            if (nameChanged)
                            {
                                string checkDuplicateQuery = "SELECT COUNT(*) FROM drugs WHERE drug_name = @drugName";
                                SqlCommand checkDuplicateCommand = new SqlCommand(checkDuplicateQuery, con);
                                checkDuplicateCommand.Parameters.AddWithValue("@drugName", newDrugName);
                                int duplicateCount = (int)checkDuplicateCommand.ExecuteScalar();

                                if (duplicateCount > 0)
                                {
                                    MessageBox.Show("Un médicament avec ce nom existe déjà.");
                                    return;
                                }
                            }

                            // Update drug details
                            string updateDrugQuery = "UPDATE drugs SET drug_name = @newDrugName, last_dosage = @newDrugDosage, last_periode = @newDrugPeriode WHERE drug_name = @oldDrugName";
                            SqlCommand updateDrugCommand = new SqlCommand(updateDrugQuery, con);
                            updateDrugCommand.Parameters.AddWithValue("@newDrugName", newDrugName);
                            updateDrugCommand.Parameters.AddWithValue("@newDrugDosage", newDosage);
                            updateDrugCommand.Parameters.AddWithValue("@newDrugPeriode", newPeriode);
                            updateDrugCommand.Parameters.AddWithValue("@oldDrugName", existingDrugName);
                            int rowsAffected = updateDrugCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Médicament mis à jour avec succès.");
                            }
                            else
                            {
                                MessageBox.Show("Échec de la mise à jour du médicament.");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Une erreur s'est produite : " + ex.Message);
                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Aucun changement détecté.");
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner un médicament à mettre à jour.");
                }
            }
            else
            {
                MessageBox.Show("Veuillez entrer un nom de médicament.");
            }

            listBox2.Items.Clear();
            updateForm();
            textBox14.Clear();
            textBox15.Clear();
            textBox16.Clear();
        }

        private void updateListMed(string query)
        {
            command = con.CreateCommand();
            command.CommandText = "SELECT drug_name FROM drugs WHERE  drug_name LIKE @query";
            command.Parameters.AddWithValue("@query", query + "%");
            con.Open();

            SqlDataReader reader = command.ExecuteReader();
            listBox3.Items.Clear();

            while (reader.Read())
            {
                listBox3.Items.Add(reader.GetString(0));
            }

            con.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {

            if (listBox3.SelectedItem != null)
            {
                string selectedDrug = listBox3.SelectedItem.ToString();

                try
                {
                    con.Open();

                    // Check if the drug already exists in the prescription
                    string checkExistingQuery = "SELECT COUNT(*) FROM prescription_details WHERE prescription_id = @prescriptionId AND drug_name = @drugName";
                    SqlCommand checkExistingCommand = new SqlCommand(checkExistingQuery, con);
                    checkExistingCommand.Parameters.AddWithValue("@prescriptionId", prescriptionId);
                    checkExistingCommand.Parameters.AddWithValue("@drugName", selectedDrug);
                    int existingCount = (int)checkExistingCommand.ExecuteScalar();

                    if (existingCount > 0)
                    {
                        MessageBox.Show("Le médicament est déjà associé à l'ordonnance.");
                        return;
                    }

                    // Add the selected drug to the prescription_details table
                    string insertQuery = "INSERT INTO prescription_details (prescription_id, drug_name) VALUES (@prescriptionId, @drugName)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, con);
                    insertCommand.Parameters.AddWithValue("@prescriptionId", prescriptionId);
                    insertCommand.Parameters.AddWithValue("@drugName", selectedDrug);
                    int rowsAffected = insertCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Le médicament a été ajouté à l'ordonnance avec succès.");
                    }
                    else
                    {
                        MessageBox.Show("Échec de l'ajout du médicament à l'ordonnance.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Une erreur s'est produite : " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un médicament à ajouter à l'ordonnance.");
            }
            listBox2.Items.Clear();
            updateForm();
            textBox14.Clear();
            textBox15.Clear();
            textBox16.Clear();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox14.Text))
            {
                string drugName = textBox14.Text;
                string dosage = textBox15.Text;
                string periode = textBox16.Text;

                try
                {
                    con.Open();

                    // Check if the drug already exists in the drugs table
                    string checkDuplicateQuery = "SELECT COUNT(*) FROM drugs WHERE drug_name = @drugName";
                    SqlCommand checkDuplicateCommand = new SqlCommand(checkDuplicateQuery, con);
                    checkDuplicateCommand.Parameters.AddWithValue("@drugName", drugName);
                    int duplicateCount = (int)checkDuplicateCommand.ExecuteScalar();

                    if (duplicateCount == 0)
                    {
                        // Add the new drug to the drugs table
                        string addDrugQuery = "INSERT INTO drugs (drug_name, last_dosage, last_periode) VALUES (@drugName, @dosage, @periode)";
                        SqlCommand addDrugCommand = new SqlCommand(addDrugQuery, con);
                        addDrugCommand.Parameters.AddWithValue("@drugName", drugName);
                        addDrugCommand.Parameters.AddWithValue("@dosage", dosage);
                        addDrugCommand.Parameters.AddWithValue("@periode", periode);
                        int rowsAffected = addDrugCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Add the new drug to the prescription details
                            string addPrescriptionDetailsQuery = "INSERT INTO prescription_details (prescription_id, drug_name) VALUES (@prescriptionId, @drugName)";
                            SqlCommand addPrescriptionDetailsCommand = new SqlCommand(addPrescriptionDetailsQuery, con);
                            addPrescriptionDetailsCommand.Parameters.AddWithValue("@prescriptionId", prescriptionId);
                            addPrescriptionDetailsCommand.Parameters.AddWithValue("@drugName", drugName);
                            int prescriptionDetailsRowsAffected = addPrescriptionDetailsCommand.ExecuteNonQuery();

                            if (prescriptionDetailsRowsAffected > 0)
                            {
                                MessageBox.Show("Nouveau médicament ajouté avec succès.");
                                // Update the prescription form
                                updateForm();
                            }
                            else
                            {
                                MessageBox.Show("Échec de l'ajout du médicament à l'ordonnance.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Échec de l'ajout du médicament.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Un médicament avec ce nom existe déjà.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez remplir le nom pour ajouter un nouveau médicament.");
            }
            listBox2.Items.Clear();
            updateForm();
            textBox14.Clear();
            textBox15.Clear();
            textBox16.Clear();
            updateListMed(textBox17.Text);
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            updateListMed(textBox17.Text);
        }

        private void button9_Click(object sender, EventArgs e)
        {

            // Check if an item is selected in listBox2
            if (listBox2.SelectedItem != null)
            {
                // Ask the user for confirmation before proceeding with deletion
                DialogResult result = MessageBox.Show("Are you sure you want to delete the selected drug?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Get the selected drug name from listBox2
                    string selectedDrug = listBox2.SelectedItem.ToString();

                    // Split the selected string to extract the drug name
                    string drugName = selectedDrug.Split('-')[0].Trim();

                    try
                    {
                        con.Open();

                        // Delete the drug from the prescription_details table
                        string deletePrescriptionDetailsQuery = "DELETE FROM prescription_details WHERE drug_name = @drugName";
                        SqlCommand deletePrescriptionDetailsCommand = new SqlCommand(deletePrescriptionDetailsQuery, con);
                        deletePrescriptionDetailsCommand.Parameters.AddWithValue("@drugName", drugName);
                        int prescriptionDetailsDeleted = deletePrescriptionDetailsCommand.ExecuteNonQuery();

                        // Delete the drug from the drugs table
                        string deleteDrugQuery = "DELETE FROM drugs WHERE drug_name = @drugName";
                        SqlCommand deleteDrugCommand = new SqlCommand(deleteDrugQuery, con);
                        deleteDrugCommand.Parameters.AddWithValue("@drugName", drugName);
                        int drugsDeleted = deleteDrugCommand.ExecuteNonQuery();

                        if (drugsDeleted > 0 && prescriptionDetailsDeleted > 0)
                        {
                            MessageBox.Show("Drug deleted successfully from both tables.");
                            // Update the prescription form
                            updateForm();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete drug from one or both tables.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a drug to delete.");
            }
            listBox2.Items.Clear();
            updateForm();
            textBox14.Clear();
            textBox15.Clear();
            textBox16.Clear();
            updateListMed(textBox17.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            // Check if a prescription is selected
            if (listBox1.SelectedItem != null)
            {
                // Get the selected prescription
                prescription selectedPrescription = (prescription)listBox1.SelectedItem;

                try
                {
                    con.Open();

                    // Update the prescription in the database
                    string updateQuery = "UPDATE prescriptions SET patient_id = @patientId, doctor_id = @doctorId, prescription_date = @prescriptionDate, prescription_lieu = @prescriptionLieu WHERE prescription_id = @prescriptionId";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, con);
                    updateCommand.Parameters.AddWithValue("@patientId", selectedPrescription.patient_id);
                    updateCommand.Parameters.AddWithValue("@doctorId", selectedPrescription.doctor_id);
                    DateTime prescriptionDate = dateTimePicker2.Value.Date;
                    updateCommand.Parameters.AddWithValue("@prescriptionDate", prescriptionDate);
                    updateCommand.Parameters.AddWithValue("@prescriptionLieu", textBox3.Text);
                    updateCommand.Parameters.AddWithValue("@prescriptionId", selectedPrescription.id);

                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Current prescription saved successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to save current prescription.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    con.Close();
                }
            }
            else
            {
                MessageBox.Show("Please select a prescription to save.");
            }
        }

        private void updateListPatient(string query)
        {
            command = con.CreateCommand();
            command.CommandText = "SELECT account_id, account_name, account_type FROM account WHERE account_type=2 AND (account_name LIKE @query OR account_phone LIKE @query)";
            command.Parameters.AddWithValue("@query", query + "%");
            con.Open();

            SqlDataReader reader = command.ExecuteReader();
            listBox4.Items.Clear();

            while (reader.Read())
            {
                listBox4.Items.Add(new account(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
            }

            con.Close();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            updateListPatient(textBox4.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {

            // Vérifier si une ordonnance est sélectionnée
            if (listBox1.SelectedItem != null)
            {
                // Récupérer l'ID de l'ordonnance sélectionnée
                int selectedPrescriptionId = ((prescription)listBox1.SelectedItem).id;

                // Demander confirmation avant de supprimer l'ordonnance
                DialogResult result = MessageBox.Show("Voulez-vous vraiment supprimer l'ordonnance sélectionnée ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        con.Open();

                        // Supprimer l'enregistrement de l'ordonnance
                        string deleteQuery = "DELETE FROM prescriptions WHERE prescription_id = @prescriptionId";
                        SqlCommand deleteCommand = new SqlCommand(deleteQuery, con);
                        deleteCommand.Parameters.AddWithValue("@prescriptionId", selectedPrescriptionId);
                        int rowsAffected = deleteCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Ordonnance supprimée avec succès.");
                        }
                        else
                        {
                            MessageBox.Show("Échec de la suppression de l'ordonnance.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une ordonnance à supprimer.");
            }
            listBox1.Items.Clear();
            updateList();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            updateForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Check if a patient is selected
            if (listBox4.SelectedItem != null)
            {
                // Retrieve the selected patient's information
                int selectedPatient = ((account)listBox4.SelectedItem).getId();

                // Get the doctor's ID from the global variable
                int doctorId = account_id;

                // Get the current date
                DateTime currentDate = DateTime.Now.Date;

                try
                {
                    con.Open();

                    // Insert a new prescription record
                    string insertQuery = "INSERT INTO prescriptions (patient_id, doctor_id, prescription_date , prescription_lieu) VALUES (@patientId, @doctorId, @prescriptionDate , @prescription_lieu); SELECT SCOPE_IDENTITY();";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, con);
                    insertCommand.Parameters.AddWithValue("@patientId", selectedPatient);
                    insertCommand.Parameters.AddWithValue("@doctorId", doctorId);
                    insertCommand.Parameters.AddWithValue("@prescriptionDate", currentDate);
                    insertCommand.Parameters.AddWithValue("@prescription_lieu", "Jijel");
                    int newPrescriptionId = Convert.ToInt32(insertCommand.ExecuteScalar());

                    if (newPrescriptionId > 0)
                    {
                        MessageBox.Show("New prescription saved successfully.");
                        prescriptionId = newPrescriptionId;
                        // Refresh the prescription list
                        updateList();
                    }
                    else
                    {
                        MessageBox.Show("Failed to save new prescription.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    con.Close();
                }
            }
            else
            {
                MessageBox.Show("Please select a patient.");
            }
            listBox1.Items.Clear();
            updateList();
            // Select the newly added prescription in listBox1
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                prescription item = (prescription)listBox1.Items[i];
                if (item.id == prescriptionId)
                {
                    listBox1.SelectedIndex = i;
                    break;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            GeneratePrescriptionPDF();
        }

        private void GeneratePrescriptionPDF()
        {
            // Define output path for the PDF
            string outputPath = "prescription.pdf";

            // Get the prescription information from the textboxes and listbox
            DateTime selectedDate = dateTimePicker2.Value.Date;
            string formattedDate = selectedDate.ToString("dd/MM/yyyy");


            string patientInfo = $"{textBox3.Text}, le: {formattedDate}\nNom et Prénom: {textBox5.Text}\nNé(e) en: {textBox6.Text}\nNuméro Tel: {textBox10.Text}";
            string NomCabinet = textBox2.Text ;
            string DoctorName = textBox7.Text ;
            string DoctorSpe = textBox8.Text;
            string DoctorNum = textBox9.Text;
            string PresNum = textBox11.Text;
            string Address = textBox12.Text;
            string email = textBox13.Text;
            ListBox list = listBox2;

            // Generate prescription PDF
            GeneratePrescriptionPDF(outputPath, NomCabinet, DoctorName, DoctorSpe, DoctorNum, PresNum, patientInfo, Address, email , list);

            MessageBox.Show("Prescription generated successfully.");
        }

        static void GeneratePrescriptionPDF(string outputPath, string NomCabinet, string DoctorName, string DoctorSpe, string DoctorNum, string PresNum, string patientInfo,  string Address, string email , ListBox list)
        {
            // Create a new PDF document
            using (PdfWriter writer = new PdfWriter(outputPath))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    // Set page size to a fraction of A4
                    PageSize halfA4 = new PageSize(PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight() / 2);

                    // Create a Document
                    Document document = new Document(pdf, halfA4);

                    // Load a font that supports Arabic characters
                    PdfFont arabicFont = PdfFontFactory.CreateFont(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf"), PdfEncodings.IDENTITY_H);

                    // Add content to the document
                    // Cabinet name
                    Paragraph cabinetName = new Paragraph(NomCabinet)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(10)
                        .SetUnderline();
                    // Calculate the position for centering the text horizontally
                    float centerX = (halfA4.GetWidth() / ((halfA4.GetWidth() - cabinetName.ToString().Length) / 2));
                    // Set the fixed position at the top center of the page
                    cabinetName.SetFixedPosition(centerX, halfA4.GetTop() - 20, halfA4.GetWidth());
                    document.Add(cabinetName);

                    // Create a table for patient and doctor info
                    Table patientDoctorTable = new Table(4).UseAllAvailableWidth();

                    // Doctor information
                    Paragraph doctorInfo = new Paragraph();
                    doctorInfo.Add(DoctorName+"\n")
                        .SetFont(arabicFont)
                        .SetFontSize(8)
                        .SetBaseDirection(BaseDirection.RIGHT_TO_LEFT);

                    // Add the specialized part with a smaller font size
                    Paragraph specializedInfo = new Paragraph(DoctorSpe+"\n")
                        .SetFont(arabicFont)
                        .SetFontSize(6)
                        .SetBaseDirection(BaseDirection.RIGHT_TO_LEFT);

                    doctorInfo.Add(specializedInfo);

                    // Add the rest of the doctor's information with the default font size
                    doctorInfo.Add("\n"+DoctorNum +"\n"+"Or n° :" +PresNum + "\n");

                    patientDoctorTable.AddCell(new Cell().Add(doctorInfo)
                        .SetBorder(Border.NO_BORDER));

                    patientDoctorTable.AddCell(new Cell().Add(new Paragraph()).SetBorder(Border.NO_BORDER));
                    patientDoctorTable.AddCell(new Cell().Add(new Paragraph()).SetBorder(Border.NO_BORDER));

                    // Patient information
                    Paragraph patient_Info = new Paragraph(patientInfo)
                        .SetTextAlignment(TextAlignment.RIGHT)
                        .SetFontSize(8);
                    patientDoctorTable.AddCell(new Cell().Add(patient_Info)
                        .SetBorder(Border.NO_BORDER));

                    document.Add(patientDoctorTable);

                    // Prescription title
                    Paragraph prescriptionTitle = new Paragraph("ORDONNANCE")
                        .SetUnderline()
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(10);
                    document.Add(prescriptionTitle);

                    // Prescription drugs
                    Table drugsTable = new Table(2).UseAllAvailableWidth()
                        .SetTextAlignment(TextAlignment.CENTER);

                    // Iterate through the items in listBox2
                    foreach (var item in list.Items)
                    {
                        // Split the item string into an array to extract drug name, dosage, and period
                        string[] drugInfo = item.ToString().Split(new string[] { " - " }, StringSplitOptions.None);

                        // Extract drug name, dosage, and period
                        string drugName = drugInfo[0];
                        string[] doseInfo = drugInfo[1].Split(new string[] { ": " }, StringSplitOptions.None);
                        string dosage = doseInfo[1];
                        string[] periodInfo = drugInfo[2].Split(new string[] { ": " }, StringSplitOptions.None);
                        string period = periodInfo[1];

                        // Add drug name, dosage, and period to the drugsTable
                        Paragraph drug_Name = new Paragraph(drugName)
                            .SetTextAlignment(TextAlignment.LEFT)
                            .SetFontSize(8);
                        drugsTable.AddCell(new Cell().Add(drug_Name)
                            .SetBorder(Border.NO_BORDER));

                        Paragraph drug_Dosage = new Paragraph(dosage+"\n"+ period)
                            .SetTextAlignment(TextAlignment.RIGHT)
                            .SetFontSize(8);
                        drugsTable.AddCell(new Cell().Add(drug_Dosage)
                            .SetBorder(Border.NO_BORDER));
                    }

                    document.Add(drugsTable);

                    // Address and email
                    Paragraph addressAndEmail = new Paragraph(Address+"\n"+ email)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(8);

                    // Set a fixed position for the address and email
                    string addressAndEmailText = addressAndEmail.ToString();
                    int length = addressAndEmailText.Length;

                    addressAndEmail.SetFixedPosition(halfA4.GetWidth() / ((halfA4.GetWidth() - length) / 2), halfA4.GetBottom(), halfA4.GetWidth());

                    document.Add(addressAndEmail);

                    // Close the document
                    document.Close();
                }
            }
        }

    }
}
