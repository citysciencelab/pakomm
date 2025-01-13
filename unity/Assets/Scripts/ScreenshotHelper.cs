using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class ScreenshotHelper : MonoBehaviour
{
    public static ScreenshotHelper shared = null;

    private const int thumbnailWidth = 73;
    private const int thumbnailHeight = 73;

    [SerializeField]
    private Sprite previewThumbnail = null;


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
    }

    public IEnumerator SaveScreenshot(Texture2D screenshot, int sessionNumber, Pose cameraPose)
    {
        Debug.Log($"FirebaseHelper: Uploading screenshot {screenshot.ToString()} for session {sessionNumber} with camera's position {cameraPose.position.ToString()} and rotation {cameraPose.rotation.ToString()}.");

        string relativePath = $"Session_{sessionNumber}_{DateTime.Now:yyyy'-'MM'-'dd'T'HH'-'mm'-'ss}.png";

        var metadata = new Dictionary<string, string>()
        {
            { "Position", cameraPose.position.ToString() },
            { "Rotation", cameraPose.rotation.ToString() }
        };

        yield return FirebaseHelper.shared.UploadImageCoroutine(screenshot, relativePath, metadata);

        DgraphQuery.DQ.UpdateEventSessionScreenshotPath(sessionNumber, relativePath);

        LocalStorageHelper.shared.SaveImage(screenshot, relativePath);
    }


    public IEnumerator LoadThumbnailCoroutine(string fileName, Image imageToUpdate)
    {
        Debug.Log($"ScreenshotHelper: Loading thumbnail ...");

        var texture = new Texture2D(thumbnailWidth, thumbnailHeight);

        try
        {
            UpdateThumbnail(previewThumbnail, imageToUpdate);
        }
        catch (Exception e)
        {
            Debug.LogError($"ScreenshotHelper: Failed to load skeleton sprite: {e.Message}");
        }

        Debug.Log($"ScreenshotHelper: fileName: {fileName}, imageToUpdate: {imageToUpdate}");

        if (fileName == null || fileName == "")
        {
            Debug.LogError($"ScreenshotHelper: Path to thumbnail not available, thumbnail is skipped.");
            yield break;
        }

        if (imageToUpdate == null)
        {
            Debug.LogError($"ScreenshotHelper: Nullreference for imageToUpdate not allowed, thumbnail is skipped.");
            yield break;
        }

        yield return new WaitUntil(() => FirebaseHelper.shared != null && FirebaseHelper.shared.isInitialized);

        var loadTask = LocalStorageHelper.shared.LoadDataAsync(fileName);
        yield return new WaitUntil(() => loadTask.IsCompleted);

        if (loadTask.Exception == null && loadTask.Result != null)
        {
            UpdateThumbnail(loadTask.Result, imageToUpdate);
            Debug.Log($"ScreenshotHelper: {fileName} found on local device and assigned to Image.");
            yield break;
        }

        Debug.LogError($"ScreenshotHelper: {fileName} not found on local device, proceeding with download from Firebase: {loadTask.Exception}");

        var downloadTask = FirebaseHelper.shared.LoadDataAsync(fileName);
        yield return new WaitUntil(() => downloadTask.IsCompleted);

        if (downloadTask.Exception == null && downloadTask.Result != null)
        {
            UpdateThumbnail(downloadTask.Result, imageToUpdate);
            Debug.Log($"ScreenshotHelper: {fileName} downloaded from Firebase and assigned to Image.");
            
            LocalStorageHelper.shared.SaveData(downloadTask.Result, fileName);
            Debug.Log($"ScreenshotHelper: {fileName} cached locally.");
            yield break;
        }

        Debug.LogError($"ScreenshotHelper: Cannot load image in Firebase or locally. File might not exist: {fileName}: {loadTask.Exception}");
    }


    private void UpdateThumbnail(byte[] textureBytes, Image imageToUpdate)
    {
        if (textureBytes == null || imageToUpdate == null)
        {
            Debug.LogError($"ScreenshotHelper: Nullreference, cannot assign texture to image.");
            return;
        }

        var texture = new Texture2D(thumbnailWidth, thumbnailHeight);
        if (!texture.LoadImage(textureBytes))
        {
            Debug.LogError($"ScreenshotHelper: Failed to load texture from bytes.");
            return;
        }

        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        imageToUpdate.sprite = sprite;
    }

    private void UpdateThumbnail(Sprite sprite, Image imageToUpdate)
    {
        //if (sprite == null || imageToUpdate == null)
        //{
        //    Debug.LogError($"ScreenshotHelper: Nullreference, cannot assign texture to image.");
        //    return;
        //}

        //var sprite = Sprite.Create(texture, new Rect(0, 0, thumbnailWidth, thumbnailHeight), new Vector2(0.5f, 0.5f));
        imageToUpdate.sprite = sprite;

        Debug.Log($"ScreenshotHelper: Applied preview thumbnail");
    }


}
