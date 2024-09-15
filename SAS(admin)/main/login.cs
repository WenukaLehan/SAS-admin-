using Google.Cloud.Firestore;
using SAS_admin_.admin;
using SAS_admin_.attender;
using SAS_admin_.main;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SAS_admin_.source_codes
{

    public partial class login : Form
    {
        FirestoreDb db;

        public login()
        {
            InitializeComponent();

            db = dbCon.getDb();

        }

        private void iconPictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private async void loginBtn_Click(object sender, EventArgs e)
        {
            string user = username.Text;
            string pass = password.Text;
            CollectionReference colRef = db.Collection("admins");
            Query query = colRef.WhereEqualTo("username", user);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();


            if (querySnapshot.Documents.Count > 0)
            {

                for (int i = 0; i < querySnapshot.Documents.Count; i++)
                {
                    Dictionary<string, object> data = querySnapshot.Documents[i].ToDictionary();

                    if (data["password"].ToString() == pass)
                    {
                        MessageBox.Show("Login successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (data["type"].ToString() == "admin")
                        {
                            aDashboard ad = new aDashboard();
                            ad.Show();
                            this.Hide();
                        }
                        if (data["type"].ToString() == "attend")
                        {
                            Form1 ad = new Form1();
                            ad.Show();
                            this.Hide();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Wrong password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("username not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        // Function to write data
        static void AddDocument(FirestoreDb db)
        {
            // Create a new document reference
            DocumentReference docRef = db.Collection("users").Document("user3");

            // Define a dictionary with data to be stored
            var user = new
            {
                FirstName = "wenuka",
                LastName = "amma",
                Age = 78,
                Email = "john.doe@adooo.com"
            };

            // Add the document to Firestore
            docRef.SetAsync(user);
            MessageBox.Show("Document added successfully!");
        }

        // Function to read data
        static async void ReadDocument(FirestoreDb db)
        {
            // Reference to the document to be read
            DocumentReference docRef = db.Collection("users").Document("user3");

            // Fetch the document snapshot
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> data = snapshot.ToDictionary();
                MessageBox.Show("Document data: " + data["FirstName"] + " " + data["LastName"]);
            }
            else
            {
                Console.WriteLine("No document found.");
            }
        }



    }

}
