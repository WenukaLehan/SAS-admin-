using Google.Cloud.Firestore;
using System;

namespace SAS_admin_.main
{
    internal class dbCon
    {
        public static FirestoreDb firestoreDb;

        public dbCon()
        {



        }

        public static FirestoreDb getDb(FirestoreDb firestoreDb)
        {
            // Path to your service account key filefolder
            string path = AppDomain.CurrentDomain.BaseDirectory + @"admin-sdk.json";
            // Set the environment variable to authenticate using the service account
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            firestoreDb = FirestoreDb.Create("sas-wl03");
            Console.WriteLine("Firestore client initialized.");
            return firestoreDb;
        }
    }
}
