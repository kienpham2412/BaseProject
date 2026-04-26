using UnityEngine;
using UnityEditor;
using System.IO;

public class AddGithubWorkflow
{
    [MenuItem("Tools/Move GitHub Folder to Root")]
    public static void MoveGitHubFolder()
    {
        string sourcePath = Path.Combine(Application.dataPath, "Modules", "Essentials", "GitHubWorkFlow", "github");
        string projectRootPath = Directory.GetParent(Application.dataPath).FullName;
        string destinationPath = Path.Combine(projectRootPath, ".github");

        if (!Directory.Exists(sourcePath))
        {
            Debug.LogError($"[GitHub Mover] Thất bại: Không tìm thấy thư mục nguồn tại: {sourcePath}");
            return;
        }

        if (Directory.Exists(destinationPath))
        {
            Debug.LogWarning($"[GitHub Mover] Cảnh báo: Thư mục đích đã tồn tại tại: {destinationPath}. Vui lòng xóa hoặc di chuyển nó trước.");
            return;
        }

        try
        {
            // 1. Di chuyển thư mục và đổi tên thành .github
            Directory.Move(sourcePath, destinationPath);

            // 2. Xóa file .meta của chính thư mục nguồn (còn sót lại ở Assets/...)
            string rootMetaFilePath = sourcePath + ".meta";
            if (File.Exists(rootMetaFilePath))
            {
                File.Delete(rootMetaFilePath);
            }

            // 3. QUAN TRỌNG: Quét và xóa toàn bộ file .meta bên trong thư mục .github vừa chuyển ra ngoài
            string[] metaFiles = Directory.GetFiles(destinationPath, "*.meta", SearchOption.AllDirectories);
            foreach (string metaFile in metaFiles)
            {
                File.Delete(metaFile);
            }

            // 4. Làm mới lại AssetDatabase
            AssetDatabase.Refresh();

            Debug.Log($"[GitHub Mover] Thành công! Đã di chuyển thành {destinationPath} và dọn dẹp sạch {metaFiles.Length} file .meta thừa.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GitHub Mover] Đã xảy ra lỗi trong quá trình di chuyển: {e.Message}");
        }
    }
}