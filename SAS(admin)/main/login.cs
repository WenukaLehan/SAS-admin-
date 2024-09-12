using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SAS_admin_.source_codes
{

    public partial class login : Form
    {
        FirestoreDb firestoreDb;

        public login()
        {
            InitializeComponent();
            // Path to your service account key filefolder
            string path = AppDomain.CurrentDomain.BaseDirectory + @"admin-sdk.json";

            // Set the environment variable to authenticate using the service account
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            // Initialize Firestore
            firestoreDb = FirestoreDb.Create("sas-wl03");
            Console.WriteLine("Firestore client initialized.");




        }

        private void iconPictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            // Example: Writing data to Firestore
            AddDocument(firestoreDb);

            // Example: Reading data from Firestore
            //ReadDocument(firestoreDb);
        }

        // Function to write data
        static void AddDocument(FirestoreDb db)
        {
            // Create a new document reference
            DocumentReference docRef = db.Collection("users").Document("user2");

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
            DocumentReference docRef = db.Collection("users").Document("user2");

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
