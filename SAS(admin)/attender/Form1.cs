using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using SAS_admin_.main;
using System;
using System.Collections.Generic;
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
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

            string data = _serialPort.ReadLine();
            if (data != oldData)
            {
                oldData = data;
                string token = getToken(data).GetValue(0).ToString();
                string id = getToken(data).GetValue(1).ToString();
                string currentTime = DateTime.Now.ToString("HH:mm:ss");
                string currentDate = DateTime.Now.ToString("dd/MM/yyyy");
                DateTime compareTime = DateTime.Today.Add(new TimeSpan(13, 20, 0));

                if (compareTime > DateTime.Now)
                {
                    Dictionary<string, object> mapAtt = new Dictionary<string, object>();
                    mapAtt.Add("in_time", currentTime);
                    mapAtt.Add("id", id);
                    mapAtt.Add("out_time", "-");
                    AddMapToDocumentAsync(currentDate, mapAtt);

                    SendPushNotification("attendence marked", "your child is in school on " + currentDate + " at " + currentTime, token).Wait();
                }
                else
                {
                    Dictionary<string, object> mapAtt = new Dictionary<string, object>();
                    mapAtt.Add("in_time", currentTime);
                    mapAtt.Add("id", id);
                    mapAtt.Add("out_time", "-");
                    AddMapToDocumentAsync(currentDate, mapAtt);

                    SendPushNotification("attendence marked", "your child is in school on " + currentDate + " at " + currentTime, token).Wait();
                }


            }


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
            MessageBox.Show(documentId, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        public static void updateAttendence(string documentId, Dictionary<string, object> data)
        {
            DocumentReference docRef = db.Collection("attendence").Document(documentId);
            docRef.UpdateAsync(data);
        }



        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> user = new Dictionary<string, object>
            {
                ["rfid"] = "1234567890",
                ["name"] = "John Doe",
                ["age"] = 12,
                ["gender"] = "Male"
            };
            AddMapToDocumentAsync("wenuka", user);
        }
    }
}
