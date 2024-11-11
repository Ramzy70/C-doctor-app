using System;
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

namespace PrescriptionGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define output path for the PDF
            string outputPath = "prescription.pdf";

            // Generate prescription PDF
            GeneratePrescriptionPDF(outputPath);

            Console.WriteLine("Prescription generated successfully.");
        }

        static void GeneratePrescriptionPDF(string outputPath)
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
                    Paragraph cabinetName = new Paragraph("Cabinet Médical spécialisé en Médecine Interne")
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
                    doctorInfo.Add("الدكتور : بن عزيزة عادل\n")
                        .Add("Docteur BENAZIZA Adel\n")
                        .SetFont(arabicFont)
                        .SetFontSize(8)
                        .SetBaseDirection(BaseDirection.RIGHT_TO_LEFT);

                    // Add the specialized part with a smaller font size
                    Paragraph specializedInfo = new Paragraph("Spécialiste en Médecine Interne\nMaladies cardio-vasculaires\nEchodoppler cardiaque et vasculaire")
                        .SetFont(arabicFont)
                        .SetFontSize(6)
                        .SetBaseDirection(BaseDirection.RIGHT_TO_LEFT);

                    doctorInfo.Add(specializedInfo);

                    // Add the rest of the doctor's information with the default font size
                    doctorInfo.Add("\nECG-MAPA\nn°C0 : 8865\n0673454697");

                    patientDoctorTable.AddCell(new Cell().Add(doctorInfo)
                        .SetBorder(Border.NO_BORDER));

                    patientDoctorTable.AddCell(new Cell().Add(new Paragraph()).SetBorder(Border.NO_BORDER));
                    patientDoctorTable.AddCell(new Cell().Add(new Paragraph()).SetBorder(Border.NO_BORDER));

                    // Patient information
                    Paragraph patientInfo = new Paragraph("Jijel, le : 12/02/2024\nNom : LAOUICI\nPrénom : HOURIA\nNé(e) en : 1959")
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetFontSize(8);
                    patientDoctorTable.AddCell(new Cell().Add(patientInfo)
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

                    // Add drug names and dosages
                    Paragraph drugName = new Paragraph("LEVOTHYROX 100 uG comp")
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetFontSize(8);
                    drugsTable.AddCell(new Cell().Add(drugName)
                        .SetBorder(Border.NO_BORDER));

                    Paragraph drugDosage = new Paragraph("Qsp 3 mois\n1cp / j")
                        .SetTextAlignment(TextAlignment.RIGHT)
                        .SetFontSize(8);
                    drugsTable.AddCell(new Cell().Add(drugDosage)
                        .SetBorder(Border.NO_BORDER));

                    document.Add(drugsTable);

                    // Address and email
                    Paragraph addressAndEmail = new Paragraph("Cité 150 logt Bt (A4) N°5 Jijel (en face l'entrée pricipale de l'hopital )\nadel.benaz@yahoo.fr")
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
