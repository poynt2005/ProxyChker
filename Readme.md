# Simple ProxyChker

### 說明

這個小工具是用來檢查 Windows 目前的全局 proxy 是使用哪一個 server  
並且可以測試這個 proxy 是否可用

### 更新紀錄

1. 2024-01-23: 新建倉庫，推代碼

### 原理

使用 COM 的 WScript 來進行 Registry 查詢，主要查詢 proxy 的欄位  
查到後，使用 COM 的 HttpRequest 使用 proxy 送 request 到微軟測試連線的 api，測試 proxy 是否可用

### 使用

打開 ProxyChker.NET.exe 後，可以看到目前電腦上使用的 proxy server  
點擊左側按鈕可以測試這個 proxy 是否可以正常工作

### 建置環境

1. .NET SDK > 8.0
2. Microsoft Build Version 143
3. Nodejs (with NPM) > 20

### 建置

1. 右鍵 build.ps1 以 powershell 運行
2. 等待生成 Release.zip

### 備註

1. 原本 libproxychker core lib 使用 _COINIT_MULTITHREADED_ 進行 COM library 初始化，不過不知道為啥 winforms STAThread 這邊一直會讓 CoInitializeEx 掛掉，[另一個項目](https://github.com/poynt2005/DriveUnlocker)就不會，所以只能改成使用 _COINIT_APARTMENTTHREADED_ (這個 flag 是專門用於 STAThread 程序的)
