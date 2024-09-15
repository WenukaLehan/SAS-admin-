using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using SAS_admin_.main;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAS_admin_.attender
{
    public partial class Form1 : Form
    {
        private static FirestoreDb db;
        private static SerialPort _serialPort;

        public Form1()
        {
            InitializeComponent();
            showPorts();
            db = dbCon.getDb();
            LoadData();

            // Path to the Firebase service account key (JSON file downloaded from Firebase Console)
            string firebaseCredentialsPath = AppDomain.CurrentDomain.BaseDirectory + @"admin-sdk.json";
            // Initialize Firebase App
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(firebaseCredentialsPath)
                });
            }

        }

        public void LoadData()
        {

            try
            {
                DocumentReference docRef = db.Collection("attendence").Document(DateTime.Today.ToString("dd-MM-yyyy"));
                CollectionReference collectionRef = docRef.Collection("students");

                // Get all documents in the collection
                QuerySnapshot snapshot = collectionRef.GetSnapshotAsync().Result;

                // Create a DataTable to store the data
                DataTable dataTable = new DataTable();

                // Loop through documents and build table columns and rows dynamically
                bool columnsAdded = false;
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    Dictionary<string, object> docData = document.ToDictionary();

                    if (!columnsAdded)
                    {
                        // Add columns to the DataTable based on the document fields
                        /*foreach (var field in docData)
                        {
                            dataTable.Columns.Add(field.Key);
                        }*/
                        //dataGrid.DataSource = dataTable;
                        dataTable.Columns.Add("id");
                        dataTable.Columns.Add("intime");
                        dataTable.Columns.Add("outtime");
                        columnsAdded = true;

                    }

                    // Add rows with document field values
                    DataRow row = dataTable.NewRow();
                    /*foreach (var field in docData)
                    {
                        row[field.Key] = field.Value ?? DBNull.Value;
                    }*/
                    row["id"] = document.Id;
                    row["intime"] = docData["in_time"];
                    row["outtime"] = docData["out_time"];
                    dataTable.Rows.Add(row);
                }

                // Bind the DataTable to the DataGridView
                attendenceGrid.DataSource = dataTable;


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving data: {ex.Message}");

            }



        }

        private void showPorts()
        {
            comboBoxComPorts.Items.Clear();

            // Get all available COM ports
            string[] comPorts = SerialPort.GetPortNames();

            foreach (string port in comPorts)
            {
                string deviceInfo = GetDeviceDescription(port);
                comboBoxComPorts.Items.Add($"{port} - {deviceInfo}");
            }

            // Set the first item as selected (optional)
            if (comboBoxComPorts.Items.Count > 0)
            {
                comboBoxComPorts.SelectedIndex = 0;
            }
        }

        private string GetDeviceDescription(string port)
        {
            try
            {
                // Use ManagementObjectSearcher to query device details
                string query = $"SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%({port})%'";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj["Caption"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving device information: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Return port name if no description is found
            return "Unknown device";
        }

        private void startread(string com)
        {

            // Set up the serial port
            _serialPort = new SerialPort();
            _serialPort.PortName = com;  // Replace with the appropriate COM port
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            try
            {
                _serialPort.Open();
                MessageBox.Show("attendence marking started", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private static string oldData = "null";
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

            var data = _serialPort.ReadLine();
            if (data != oldData)
            {
                oldData = data;

                if (getToken(data) != null)
                {
                    string token = getToken(data).GetValue(0).ToString();
                    string id = getToken(data).GetValue(1).ToString();
                    string currentTime = DateTime.Now.ToString("HH:mm:ss");
                    string currentDate = DateTime.Now.ToString("dd-MM-yyyy");
                    DateTime compareTime = DateTime.Today.Add(new TimeSpan(13, 20, 0));

                    if (!checkAtt(id))
                    {
                        if (compareTime > DateTime.Now)
                        {
                            string docid = getdocId(id, currentDate);

                            if (docid != null)
                            {
                                updateAttendence(currentDate, docid, currentTime);
                                RefreshDataGrid();
                                SendPushNotification("attendence marked", "your child is out of school on " + currentDate + " at " + currentTime, token).Wait();

                            }
                        }
                        else
                        {
                            Dictionary<string, object> mapAtt = new Dictionary<string, object>();
                            mapAtt.Add("in_time", currentTime);
                            mapAtt.Add("id", id);
                            mapAtt.Add("out_time", "-");
                            AddMapToDocumentAsync(currentDate, mapAtt);
                            RefreshDataGrid();
                            SendPushNotification("attendence marked", "your child is in school on " + currentDate + " at " + currentTime, token).Wait();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid RFID data received: " + data, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


            }


        }


        //check allready attended
        private bool checkAtt(string id)
        {

            DocumentReference docref = db.Collection("attendence").Document(DateTime.Now.ToString("dd-MM-yyyy"));
            CollectionReference co2 = docref.Collection("students");
            Query query = co2.WhereEqualTo("id", id);
            QuerySnapshot querySnapshot = query.GetSnapshotAsync().Result;
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    return true;
                }
            }
            return false;
        }

        private static string getdocId(string id, string documentID)
        {
            DocumentReference collectionReference = db.Collection("attendence").Document(documentID);
            CollectionReference co2 = collectionReference.Collection("students");
            Query query = co2.WhereEqualTo("id", id);
            QuerySnapshot querySnapshot = query.GetSnapshotAsync().Result;
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    string docid = documentSnapshot.Id;
                    return docid;
                }
            }
            return null;
        }

        private static Array getToken(string data)
        {
            CollectionReference collectionReference = db.Collection("users");
            Query query = collectionReference.WhereEqualTo("rfid", data);
            QuerySnapshot querySnapshot = query.GetSnapshotAsync().Result;
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    string id = documentSnapshot.Id;
                    string token = documentSnapshot.GetValue<string>("token");
                    string[] arr = new string[] { token, id };
                    return arr;

                }
            }
            return null;
        }

        public async static Task SendPushNotification(string title, string msg, string token)
        {

            // Create a message to send
            var message = new FirebaseAdmin.Messaging.Message()
            {
                Notification = new Notification
                {
                    Title = title,
                    Body = msg,
                },
                Token = token,  // FCM token of the device you want to send the notification to
            };

            // Send the message and get the response
            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBoxComPorts_Click(object sender, EventArgs e)
        {
            showPorts();
        }

        //save attendence to firebase
        public static void AddMapToDocumentAsync(string documentId, Dictionary<string, object> data)
        {
            // Get a reference to the document
            DocumentReference docRef = db.Collection("attendence").Document(documentId);

            QuerySnapshot snapshot = docRef.Collection("students").GetSnapshotAsync().Result;

            string newid;
            int docCount = snapshot.Count;
            // Get the last document ID
            string lastDocumentId = null;
            if (docCount > 0)
            {
                // If there are documents, sort by document ID and get the last one
                var documents = snapshot.Documents.ToList();
                documents.Sort((x, y) => string.Compare(x.Id, y.Id)); // Sort documents by ID
                lastDocumentId = documents.Last().Id; // Get the ID of the last document
                Console.WriteLine("Last document ID: " + lastDocumentId);
                // Add one to the last document ID
                int lastNumber = int.Parse(lastDocumentId.Substring(2));
                newid = "st" + (lastNumber + 1).ToString("D2");
                Console.WriteLine("New document ID: " + newid);

            }
            else
            {

                newid = "st01";
            }

            DocumentReference docRef2 = docRef.Collection("students").Document(newid);
            docRef2.SetAsync(data);

        }

        public static void updateAttendence(string documentId, string stid, string outtime)
        {
            DocumentReference docRef = db.Collection("attendence").Document(documentId);
            DocumentReference docref2 = docRef.Collection("students").Document(stid);

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                ["out_time"] = outtime
            };
            docref2.UpdateAsync(data);
            MessageBox.Show("success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }



        private void button1_Click(object sender, EventArgs e)
        {
            string com = comboBoxComPorts.SelectedItem.ToString();
            startread(com.Substring(0, 4));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _serialPort.Close();
            MessageBox.Show("attendence marking stopped", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RefreshDataGrid()
        {
            Invoke((System.Action)(() =>
            {
                // Clear the current data binding
                attendenceGrid.DataSource = null;
                LoadData();
                attendenceGrid.Refresh();
            }));

        }

    }
}
