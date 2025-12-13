// 3. The Path Logic (Handling OS differences)

using System.Runtime.InteropServices;

class PathResolver
{
  private readonly string[] _paths;
  private readonly bool _isWindows;

  public PathResolver()
  {
    var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
    var separator = Path.PathSeparator; // ';' on Windows, ':' on Linux

    _paths = pathEnv.Split(separator, StringSplitOptions.RemoveEmptyEntries);
    _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
  }

  public string? FindExecutable(string command)
  {
    foreach (var dir in _paths)
    {
      if (!Directory.Exists(dir)) continue;

      // Strategy 1: Exact match (Linux mostly)
      var fullPath = Path.Combine(dir, command);
      if (IsExecutable(fullPath)) return fullPath;

      // Strategy 2: Windows Extensions (cmd.exe, git.exe, etc.)
      if (!_isWindows) continue;
      string[] extensions = { ".exe", ".bat", ".cmd", ".com" };
      foreach (var ext in extensions)
      {
        var fullPathWithExt = fullPath + ext;
        if (File.Exists(fullPathWithExt)) return fullPathWithExt;
      }
    }

    return null;
  }

  private bool IsExecutable(string filePath)
  {
    if (!File.Exists(filePath)) return false;

    // If Windows, existence is enough (we handled extensions above)
    if (_isWindows) return true;

    // If Linux/Mac, check the +x permission bit
    try
    {
      var mode = File.GetUnixFileMode(filePath);
      return (mode & (UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute)) != 0;
    }
    catch
    {
      return false;
    }
  }
}