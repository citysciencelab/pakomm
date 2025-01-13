using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class LocalStorageHelper : MonoBehaviour
{
    public static LocalStorageHelper shared = null;

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


    public void SaveImage(Texture2D image, string fileName)
    {
        byte[] bytes = image.EncodeToPNG();
        SaveData(bytes, fileName);
    }

    public void SaveData(byte[] data, string fileName)
    {
        Debug.Log($"LocalStorageHelper: {fileName} will be cached on device.");

        string filePath = GetAbsoluteFilePath(fileName);
        File.WriteAllBytes(filePath, data);
    }


    public async Task<byte[]> LoadDataAsync(string fileName)
    {
        Debug.Log($"LocalStorageHelper: {fileName} will be tried to load from device.");

        if (!FileExistsInLocalStorage(fileName))
        {
            Debug.Log($"LocalStorageHelper: {fileName} not found on local device.");
            return null;
        }
        else
        {
            var data = await File.ReadAllBytesAsync(GetAbsoluteFilePath(fileName));
            Debug.Log($"LocalStorageHelper: {fileName} found on local device.");
            return data;
        }
    }


    private bool FileExistsInLocalStorage(string fileName)
    {
        var filePath = GetAbsoluteFilePath(fileName);
        return File.Exists(GetAbsoluteFilePath(filePath));
    }

    private string GetAbsoluteFilePath(string fileName)
    {
        //Debug.Log($"LocalStorageHelper: Absolute path for {fileName} is {Path.Combine(Application.temporaryCachePath, fileName)}.");
        //return Path.Combine(Application.temporaryCachePath, fileName);
        return Path.Combine(Application.persistentDataPath, fileName);
//#if !UNITY_EDITOR
//		return Path.Combine(Application.dataPath, fileName);
//#endif
//#if UNITY_EDITOR
//        return Path.Combine(Path.GetDirectoryName(Application.dataPath),fileName);
//#endif
    }
}