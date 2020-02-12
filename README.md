# KeyManager
Key manager is a POC library that can be used to store and retrieve keys/tokens securely on Linux, Windows and MacOS (Not supported).

## Linux
On Linux, it uses [Kernel Key Retention Service](https://www.kernel.org/doc/html/latest/security/keys/core.html#id2) to cache authentication tokens in the kernel for use by userspace apps.

## Windows
Windows [DPAPI](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.protecteddata?view=netframework-4.8) is used to protect and unprotect data before writing it to and reading it from a file.

## MacOS
To be implemented.

# Running the Sample
1. Clone project from GitHub to your computer (Windows or Linux).
2. Install DotNet core on your computer if not yet installed.
3. Navigate to the cloned project and run `dotnet build -c Release -r win10-x64` for Windows and `dotnet build -c Release -r ubuntu.16.10-x64` for Ubuntu. This builds the projects, and generates an executable.
4. Navigate to `KeyManager\TestConsoleApp\bin\` and find `TestConsoleApp.exe` on Windows and `TestConsoleApp` on Ubuntu.
5. Run `.\TestConsoleApp.exe -c {CLIENT-ID} -d {CACHE_FILE_DIR}` on Windows and `.\TestConsoleApp -c {CLIENT-ID} -d {CACHE_FILE_DIR}` on Ubuntu.
6. You should authenticate once, and subsequent calls will use the token cache.
