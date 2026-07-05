using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.IO;
using System.Threading.Tasks;
using System;

public class EssentialsPackageUploader : EditorWindow
{
    private string githubToken = "";
    private string versionTag = "v1.0.0";
    private string releaseTitle = "Bản cập nhật Essentials";
    private string releaseNotes = "Mô tả nội dung bản cập nhật...";
    
    private const string REPO_OWNER = "kienpham2412";
    private const string REPO_NAME = "BaseProject";
    private const string FOLDER_TO_EXPORT = "Assets/Modules/Essentials";
    private const string PACKAGE_NAME = "Essentials.unitypackage";
    private const string PREFS_TOKEN_KEY = "GitHub_PAT_Token";

    [MenuItem("Tools/Export and Upload Essentials")]
    public static void ShowWindow()
    {
        GetWindow<EssentialsPackageUploader>("Essentials Uploader");
    }

    private void OnEnable()
    {
        githubToken = EditorPrefs.GetString(PREFS_TOKEN_KEY, "");
    }

    private void OnGUI()
    {
        GUILayout.Label("Cấu hình GitHub Release", EditorStyles.boldLabel);

        githubToken = EditorGUILayout.TextField("GitHub Personal Access Token:", githubToken);
        versionTag = EditorGUILayout.TextField("Version Tag (vd: v1.0.1):", versionTag);
        releaseTitle = EditorGUILayout.TextField("Tiêu đề Release:", releaseTitle);
        
        GUILayout.Label("Ghi chú Release:");
        releaseNotes = EditorGUILayout.TextArea(releaseNotes, GUILayout.Height(60));

        if (GUI.changed)
        {
            EditorPrefs.SetString(PREFS_TOKEN_KEY, githubToken);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Export & Upload to GitHub", GUILayout.Height(40)))
        {
            if (string.IsNullOrEmpty(githubToken))
            {
                EditorUtility.DisplayDialog("Lỗi", "Vui lòng nhập GitHub Personal Access Token.", "OK");
                return;
            }
            
            if (!AssetDatabase.IsValidFolder(FOLDER_TO_EXPORT))
            {
                EditorUtility.DisplayDialog("Lỗi", $"Không tìm thấy thư mục: {FOLDER_TO_EXPORT}", "OK");
                return;
            }

            ExportAndUpload();
        }
    }

    private async void ExportAndUpload()
    {
        string exportPath = Path.Combine(Application.dataPath, "../", PACKAGE_NAME);

        try
        {
            // 1. Export Package
            EditorUtility.DisplayProgressBar("Tiến trình", "Đang export unitypackage...", 0.2f);
            AssetDatabase.ExportPackage(FOLDER_TO_EXPORT, exportPath, ExportPackageOptions.Recurse);
            Debug.Log($"Đã export package thành công tại: {exportPath}");

            // 2. Tạo Release trên GitHub
            EditorUtility.DisplayProgressBar("Tiến trình", "Đang tạo GitHub Release...", 0.5f);
            string releaseId = await CreateGitHubRelease();
            
            if (string.IsNullOrEmpty(releaseId))
            {
                throw new Exception("Không thể tạo Release. Vui lòng kiểm tra lại Token hoặc kết nối.");
            }

            // 3. Upload File Package
            EditorUtility.DisplayProgressBar("Tiến trình", "Đang upload package lên Release...", 0.8f);
            await UploadAssetToRelease(releaseId, exportPath);

            EditorUtility.DisplayDialog("Thành công", $"Đã upload thành công {PACKAGE_NAME} lên GitHub Releases!", "Tuyệt vời");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Lỗi trong quá trình xử lý: {ex.Message}");
            EditorUtility.DisplayDialog("Lỗi", $"Có lỗi xảy ra, xem Console để biết chi tiết.\n{ex.Message}", "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            // Tùy chọn: Xóa file tạm sau khi upload
            if (File.Exists(exportPath))
            {
                File.Delete(exportPath);
            }
        }
    }

    private async Task<string> CreateGitHubRelease()
    {
        string url = $"https://api.github.com/repos/{REPO_OWNER}/{REPO_NAME}/releases";

        var requestData = new ReleaseData
        {
            tag_name = versionTag,
            name = releaseTitle,
            body = releaseNotes,
            draft = false,
            prerelease = false
        };

        string json = JsonUtility.ToJson(requestData);

        using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Authorization", $"Bearer {githubToken}");
            req.SetRequestHeader("Accept", "application/vnd.github+json");
            req.SetRequestHeader("X-GitHub-Api-Version", "2022-11-28");
            req.SetRequestHeader("User-Agent", "Unity-Editor");

            var operation = req.SendWebRequest();
            while (!operation.isDone) await Task.Delay(100);

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Lỗi tạo Release: {req.error}\nResponse: {req.downloadHandler.text}");
                return null;
            }

            ReleaseResponse response = JsonUtility.FromJson<ReleaseResponse>(req.downloadHandler.text);
            return response.id.ToString();
        }
    }

    private async Task UploadAssetToRelease(string releaseId, string filePath)
    {
        string url = $"https://uploads.github.com/repos/{REPO_OWNER}/{REPO_NAME}/releases/{releaseId}/assets?name={PACKAGE_NAME}";
        byte[] fileData = File.ReadAllBytes(filePath);

        using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(fileData);
            req.downloadHandler = new DownloadHandlerBuffer();
            
            // Octet-stream dùng cho các file nhị phân
            req.SetRequestHeader("Content-Type", "application/octet-stream"); 
            req.SetRequestHeader("Authorization", $"Bearer {githubToken}");
            req.SetRequestHeader("Accept", "application/vnd.github+json");
            req.SetRequestHeader("X-GitHub-Api-Version", "2022-11-28");
            req.SetRequestHeader("User-Agent", "Unity-Editor");

            var operation = req.SendWebRequest();
            while (!operation.isDone) await Task.Delay(100);

            if (req.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Lỗi upload file: {req.error}\nResponse: {req.downloadHandler.text}");
            }
            
            Debug.Log("Upload thành công!");
        }
    }

    [Serializable]
    private class ReleaseData
    {
        public string tag_name;
        public string name;
        public string body;
        public bool draft;
        public bool prerelease;
    }

    [Serializable]
    private class ReleaseResponse
    {
        public int id;
        public string upload_url;
    }
}