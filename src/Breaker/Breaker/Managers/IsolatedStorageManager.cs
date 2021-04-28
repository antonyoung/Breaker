using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Collections.Generic;

using Breaker.Wp7.Xna4.Data;

namespace Breaker.Wp7.Xna4.Managers
{

    /// <summary>
    /// Handles all the loading and saving of breaker game data in the Isolated Storage
    /// </summary>
    public class IsolatedStorageManager
    {

        #region constants

        const string ROOTDIRECTORY = "BREAKER";

        const string SETTINGSFILENAME = "settings.brk";
        const string SETTINGSDIRECTORY = "SETTINGS";
        
        const string TROPHIESFILENAME = "trophies.brk";
        const string TROPHIESDIRECTORY = "TROPHIES";

        const string CHEATSFILENAME = "cheats.brk";
        const string CHEATSDIRECTORY = "CHEATS";

        #endregion
        
        #region public Load sections

       
        /// <summary>
        /// Loads the user settings from the Breaker Isolated Storage
        /// </summary>
        /// <returns>SettingsData containing all the breaker settings needed for the game.</returns>
        public SettingsData LoadSettings()
        {
            // load settings data from isolated storage 
            byte[] settings = LoadFromIsolatedStorage(
                ROOTDIRECTORY + "\\" + SETTINGSDIRECTORY + "\\" + SETTINGSFILENAME);

            // no settings data
            if (settings == null) return new SettingsData();

            // deserialize settings data to a Data.Settings class
            return DeserializeSettings(settings);
        }


        /// <summary>
        /// Loads the trophies data from the Isolated Storage
        /// </summary>
        /// <returns></returns>
        public List<TrophieData> LoadTrophies()
        {
            byte[] trophies = null;
            try
            {
                // load usersettings
                trophies = LoadFromIsolatedStorage(
                    ROOTDIRECTORY + "\\" + TROPHIESDIRECTORY + "\\" + TROPHIESFILENAME);
            }
            catch(IsolatedStorageException ex)
            {
                //
            }
            // validation
            if (trophies == null) return new List<TrophieData>();

            // deserialize file to Entities.UserSettings
            return DeserializeTrophies(trophies);
        }


        /// <summary>
        /// Loads the Cheats data from the Isolated Storage
        /// </summary>
        /// <returns></returns>
        public List<TrophieData> LoadCheats()
        {
            // load usersettings
            byte[] trophies = LoadFromIsolatedStorage(TROPHIESFILENAME);

            // validation
           if (trophies == null) return new List<TrophieData>();

            // deserialize file to Entities.UserSettings
            return DeserializeTrophies(trophies);
        }

        #endregion

        #region public Save sections

        
        /// <summary>
        /// Stores the settings data in the Isolated Storage, so the settings data can be used another time.
        /// </summary>
        /// <param name="data">used as the settings to be saved into the Isolated Storga</param>
        /// <returns>true, if storage was succesful</returns>
        public bool SaveSettings(SettingsData data)
        {
            bool isSucces = false;
            try
            {
                
                byte[] usersettings = SerializeSettingsData(data);

                using (var store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.DirectoryExists(ROOTDIRECTORY))
                    {
                        store.CreateDirectory(ROOTDIRECTORY);
                    }

                    if (!store.DirectoryExists(ROOTDIRECTORY + "\\" + SETTINGSDIRECTORY))
                    {
                        store.CreateDirectory(ROOTDIRECTORY + "\\" + SETTINGSDIRECTORY);
                    }
                    if (store.FileExists(ROOTDIRECTORY + "\\" + SETTINGSDIRECTORY + "\\" + SETTINGSFILENAME))
                    {
                        store.DeleteFile(ROOTDIRECTORY + "\\" + SETTINGSDIRECTORY + "\\" + SETTINGSFILENAME);
                    }


                    System.IO.IsolatedStorage.IsolatedStorageFileStream fileStream;
                    //// create the file
                    fileStream = store.CreateFile(ROOTDIRECTORY + "\\" + SETTINGSDIRECTORY + "\\" + SETTINGSFILENAME);
                    //// write out the stream to isostore

                    WriteStream(usersettings, fileStream);
                    fileStream.Close();
                }
                // 01 save file in isolated storage
                isSucces = true;
            }
            catch (IsolatedStorageException ex)
            {
                string test = ex.Message;
                isSucces = false;
            }
            return isSucces;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SaveTrophies(List<TrophieData> data)
        {
            //DebugTrophies(data);

            bool isSucces = false;
            try
            {
                byte[] trophiesData = SerializeTrophiesData(data);

                using (var store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.DirectoryExists(ROOTDIRECTORY))
                    {
                        store.CreateDirectory(ROOTDIRECTORY);
                    }

                    if (!store.DirectoryExists(ROOTDIRECTORY + "\\" + TROPHIESDIRECTORY))
                    {
                        store.CreateDirectory(ROOTDIRECTORY + "\\" + TROPHIESDIRECTORY);
                    }

                    if(store.FileExists(ROOTDIRECTORY + "\\" + TROPHIESDIRECTORY + "\\" + TROPHIESFILENAME))
                    {
                        store.DeleteFile(ROOTDIRECTORY + "\\" + TROPHIESDIRECTORY + "\\" + TROPHIESFILENAME);
                    }

                    System.IO.IsolatedStorage.IsolatedStorageFileStream fileStream;

                    //fileStream = new IsolatedStorageFileStream(ROOTDIRECTORY + "\\" + TROPHIESDIRECTORY + "\\" + TROPHIESFILENAME,
                    //    FileMode.Create, store);
                    // create the file
                    fileStream = store.CreateFile(ROOTDIRECTORY + "\\" + TROPHIESDIRECTORY + "\\" + TROPHIESFILENAME);
                    // write out the stream to isostore

                    WriteStream(trophiesData, fileStream);
                    fileStream.Close();
                }
                
                isSucces = true;
            }
            catch (IsolatedStorageException ex)
            {
                string test = ex.Message;
                isSucces = false;
            }
            return isSucces;
        }

        #endregion

        #region isolated storage IO helper methods

        private byte[] LoadFromIsolatedStorage(string filePathName)
        {
            IsolatedStorageFile isFile = IsolatedStorageFile.GetUserStoreForApplication();

            if (!isFile.FileExists(filePathName))
            {
                //isf.Close();
                isFile.Dispose();
                return null;
            }

            IsolatedStorageFileStream isFileStream =
              new IsolatedStorageFileStream(filePathName, FileMode.Open, isFile);

            StreamReader sr = new StreamReader(isFileStream);
            string settings = sr.ReadToEnd();
            sr.Close();

            //isf.Close();
            isFileStream.Close();
            sr.Dispose();
            isFileStream.Dispose();
            isFile.Dispose();

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(settings);
        }

        private void WriteStream(byte[] data, System.IO.IsolatedStorage.IsolatedStorageFileStream fileStream)
        {
            MemoryStream ms = new MemoryStream(data);
            int bytesRead;
            while ((bytesRead = ms.Read(data, 0, data.Length)) != 0)
            {
                fileStream.Write(data, 0, bytesRead);
            }
        }

        #endregion

        #region deserialization helper methods

        /// <summary>
        /// Helper method to deserialize the binary data from the Isolated Storage
        /// to a SettingsData class.
        /// </summary>
        /// <param name="b">used as the binary data to be deserialized as</param>
        /// <returns>SettingsData, used as the current settings data in the game</returns>
        private SettingsData DeserializeSettings(byte[] b)
        {
            SettingsData settingsData = new SettingsData();

            // read binary data as a stream
            using (MemoryStream ms = new MemoryStream(b))
            {
                // initialize serializer as the type
                DataContractSerializer dcSerializer
                    = new DataContractSerializer(typeof(SettingsData));

                settingsData // read the stream to a SettingsData class
                    = (SettingsData)dcSerializer.ReadObject(ms);
            }

            return settingsData;
        }


        /// <summary>
        /// Helper method to deserialize the binary data from the Isolated Storage
        /// to a List of TrophieData.
        /// </summary>
        /// <param name="b">used as the binary data to be deserialized as</param>
        /// <returns>List of TrophieData, used as the current collection of trophies data in the game</returns>
        private List<TrophieData> DeserializeTrophies(byte[] b)
        {
            List<TrophieData> trophiesData = new List<TrophieData>();

            // read binary data as a stream
            using (MemoryStream ms = new MemoryStream(b))
            {
                // initialize serializer as the type
                DataContractSerializer dcSerializer
                    = new DataContractSerializer(typeof(List<TrophieData>));

                trophiesData // read the stream to a List<TrophieData>
                    = (List<TrophieData>)dcSerializer.ReadObject(ms);
            }

            DebugTrophies(trophiesData);

            return trophiesData;
        }

        #endregion

        #region serialization helper methods

        // serialize object
        private byte[] SerializeSettingsData(SettingsData data)
        {
            DataContractSerializer dc =
                new DataContractSerializer(data.GetType());

            MemoryStream ms = new MemoryStream();
            dc.WriteObject(ms, data);
            byte[] buff = ms.GetBuffer();

            ms.Close();
            ms.Dispose();

            return buff;

        }


        // serialize objects
        private byte[] SerializeTrophiesData(List<TrophieData> data)
        {
            DataContractSerializer dc =
                new DataContractSerializer(data.GetType());

            MemoryStream ms = new MemoryStream();
            dc.WriteObject(ms, data);
            byte[] buff = ms.GetBuffer();

            ms.Close();
            ms.Dispose();

            return buff;
        }

        //// serialize object
        //private string SerializeUserSettings()
        //{
        //    DataContractSerializer dc =
        //        new DataContractSerializer(Global.UserSettings.GetType());

        //    MemoryStream ms = new MemoryStream();
        //    dc.WriteObject(ms, Global.UserSettings);
        //    byte[] buff = ms.ToArray();

        //    ms.Close();
        //    ms.Dispose();


        //    return System.Text.Encoding.UTF8.GetString(buff, 0, buff.Length);
        //}

        #endregion

        #region isolatedstorage settings


        /*
         * create isoloated storage
         * increase size isolated storage
         * validations (file exits, isolated storage enabled, etc)
         */

        #endregion


        void DebugTrophies(List<TrophieData> TrophieElements)
        {
            System.Diagnostics.Debug.WriteLine("*** START ***");

            foreach (TrophieData t in TrophieElements)
            {
                System.Diagnostics.Debug.WriteLine(string.Empty);
                System.Diagnostics.Debug.WriteLine("t.Type = {0}", t.Type);
                System.Diagnostics.Debug.WriteLine("t.Value = {0}", t.Value);
                System.Diagnostics.Debug.WriteLine("t.Count = {0}", t.Count);
                //System.Diagnostics.Debug.WriteLine("t.DateTimeAchievement = {0}", t.DateTimeAchievement.HasValue);

            }

            System.Diagnostics.Debug.WriteLine("*** END ***");

        }
   
    }
}
