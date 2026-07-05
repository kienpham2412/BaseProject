using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class GitHubPackageAutoDownloader : EditorWindow
{
    private string repoUrl = "https://github.com/kienpham2412/BaseProject";

    // --- Các class bọc dữ liệu để parse JSON từ GitHub API ---
    [System.Serializable]
    private class GitHubRelease
    {
        public string tag_name;
        public GitHubAsset[] assets;
    }

    [System.Serializable]
    private class GitHubAsset
    {
        public string name;
        public string browser_download_url;
    }

    [MenuItem("Tools/GitHub Auto Package Downloader")]
    public static void ShowWindow()
    {
        GetWindow<GitHubPackageAutoDownloader>("Auto Package Downloader");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Tự động tải .unitypackage từ Release mới nhất", EditorStyles.boldLabel);
        
        GUILayout.Space(5);
        EditorGUILayout.HelpBox("Nhập đường dẫn trang chủ của Repository (VD: https://github.com/user/repo). Script sẽ tự động tìm file .unitypackage ở bản Release mới nhất.", MessageType.Info);
        
        repoUrl = EditorGUILayout.TextField("GitHub Repo URL:", repoUrl);

        GUILayout.Space(10);
        if (GUILayout.Button("Tìm, Tải xuống và Import", GUILayout.Height(30)))
        {
            FetchLatestReleaseAndDownload(repoUrl);
        }
    }

    private async void FetchLatestReleaseAndDownload(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("[PackageDownloader] URL không được để trống!");
            return;
        }

        // Dùng Regex để tách owner và repo từ URL
        Match match = Regex.Match(url, @"github\.com/([^/]+)/([^/]+)");
        if (!match.Success)
        {
            Debug.LogError("[PackageDownloader] URL không đúng định dạng GitHub Repository!");
            return;
        }

        string owner = match.Groups[1].Value;
        string repo = match.Groups[2].Value.Replace(".git", ""); // Loại bỏ đuôi .git nếu người dùng copy nhầm
        string apiUrl = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";

        EditorUtility.DisplayProgressBar("Fetching Data", "Đang quét GitHub API để tìm bản Release mới nhất...", 0f);

        try
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
            {
                // GitHub API BẮT BUỘC phải có header User-Agent, nếu không sẽ bị lỗi 403 Forbidden
                webRequest.SetRequestHeader("User-Agent", "Unity-Editor-Auto-Downloader");

                var operation = webRequest.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[PackageDownloader] Không thể lấy thông tin Release (Có thể repo không có Release nào hoặc lỗi mạng): {webRequest.error}");
                    return;
                }

                // Parse JSON response
                string jsonResponse = webRequest.downloadHandler.text;
                GitHubRelease latestRelease = JsonUtility.FromJson<GitHubRelease>(jsonResponse);

                if (latestRelease == null || latestRelease.assets == null || latestRelease.assets.Length == 0)
                {
                    Debug.LogError("[PackageDownloader] Bản Release mới nhất không chứa bất kỳ file (asset) đính kèm nào.");
                    return;
                }

                // Tìm file có đuôi .unitypackage
                string downloadUrl = null;
                string packageName = null;

                foreach (var asset in latestRelease.assets)
                {
                    if (asset.name.EndsWith(".unitypackage"))
                    {
                        downloadUrl = asset.browser_download_url;
                        packageName = asset.name;
                        break; // Lấy file đầu tiên tìm thấy
                    }
                }

                if (string.IsNullOrEmpty(downloadUrl))
                {
                    Debug.LogError($"[PackageDownloader] Phiên bản {latestRelease.tag_name} không có file .unitypackage nào!");
                    return;
                }

                Debug.Log($"[PackageDownloader] Tìm thấy: {packageName} (Phiên bản: {latestRelease.tag_name}). Đang tiến hành tải...");
                
                // Chuyển qua hàm tải file
                await DownloadAndImportPackageTask(downloadUrl);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private async Task DownloadAndImportPackageTask(string downloadUrl)
    {
        string tempFilePath = Path.Combine(Application.temporaryCachePath, "downloaded_package.unitypackage");

        try
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(downloadUrl))
            {
                var operation = webRequest.SendWebRequest();

                while (!operation.isDone)
                {
                    EditorUtility.DisplayProgressBar(
                        "Downloading Package", 
                        $"Đang tải... {webRequest.downloadProgress * 100:0.0}%", 
                        webRequest.downloadProgress
                    );
                    await Task.Yield();
                }

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[PackageDownloader] Lỗi khi tải package: {webRequest.error}");
                    return;
                }

                File.WriteAllBytes(tempFilePath, webRequest.downloadHandler.data);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        // --- Đăng ký Callbacks và Import ---
        AssetDatabase.importPackageCompleted += OnImportCompleted;
        AssetDatabase.importPackageCancelled += OnImportCancelled;
        AssetDatabase.importPackageFailed += OnImportFailed;

        AssetDatabase.ImportPackage(tempFilePath, true);

        void OnImportCompleted(string packageName)
        {
            Debug.Log($"[PackageDownloader] Import thành công package: {packageName}");
            CleanUpTempFile();
        }

        void OnImportCancelled(string packageName)
        {
            Debug.Log("[PackageDownloader] Người dùng đã hủy quá trình Import.");
            CleanUpTempFile();
        }

        void OnImportFailed(string packageName, string errorMessage)
        {
            Debug.LogError($"[PackageDownloader] Import thất bại: {errorMessage}");
            CleanUpTempFile();
        }

        void CleanUpTempFile()
        {
            AssetDatabase.importPackageCompleted -= OnImportCompleted;
            AssetDatabase.importPackageCancelled -= OnImportCancelled;
            AssetDatabase.importPackageFailed -= OnImportFailed;

            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }
}