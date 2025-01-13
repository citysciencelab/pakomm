using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Firebase;
using Firebase.Storage;
using Firebase.Auth;
using System.Linq;
using System.Threading.Tasks;

public class FirebaseHelper : MonoBehaviour
{
    [SerializeField, Tooltip("Path in the Firebase Store where the thumbnails are saved, e.g. /thumbnails/.")]

    public static FirebaseHelper shared;

    public bool isInitialized { get; private set; }


    // Start is called before the first frame update
    void Awake()
    {
        if (shared != null)
        {
            GameObject.Destroy(shared);
        }
        else
        {
            shared = this;
        }

        this.InitFirebase();
    }


    private void InitFirebase()
    {
        Debug.Log("InitFirebase");
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Set a flag here to indicate whether Firebase is ready to use by your app.
                this.isInitialized = true;
                if (FirebaseAuth.DefaultInstance.CurrentUser == null)
                {
                    Debug.Log("FirebaseHelper: Login requiered ...");
                    CreateOrLoginToAnonymousUser();
                }
                else
                {
                    Debug.Log($"FirebaseHelper: {FirebaseAuth.DefaultInstance.CurrentUser.UserId} is still logged in.");
                }
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }


    /// <summary>
    /// Anonymously login to Firebase to get a UserID.
    /// </summary>
    private void CreateOrLoginToAnonymousUser()
    {
        Debug.Log("CreateOrLoginToAnonymousUser");

        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            // _currentUserID = newUser.UserId;
            // PlayerPrefs.SetString(FIREBASE_USERID_KEY, newUser.UserId);
            // PlayerPrefs.Save();

            Debug.Log("FirebaseReady!");
        });
    }


    public IEnumerator UploadImageCoroutine(Texture2D image, string relativePath, Dictionary<string, string> metadata)
    {
        Debug.Log($"FirebaseHelper: Uploading image {image.ToString()} to {relativePath} with metadata{metadata.Select(kvp => $" {kvp.Key}:{kvp.Value}")}.");

        var bytes = image.EncodeToPNG();

        var metadataChange = new MetadataChange()
        {
            ContentEncoding = "image/png",
            CustomMetadata = metadata,
        };

        var screenshotReference = FirebaseStorage.DefaultInstance.GetReference(relativePath);

        var uploadTask = screenshotReference.PutBytesAsync(bytes, metadataChange);
        yield return new WaitUntil(() => uploadTask.IsCompleted);

        if (uploadTask.Exception != null)
        {
            Debug.LogError($"Failed to upload because {uploadTask.Exception}");
            yield break;
        }

        var getUrlTask = screenshotReference.GetDownloadUrlAsync();
        yield return new WaitUntil(() => getUrlTask.IsCompleted);

        if (getUrlTask.Exception != null)
        {
            Debug.LogError($"Failed to upload because {uploadTask.Exception}");
            yield break;
        }

        Debug.Log($"Download from {getUrlTask.Result}");

        // DgraphQuery.DQ.AddScreenshotPath(int sessionNumber, string path);

        Debug.Log($"Screenshot's Firebase path added to EventSession in database {getUrlTask.Result}");
    }



    public IEnumerator DownloadImageCoroutine(string screenshotFileName, Action<byte[]> callback)
    {
        var screenshotReference = FirebaseStorage.DefaultInstance.GetReference(screenshotFileName);

        var downloadTask = screenshotReference.GetBytesAsync(long.MaxValue);
        yield return new WaitUntil(() => downloadTask.IsCompleted);

        callback(downloadTask.Result);
    }



    public Task<byte[]> LoadDataAsync(string fileName)
    {
        Debug.Log($"FirebaseHelper: Will download data from Firebase: {fileName}");

        var screenshotReference = FirebaseStorage.DefaultInstance.GetReference(fileName);

        return screenshotReference.GetBytesAsync(long.MaxValue);
    }
}