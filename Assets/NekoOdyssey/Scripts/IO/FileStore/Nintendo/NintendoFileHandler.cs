using System.Text;
using UnityEngine;

namespace NekoOdyssey.Scripts.IO.FileStore.Nintendo
{
#if UNITY_SWITCH
    /// <summary>
    /// This static class handles all initialization and error handling for reading and writing save data.
    /// Although this sample only saves and loads string data, the same concept can be used to save other data types
    /// when combined with various serialization techniques.
    /// </summary>
    public class NintendoFileHandler
    {
        // The UserHandle might be needed by other SDK and library features, so keep it accessible
        public static nn.account.UserHandle userHandle = new nn.account.UserHandle();

        /// <summary>
        /// Define a private constructor to prevent accidental instantiation.
        /// </summary>
        private NintendoFileHandler()
        {
        }

        // Stores the name of the save data mount point. This is like the drive letter.
        private static readonly string _mountName = "save";

        /// <summary>
        /// The static constructor initializes the account library and invokes the Initialize method.
        /// </summary>
        static NintendoFileHandler()
        {
            // The account library only needs to be initialized once for the entire application.
            nn.account.Account.Initialize();

            // Invoke the Initialize method to get the preselected user and mount the save data archive.
            NintendoFileHandler.Initialize();
        }

        /// <summary>
        /// Gets the preselected user information and mounts the save data drive.
        /// </summary>
        public static void Initialize()
        {
            nn.account.Uid userId = nn.account.Uid.Invalid;
            nn.Result result;

            // To use the nn.account.Account.OpenUser() method (required when Startup User Account is set to None),
            // NN_ACCOUNT_OPENUSER_ENABLE must be defined in Player > Other Settings > Scripting Define Symbols.
#if NN_ACCOUNT_OPENUSER_ENABLE
                Debug.Log("NN_ACCOUNT_OPENUSER_ENABLE is defined--assuming Startup User Account is None");

                // Assume Startup User Account is set to None
                result = nn.account.Account.ShowUserSelector(ref userId);
                while (nn.account.Account.ResultCancelledByUser.Includes(result))
                {
                    Debug.LogError("You must select a user account");
                    result = nn.account.Account.ShowUserSelector(ref userId);
                }
                
                // Open the selected user account
                result = nn.account.Account.OpenUser(ref userHandle, userId);
                if (!result.IsSuccess())
                {
                    // Do not let the player proceed to gameplay without opening an account.
                    // Handle this in a suitable way based on your game design. Do not just crash the program.
                    nn.Nn.Abort($"Failed to open the selected user account: {result.ToString()}");
                }

                result = nn.fs.SaveData.Ensure(userId);
                if (nn.fs.SaveData.ResultUsableSpaceNotEnoughForSaveData.Includes(result))
                {
                    // This means the system memory is full, and the user did not free any space when prompted.
                    // Handle this in a suitable way based on your game design. Do not just crash the program.
                    nn.Nn.Abort("nn.fs.SaveData.Ensure() failed due to insufficient free space");
                }
#else
            Debug.Log("NN_ACCOUNT_OPENUSER_ENABLE is not defined--assuming Startup User Account is Required");

            // Assume Startup User Account is set to Required
            // Open the user that was selected before the application started.
            if (nn.account.Account.TryOpenPreselectedUser(ref userHandle))
            {
                // Get the user ID of the preselected user account.
                result = nn.account.Account.GetUserId(ref userId, userHandle);
            }
            else
            {
                // This should not be possible in retail
                nn.Nn.Abort("TryOpenPreselectedUser failed");
            }
#endif

            // Mount the save data archive as "save" for the selected user account.
            Debug.Log("Mounting save data archive");
            result = nn.fs.SaveData.Mount(_mountName, userId);

            // This error handling is optional.
            // The mount operation will not fail unless the save data is already mounted or the mount name is in use.
            if (nn.fs.FileSystem.ResultTargetLocked.Includes(result))
            {
                // Save data for specified user ID is already mounted. Get account name and display an error.
                nn.account.Nickname nickname = new nn.account.Nickname();
                nn.account.Account.GetNickname(ref nickname, userId);
                Debug.LogErrorFormat("The save data for {0} is already mounted: {1}", nickname.name, result.ToString());
            }
            else if (nn.fs.FileSystem.ResultMountNameAlreadyExists.Includes(result))
            {
                // The specified mount name is already in use.
                Debug.LogErrorFormat("The mount name '{0}' is already in use: {1}", _mountName, result.ToString());
            }

            // Abort if any of the initialization steps failed.
            result.abortUnlessSuccess();
        }

        /// <summary>
        /// Unmounts the save data archive and frees all resources.
        /// The save data drive is automatically unmounted by the system when the application terminates.
        /// If you want to use save data for the entire duration of the application, you do not need to call this method.
        /// </summary>
        public static void Cleanup()
        {
            Debug.Log("Unmounting save data archive");
            nn.fs.FileSystem.Unmount(_mountName);
        }

        public static bool Delete(string filename)
        {
            // An nn.Result object is used to get the result of NintendoSDK plug-in operations.
            nn.Result result;
            string filePath = string.Format("{0}:/{1}", _mountName, filename);

            // The NintendoSDK plug-in uses a FileHandle object for file operations.
            nn.fs.FileHandle handle = new nn.fs.FileHandle();

            // Attempt to open the file in read-only mode.
            result = nn.fs.File.Delete(filePath);

            return result.IsSuccess();
        }

        /// <summary>
        /// Loads user account save data from the specified filename into the specified string buffer.
        /// If the specified filename is not found, the content of the buffer is unchanged.
        /// </summary>
        /// <param name="saveData">A string buffer to store the save data.</param>
        /// <param name="filename">The name of the file that contains the data to load.</param>
        /// <returns>Returns whether the load operation was successful.</returns>
        public static bool Load(ref string saveData, string filename)
        {
            // An nn.Result object is used to get the result of NintendoSDK plug-in operations.
            nn.Result result;

            // Append the filename to the mount name to get the full path to the save data file.
            string filePath = string.Format("{0}:/{1}", _mountName, filename);

            // The NintendoSDK plug-in uses a FileHandle object for file operations.
            nn.fs.FileHandle handle = new nn.fs.FileHandle();

            // Attempt to open the file in read-only mode.
            result = nn.fs.File.Open(ref handle, filePath, nn.fs.OpenFileMode.Read);
            if (!result.IsSuccess())
            {
                if (nn.fs.FileSystem.ResultPathNotFound.Includes(result))
                {
                    Debug.LogFormat(">>save_game<< File not found: {0}", filePath);

                    return false;
                }
                else
                {
                    Debug.LogErrorFormat(">>save_game<< Unable to open {0}: {1}", filePath, result.ToString());

                    return false;
                }
            }

            // Get the file size.
            long fileSize = 0;
            nn.fs.File.GetSize(ref fileSize, handle);
            // Allocate a buffer that matches the file size.
            byte[] data = new byte[fileSize];
            // Read the save data into the buffer.
            nn.fs.File.Read(handle, 0, data, fileSize);
            // Close the file.
            nn.fs.File.Close(handle);
            // Decode the UTF8-encoded data and store it in the string buffer.
            saveData = Encoding.UTF8.GetString(data);

            return true;
        }

        /// <summary>
        /// Saves the specified string to the specified file located in the user account save data.
        /// This method prevents the application from exiting until the write operation is complete.
        /// </summary>
        /// <param name="saveData">The string data to save to user account save data.</param>
        /// <param name="filename">The name of the file to save the string to.</param>
        /// <returns>Returns whether the save was successful.</returns>
        public static bool Save(string saveData, string filename, out string filePath)
        {
#if UNITY_SWITCH && !UNITY_EDITOR
                // This method prevents the user from quitting the game while saving.
                // This method must be called on the main thread.
                UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif
            bool result = _Save(saveData, filename, out filePath);

#if UNITY_SWITCH && !UNITY_EDITOR
                // Stop preventing the system from terminating the game while saving.
                UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif

            return result;
        }

        /// <summary>
        /// Saves the specified string to the specified file located in the user account save data.
        /// Use this version of the Save method if you are calling it from something other than the main thread.
        /// Be sure to call UnityEngine.Switch.Notification.EnterExitRequestHandlingSection from the main thread before invoking this function.
        /// </summary>
        /// <param name="saveData">The string data to save to user account save data.</param>
        /// <param name="filename">The name of the file to save the string to.</param>
        /// <returns>Returns whether the save was successful.</returns>
        static bool _Save(string saveData, string filename, out string filePath)
        {
            // An nn.Result object is used to get the result of NintendoSDK plug-in operations.
            nn.Result result;

            // Append the filename to the mount name to get the full path to the save data file.
            filePath = string.Format("{0}:/{1}", _mountName, filename);

            // Convert the text to UTF-8-encoded bytes.
            byte[] data = Encoding.UTF8.GetBytes(saveData);

            // The NintendoSDK plug-in uses a FileHandle object for file operations.
            nn.fs.FileHandle handle = new nn.fs.FileHandle();

            while (true)
            {
                // Attempt to open the file in write mode.
                result = nn.fs.File.Open(ref handle, filePath, nn.fs.OpenFileMode.Write);
                // Check if file was opened successfully.
                if (result.IsSuccess())
                {
                    // Exit the loop because the file was successfully opened.
                    break;
                }
                else
                {
                    if (nn.fs.FileSystem.ResultPathNotFound.Includes(result))
                    {
                        // Create a file with the size of the encoded data if no entry exists.
                        result = nn.fs.File.Create(filePath, data.LongLength);

                        // Check if the file was successfully created.
                        if (!result.IsSuccess())
                        {
                            Debug.LogErrorFormat("Failed to create {0}: {1}", filePath, result.ToString());

                            return false;
                        }
                    }
                    else
                    {
                        // Generic fallback error handling for debugging purposes.
                        Debug.LogErrorFormat("Failed to open {0}: {1}", filePath, result.ToString());

                        return false;
                    }
                }
            }

            // Write the data to the journaling area.
            // Verify that the journaling area is sufficiently large to hold the encoded data.
            // The size of the journaling area is set under PlayerSettings > Publishing Settings > User Account and Save Data > Save Data Journal Size.
            // NOTE: 32 KB of the journaling area is used by the file system.
            //       The file system uses a 16 KB block size, so each file will consume at least 16384 bytes.
            Debug.LogFormat("Writing {0} bytes to save data...", data.LongLength);

            // Set the file to the size of the binary data.
            result = nn.fs.File.SetSize(handle, data.LongLength);

            // You do not need to handle this error if you are sure there will be enough space.
            if (nn.fs.FileSystem.ResultUsableSpaceNotEnough.Includes(result))
            {
                Debug.LogErrorFormat("Insufficient space to write {0} bytes to {1}", data.LongLength, filePath);
                nn.fs.File.Close(handle);

                return false;
            }

            // NOTE: Calling File.Write() with WriteOption.Flush incurs two write operations.
            result = nn.fs.File.Write(handle, 0, data, data.LongLength, nn.fs.WriteOption.Flush);

            // You do not need to handle this error here if you are not using nn.fs.OpenFileMode.AllowAppend
            if (nn.fs.FileSystem.ResultUsableSpaceNotEnough.Includes(result))
            {
                Debug.LogErrorFormat("Insufficient space to write {0} bytes to {1}", data.LongLength, filePath);
            }

            // The file must be closed before committing.
            nn.fs.File.Close(handle);

            // Verify that the write operation was successful before committing.
            if (!result.IsSuccess())
            {
                Debug.LogErrorFormat("Failed to write {0}: {1}", filePath, result.ToString());
                return false;
            }

            // This method moves the data from the journaling area to the main storage area.
            // If you do not call this method, all changes will be lost when the application closes.
            // Only call this when you are sure that all previous operations succeeded.
            result = nn.fs.FileSystem.Commit(_mountName);

            return result.IsSuccess();
        }

        public static bool Commit()
        {
            // An nn.Result object is used to get the result of NintendoSDK plug-in operations.
            nn.Result result;

            // This method moves the data from the journaling area to the main storage area.
            // If you do not call this method, all changes will be lost when the application closes.
            // Only call this when you are sure that all previous operations succeeded.
            result = nn.fs.FileSystem.Commit(_mountName);

            return result.IsSuccess();
        }

        public static bool SaveRaw(byte[] data, string filename)
        {
            // An nn.Result object is used to get the result of NintendoSDK plug-in operations.
            nn.Result result;

            // Append the filename to the mount name to get the full path to the save data file.
            string filePath = string.Format("{0}:/{1}", _mountName, filename);

            // The NintendoSDK plug-in uses a FileHandle object for file operations.
            nn.fs.FileHandle handle = new nn.fs.FileHandle();

            while (true)
            {
                // Attempt to open the file in write mode.
                result = nn.fs.File.Open(ref handle, filePath, nn.fs.OpenFileMode.Write);
                // Check if file was opened successfully.
                if (result.IsSuccess())
                {
                    // Exit the loop because the file was successfully opened.
                    break;
                }
                else
                {
                    if (nn.fs.FileSystem.ResultPathNotFound.Includes(result))
                    {
                        // Create a file with the size of the encoded data if no entry exists.
                        result = nn.fs.File.Create(filePath, data.LongLength);

                        // Check if the file was successfully created.
                        if (!result.IsSuccess())
                        {
                            Debug.LogErrorFormat("Failed to create {0}: {1}", filePath, result.ToString());

                            return false;
                        }
                    }
                    else
                    {
                        // Generic fallback error handling for debugging purposes.
                        Debug.LogErrorFormat("Failed to open {0}: {1}", filePath, result.ToString());

                        return false;
                    }
                }
            }

            // Write the data to the journaling area.
            // Verify that the journaling area is sufficiently large to hold the encoded data.
            // The size of the journaling area is set under PlayerSettings > Publishing Settings > User Account and Save Data > Save Data Journal Size.
            // NOTE: 32 KB of the journaling area is used by the file system.
            //       The file system uses a 16 KB block size, so each file will consume at least 16384 bytes.
            Debug.LogFormat("Writing {0} bytes to save data...", data.LongLength);

            // Set the file to the size of the binary data.
            result = nn.fs.File.SetSize(handle, data.LongLength);

            // You do not need to handle this error if you are sure there will be enough space.
            if (nn.fs.FileSystem.ResultUsableSpaceNotEnough.Includes(result))
            {
                Debug.LogErrorFormat("Insufficient space to write {0} bytes to {1}", data.LongLength, filePath);
                nn.fs.File.Close(handle);

                return false;
            }

            // NOTE: Calling File.Write() with WriteOption.Flush incurs two write operations.
            result = nn.fs.File.Write(handle, 0, data, data.LongLength, nn.fs.WriteOption.Flush);

            // You do not need to handle this error here if you are not using nn.fs.OpenFileMode.AllowAppend
            if (nn.fs.FileSystem.ResultUsableSpaceNotEnough.Includes(result))
            {
                Debug.LogErrorFormat("Insufficient space to write {0} bytes to {1}", data.LongLength, filePath);
            }

            // The file must be closed before committing.
            nn.fs.File.Close(handle);
            Debug.Log($">>file<< close {filePath}");

            // Verify that the write operation was successful before committing.
            if (!result.IsSuccess())
            {
                Debug.LogErrorFormat("Failed to write {0}: {1}", filePath, result.ToString());
                return false;
            }

            // This method moves the data from the journaling area to the main storage area.
            // If you do not call this method, all changes will be lost when the application closes.
            // Only call this when you are sure that all previous operations succeeded.
            result = nn.fs.FileSystem.Commit(_mountName);
            Debug.Log($">>file<< commit {_mountName} {result.IsSuccess()}");

            return result.IsSuccess();
        }

        public static bool Exists(string filename)
        {
            // An nn.Result object is used to get the result of NintendoSDK plug-in operations.
            nn.Result result;

            // Append the filename to the mount name to get the full path to the save data file.
            string filePath = string.Format("{0}:/{1}", _mountName, filename);

            // The NintendoSDK plug-in uses a FileHandle object for file operations.
            nn.fs.FileHandle handle = new nn.fs.FileHandle();

            // Attempt to open the file in read-only mode.
            result = nn.fs.File.Open(ref handle, filePath, nn.fs.OpenFileMode.Read);
            if (!result.IsSuccess())
            {
                if (nn.fs.FileSystem.ResultPathNotFound.Includes(result))
                {
                    Debug.LogFormat(">>save_game<< File not found: {0}", filePath);

                    return false;
                }
                else
                {
                    Debug.LogErrorFormat(">>save_game<< Unable to open {0}: {1}", filePath, result.ToString());

                    return false;
                }
            }

            // Close the file.
            nn.fs.File.Close(handle);

            return true;
        }
    }
#endif
}