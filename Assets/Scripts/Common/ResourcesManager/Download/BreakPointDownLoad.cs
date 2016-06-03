using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;


namespace GameLoader
{
    /// <summary>
    /// 断点续传异步下载--包装类
    /// 支持断点续传
    /// </summary>
    public class BreakPointDownLoad
    {
        private string url;
        private string localAbsPath;
        private bool isShowProgress;
        private bool isSetNetwork;
        private Action<string> onCompleted;
        private Action<string> onFail;
        private long bytesDownloaded = 0;
        private long needDownloadSize = 0;
        private long totalFileSize = 0;

        private Uri uri;
        private Thread thread;

        public const int TIMEOUT = 60 * 1000;
        private const int BYTECOUNT = 10240;

        public BreakPointDownLoad() 
        {

        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (thread != null)
                {
                    thread.Abort();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("[断点续传]终止下载线程失败:{0}",ex.StackTrace));
            }
            Clear();
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="localAbsPath">下载文件本地保存路径</param>
        /// <param name="isShowProgress">是否显示加载进度[true:显示,false:不显示]</param>
        /// <param name="onCompleted">下载完成回调onCompleted(url)</param>
        /// <param name="onFail">下载失败回调onFail(url)</param>
        public void Download(string url,string localAbsPath,bool isShowProgress = false,Action<string> onCompleted = null,Action<string> onFail = null)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                this.url = url;
                this.localAbsPath = localAbsPath;
                this.isShowProgress = isShowProgress;
                this.onCompleted = onCompleted;
                this.onFail = onFail;
                this.bytesDownloaded = 0;
                //1:    获取需要下载的字节数
                uri = new Uri(url);
                request = (HttpWebRequest)WebRequest.Create(uri);
                request.Timeout = TIMEOUT;
                request.ReadWriteTimeout = TIMEOUT;
                request.Proxy = null;       //听说能解决第一次连接慢的问题
                response = (HttpWebResponse)request.GetResponse();
                totalFileSize = response.ContentLength;
                needDownloadSize = totalFileSize;
                response.Close();
                request.Abort();
                response = null;
                request = null;
                //
                string folder = Path.GetDirectoryName(localAbsPath);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                if (File.Exists(localAbsPath))
                {
                    using (FileStream fs = new FileStream(localAbsPath,FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.ReadWrite))
                    {
                        bytesDownloaded = fs.Length;
                        needDownloadSize = totalFileSize - bytesDownloaded;
                    }
                }
                Debug.Log(string.Format("[断点续传]文件:{0},总字节数:{1}-已下载字节:{2},-需下载字节:{3}",url,totalFileSize,bytesDownloaded,needDownloadSize));
                //2:    
                if (needDownloadSize > 0)
                {
                    thread = new Thread(Download);
                    thread.Start();

                }
                else
                {
                    if (onCompleted != null) onCompleted(url);
                }

            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("文件:{0}下载失败!reason:{1}", url, ex.StackTrace));
                HandlerError(ex.Message);
            }
            finally
            {
                if (response != null) response.Close();
                if (request != null) request.Abort();
                response = null;
                request = null;
            }

        }

        private void Download()
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(uri);
                request.AddRange((int)bytesDownloaded);
                request.Timeout = TIMEOUT;
                request.ReadWriteTimeout = TIMEOUT;
                request.Proxy = null;
                response = (HttpWebResponse)request.GetResponse();
                int nOffset = 0;
                using (Stream s = response.GetResponseStream())
                {
                    byte[] buffer = new byte[BYTECOUNT];
                    int nReceivedByteCount = s.Read(buffer, nOffset, BYTECOUNT);
                    while (nReceivedByteCount != 0)
                    {
                        using (FileStream fs = new FileStream(localAbsPath,FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.ReadWrite))
                        {
                            fs.Position = bytesDownloaded;
                            fs.Write(buffer, 0, nReceivedByteCount);
                            fs.Close();
                        }
                        bytesDownloaded += nReceivedByteCount;
                        nOffset += nReceivedByteCount;
                        nReceivedByteCount = s.Read(buffer,0,BYTECOUNT);
                    }
                }
                //下载完成
                Debug.Log(string.Format("[断点续传]文件:{0}下载[OK],总字节数:{1},下载字节:{2}", url, totalFileSize,needDownloadSize));
                response.Close();
                request.Abort();
                response = null;
                request = null;
                //不能在子线程中调用主线程的方法onCompleted,因此延时50毫秒再在主线程回调onCompleted,切记
                //TimerHeap.AddTimer(50,0,OnCompleted2);

            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("文件:{0}下载失败!reason:{1}", url, ex.StackTrace));
                HandlerError(ex.Message);
            }
            finally
            {
                if (response != null) response.Close();
                if (request != null) request.Abort();
                response = null;
                request = null;
            }
        }

        private void OnCompleted2()
        {
            if (onCompleted != null) onCompleted(url);
        }

        private void HandlerError(string msg)
        {
            Debug.LogError(string.Format("[断点续传]失败:{0}", msg));
        }

        private void Clear()
        {
            thread = null;
        }


    }
}
